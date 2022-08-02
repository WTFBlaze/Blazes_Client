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
using System.Text;
using System.Threading.Tasks;
using Transmtn.DTO.Notifications;
using UnityEngine;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Blaze.Utils
{
    internal static class Functions
    {
        internal static System.Random random = new();
        internal static string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        internal static char GetRandomCharacter()
        {
            int index = random.Next(Letters.Length);
            return Letters[index];
        }

        internal static bool CheckLoaderVersion(string loaderID = null)
        {
            return loaderID == "5d7b7335-c69d-4bf7-867f-682df1e6ca42";
        }

        internal static IEnumerable<string> GetCommandLineArgs()
        {
            var text = Environment.CommandLine.ToLower();
            return text.Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static bool IsDevMode()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower() == "--wtfblaze.devmode");
        }

        internal static BlazesPlayerInfo GetInfoComp(string userid)
        {
            BlazeInfo.CachedPlayers.TryGetValue(userid, out BlazesPlayerInfo comp);
            if (comp != null) return comp;
            return null;
        }

        internal static void ForceQuit()
        {
            try { Process.GetCurrentProcess().Kill(); }
            catch { }

            try { Environment.Exit(0); }
            catch { }

            try { Application.Quit(); }
            catch { }
        }

        internal static void Restart()
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
            Process.Start(psi);
            ForceQuit();
        }

        internal static Player GetPlayerByUserID(string userID)
        {
            return WorldUtils.GetPlayers().FirstOrDefault(p => p.prop_APIUser_0.id == userID);
        }

        internal static void Delay(Action action, float delayTime) => MelonCoroutines.Start(ProcessDelay(action, delayTime));

        private static IEnumerator ProcessDelay(Action action, float delayTime)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            action.Invoke();
        }

        internal static void EmojiRPC (int i)
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

        internal static void PlayEmoteRPC(int i)
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

        internal static readonly List<string> EmojiType = new List<string>
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

        internal static readonly List<string> EmoteType = new List<string>
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

        internal static void TeleportByUserID(string userID)
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

        internal static void TeleportByName(string displayName)
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

        internal static Player GetPlayerID(this VRC.PlayerManager Instance, int playerID)
        {
            var Players = Instance.prop_ArrayOf_Player_0;
            foreach (Player player in Players.ToArray())
                if (player.GetVRCPlayerApi().playerId == playerID)
                    return player;
            return null;
        }

        internal static void GlobalDestroyPortals()
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

        internal static void LocallyDestroyPortals()
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

        internal static void ResetPickups()
        {
            foreach (VRC_Pickup vrc_Pickup in UnityEngine.Object.FindObjectsOfType<VRC_Pickup>())
            {
                Networking.LocalPlayer.TakeOwnership(vrc_Pickup.gameObject);
                vrc_Pickup.transform.position = new Vector3(0f, -10000000f, 0f);
            }
        }

        internal static void DropPortal(string RoomId)
        {
            var Location = RoomId.Split(':');
            DropPortal(Location[0], Location[1], 0,
                PlayerUtils.CurrentUser().transform.position + PlayerUtils.CurrentUser().transform.forward * 2f, PlayerUtils.CurrentUser().transform.rotation);
        }

        internal static void DropPortal(string WorldID, string InstanceID, int players, Vector3 vector3, Quaternion quaternion)
        {
            GameObject gameObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", vector3, quaternion);
            var world = WorldID;
            var instance = InstanceID;
            var count = players;
            Networking.RPC(RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new[]
            {
                (Il2CppSystem.String)world,
                (Il2CppSystem.String)instance,
                new Il2CppSystem.Int32
                {
                    m_value = count
                }.BoxIl2CppObject()
            });
            //MelonCoroutines.Start(MiscUtility.DestroyDelayed(1f, gameObject.GetComponent<PortalInternal>()));
        }

        internal static void DropPortal(string WorldID, VRCPlayer target)
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
        }

        internal static void DropCrashPortal(VRCPlayer Target)
        {
            GameObject portal = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", Target.transform.position + Target.transform.forward * 1.505f, Target.transform.rotation);
            string world = "wrld_5b89c79e-c340-4510-be1b-476e9fcdedcc";
            string instance = "\n[BLAZESCLIENT]\n" + Target._player.field_Private_APIUser_0.displayName + "\0";
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
        }

        internal static void SelfHide(bool state)
        {
            APIStuff.GetAvatarPreviewBase().SetActive(!state);
            PlayerUtils.CurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(!state);
            AssetBundleDownloadManager.prop_AssetBundleDownloadManager_0.gameObject.SetActive(!state);
        }

        internal static void IncreaseGain(int amount)
        {
            try
            {
                USpeakerUtils.SetGain(USpeakerUtils.GetGain() + amount);
            }
            catch {}
        }

        internal static void DecreaseGain(int amount)
        {
            try
            {
                USpeakerUtils.SetGain(USpeakerUtils.GetGain() - amount);
            }
            catch {}
        }

        internal static void IncreaseBitRate(int amount)
        {
            try
            {
                //if (USpeakerUtils.GetBitRate() != BitRate.BitRate_512k)
                //   USpeakerUtils.SetBitrate((BitRate)Convert.ChangeType(USpeakerUtils.GetBitRate(), USpeakerUtils.GetBitRate().GetTypeCode()) + amount);

                if (USpeakerUtils.GetBitRate().Next(amount) > BitRate.BitRate_24K) return;
                USpeakerUtils.SetBitrate(USpeakerUtils.GetBitRate().Next(amount));
            }
            catch {}
        }

        internal static void DecreaseBitRate(int amount)
        {
            try
            {
                //if (USpeakerUtils.GetBitRate() != BitRate.BitRate_8K)
                //    USpeakerUtils.SetBitrate((BitRate)Convert.ChangeType(USpeakerUtils.GetBitRate(), USpeakerUtils.GetBitRate().GetTypeCode()) - amount);
                USpeakerUtils.SetBitrate(USpeakerUtils.GetBitRate().Previous(amount));
            }
            catch {}
        }

        internal static IEnumerator ProcessFriendsImport()
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
    }
}
