using Blaze.Modules;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI.Elements.Menus;

namespace Blaze.Utils.VRChat
{
    public static class PlayerUtils
    {
        public static VRCPlayer CurrentUser()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }

        public static VRCPlayer GetSelectedUser()
        {
            var a = UnityEngine.Object.FindObjectOfType<SelectedUserMenuQM>().field_Private_IUser_0;
            return Functions.GetPlayerByUserID(a.prop_String_0)._vrcplayer;
        }

        #region Get API User

        public static APIUser GetAPIUser(this VRCPlayer instance)
        {
            return instance._player.field_Private_APIUser_0;
        }

        public static APIUser GetAPIUser(this Player instance)
        {
            return instance.field_Private_APIUser_0;
        }

        #endregion

        #region Get VRCPlayer

        public static VRCPlayer GetVRCPlayer(this Player instance)
        {
            return instance._vrcplayer;
        }

        #endregion

        #region Get VRCPlayerApi

        public static VRCPlayerApi GetVRCPlayerApi(this VRCPlayer instance)
        {
            return instance.prop_VRCPlayerApi_0;
        }

        public static VRCPlayerApi GetVRCPlayerApi(this Player instance)
        {
            return instance.prop_VRCPlayerApi_0;
        }

        #endregion

        #region Get PlayerNet

        public static PlayerNet GetPlayerNet(this VRC.Player instance)
        {
            return instance.prop_PlayerNet_0;
        }

        public static PlayerNet GetPlayerNet(this VRCPlayer instance)
        {
            return instance.prop_PlayerNet_0;
        }

        #endregion

        #region Get PC Avatar

        public static ApiAvatar GetPCAvatar(this VRCPlayer instance)
        {
            return instance.field_Private_ApiAvatar_0;
        }

        public static ApiAvatar GetPCAvatar(this Player instance)
        {
            return instance.prop_ApiAvatar_0;
        }

        #endregion

        #region Get Quest Avatar

        public static ApiAvatar GetQuestAvatar(this VRCPlayer instance)
        {
            return instance.field_Private_ApiAvatar_1;
        }

        public static ApiAvatar GetQuestAvatar(this Player instance)
        {
            return instance._vrcplayer.prop_ApiAvatar_1;
        }

        #endregion

        #region Get User ID

        public static string GetUserID(this VRCPlayer instance)
        {
            return instance.GetAPIUser().id;
        }

        public static string GetUserID(this Player instance)
        {
            return instance.GetAPIUser().id;
        }

        #endregion

        #region Get Display Name

        public static string GetDisplayName(this VRCPlayer instance)
        {
            return instance.GetAPIUser().displayName;
        }

        public static string GetDisplayName(this Player instance)
        {
            return instance.GetAPIUser().displayName;
        }

        public static string GetDisplayName(this VRCPlayerApi instance)
        {
            return instance.displayName;
        }

        #endregion

        #region Get Registered Name

        public static string GetRegisteredName(this VRCPlayer instance)
        {
            return instance.GetAPIUser().username;
        }

        public static string GetRegisteredName(this Player instance)
        {
            return instance.GetAPIUser().username;
        }

        #endregion

        #region Get Actor Number

        public static int GetActorNumber(this Player instance)
        {
            return instance.field_Private_VRCPlayerApi_0.playerId;
        }

        public static int GetActorNumber(this VRCPlayer instance)
        {
            return instance.field_Private_VRCPlayerApi_0.playerId;
        }

        #endregion

        #region GetPlatform

        public static string GetPlatform(this VRCPlayer instance)
        {
            if (instance.GetVRCPlayerApi().IsUserInVR())
            {
                return instance.GetAPIUser().last_platform == "android" ? "Quest" : "VR";
            }
            return "Desktop";
        }

        public static string GetPlatform(this Player instance)
        {
            if (instance.GetVRCPlayerApi().IsUserInVR())
            {
                return instance.GetAPIUser().last_platform == "android" ? "Quest" : "VR";
            }
            return "Desktop";
        }

        #endregion

        #region IsFriend

        public static bool IsFriend(this VRCPlayer instance)
        {
            return CurrentUser().GetAPIUser().friendIDs.Contains(instance.GetUserID());
        }

        public static bool IsFriend(this Player instance)
        {
            return CurrentUser().GetAPIUser().friendIDs.Contains(instance.GetUserID());
        }

        #endregion

        #region Is World Creator

