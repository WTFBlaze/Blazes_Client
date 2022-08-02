using Blaze.Utils;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using VRC;
using VRC.Core;

namespace Blaze.Modules
{
    class TrueRank : BModule
    {
        public static readonly List<APIUser> CachedApiUsers = new();
        private static readonly Queue<string> UsersToFetch = new();
        private static readonly System.Random Random = new();
        private static MethodBase _showSocialRankMethod;
        private static PropertyInfo VRCPlayer_ModTag = null;

        public override void Start()
        {
            var mainRef = typeof(VRCPlayer).GetMethods().FirstOrDefault(it => !it.Name.Contains("PDM") && it.ReturnType.ToString().Equals("System.String") && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType.ToString().Equals("VRC.Core.APIUser"));
            _showSocialRankMethod = XrefScanner.XrefScan(mainRef).Single(x =>
            {
                if (x.Type != XrefType.Method)
                    return false;

                var m = x.TryResolve();
                if (m == null || !m.IsStatic || m.DeclaringType != typeof(VRCPlayer))
                    return false;

                var asInfo = m as MethodInfo;
                if (asInfo == null || asInfo.ReturnType != typeof(bool))
                    return false;

                if (m.GetParameters().Length != 1 && m.GetParameters()[0].ParameterType != typeof(APIUser))
                    return false;

                return XrefScanner.XrefScan(m).Count() > 1;
            }).TryResolve();

            MelonCoroutines.Start(FetchAPIUsers());
        }

        private static bool GetFriendlyDetailedNameForSocialRank(APIUser __0, ref string __result)
        {
            if ((__0 != null) && MelonPreferences.GetEntryValue<bool>("ogtrustranks", "enabled"))
            {
                Player player = GetPlayerByUserId(__0.id);
                if (!__0.hasVIPAccess || (__0.hasModerationPowers && ((!(null != player) || !(null != player.prop_VRCPlayer_0) ? !__0.showModTag : string.IsNullOrEmpty((string)VRCPlayer_ModTag.GetGetMethod().Invoke(player.prop_VRCPlayer_0, null))))))
                {
                    TrustRanks rank = GetTrustRankEnum(__0);
                    if (rank == TrustRanks.Legendary)
                    {
                        __result = "Legendary User";
                        return false;
                    }
                    else if (rank == TrustRanks.Veteran)
                    {
                        __result = "Veteran User";
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool GetColorForSocialRank(APIUser __0, ref Color __result)
        {
            if ((__0 != null) && !APIUser.IsFriendsWith(__0.id))
            {
                Player player = GetPlayerByUserId(__0.id);
                if (!__0.hasVIPAccess || (__0.hasModerationPowers && ((!(null != player) || !(null != player.prop_VRCPlayer_0) ? !__0.showModTag : string.IsNullOrEmpty((string)VRCPlayer_ModTag.GetGetMethod().Invoke(player.prop_VRCPlayer_0, null))))))
                {
                    TrustRanks rank = GetTrustRankEnum(__0);
                    if (rank == TrustRanks.Legendary)
                    {
                        __result = new Color32(224, 88, 88, 255);
                        return false;
                    }
                    else if (rank == TrustRanks.Veteran)
                    {
                        __result = new Color32(194, 189, 54, 255);
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool TrueRanksColorPatch(APIUser __0, ref Color __result)
        {
            if (__0 == null || APIUser.IsFriendsWith(__0.id)) return true;

            if (GetPlayerByUserId(__0.id) != null)
            {
                var showSocialRank = (bool)_showSocialRankMethod.Invoke(null, new object[] { __0 });
                if (!showSocialRank)
                {
                    return true;
                }
            }

            var apiUser = CachedApiUsers.Find(x => x.id == __0.id) ?? __0;
            var rank = GetTrustRankEnum(apiUser);
            switch (rank)
            {
                case TrustRanks.Known:
                    __result = new Color32(219, 135, 0, 255);
                    return false;
                case TrustRanks.Trusted:
                    __result = new Color32(120, 0, 219, 255);
                    return false;
                case TrustRanks.Veteran:
                    __result = new Color32(194, 189, 54, 255);
                    return false;
                case TrustRanks.Legendary:
                    __result = new Color32(224, 88, 88, 255);
                    return false;
                case TrustRanks.Ignore:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        private static TrustRanks GetTrustRankEnum(APIUser user)
        {
            if (user?.tags == null || user.tags.Count <= 0)
                return TrustRanks.Ignore;

            if (user.tags.Contains("system_legend") && user.tags.Contains("system_trust_legend") && user.tags.Contains("system_trust_trusted"))
                return TrustRanks.Legendary;
            if (user.tags.Contains("system_trust_legend") && user.tags.Contains("system_trust_trusted"))
                return TrustRanks.Veteran;
            if (user.tags.Contains("system_trust_veteran") && user.tags.Contains("system_trust_trusted"))
                return TrustRanks.Trusted;
            if (user.tags.Contains("system_trust_trusted") && user.tags.Contains("system_trust_known"))
                return TrustRanks.Known;
            return TrustRanks.Ignore;
        }

        private enum TrustRanks
        {
            Ignore,
            Known,
            Trusted,
            Veteran,
            Legendary,
        }

        public override void PlayerJoined(Player instance)
        {
            if (instance == null)
                return;
            var apiUser = instance.prop_APIUser_0;
            if (apiUser == null)
                return;

            if (!apiUser.tags.Contains("system_trust_trusted"))
                return;

            if (CachedApiUsers.Exists(x => x.id == apiUser.id))
                return;

            if (UsersToFetch.Contains(apiUser.id))
                return;

            UsersToFetch.Enqueue(apiUser.id);
        }

        private static IEnumerator FetchAPIUsers()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                while (UsersToFetch.Count > 0)
                {
                    var id = UsersToFetch.Dequeue();
                    APIUser.FetchUser(id, new Action<APIUser>(user =>
                    {
                        CachedApiUsers.Add(user);
                    }), new Action<string>(error =>
                    {
                        Logs.Error($"Could not fetch APIUser object of {id}");
                    }));
                    yield return new WaitForSeconds(Random.Next(2, 5));
                }
            }
        }

        private static Player GetPlayerByUserId(string userId)
        {
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                if (player.prop_APIUser_0 != null && player.prop_APIUser_0.id == userId)
                    return player;
            return null;
        }
    }
}
