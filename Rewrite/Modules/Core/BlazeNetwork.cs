using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using BlazeSocket;
using static Blaze.Utils.Objects.ModObjects;
using static Blaze.Utils.Objects.NetworkObjects;

namespace Blaze.Modules
{
    public class BlazeNetwork : BModule
    {
        public static WebSocket ws;
        public static bool IsConnected;
        public static bool IsReconnecting;
        public static bool AllowReconnect = true;
        public static List<ModPayload> PayloadQueue = new();

        public override void Start()
        {
            ws = new WebSocket($"wss://ws.wtfblaze.com/v3/connect/{Main.authKey}/{Main.userHash}");
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            ws.Log.Output = (_, __) => { };
            ws.ConnectAsync();
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
            {
                ws.SendAsync(JsonConvert.SerializeObject(new
                {
                    payload = new
                    {
                        type = "FetchTags",
                    }
                }), null);
            }
        }

        public override void LocalPlayerLoaded()
        {
            // Submit Current World Info to the API
            ws.SendAsync(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    type = "VRChatAPIInfo",
                    data = new
                    {
                        world_id = WorldUtils.GetJoinID(),
                        user_id = PlayerUtils.CurrentUser().GetUserID()
                    }
                }
            }), null);

            /*// Retrieve other users in this world
            ws.SendAsync(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    type = "FindBlazeUser",
                    data = new
                    {
                        world_id = WorldUtils.GetJoinID()
                    }
                }
            }), null);*/
        }

        public override void PlayerJoined(Player player)
        {
            ws.SendAsync(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    type = "FindBlazeUser",
                    data = new
                    {
                        world_id = WorldUtils.GetJoinID()
                    }
                }
            }), null);
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            var result = JsonConvert.DeserializeObject<PayloadPart1>(e.Data);
            switch (result.payload.type)
            {
                case PayloadType.FetchedTags:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.FetchedTags,
                        content = JsonConvert.DeserializeObject<FetchTagsResults>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.SearchResult:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.SearchResult,
                        content = JsonConvert.DeserializeObject<AviSearchResults>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.AvatarResponse:
                    break;

                case PayloadType.FoundBlazeUsers:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.FoundBlazeUsers,
                        content = JsonConvert.DeserializeObject<FindUsersResults>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.ReceivedUserInfo:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.ReceivedUserInfo,
                        content = JsonConvert.DeserializeObject<ModUser>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.MessageAll:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.MessageAll,
                        content = JsonConvert.DeserializeObject<MessageAllResults>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserOnline:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserOnline,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserOffline:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserOffline,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserAttemptUnAuthorised:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserAttemptUnAuthorised,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserAttemptIncorrectHash:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserAttemptIncorrectHash,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserAttemptDuplicate:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserAttemptDuplicate,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.OnUserAttemptBanned:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.OnUserAttemptBanned,
                        content = JsonConvert.DeserializeObject<OnUserState>(result.payload.data.ToString())
                    });
                    break;

                case PayloadType.SendOnline:
                    PayloadQueue.Add(new ModPayload
                    {
                        type = PayloadType.SendOnline,
                        content = JsonConvert.DeserializeObject<SendOnlineResults>(result.payload.data.ToString())
                    });
                    break;
            }
        }

        private void HandleQueue(ModPayload payload)
        {
            switch (payload.type)
            {
                case PayloadType.FetchedTags:
                    var FTInfo = (FetchTagsResults)payload.content;
                    Main.Tags = FTInfo.tags;
                    MelonCoroutines.Start(Nameplates.DelayedRefresh());
                    break;

                case PayloadType.SearchResult:
                    AvatarSearch.ProcessAvatars((AviSearchResults)payload.content);
                    break;

                case PayloadType.AvatarResponse: // TODO: Label showing how many avatars the user has helped contribute this session
                    break;

                case PayloadType.FoundBlazeUsers:
                    var BUInfo = (FindUsersResults)payload.content;
                    Main.OtherUsers = BUInfo.Users;
                    Nameplates.Refresh();
                    break;

                case PayloadType.ReceivedUserInfo:
                    Main.CurrentUser = (ModUser)payload.content;
                    break;

                case PayloadType.MessageAll:
                    var MAInfo = (MessageAllResults)payload.content;
                    switch (MAInfo.message_type)
                    {
                        case "HUD":
                            Logs.Log($"[ALERT] [{MAInfo.devname}] {MAInfo.message}", ConsoleColor.Red);
                            Logs.RawHUD($"[ALERT] <color=red>[{MAInfo.devname}]</color> {MAInfo.message}", 6);
                            break;
                    }
                    break;

                case PayloadType.OnUserOnline:
                    var OUOnInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUOnInfo.name} has logged in!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUOnInfo.name}</color> has <color=green>logged in</color>!");
                    
                    break;

                case PayloadType.OnUserOffline:
                    var OUOffInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUOffInfo.name} has logged out!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUOffInfo.name}</color> has <color=red>logged out</color>!");
                    break;

                case PayloadType.OnUserAttemptUnAuthorised:
                    var OUAUAInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUAUAInfo.name} Unauthorized login!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUAUAInfo.name}</color> <color=yellow>Unauthorized login</color>!");
                    break;

                case PayloadType.OnUserAttemptIncorrectHash:
                    var OUAIHInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUAIHInfo.name} logged in with an incorrect hash!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUAIHInfo.name}</color> logged in with an <color=yellow>incorrect hash</color>!");
                    break;

                case PayloadType.OnUserAttemptDuplicate:
                    var OUADInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUADInfo.name} attempted Double Connecting!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUADInfo.name}</color> attempted <color=yellow>Double Connecting</color>!");
                    break;

                case PayloadType.OnUserAttemptBanned:
                    var OUABInfo = (OnUserState)payload.content;
                    Logs.Log($"[API] {OUABInfo.name} logged in but is banned!");
                    Logs.Debug($"<color={Colors.OrangeHex}>[API]</color> <color={Colors.ModUserHex}>{OUABInfo.name}</color> <color=red>logged in but is banned</color>!");
                    break;

                case PayloadType.SendOnline:
                    /*var SOInfo = (SendOnlineResults)payload.content;
                    foreach (var p in SOInfo.data)
                    {
                        BlazeStaff.OnlineScroll.Add(new QMSingleButton(BlazeStaff.OnlineScroll.BaseMenu, 0, 0, $"<color={(p.Value.AccessType == "Developer" ? "red" : "white")}>{p.Value.DiscordName}</color>", delegate { }, "Blaze's Client User"));
                    }
                    BlazeStaff.OnlineScroll.Refresh2();*/
                    break;

                default:
                    _ = "Nigga Balls lol";
                    break;
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Logs.Log("[NETWORK] You have been connected to Blaze's Network!", ConsoleColor.Green);
            IsConnected = true;
            IsReconnecting = false;
            MelonCoroutines.Start(ProcessPayloads());
            if (Main.CurrentUser == null)
            {
                ws.SendAsync(JsonConvert.SerializeObject(new
                {
                    payload = new
                    {
                        type = "FetchUserInfo"
                    }
                }), null);
            }
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            if (e.Code == 1002 && IsReconnecting) return;
            if (e.Code == 1008)
            {
                Logs.Log("[NETWORK] You have been disconnected from Blaze's Network! There was an error with authorization and you have been disconnected.", ConsoleColor.Red);
            }
            else
            {
                Logs.Log("[NETWORK] You have been disconnected from Blaze's Network! Attempting to reconnect...", ConsoleColor.Red);
                IsConnected = false;

                MelonCoroutines.Start(ReconnectLoop());
            }
        }

        private IEnumerator ProcessPayloads()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(2.5f);
                if (PayloadQueue.Count != 0)
                {
                    try
                    {
                        if (PayloadQueue[0] != null)
                        {
                            HandleQueue(PayloadQueue[0]);
                            PayloadQueue.RemoveAt(0);
                        }
                    }
                    catch { }
                }
            }
        }

        private static IEnumerator ReconnectLoop()
        {
            if (!AllowReconnect) yield break;
            int retryCount = 0;
            IsReconnecting = true;
            while (!IsConnected)
            {
                if (retryCount >= 15)
                {
                    Logs.Log("[NETWORK] After 15 attempts at reconnecting the reconnect process has been stopped. If you would like to access Blaze Network Features such as Avi Search please restart your game.", ConsoleColor.Red);
                }
                else
                {
                    try
                    {
                        ws.Connect();
                    }
                    catch { }
                    yield return new WaitForSecondsRealtime(5);
                }
            }
        }

        public class ModPayload
        {
            public PayloadType type { get; set; }
            public object content { get; set; }
        }
    }
}
