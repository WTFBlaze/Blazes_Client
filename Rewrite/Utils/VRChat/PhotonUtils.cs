using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using PhotonHandler = MonoBehaviour1PrivateObInPrInBoInInInInUnique;

namespace Blaze.Utils.VRChat
{
    public static class PhotonUtils
    {
        public static LoadBalancingClient LoadBalancingPeer => PhotonHandler.prop_LoadBalancingClient_0;
        public static PhotonHandler PhotonHandler => PhotonHandler.field_Internal_Static_MonoBehaviour1PrivateObInPrInBoInInInInUnique_0;

        public static string GetUserID(this Player player)
        {
            if (player.GetRawHashtable().ContainsKey("user"))
                if (player.GetHashtable()["user"] is Dictionary<string, object> dict)
                    return (string)dict["id"];
            return "No ID";
        }

        public static string GetDisplayName(this Player player)
        {
            if (player.GetRawHashtable().ContainsKey("user"))
                if (player.GetHashtable()["user"] is Dictionary<string, object> dict)
                    return (string)dict["displayName"];
            return "No DisplayName";
        }

        public static int GetPhotonID(this Player player)
            => player.field_Private_Int32_0;

        public static VRC.Player GetPlayer(this Player player)
            => player.field_Public_Player_0;

        public static bool IsInvisible(this Player player)
        {
            foreach (var p in GetAllPhotonPlayers())
            {
                if (p.GetUserID() == player.GetUserID())
                {
                    if (player.GetPlayer() == null) return true;
                    else return false;
                }
            }
            return false;
        }

        public static System.Collections.Hashtable GetHashtable(this Player player)
            => SerializationUtils.FromIL2CPPToManaged<System.Collections.Hashtable>(player.GetRawHashtable());

        public static Il2CppSystem.Collections.Hashtable GetRawHashtable(this Player player)
            => player.prop_Hashtable_0;

        public static List<Player> GetAllPhotonPlayers(this LoadBalancingClient Instance)
        {
            var result = new List<Player>();
            foreach (var x in Instance.prop_Room_0.prop_Dictionary_2_Int32_Player_0)
                result.Add(x.Value);
            return result;
        }

        public static List<Player> GetAllPhotonPlayers()
        {
            var result = new List<Player>();
            foreach (var x in LoadBalancingPeer.prop_Room_0.prop_Dictionary_2_Int32_Player_0)
                result.Add(x.Value);
            return result;
        }

        public static Player GetPhotonPlayer(this LoadBalancingClient Instance, int photonID)
        {
            foreach (var x in Instance.GetAllPhotonPlayers())
                if (x.GetPhotonID() == photonID)
                    return x;
            return null;
        }

        public static Player GetPhotonPlayer(int photonID)
        {
            foreach (var x in LoadBalancingPeer.GetAllPhotonPlayers())
                if (x.GetPhotonID() == photonID)
                    return x;
            return null;
        }

        public static Player GetPhotonPlayer(this LoadBalancingClient Instance, string userID)
        {
            foreach (var x in Instance.GetAllPhotonPlayers())
                if (x.GetUserID() == userID)
                    return x;
            return null;
        }

        public static Player GetPhotonPlayer(string userID)
        {
            foreach (var x in LoadBalancingPeer.GetAllPhotonPlayers())
                if (x.GetUserID() == userID)
                    return x;
            return null;
        }

        public static void OpRaiseEvent(byte code, object customObject, RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
        {
            Il2CppSystem.Object Object = SerializationUtils.FromManagedToIL2CPP<Il2CppSystem.Object>(customObject);
            OpRaiseEvent(code, Object, RaiseEventOptions, sendOptions);
        }

        public static void OpRaiseEvent(byte code, Il2CppSystem.Object customObject, RaiseEventOptions RaiseEventOptions, SendOptions sendOptions)
           => PhotonNetwork.Method_Public_Static_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0
            (code,
             customObject,
             RaiseEventOptions,
             sendOptions);
    }
}
