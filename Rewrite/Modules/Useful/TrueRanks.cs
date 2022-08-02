using Blaze.API;
using Blaze.Utils;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;

namespace Blaze.Modules
{
    public class TrueRanks : BModule
    {
        public static readonly List<APIUser> CachedApiUsers = new();
        private static readonly Queue<string> UsersToFetch = new();
        private static readonly PropertyInfo VRCPlayer_ModTag = null;

        public override void Start()
        {
            MelonCoroutines.Start(FetchAPIUsers());
        }

        private static bool GetFriendlyDetailedNameForSocialRank(APIUser __0, ref string __result)
        {
            if ((__0 != null) && MelonPreferences.GetEntryValue<bool>("ogtrustranks", "enabled"))
            {
                Player player = Functions.GetPlayerByUserID(__0.id);
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
                Player player = Functions.GetPlayerByUserID(__0.id);
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

            if (Functions.GetPlayerByUserID(__0.id) != null)
            {
                var showSocialRank = (bool)BlazesXRefs.ShowSocialRankMethod.Invoke(null, new object[] { __0 });
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

        public override void PlayerJoined(Player player)
        {
            if (player == null) return;
            var apiUser = player.prop_APIUser_0;
            if (apiUser == null) return;
            if (!apiUser.tags.Contains("system_trust_trusted")) return;
            if (CachedApiUsers.Exists(x => x.id == apiUser.id)) return;
            if (UsersToFetch.Contains(apiUser.id)) return;
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
                    yield return new WaitForSeconds(APIStuff.rnd.Next(2, 5));
                }
            }
        }
    }
}
