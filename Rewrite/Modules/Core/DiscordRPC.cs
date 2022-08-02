using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VRC.Core;
using static Blaze.Utils.Discord.DiscordRichPresence;
using static Blaze.Utils.Objects.VRChatObjects;

namespace Blaze.Modules
{
    public class DiscordRPC : BModule
    {
        private static RichPresence presence;
        private static EventHandlers eventHandlers;
        private static bool IsStarted = false;

        private static readonly string DetailsString = "Cope harder fat bitch";
        private static readonly string LargeImageText = "Blaze's Client by WTFBlaze";
        private static readonly string SmallImageKey = "logo_transparent";
        private static readonly string SmallImageText = "You look like you pick your toenails fat bitch.";
        private static readonly string PartyID = Guid.NewGuid().ToString();

        public override void Start()
        {
            new System.Threading.Thread(async () =>
            {
                if (!File.Exists(ModFiles.DiscordRPCFile))
                    await DownloadDiscordDLL();
                else
                {
                    if (FileManager.ReadAllBytesOfFile(ModFiles.DiscordRPCFile).Length <= 0)
                        await DownloadDiscordDLL();
                }
                try
                {
                    var vrcConfigLocation = Directory.GetParent(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\VRChat\\Config.json";
                    if (File.Exists(vrcConfigLocation))
                    {
                        var vrcConfig = JsonConvert.DeserializeObject<VRCConfig>(File.ReadAllText(vrcConfigLocation));
                        if (vrcConfig.disableRichPresence == false)
                        {
                            vrcConfig.disableRichPresence = true;
                            File.WriteAllText(vrcConfigLocation, JsonConvert.SerializeObject(vrcConfig, Formatting.Indented));
                        }
                    }
                    else
                    {
                        File.WriteAllText(vrcConfigLocation, JsonConvert.SerializeObject(new VRCConfig
                        {
                            disableRichPresence = true
                        }, Formatting.Indented));
                    }
                }
                catch { }

                Logs.Log("[DiscordRPC] Started Rich Presence!", ConsoleColor.Green);
                eventHandlers = default;
                presence = new RichPresence
                {
                    details = DetailsString,
                    state = "Loading Client...",
                    largeImageKey = "logo",
                    smallImageKey = SmallImageKey,
                    largeImageText = LargeImageText,
                    smallImageText = SmallImageText,
                    partyId = PartyID,
                    partySize = 0,
                    partyMax = 0,
                    startTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                };
                ToggleRPC();
                Timer timer = new(15000.0);
                timer.Elapsed += Update;
                timer.AutoReset = true;
                timer.Enabled = true;
            }).Start();
        }

        public override void UI()
        {
            new QMToggleButton(BlazeQM.Settings, 1, 1, "DiscordRPC", delegate
            {
                Config.Main.UseDiscordRPC = true;
                ToggleRPC();
            }, delegate
            {
                Config.Main.UseDiscordRPC = false;
                ToggleRPC();
            }, "Toggles showing Blaze's Client as your game in Discord", Config.Main.UseDiscordRPC);
        }

        public static void ToggleRPC()
        {
            if (!IsStarted)
            {
                Initialize("894782505473949737", ref eventHandlers, true, "438100");
                IsStarted = true;
            }
            if (Config.Main.UseDiscordRPC)
            {
                UpdatePresence(presence);
            }
            else
            {
                Shutdown();
                IsStarted = false;
            }
        }

        public static void Update(object sender, ElapsedEventArgs args)
        {
            if (Config.Main.UseDiscordRPC)
            {
                if (APIUser.CurrentUser == null)
                {
                    eventHandlers = default;
                    presence.largeImageKey = "instance_changing";
                    presence.partySize = 0;
                    presence.partyMax = 0;
                    UpdatePresence(presence);
                    return;
                }
                var room = WorldUtils.CurrentInstance();
                if (room != null)
                {
                    presence.partySize = WorldUtils.GetPlayerCount();
                    presence.partyMax = WorldUtils.CurrentWorld().capacity;
                    switch (room.type)
                    {
                        default:
                            presence.partyMax = 0;
                            presence.partySize = 0;
                            presence.state = "Switching Rooms...";
                            presence.largeImageKey = "instance_changing";
                            break;

                        case InstanceAccessType.Public:
                            presence.state = "[Public] " + room.world.name;
                            presence.largeImageKey = "instance_public";
                            break;

                        case InstanceAccessType.FriendsOfGuests:
                            presence.state = "[Friends+] " + room.world.name;
                            presence.largeImageKey = "instance_friends_plus";
                            break;

                        case InstanceAccessType.FriendsOnly:
                            presence.state = "[Friends] " + room.world.name;
                            presence.largeImageKey = "instance_friends";
                            break;

                        case InstanceAccessType.InvitePlus:
                            presence.state = "[Invite+] " + room.world.name;
                            presence.largeImageKey = "instance_invite_plus";
                            break;

                        case InstanceAccessType.InviteOnly:
                            presence.state = "[Private] " + room.world.name;
                            presence.largeImageKey = "instance_invite";
                            break;
                    }
                }
                else
                {
                    presence.partyMax = 0;
                    presence.partySize = 0;
                    presence.state = "Switching Rooms...";
                    presence.largeImageKey = "instance_changing";
                }
                presence.partyId = PartyID;
                UpdatePresence(presence);
            }
        }

        public static async Task DownloadDiscordDLL()
        {
            var webclient = new HttpClient();
            var bytes = await webclient.GetByteArrayAsync("https://cdn.wtfblaze.com/downloads/discordrpc.dll");
            webclient.Dispose();
            if (bytes.Length > 0)
            {
                FileManager.WriteAllBytesToFile(ModFiles.DiscordRPCFile, bytes);
                Logs.Log("[DiscordRPC] Successfully downloaded dependency file!", ConsoleColor.Green);
            }
            else
            {
                Logs.Error("Problem downloading Discord-rpc.dll | Contact Blaze");
                return;
            }
        }
    }
}