        /*public static bool IsWorldCreator(this VRCPlayer instance)
        {
            return WorldUtils.CurrentWorld().authorId.Equals(instance.GetUserID());
        }

        public static bool IsWorldCreator(this Player instance)
        {
            return WorldUtils.CurrentWorld().authorId.Equals(instance.GetUserID());
        }*/

        #endregion

        #region Is Instance Master

        public static bool IsMaster(this VRCPlayer instance)
        {
            return instance.GetVRCPlayerApi().isMaster;
        }

        public static bool IsMaster(this Player instance)
        {
            return instance.GetVRCPlayerApi().isMaster;
        }

        #endregion

        #region Is Instance Creator

        public static bool IsInstanceCreator(this VRCPlayer instance)
        {
            return instance.GetVRCPlayerApi().isInstanceOwner;
        }

        public static bool IsInstanceCreator(this Player instance)
        {
            return instance.GetVRCPlayerApi().isInstanceOwner;
        }

        public static bool IsInstanceCreator(this VRCPlayerApi instance)
        {
            return instance.isInstanceOwner;
        }

        #endregion

        #region Is Target

        public static bool IsTarget(this VRCPlayer instance)
        {
            return instance._player == Main.Target;
        }

        public static bool IsTarget(this Player instance)
        {
            return instance == Main.Target;
        }

        #endregion

        #region Is KOS

        public static bool IsKOS(this VRCPlayer instance)
        {
            return Config.KOS.list.Exists(x => x.UserID == instance.GetUserID());
        }

        public static bool IsKOS(this Player instance)
        {
            return Config.KOS.list.Exists(x => x.UserID == instance.GetUserID());
        }

        #endregion

        #region Is Bot

        public static bool IsBot(this VRCPlayer instance)
        {
            // Check Ping & Frames
            if (instance.GetPing() == 0 && instance.GetFrames() <= -1) return true;

            // Check Position
            if (instance.PositionIsZero()) return true;
            return false;
        }

        #endregion

        #region Is Blocked

        public static bool IsBlocked(this APIUser instance)
        {
            return ModerationUtils.IsBlocked(instance);
        }

        public static bool IsBlocked(this VRCPlayer instance)
        {
            return ModerationUtils.IsBlocked(instance.GetAPIUser());
        }

        public static bool IsBlocked(this Player instance)
        {
            return ModerationUtils.IsBlocked(instance.GetAPIUser());
        }

        #endregion

        #region Is Muted

        public static bool IsMuted(this APIUser instance)
        {
            return ModerationUtils.IsMuted(instance);
        }

        public static bool IsMuted(this VRCPlayer instance)
        {
            return ModerationUtils.IsMuted(instance.GetAPIUser());
        }

        public static bool IsMuted(this Player instance)
        {
            return ModerationUtils.IsMuted(instance.GetAPIUser());
        }

        #endregion

        #region Is Blocked By

        public static bool IsBlockedBy(this APIUser instance)
        {
            return Patching.KnownBlocks.Contains(instance.displayName);
        }

        public static bool IsBlockedBy(this VRCPlayer instance)
        {
            return Patching.KnownBlocks.Contains(instance.GetDisplayName());
        }

        public static bool IsBlockedBy(this Player instance)
        {
            return Patching.KnownBlocks.Contains(instance.GetDisplayName());
        }

        #endregion

        #region Is Muted By

        public static bool IsMutedBy(this APIUser instance)
        {
            return Patching.KnownMutes.Contains(instance.displayName);
        }

        public static bool IsMutedBy(this VRCPlayer instance)
        {
            return Patching.KnownMutes.Contains(instance.GetDisplayName());
        }

        public static bool IsMutedBy(this Player instance)
        {
            return Patching.KnownMutes.Contains(instance.GetDisplayName());
        }

        #endregion

        #region Position is Zero

        public static bool PositionIsZero(this VRCPlayer instance)
        {
            return instance.gameObject.transform.position == Vector3.zero;
        }

        public static bool PositionIsZero(this Player instance)
        {
            return instance._vrcplayer.gameObject.transform.position == Vector3.zero;
        }

        #endregion

        #region Get Rank

