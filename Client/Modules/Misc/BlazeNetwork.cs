using Blaze.Utils;
using Blaze.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using static Blaze.Utils.Objects.ModObjects;
using static Blaze.Utils.Objects.NetworkObjects;

namespace Blaze.Modules
{
    class BlazeNetwork : BModule
    {
        internal static WebSocket ws;
        internal static bool IsConnected;
        internal static bool IsReconnecting;
        private static List<ModPayload> PayloadQueue = new();

        public override void Start()
        {
            //Logs.Log($"wss://ws.wtfblaze.com/v3/connect/{Main.authKey}/{Main.userHash}");
            ws = new WebSocket($"wss://ws.wtfblaze.com/v3/connect/{Main.authKey}/{Main.userHash}");
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            ws.Log.Output = (_, __) => { };
            ws.ConnectAsync();
        }

        public override void LocalPlayerLoaded()
        {
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
            }
        }

        private void HandleQueue(ModPayload payload)
        {
            switch (payload.type)
            {
                case PayloadType.FetchedTags:
                    //var FTInfo = (FetchTagsResults)payload.content;
                    //Nameplates.Tags = FTInfo.tags;
                    //MelonCoroutines.Start(Nameplates.DelayedRefresh());
                    break;

                case PayloadType.SearchResult:
                    AvatarSearch.ProcessAvatars((AviSearchResults)payload.content);
                    break;

                case PayloadType.AvatarResponse:
                    break;

                case PayloadType.FoundBlazeUsers:
                    //var BUInfo = (FindUsersResults)payload.content;
                    //Nameplates.BlazeUsers = BUInfo.Users;
                    //Nameplates.Refresh();
                    break;

                case PayloadType.ReceivedUserInfo:
                    BlazeInfo.CurrentUser = (ModUser)payload.content;
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
            if (BlazeInfo.CurrentUser == null)
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

        class ModPayload
        {
            public PayloadType type { get; set; }
            public object content { get; set; }
        }
    }
}
