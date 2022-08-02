using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

namespace Blaze.Utils.VRChat
{
    public static class WorldUtils
    {
        public static ApiWorld CurrentWorld()
        {
            return RoomManager.field_Internal_Static_ApiWorld_0;
        }

        public static ApiWorldInstance CurrentInstance()
        {
            return RoomManager.field_Internal_Static_ApiWorldInstance_0;
        }

        public static IEnumerable<Player> GetPlayers()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
        }

        public static Player GetMaster()
        {
            foreach (var p in GetPlayers())
            {
                if (p.IsMaster()) return p;
            }
            return null;
        }

        public static Il2CppSystem.Collections.Generic.List<Player> GetPlayers2()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }

        public static bool IsInRoom()
        {
            return CurrentWorld() != null && CurrentWorld() != null;
        }

        public static int GetPlayerCount()
        {
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count;
        }

        public static string GetJoinID()
        {
            return CurrentInstance().id;
        }

        public static UdonBehaviour[] GetUdonScripts()
        {
            return UnityEngine.Object.FindObjectsOfType<UdonBehaviour>();
        }

        public static VRC_Pickup[] GetPickups()
        {
            return UnityEngine.Object.FindObjectsOfType<VRC_Pickup>();
        }

        public static string GetSDKType()
        {
            if (GetSDK2Descriptor() != null) return "SDK2";
            return GetSDK3Descriptor() != null ? "SDK3" : "null";
        }

        public static string GetSDKNumber()
        {
            if (GetSDK2Descriptor() != null) return "2";
            return GetSDK3Descriptor() != null ? "3" : "null";
        }

        public static int SelfObjectOwnedCount()
        {
            int result = 0;
            foreach (var p in GetPickups())
            {
                if (Networking.GetOwner(p.gameObject) == Networking.LocalPlayer)
                {
                    result++;
                }
            }
            return result;
        }

        public static bool IsDefaultScene(string name)
        {
            var lower = name.ToLower();
            string[] scenes = { "application2", "ui", "empty", "dontdestroyonload", "hideanddontsave", "samplescene" };
            return ((IList)scenes).Contains(lower);
        }

        public static VRCSDK2.VRC_SceneDescriptor GetSDK2Descriptor()
        {
            return UnityEngine.Object.FindObjectOfType<VRCSDK2.VRC_SceneDescriptor>();
        }

        public static VRC.SDK3.Components.VRCSceneDescriptor GetSDK3Descriptor()
        {
            return UnityEngine.Object.FindObjectOfType<VRC.SDK3.Components.VRCSceneDescriptor>();
        }

        public static void JoinRoom(string id)
        {
            new PortalInternal().Method_Private_Void_String_String_PDM_0(id.Split(':')[0], id.Split(':')[1]);
        }

        public static void JoinRoom(string worldID, string instanceID)
        {
            new PortalInternal().Method_Private_Void_String_String_PDM_0(worldID, instanceID);
        }

        public static void JoinRoom2(string id)
        {
            Networking.GoToRoom(id);
        }

        public static void JoinRoom2(string worldID, string instanceID) => JoinRoom2($"{worldID}:{instanceID}");

        public static void RejoinInstance()
        {
            Networking.GoToRoom(GetJoinID());
        }

        public static int GetFriendCount()
        {
            int results = 0;
            foreach (var p in GetPlayers2())
            {
                if (p.IsFriend())
                    results++;
            }
            return results;
        }
    }
}
