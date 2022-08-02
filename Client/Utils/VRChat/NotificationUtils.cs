using System;
using Transmtn.DTO.Notifications;

namespace Blaze.Utils.VRChat
{
    internal static class NotificationUtils
    {
        internal static void SendInvite(string WorldID, string worldname, string playerID)
        {
            try
            {
                NotificationDetails notificationDetails = new();
                notificationDetails.Add("worldId", (Il2CppSystem.String)WorldID);
                notificationDetails.Add("instanceId", (Il2CppSystem.String)WorldID);
                notificationDetails.Add("worldName", (Il2CppSystem.String)worldname);
                Notification xx = Notification.Create(playerID, "invite", "", notificationDetails);
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(xx);
                Logs.Log($"[INVITES] Successfully force invited {playerID}!", ConsoleColor.Green);
            }
            catch
            {
                Logs.Log($"[INVITES] Failed to force invite {playerID}!", ConsoleColor.Red);
            }
        }

        internal static void SendFriendRequest(string userID)
        {
            try
            {
                Notification notif = FriendRequest.Create(userID);
                SendNotification(notif);
            }
            catch
            {
                Logs.Log($"[FRIEND REQUEST] Failed to friend request {userID}!", ConsoleColor.Red);
            }
        }

        internal static void SendNotification(Notification notif)
        {
            VRCWebSocketsManager.prop_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(notif);
        }
    }
}