        public static string GetRank(this APIUser Instance)
        {
            if (Instance.hasModerationPowers || Instance.tags.Contains("admin_moderator"))
                return "Moderation User";
            if (Instance.hasSuperPowers || Instance.tags.Contains("admin_"))
                return "Admin User";
            if (Instance.tags.Contains("system_legend") && Instance.tags.Contains("system_trust_legend") && Instance.tags.Contains("system_trust_trusted"))
                return "Legend";
            if (Instance.tags.Contains("system_trust_legend") && Instance.tags.Contains("system_trust_trusted"))
                return "Veteran";
            if (Instance.hasVeteranTrustLevel)
                return "Trusted";
            if (Instance.hasTrustedTrustLevel)
                return "Known";
            if (Instance.hasKnownTrustLevel)
                return "User";
            if (Instance.hasBasicTrustLevel || Instance.isNewUser)
                return "New User";
            if (Instance.hasNegativeTrustLevel || Instance.tags.Contains("system_probable_troll"))
                return "NegativeTrust";
            return Instance.hasVeryNegativeTrustLevel ? "VeryNegativeTrust" : "Visitor";
        }

        #endregion

        #region Get Rank Color

        public static string GetRankColor(this APIUser Instance)
        {
            var playerRank = Instance.GetRank();
            return playerRank.ToLower() switch
            {
                "moderation user" => "#5e0000",
                "admin user" => "#5e0000",
                "legend" => "#ff5e5e",
                "veteran" => "#fff821",
                "trusted" => "#a621ff",
                "known" => "#ffa200",
                "user" => "#00e62a",
                "new user" => "#00aeff",
                "visitor" => "#bababa",
                _ => "#303030"
            };
        }

        public static string GetRankColor(string rank)
        {
            return rank.ToLower() switch
            {
                "moderation user" => "#5e0000",
                "admin user" => "#5e0000",
                "legend" => "#ff5e5e",
                "veteran" => "#fff821",
                "trusted" => "#a621ff",
                "known" => "#ffa200",
                "user" => "#00e62a",
                "new user" => "#00aeff",
                "visitor" => "#bababa",
                _ => "#303030"
            };
        }

        public static Color GetRankUnityColor(string rank)
        {
            return rank.ToLower() switch
            {
                "moderation user" => new Color32(94, 0, 0, 255),
                "admin user" => new Color32(94, 0, 0, 255),
                "legend" => new Color32(255, 94, 94, 255),
                "veteran" => new Color32(255, 248, 33, 255),
                "trusted" => new Color32(166, 33, 255, 255),
                "known" => new Color32(255, 162, 0, 255),
                "user" => new Color32(0, 230, 42, 255),
                "new user" => new Color32(000, 174, 255, 255),
                "visitor" => new Color32(186, 186, 186, 255),
                _ => new Color32(48, 48, 48, 255),
            };
        }

        #endregion

        #region Get Ping

        public static short GetPing(this VRCPlayer instance)
        {
            return instance.GetPlayerNet().field_Private_Int16_0;
        }

        public static short GetPing(this VRC.Player instance)
        {
            return instance.GetPlayerNet().field_Private_Int16_0;
        }

        #endregion

        #region Get Frames

        public static int GetFrames(this VRC.Player instance)
        {
            return instance.GetPlayerNet().prop_Byte_0 != 0 ? (int)(1000f / instance.GetPlayerNet().prop_Byte_0) : 0;
        }

        public static int GetFrames(this VRCPlayer instance)
        {
            return instance.GetPlayerNet().prop_Byte_0 != 0 ? (int)(1000f / instance.GetPlayerNet().prop_Byte_0) : 0;
        }

        #endregion

        // other functions

        public static string GetTrueRank(this APIUser instance)
        {
            var apiUser = TrueRanks.CachedApiUsers.Find(x => x.id == instance.id) ?? instance;
            return GetRank(apiUser);
        }

        public static string GetTrueRankColor(this APIUser instance)
        {
            var apiUser = TrueRanks.CachedApiUsers.Find(x => x.id == instance.id) ?? instance;
            return GetRankColor(apiUser);
        }

        public static Color GetTrueRankUnityColor(this APIUser instance)
        {
            var apiUser = TrueRanks.CachedApiUsers.Find(x => x.id == instance.id) ?? instance;
            return GetRankUnityColor(apiUser.GetTrueRank());
        }

        public static APIUser GetCachedAPIUser(string userID)
        {
            return TrueRanks.CachedApiUsers.Find(x => x.id == userID);
        }

        public static bool APIUserIsCached(string userID)
        {
            return TrueRanks.CachedApiUsers.Exists(x => x.id == userID);
        }
    }
}
