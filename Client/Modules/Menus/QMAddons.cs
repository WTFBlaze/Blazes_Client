using Blaze.API;
using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Attributes;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.UI.Elements;

namespace Blaze.Modules
{
    class QMAddons : BModule
    {
        internal static QMInfo DebugPanel;
        internal static QMInfo playerList;
        internal static Text hudLog;
        private static bool WelcomeNoticeDone;
        public static GameObject Panel;
        public static TextMeshProUGUI ClockText;
        public static GameObject HudFPSObj;
        public static Text HudFPSTxt;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesQuickMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<BlazesSocialMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<BlazesHUDFPS>();
        }

        public override void QuickMenuUI()
        {
            var QMListener = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/MicButton").AddComponent<EnableDisableListener>();
            QMListener.OnEnabled += () =>
            {
                BlazeInfo.QMIsOpened = true;
            };
            QMListener.OnDisabled += () =>
            {
                BlazeInfo.QMIsOpened = false;
            };

            var SMListener = GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Background").AddComponent<EnableDisableListener>();
            SMListener.OnEnabled += () =>
            {
                BlazeInfo.SMIsOpened = true;
                if (Config.Main.DesktopPlayerList && DesktopPlayerList.BackgroundObject != null)
                {
                    DesktopPlayerList.BackgroundObject.SetActive(false);
                }
                if (BlazeMenu.HeadFlipper != null)
                {
                    BlazeMenu.HeadFlipper.SetToggleState(false);
                }
            };
            SMListener.OnDisabled += () =>
            {
                BlazeInfo.SMIsOpened = false;
                if (Config.Main.DesktopPlayerList && DesktopPlayerList.BackgroundObject != null)
                {
                    DesktopPlayerList.BackgroundObject.SetActive(true);
                }
            };

            var RAWListener = GameObject.Find("UserInterface/ActionMenu/Container/MenuR/ActionMenu").AddComponent<EnableDisableListener>();
            RAWListener.OnEnabled += () =>
            {
                BlazeInfo.AWIsOpened = true;
            };
            RAWListener.OnDisabled += () =>
            {
                BlazeInfo.AWIsOpened = false;
            };

            var LAWListener = GameObject.Find("UserInterface/ActionMenu/Container/MenuL/ActionMenu").AddComponent<EnableDisableListener>();
            LAWListener.OnEnabled += () =>
            {
                BlazeInfo.AWIsOpened = true;
            };
            LAWListener.OnDisabled += () =>
            {
                BlazeInfo.AWIsOpened = false;
            };

            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)").AddComponent<BlazesQuickMenu>();
            GameObject.Find("UserInterface/MenuContent/Screens/Social").AddComponent<BlazesSocialMenu>();

            #region Debug Panel
            DebugPanel = new QMInfo(APIStuff.Right.WingOpen, 600, 0, 1000, 1000, "")
            {
                InfoText =
                {
                    color = Color.white,
                    supportRichText = true,
                    fontSize = 35,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperLeft
                },
                InfoBackground =
                {
                    color = new Color(0, 0, 0, 0.85f)
                }
            };
            Logs.Debug($"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color> Debugger is now active! <color=red><3</color>");
            DebugPanel.SetActive(Config.Main.DebugPanel);
            #endregion

            #region Player List
            playerList = new QMInfo(APIStuff.Left.WingOpen, -450, 0, 700, 1200, "")
            {
                InfoText =
                {
                    color = Color.white,
                    supportRichText = true,
                    fontSize = 31,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperRight
                },
                InfoBackground =
                {
                    color = new Color(0, 0, 0, 0.85f)
                }
            };
            playerList.SetActive(Config.Main.PlayerList);
            #endregion

