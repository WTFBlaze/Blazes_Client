using System.Collections.Generic;
using VRC.Core;
using VRC.Management;

namespace Blaze.Utils.VRChat
{
    public static class ModerationUtils
    {
        private static Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<ApiPlayerModeration>> moderations => ModerationManager.prop_ModerationManager_0.field_Private_Dictionary_2_String_List_1_ApiPlayerModeration_0;

        public static List<ApiPlayerModeration> GetModerationsOfUser(string userID)
        {
            List<ApiPlayerModeration> result = new();
            moderations.TryGetValue(userID, out Il2CppSystem.Collections.Generic.List<ApiPlayerModeration> list);
            foreach (var m in list)
            {
                result.Add(m);
            }
            return result;
        }

        public static bool UserExistsInList(string userID)
        {
            if (moderations.ContainsKey(userID)) return true;
            return false;
        }

        public static bool IsBlocked(this APIUser instance)
        {
            if (UserExistsInList(instance.id))
            {
                var list = GetModerationsOfUser(instance.id);
                foreach (var m in list)
                {
                    if (m.moderationType == ApiPlayerModeration.ModerationType.Block)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsMuted(this APIUser instance)
        {
            if (UserExistsInList(instance.id))
            {
                var list = GetModerationsOfUser(instance.id);
                foreach (var m in list)
                {
                    if (m.moderationType == ApiPlayerModeration.ModerationType.Mute)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsShown(this APIUser instance)
        {
            if (UserExistsInList(instance.id))
            {
                var list = GetModerationsOfUser(instance.id);
                foreach (var m in list)
                {
                    if (m.moderationType == ApiPlayerModeration.ModerationType.ShowAvatar)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsHidden(this APIUser instance)
        {
            if (UserExistsInList(instance.id))
            {
                var list = GetModerationsOfUser(instance.id);
                foreach (var m in list)
                {
                    if (m.moderationType == ApiPlayerModeration.ModerationType.HideAvatar)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
