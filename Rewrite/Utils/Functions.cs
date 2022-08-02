using Blaze.API;
using Blaze.Modules;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.SDKBase;

namespace Blaze.Utils
{
    public static class Functions
    {
        public static string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static char GetRandomCharacter()
        {
            int index = APIStuff.rnd.Next(Letters.Length);
            return Letters[index];
        }

        public static bool CheckLoaderVersion(string loaderID = null)
        {
            return loaderID == "5d7b7335-c69d-4bf7-867f-682df1e6ca42";
        }

        public static IEnumerable<string> GetCommandLineArgs()
        {
            var text = Environment.CommandLine.ToLower();
            return text.Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsDevMode()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower() == "--wtfblaze.devmode");
        }

        public static bool IsRejoinRestart()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower().StartsWith("--wtfblaze.rejoin="));
        }

        public static string GetRejoinRestartID()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.ToList().Find(x => x.ToLower().StartsWith("--wtfblaze.rejoin="));
        }

        public static Player GetPlayerByUserID(string userID)
        {
            return WorldUtils.GetPlayers().FirstOrDefault(p => p.prop_APIUser_0.id == userID);
        }

        public static Player GetPlayerByActorID(int actorID)
        {
            return WorldUtils.GetPlayers().FirstOrDefault(p => p._vrcplayer.prop_VRCPlayerApi_0.playerId == actorID);
        }

        public static void TeleportByUserID(string userID)
        {
            foreach (var p in WorldUtils.GetPlayers())
            {
                if (p.GetAPIUser().id == userID)
                {
                    PlayerUtils.CurrentUser().transform.position = p._vrcplayer.transform.position;
                    return;
                }
            }
        }

        public static void SelectPlayer(Player player)
        {
            BlazesXRefs.SelectUserMethod.Invoke(UserSelectionManager.prop_UserSelectionManager_0, new object[1] { player.field_Private_APIUser_0 });
        }

        public static void SelectPlayer(APIUser player)
        {
            BlazesXRefs.SelectUserMethod.Invoke(UserSelectionManager.prop_UserSelectionManager_0, new object[1] { player });
        }

        public static void SelectPlayer(VRCPlayer player)
        {
            BlazesXRefs.SelectUserMethod.Invoke(UserSelectionManager.prop_UserSelectionManager_0, new object[1] { player._player.field_Private_APIUser_0 });
        }

        public static void ForceQuit()
        {
            if (BlazeNetwork.IsConnected)
            {
                BlazeNetwork.AllowReconnect = false;
                BlazeNetwork.ws.Close();
            }

            try { Process.GetCurrentProcess().Kill(); }
            catch { }

            try { Environment.Exit(0); }
            catch { }

            try { Application.Quit(); }
            catch { }
        }

        public static void Restart(string restartJoinID = null)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + "\\VRChat.exe",
                Arguments = $"-screen-width {Screen.width} -screen-height {Screen.height}"
            };
            if (IsDevMode())
            {
                psi.Arguments += " --wtfblaze.devmode";
            }
            if (!XRDevice.isPresent)
            {
                psi.Arguments += $" --no-vr";
            }
            if (GetCommandLineArgs().Contains("--enable-debug-gui"))
            {
                psi.Arguments += " --enable-debug-gui";
            }
            if (!string.IsNullOrEmpty(restartJoinID))
            {
                psi.Arguments += $" --wtfblaze.rejoin={restartJoinID}";
            }
            Process.Start(psi);
            ForceQuit();
        }

        public static void TeleportByName(string displayName)
        {
            foreach (var p in WorldUtils.GetPlayers())
            {
                if (p.GetAPIUser().displayName == displayName)
                {
                    PlayerUtils.CurrentUser().transform.position = p._vrcplayer.transform.position;
                    return;
                }
            }
        }

        public static void Delay(Action action, float delayTime) => MelonCoroutines.Start(ProcessDelay(action, delayTime));

        private static IEnumerator ProcessDelay(Action action, float delayTime)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            action.Invoke();
        }

        public static void EmojiRPC(int i)
        {
            try
            {
                Il2CppSystem.Int32 @int = default(Il2CppSystem.Int32);
                @int.m_value = i;
                Il2CppSystem.Object @object = @int.BoxIl2CppObject();
                Networking.RPC(0, PlayerUtils.CurrentUser().gameObject, "SpawnEmojiRPC", new Il2CppSystem.Object[]
                {
                    @object
                });
            }
            catch { }
        }

        public static void PlayEmoteRPC(int i)
        {
            try
            {
                var x = new Il2CppSystem.Int32();
                x.m_value = i;
                var obj = x.BoxIl2CppObject();
                Networking.RPC(0, PlayerUtils.CurrentUser().gameObject, "PlayEmoteRPC", new Il2CppSystem.Object[]
                {
                    obj,
                });
            }
            catch { }
        }

        public static readonly List<string> EmojiType = new List<string>
        {
            ">:(", //0
            "Blushing", //1
            "Crying", //2
            "D:", //3
            "Wave", //4
            "Hang Loose", //5
            "Heart Eyes", //6
            "Pumpkin", //7
            "Kissy Face", //8
            "xD", //9
            "Skull", //10
            ":D", //11
            "Ghost", //12
            ":|", //13
            "B|", //14
            "Thinking", //15
            "Thumbs Down", //16
            "Thumbs Up", //17
            ":P", //18
            ":O", //19
            "Bats", //20
            "Cloud", //21
            "Fire", //22
            "Snowflakes", //23
            "Snowball", //24
            "Water Droplets", //25
            "Cobwebs", //26
            "Beer", //27
            "Candy", //28
            "Candy Cane", //29
            "Candy Corn", //30
            "Cheers", //31
            "Coconut Drink", //32
            "Gingerbread Man", //33
            "Ice Cream", //34
            "Pineapple", //35
            "Pizza", //36
            "Tomato", //37
            "Beach Ball", //38
            "Rock Gift", //39
            "Confetti", //40
            "Gift", //41
            "Christmas Gifts", //42
            "Lifebuoy", //43
            "Mistletoe", //44
            "Money", //45
            "Sunglasses", //46
            "Sunscreen", //47
            "BOO!", //48
            "</3", //49
            "!", //50
            "GO", //51
            "Heart", //52
            "Music Note", //53
            "?", //54
            "NO", //55
            "ZZZ", //56
        };

        public static readonly List<string> EmoteType = new List<string>
        {
            "",
            "wave",
            "clap",
            "point",
            "cheer",
            "dance",
            "backflip",
            "die",
            "sadness",
        };

        public static void DropPortal(string RoomId)
        {
            var Location = RoomId.Split(':');
            DropPortal(Location[0], Location[1], 0,
                PlayerUtils.CurrentUser().transform.position + PlayerUtils.CurrentUser().transform.forward * 2f, PlayerUtils.CurrentUser().transform.rotation);
        }

        public static void DropPortal(string WorldID, string InstanceID, int players, Vector3 vector3, Quaternion quaternion)
        {
            GameObject gameObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", vector3, quaternion);
            Networking.RPC(RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new[]
            {
                (Il2CppSystem.String)WorldID,
                (Il2CppSystem.String)InstanceID,
                new Il2CppSystem.Int32
                {
                    m_value = players
                }.BoxIl2CppObject()
            });
            //MelonCoroutines.Start(MiscUtility.DestroyDelayed(1f, gameObject.GetComponent<PortalInternal>()));
        }

        /*public static void DropPortal(string WorldID, VRCPlayer target)
        {
            GameObject portal = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", target.transform.position + target.transform.forward * 1.505f, target.transform.rotation);
            string world = WorldID;
            string instance = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n[BLAZESCLIENT]\n" + target._player.field_Private_APIUser_0.displayName;
            System.Random r = new();
            var values = new[] { -69, -666 };
            int result = values[r.Next(values.Length)];
            int count = result;
            Networking.RPC(RPC.Destination.AllBufferOne, portal, "ConfigurePortal", new Il2CppSystem.Object[]
            {
              (Il2CppSystem.String)world,
              (Il2CppSystem.String)instance,
              new Il2CppSystem.Int32
              {
                m_value = count
              }.BoxIl2CppObject()
            });
        }*/

        public static IEnumerator ProcessFriendsImport()
        {
            if (!File.Exists(ModFiles.FriendsImportFile))
            {
                Logs.Error("Please place your Exported Friends file in VRChat/Blaze/Imports!");
                PopupUtils.InformationAlert("Please place your Exported Friends File in VRChat/Blaze/Imports and then try again!");
                yield break;
            }
            string[] Friends = File.ReadAllLines(ModFiles.FriendsImportFile);
            Logs.Log($"[Friends-Importer] About to Send {Friends.Length} Friend Requests. ETC {(Friends.Length * 5) + ((Friends.Length / 20) * 75)} Seconds");
            Stopwatch stopwatch = Stopwatch.StartNew();
            int i = 0;
            int requests = 0;
            foreach (string line in Friends)
            {
                if (!line.Contains("usr_")) continue;
                var id = line;
                if (line.Contains(","))
                {
                    var split = line.Split(',');
                    id = split[0];
                }
                if (!APIUser.IsFriendsWith(id))
                {
                    yield return new WaitForSeconds(5);
                    Logs.Log("[Friends-Importer] send a Request to " + id);
                    NotificationUtils.SendFriendRequest(id);
                    i++;
                    requests++;
                    if (i >= 20)
                    {
                        i = 0;
                        Logs.Log("[Friends-Importer] Waiting to not get rate limited!");
                        yield return new WaitForSeconds(75);
                    }
                }
                else
                {
                    Logs.Error("[Friends-Importer] Skipping " + id);
                }
                Logs.Log($"[Friends-Importer] {(float)requests * 100 / Friends.Length:0.00}%");
            }

            stopwatch.Stop();
            Logs.Log("=============================");
            Logs.Log($"Summary:\n with {File.ReadAllLines(ModFiles.FriendsImportFile).Length} friends\n Took {stopwatch.Elapsed}");
            yield break;
        }

        public static void GlobalDestroyPortals()
        {
            var deletedCount = 0;
            foreach (var p in UnityEngine.Object.FindObjectsOfType<PortalInternal>())
            {
                if (p.gameObject.name.StartsWith("(Clone ["))
                {
                    UnityEngine.Object.Destroy(p.gameObject);
                    deletedCount++;
                }
            }
            Logs.HUD($"Globally Destroyed <color=yellow>{deletedCount}</color> portals!", 3.5f);
            Logs.Log($"Globally Destroyed {deletedCount} portals!");
            Logs.Debug($"Globally Destroyed <color=yellow>{deletedCount}</color> portals!");
        }

        public static void LocallyDestroyPortals()
        {
            var deletedCount = 0;
            foreach (var p in UnityEngine.Object.FindObjectsOfType<PortalInternal>())
            {
                if (p.gameObject.name.StartsWith("(Clone ["))
                {
                    p.gameObject.SetActive(false);
                    deletedCount++;
                }
            }
            Logs.HUD($"Locally Destroyed <color=yellow>{deletedCount}</color> portals!", 3.5f);
            Logs.Log($"Locally Destroyed {deletedCount} portals!");
            Logs.Debug($"Locally Destroyed <color=yellow>{deletedCount}</color> portals!");
        }

        public static void ResetPickups()
        {
            foreach (VRC_Pickup vrc_Pickup in UnityEngine.Object.FindObjectsOfType<VRC_Pickup>())
            {
                Networking.LocalPlayer.TakeOwnership(vrc_Pickup.gameObject);
                vrc_Pickup.transform.position = new Vector3(0f, -10000000f, 0f);
            }
        }

        public static void TeleportObjectToRightHand(GameObject target)
        {
            target.transform.position = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetBonePosition(HumanBodyBones.RightHand);
        }

        public static void TeleportObjectToLeftHand(GameObject target)
        {
            target.transform.position = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetBonePosition(HumanBodyBones.LeftHand);
        }

        public static void SelfHide(bool state)
        {
            APIStuff.GetAvatarPreviewBase().SetActive(!state);
            PlayerUtils.CurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(!state);
            AssetBundleDownloadManager.prop_AssetBundleDownloadManager_0.gameObject.SetActive(!state);
        }

        public static void IncreaseGain(int amount)
        {
            try
            {
                USpeakerUtils.SetGain(USpeakerUtils.GetGain() + amount);
            }
            catch { }
        }

        public static void DecreaseGain(int amount)
        {
            try
            {
                USpeakerUtils.SetGain(USpeakerUtils.GetGain() - amount);
            }
            catch { }
        }

        public static void IncreaseBitRate(int amount)
        {
            try
            {
                //if (USpeakerUtils.GetBitRate() != BitRate.BitRate_512k)
                //   USpeakerUtils.SetBitrate((BitRate)Convert.ChangeType(USpeakerUtils.GetBitRate(), USpeakerUtils.GetBitRate().GetTypeCode()) + amount);

                if (USpeakerUtils.GetBitRate().Next(amount) > BitRate.BitRate_24K) return;
                USpeakerUtils.SetBitrate(USpeakerUtils.GetBitRate().Next(amount));
            }
            catch { }
        }

        public static void DecreaseBitRate(int amount)
        {
            try
            {
                //if (USpeakerUtils.GetBitRate() != BitRate.BitRate_8K)
                //    USpeakerUtils.SetBitrate((BitRate)Convert.ChangeType(USpeakerUtils.GetBitRate(), USpeakerUtils.GetBitRate().GetTypeCode()) - amount);
                USpeakerUtils.SetBitrate(USpeakerUtils.GetBitRate().Previous(amount));
            }
            catch { }
        }

        private static int ourRequestId;
        public static IEnumerator EnforceTargetInstanceType(VRCFlowManager manager, InstanceAccessType type, float time)
        {
            var endTime = Time.time + time;
            var currentRequestId = ++ourRequestId;
            while (Time.time < endTime && ourRequestId == currentRequestId)
            {
                manager.field_Protected_InstanceAccessType_0 = type;
                yield return null;
            }
        }

        public static void AntiLockInstance(int buildIndex)
        {
            if (buildIndex == -1)
            {
                static IEnumerator VRCPlayerWait()
                {
                    while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        yield return null;
                    }
                    if (Config.Main.AntiInstanceLock)
                    {
                        /* If New Update Test This
                        MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique.field_Internal_Static_MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        */
                        VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Private_MonoBehaviourPrivateBo1SiObNuObSiIn1UIUnique_0.field_Private_Boolean_0 = true;
                    }
                }
                MelonCoroutines.Start(VRCPlayerWait());
            }
        }
    }
}