            #region Local Clock
            var obj = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel");
            Panel = UnityEngine.Object.Instantiate(obj, obj.transform.parent, false);
            Panel.name = "ApolloClockPanel";
            Panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(614, 0);
            ClockText = Panel.transform.Find("Panel/Text_FPS").GetComponent<TextMeshProUGUI>();
            ClockText.color = Color.white;
            Panel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().sizeDelta = new Vector2(350, 0);
            Panel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().anchoredPosition = new Vector2(-165, 0);
            Panel.transform.Find("Panel/Text_FPS").name = "Text Label";
            UnityEngine.Object.Destroy(Panel.transform.Find("Panel/Text_Ping"));
            UnityEngine.Object.Destroy(Panel.GetComponent<DebugInfoPanel>());
            Panel.transform.SetSiblingIndex(1);
            Panel.SetActive(Config.Main.IRLClock);
            #endregion
        }

        public static void InitializeHUD()
        {
            #region IRL Clock
            var gameObject = new GameObject("Blaze's Hud Logs");
            hudLog = gameObject.AddComponent<Text>();
            gameObject.transform.SetParent(GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud").transform, false);
            gameObject.transform.localPosition = new Vector3(85, -350);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(850, 40);
            hudLog.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            hudLog.horizontalOverflow = HorizontalWrapMode.Wrap;
            hudLog.verticalOverflow = VerticalWrapMode.Overflow;
            hudLog.alignment = TextAnchor.LowerRight;
            hudLog.fontStyle = FontStyle.Bold;
            hudLog.supportRichText = true;
            hudLog.fontSize = 25;
            #endregion

            #region FPS
            HudFPSObj = new GameObject("Blaze's Hud FPS");
            HudFPSTxt = HudFPSObj.AddComponent<Text>();
            HudFPSObj.transform.SetParent(GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent").transform, false);
            HudFPSObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 85);
            HudFPSTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            HudFPSTxt.horizontalOverflow = HorizontalWrapMode.Wrap;
            HudFPSTxt.verticalOverflow = VerticalWrapMode.Overflow;
            HudFPSTxt.alignment = TextAnchor.MiddleCenter;
            HudFPSTxt.supportRichText = true;
            HudFPSTxt.fontStyle = FontStyle.Bold;
            HudFPSTxt.fontSize = 30;
            HudFPSObj.AddComponent<BlazesHUDFPS>();
            #endregion
        }

        public override void LocalPlayerLoaded()
        {
            if (!WelcomeNoticeDone)
            {
                Logs.RawHUD($"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color>\nby <color=red>WTFBlaze</color>", 3.5f);
                WelcomeNoticeDone = true;
            }
            Logs.HUD($"Welcome to <color=#e1add3>{WorldUtils.CurrentWorld().name}</color>!", 3f);
        }
    }

    public class BlazesQuickMenu : MonoBehaviour
    {
        public BlazesQuickMenu(IntPtr id) : base(id) { }
        private float playerListTimer = 1f;
        private string Month;
        private string Day;

        public void Awake()
        {
            if (Config.Main.PlayerList)
            {
                playerListTimer = 0f;
            }
        }

        public void Start()
        {
            var dt = DateTime.Now;
            Month = GetMonth(dt);
            Day = GetDay(dt);
        }

        public void Update()
        {
            try
            {
                if (Config.Main.PlayerList)
                {
                    playerListTimer -= Time.deltaTime;
                    if (playerListTimer <= 0)
                    {
                        string finalResult = string.Empty;
                        #region Main Line
                        var playerCount = WorldUtils.GetPlayerCount() > WorldUtils.CurrentWorld().capacity ? $"<color=red>{WorldUtils.GetPlayerCount()}</color>" : $"<color=magenta>{WorldUtils.GetPlayerCount()}</color>";
                        string mainLine = $"<size=30><b>[Friends:<color=yellow>{WorldUtils.GetFriendCount()}</color>][SDK:<color=orange>{WorldUtils.GetSDKNumber()}</color>][{InstanceType()}][{playerCount}/<color=magenta>{WorldUtils.CurrentWorld().capacity}</color>]</b></size>\n";
                        string players = string.Empty;
                        foreach (var p in PhotonUtils.GetAllPhotonPlayers())
                        {
                            string currentPlayer = string.Empty;
                            if (p.GetPlayer() == null)
                            {
                                currentPlayer = $"<color=red>INVIS</color> - <b>{p.GetDisplayName()}</b>";
                            }
                            else
                            {
                                var pl = p.GetPlayer();
                                BlazeInfo.CachedPlayers.TryGetValue(pl.GetUserID(), out BlazesPlayerInfo comp);

                                // Process Red Tags
                                bool blocked = pl.IsBlockedBy();
                                bool muted = pl.IsMutedBy();
                                bool target = pl.IsTarget();
                                bool kos = pl.IsKOS();
                                if (blocked || muted || target || kos)
                                {
                                    if (blocked) currentPlayer += "<color=red>B</color>";
                                    if (muted) currentPlayer += "<color=red>M</color>";
                                    if (target) currentPlayer += "<color=red>T</color>";
                                    if (kos) currentPlayer += "<color=red>K</color>";
                                    currentPlayer += " - ";
                                }

                                // Process Username
                                currentPlayer += $"<b><color={pl.GetAPIUser().GetTrueRankColor()}>{pl.GetDisplayName()}</color></b> <color=grey>|</color> ";

                                // Process Platform
                                currentPlayer += $"{comp.GetPlatformColored()} <color=grey>|</color> ";

                                // Process FPS / Crash / Ghosting State

                                if (comp.isGhosting && pl != PlayerUtils.CurrentUser()._player)
                                {
                                    currentPlayer += "<b><color=red>CRASHED</color></b> <color=grey>|</color> ";
                                }
                                else if (comp.isCrashed && pl != PlayerUtils.CurrentUser()._player)
                                {
                                    currentPlayer += "<b><color=orange>GHOSTING</color></b> <color=grey>|</color> ";
                                }
                                else
                                {
                                    currentPlayer += $"P:{comp.GetPingColored()} F:{comp.GetFramesColored()} <color=grey>|</color> ";
                                }

                                // Process Avi Release Status
                                if (pl.prop_ApiAvatar_0.releaseStatus == "private")
                                {
                                    currentPlayer += "<color=red>AVI</color> <color=grey>|</color> ";
                                }
                                else
                                {
                                    currentPlayer += "<color=lime>AVI</color> <color=grey>|</color> ";
                                }

                                // Process Master
                                if (pl.IsMaster())
                                {
                                    currentPlayer += "[<color=orange>M</color>]";
                                }

                                // Process Friend
                                if (pl.IsFriend())
                                {
                                    currentPlayer += "[<color=yellow>F</color>]";
                                }

                                // Process Hearing Range
                                float Distance = Vector3.Distance(PlayerUtils.CurrentUser().transform.position, pl.transform.position);
                                if (Distance < 26.5f)
                                {
                                    currentPlayer += "[<color=green>H</color>]";
                                }
                            }
                            players += currentPlayer + "\n";
                        }
                        #endregion
                        finalResult += mainLine;
                        finalResult += players;
                        QMAddons.playerList.SetText(finalResult);
                        playerListTimer = 1f;
                    }
                }
                if (Config.Main.IRLClock)
                {
                    QMAddons.ClockText.text = $"<b>{Month} <color=white>{Day} :</color> <color=yellow>{DateTime.Now:h:mm:ss tt}</color></b>";
                }
            }
            catch {}
        }

        #region Clock Functions
        [HideFromIl2Cpp]
        private static string GetMonth(DateTime dt)
        {
            var result = dt.Month switch
            {
                1 => "<color=#74c2cc>Jan</color>",
                2 => "<color=#f9ff59>Feb</color>",
                3 => "<color=#72ff59>Mar</color>",
                4 => "<color=#e01baf>Apr</color>",
                5 => "<color=#474747>May</color>",
                6 => "<color=#0034e0>Jun</color>",
                7 => "<color=#121212>Jul</color>",
                8 => "<color=#e00000>Aug</color>",
                9 => "<color=#3e00c4>Sep</color>",
                10 => "<color=#e65e09>Oct</color>",
                11 => "<color=#24bfd4>Nov</color>",
                12 => "<color=#004a09>Dec</color>",
                _ => string.Empty
            };
            return result;
        }

        [HideFromIl2Cpp]
        private static string GetDay(DateTime dt)
        {
            var number = dt.Day.ToString();
            if (number.EndsWith("11")) return number + "th";
            if (number.EndsWith("12")) return number + "th";
            if (number.EndsWith("13")) return number + "th";
            if (number.EndsWith("1")) return number + "st";
            if (number.EndsWith("2")) return number + "nd";
            if (number.EndsWith("3")) return number + "rd";
            return number + "th";
        }
        #endregion

        #region Player List Functions
        [HideFromIl2Cpp]
        private string InstanceType()
        {
            return WorldUtils.CurrentInstance().type switch
            {
                InstanceAccessType.Public => "<color=green>Public</color>",
                InstanceAccessType.FriendsOfGuests => "<color=yellow>Friends+</color>",
                InstanceAccessType.FriendsOnly => "<color=yellow>Friends</color>",
                InstanceAccessType.InvitePlus => "<color=orange>Invite+</color>",
                InstanceAccessType.InviteOnly => "<color=red>Invite</color>",
                _ => "<color=green>Public</color>",
            };
        }
        #endregion
    }

    public class BlazesSocialMenu : MonoBehaviour
    {
        public BlazesSocialMenu(IntPtr id) : base(id) { }
        private GameObject onlineFriendsObj;

        public void Start()
        {
            onlineFriendsObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends/Button/TitleText");
            onlineFriendsObj.GetComponent<Text>().supportRichText = true;
        }

        public void Update()
        {
            try
            {
                // Online Friends Label
                var onlineCount = 0;
                foreach (var p in GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends").GetComponent<UiUserList>().pickers)
                {
                    if (p.gameObject.active)
                    {
                        onlineCount++;
                    }
                }
                onlineFriendsObj.GetComponent<Text>().text = $"<color=yellow>Online Friends</color> <color=white>[</color><color={BlazeInfo.ModColor1}>{onlineCount}</color><color=white>/</color><color={BlazeInfo.ModColor2}>{PlayerUtils.CurrentUser().GetAPIUser().friendIDs.Count}</color><color=white>]</color>";
            }
            catch {}
        }
    }

    public class BlazesHUDFPS : MonoBehaviour
    {
        public BlazesHUDFPS(IntPtr id) : base(id) { }

        public void Update()
        {
            try
            {
                if (WorldUtils.IsInRoom())
                {
                    BlazeInfo.CachedPlayers.TryGetValue(PlayerUtils.CurrentUser().GetUserID(), out BlazesPlayerInfo comp);
                    QMAddons.HudFPSTxt.text = comp.GetFramesColored();
                }
                else
                {
                    QMAddons.HudFPSTxt.tag = string.Empty;
                }
            }
            catch { }
        }
    }
}
