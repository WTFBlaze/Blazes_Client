using Blaze.API;
using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.Attributes;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using Newtonsoft.Json;
using RealisticEyeMovements;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using VRC.Core;
using VRC.DataModel;
using VRC.UI.Elements;

namespace Blaze.Modules
{
    public class BlazeQM : BModule
    {
        public static QMNestedButton MainMenu;
        public static QMTabButton TabButton;
        public static QMInfo DebugPanel;
        public static QMInfo playerList;
        public static Text hudLog;
        public static GameObject ClockPanel;
        public static TextMeshProUGUI ClockText;
        public static GameObject HudFPSObj;
        public static Text HudFPSTxt;
        public static QMNestedButton Worlds;
        public static QMNestedButton Worlds2;
        public static QMNestedButton Self;
        public static QMNestedButton Movement;
        public static QMNestedButton Exploits;
        public static QMNestedButton Orbits;
        public static QMToggleButton Serialization;
        private static GameObject SerializeClone;
        public static QMToggleButton HeadFlipper;
        public static NeckRange SavedNeckRange;
        public static QMNestedButton Mic;
        public static QMSingleButton GainLabel;
        public static QMSingleButton BitRateLabel;
        public static QMNestedButton Security;
        public static QMNestedButton Spoofs;
        public static QMNestedButton Renders;
        public static QMNestedButton WorldSpecific;
        public static QMNestedButton Settings;
        public static QMNestedButton LogSettings;
        public static QMNestedButton ClientUI;
        public static QMNestedButton Nameplates;
        public static QMSingleButton NameplatesColor;
        public static QMNestedButton Selected;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<EnableDisableListener>();
            ComponentManager.RegisterIl2Cpp<BlazeHudFPS>();
            ComponentManager.RegisterIl2Cpp<BlazeVRPlayerList>();
            ComponentManager.RegisterIl2Cpp<BlazeIRLClock>();
        }

        public static void InitiliazeHud()
        {
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
        }

        public override void UI()
        {
            #region FPS Changer
            if (XRDevice.isPresent)
            {
                UnityEngine.Application.targetFrameRate = 350;
            }
            else
            {
                UnityEngine.Application.targetFrameRate = 120;
            }
            #endregion

            #region Hud FPS
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
            HudFPSObj.SetActive(Config.Main.HudFPS);
            HudFPSObj.AddComponent<BlazeHudFPS>();
            #endregion

            #region Default VRCUI Edits
            UnityEngine.Object.Destroy(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/ThankYouCharacter"));

            if (Config.Main.DisableCarousel)
            {
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").SetActive(false);
            }

            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().enabled = true;
            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().verticalScrollbar = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Scrollbar").GetComponent<Scrollbar>();
            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;

            var pasteBtn = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight"), GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight").transform.parent, false); ;
            pasteBtn.name = "Blaze's Paste Button";
            pasteBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(330, -275.8f);
            pasteBtn.GetComponent<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new Action(() => 
            {
                GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField").GetComponent<InputField>().text += Clipboard.GetText();
            }));
            pasteBtn.transform.Find("Text").GetComponent<Text>().text = "Paste";

            var dropPortalBtn = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_QM_MoreActions/MenuPanel/ScrollRect/Viewport/VerticalLayoutGroup/Button_ViewUserDetails"), GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_QM_MoreActions/MenuPanel/ScrollRect/Viewport/VerticalLayoutGroup").transform, false);
            dropPortalBtn.name = $"WTFBlaze-Portal-Drop-Btn";
            dropPortalBtn.transform.Find("Container/Text_QM_H3").GetComponent<TextMeshProUGUI>().text = "Drop Portal";
            dropPortalBtn.GetComponent<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            dropPortalBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
            {
                var notif = NotificationUtils.GetNotification(APIStuff.GetNotificationsInstance().field_Private_NotificationElement_0.prop_InterfacePublicAbstractStOb1StTeBoSt1TeDaUnique_0);
                if (notif == null)
                {
                    Logs.Error("There was a problem trying to drop a portal to that instance! Could not find Notification!");
                }
                else
                {
                    if (notif.type == "invite")
                    {
                        Functions.DropPortal(Il2CppSystem.Convert.ToString(notif.details["worldId"]));
                    }
                    else
                    {
                        PopupUtils.InformationAlert("You can only drop portals from invites!");
                    }
                }
            }));
            #endregion

            #region Main UI Shit
            var mainGroupHeader = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup"));
            var mainGroup = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup"));
            mainGroupHeader.GetComponentInChildren<TextMeshProUGUI>(true).text = "Blaze's Client";
            mainGroup.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            mainGroupHeader.transform.SetSiblingIndex(8);
            mainGroup.transform.SetSiblingIndex(9);
            for (var i = mainGroup.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(mainGroup.transform.GetChild(i).gameObject);
            }
            MainMenu = new QMNestedButton("Menu_Dashboard", "", 1, 3, $"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> <color=white>| Created by:</color> <color=red>WTFBlaze</color>", "Blaze's Client");
            MainMenu.GetMainButton().SetBackgroundImage(AssetBundleManager.Logo);
            MainMenu.GetMainButton().GetGameObject().transform.SetParent(mainGroup.transform);
            TabButton = new QMTabButton(delegate
            {
                MainMenu.GetMainButton().ClickMe();
            }, $"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> <color=white>| Created by:</color> <color=red>WTFBlaze</color>", AssetBundleManager.Logo);

            if (Config.Main.UseTabButton)
            {
                mainGroupHeader.SetActive(false);
                mainGroup.SetActive(false);
            }
            else
            {
                TabButton.SetActive(false);
            }

            var forceQuit = new QMSingleButton(MainMenu, 4, 3.5f, "<color=red>Force\nQuit</color>", delegate
            {
                if (Config.Main.ConfirmQuit)
                {
                    PopupUtils.AlertV2("Are you sure you want to quit?", "Quit", Functions.ForceQuit);
                }
                else
                {
                    Functions.ForceQuit();
                }
            }, "Instantly quit VRChat", null, true);
            //forceQuit.SetBackgroundImage(AssetBundleManager.QuitIcon);

            var restart = new QMSingleButton(MainMenu, 3, 3.5f, "<color=yellow>Restart</color>", delegate
            {
                if (Config.Main.ConfirmQuit)
                {
                    PopupUtils.AlertV2("Are you sure you want to restart?", "Restart", delegate
                    {
                        Functions.Restart();
                    });
                }
                else
                {
                    Functions.Restart();
                }
            }, "Instantly quit VRChat and restart it (if in vr loads in vr)", null, true);
            //restart.SetBackgroundImage(AssetBundleManager.RestartIcon);

            new QMSingleButton(MainMenu, 2, 3.5f, "<color=green>Save\nConfig</color>", Config.SaveAll, "Save your current Blaze's Client Settings", null, true);

            new QMSingleButton(MainMenu, 1, 3.5f, $"<color={Colors.MagentaHex}>GTFO</color>", delegate 
            {
                var pos = PlayerUtils.CurrentUser().transform.position;
                pos.y = 10000;
                PlayerUtils.CurrentUser().transform.position = pos;
            }, "AYO GET THE FUCK OUTTA HERE NIGGA! RUN BITCH RUN!", null, true);
            #endregion

            #region Worlds
            Worlds = new QMNestedButton(MainMenu, "Worlds", 1, 0, "World Functions", "Worlds");

            new QMSingleButton(Worlds, 1, 0, "Copy\nRoom ID", delegate
            {
                Clipboard.SetText(WorldUtils.GetJoinID());
            }, "Copies the current world's id to your clipboard");

            new QMSingleButton(Worlds, 2, 0, "Join\nRoom ID", delegate
            {
                PopupUtils.InputPopup("Join World", "Enter World ID Here", delegate (string s)
                {
                    if (!RegexManager.IsValidWorldID(s))
                    {
                        PopupUtils.InformationAlert("That is not a valid instance id!");
                        return;
                    }
                    WorldUtils.JoinRoom(s);
                });
            }, "Join a vrchat instance by its instance id");

            new QMSingleButton(Worlds, 3, 0, "Rejoin\nWorld", delegate
            {
                WorldUtils.RejoinInstance();
            }, "Rejoins the same instance you are currently in");

            new QMToggleButton(Worlds, 4, 0, "Friends+ Home", delegate
            {
                Config.Main.FriendsPlusHome = true;
            }, delegate
            {
                Config.Main.FriendsPlusHome = false;
            }, "Change your homeworld instance type to Friends+ always", Config.Main.FriendsPlusHome);

            new QMSingleButton(Worlds, 1, 1, "Set Jump\nHeight", delegate
            {
                PopupUtils.NumericPopup("Set Jump", "Use 3 for normal jump height", delegate (string s)
                {
                    PlayerUtils.CurrentUser().GetVRCPlayerApi().SetJumpImpulse(float.Parse(s));
                });
            }, "Forcefully change your jumping power in the current world");

            new QMSingleButton(Worlds, 2, 1, "Download\nVRCW", delegate
            {
                DownloadManager.DownloadVRCW(WorldUtils.CurrentWorld());
            }, "Downloads the Current World to your VRCW's Folder");

            new QMSingleButton(Worlds, 3, 1, "<size=22>Self Delete Portals</size>", delegate
            {
                Functions.LocallyDestroyPortals();
            }, "Delete portals for only yourself", null, true);

            new QMSingleButton(Worlds, 3, 1.5f, "<size=22>Global Delete Portals</size>", delegate
            {
                Functions.GlobalDestroyPortals();
            }, "Delete portals for everyone", null, true);

            new QMSingleButton(Worlds, 4, 1, "Drop Portal\nBy ID", delegate
            {
                PopupUtils.InputPopup("Drop Portal", "Enter World ID...", delegate (string s)
                {
                    if (!RegexManager.IsValidWorldID(s))
                    {
                        PopupUtils.InformationAlert("That is not a valid World ID!");
                        return;
                    }
                    Functions.DropPortal(s);
                });
            }, "");

            new QMSingleButton(Worlds, 2, 2, "Reset\nPickups", delegate
            {
                Functions.ResetPickups();
            }, "Reset all pickups back to their default positions and values");

            #region Page 2
            Worlds2 = new QMNestedButton(Worlds, "<color=yellow>Next\nPage</color>", 4, 3, "Click to view more options", "Worlds - Page 2");

            new QMSingleButton(Worlds2, 2, 0, "Restart & Rejoin", delegate
            {
                PopupUtils.AlertV2("This will completely restart your vrchat and put you back into the same instance. Would you like to continue?", "Restart", delegate
                {
                    Functions.Restart(WorldUtils.GetJoinID());
                });
            }, "Restart's VRChat and makes you rejoin the instance you were in before restarting");
            #endregion

            #endregion

            #region Self
            Self = new QMNestedButton(MainMenu, "Self", 2, 0, "Self Functions", "Self");

            new QMToggleButton(Self, 1, 0, "Self Hide", delegate
            {
                Functions.SelfHide(true);
            }, delegate
            {
                Functions.SelfHide(false);
            }, "Locally hide your avatar so you can world crash without hitting yourself");
            #endregion

            #region Movement
            Movement = new QMNestedButton(MainMenu, "Movement", 3, 0, "Movement Functions", "Movement");

            new QMToggleButton(Movement, 1, 1, "Beyblade Mode", delegate
            {
                if (!Flight.FlightState)
                {
                    PlayerUtils.CurrentUser().transform.position += new Vector3(0f, 1.2f, 0f);
                }
                PlayerUtils.CurrentUser().transform.rotation = new Quaternion(90f, 0f, 0f, 0f);
            }, delegate
            {
                PlayerUtils.CurrentUser().transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                if (!Flight.FlightState)
                {
                    PlayerUtils.CurrentUser().transform.position += new Vector3(0f, 0.6f, 0f);
                }
            }, "BEYBLADE BEYBLADE LET IT RIP!");

            new QMSlider(Movement, -510, -620, "Desktop Flight Speed", 0.1f, 55, Config.Main.DesktopFlySpeed, delegate (float f)
            {
                Config.Main.DesktopFlySpeed = f;
            });

            new QMSlider(Movement, -510, -740, "VR Flight Speed", 0.1f, 55, Config.Main.DesktopFlySpeed, delegate (float f)
            {
                Config.Main.VRFlySpeed = f;
            });
            #endregion

            #region Exploits
            Exploits = new QMNestedButton(MainMenu, "Exploits", 4, 0, "Exploits and Other Fun Functions", "Exploits");

            #region Orbits
            Orbits = new QMNestedButton(Exploits, "Orbits", 1, 0, "Orbit Toggles & Values", "Orbits");

            new QMToggleButton(Orbits, 1, 0, "Orbit Annoyance", delegate
            {
                Config.Main.OrbitAnnoyanceMode = true;
            }, delegate
            {
                Config.Main.OrbitAnnoyanceMode = false;
            }, "Toggles orbiting around targets head instead of feet", Config.Main.OrbitAnnoyanceMode);

            new QMSlider(Orbits, -510, -400, "Player Orbit Speed", 0.1f, 2, Config.Main.PlayerOrbitSpeed, delegate (float f)
            {
                Config.Main.PlayerOrbitSpeed = f;
            });

            new QMSlider(Orbits, -510, -520, "Player Orbit Size", 0.1f, 2, Config.Main.PlayerOrbitSize, delegate (float f)
            {
                Config.Main.PlayerOrbitSize = f;
            });

            new QMSlider(Orbits, -510, -640, "Item Orbit Speed", 0.1f, 2, Config.Main.ItemOrbitSpeed, delegate (float f)
            {
                Config.Main.ItemOrbitSpeed = f;
            });

            new QMSlider(Orbits, -510, -760, "Item Orbit Size", 0.1f, 2, Config.Main.ItemOrbitSize, delegate (float f)
            {
                Config.Main.ItemOrbitSize = f;
            });
            #endregion

            new QMToggleButton(Exploits, 2, 0, "Master Lock", delegate
            {
                Patching.Toggles.MasterLockInstance = true;
            }, delegate
            {
                Patching.Toggles.MasterLockInstance = false;
            }, "Lock the instance so that nobody else can join <color=yellow>(except others with the anti for it lol)</color> <color=red>[most likely needs master idfk I don't remember]</color>");

            Serialization = new QMToggleButton(Exploits, 3, 0, "Serialization", delegate
            {
                Patching.Toggles.Serialization = true;
                var pos = PlayerUtils.CurrentUser().transform.position;
                var rot = PlayerUtils.CurrentUser().transform.rotation;
                SerializeClone = UnityEngine.Object.Instantiate(PlayerUtils.CurrentUser().prop_VRCAvatarManager_0.prop_GameObject_0, null, true);
                Animator component = SerializeClone.GetComponent<Animator>();
                if (component != null && component.isHuman)
                {
                    Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
                    if (boneTransform != null) boneTransform.localScale = Vector3.one;
                }
                SerializeClone.name = "Blaze's Client Serialization Clone";
                component.enabled = false;
                SerializeClone.GetComponent<FullBodyBipedIK>().enabled = false;
                SerializeClone.GetComponent<LimbIK>().enabled = false;
                SerializeClone.GetComponent<VRIK>().enabled = false;
                SerializeClone.GetComponent<LookTargetController>().enabled = false;
                SerializeClone.transform.position = pos;
                SerializeClone.transform.rotation = rot;
            }, delegate
            {
                Patching.Toggles.Serialization = false;
                UnityEngine.Object.Destroy(SerializeClone);
            }, "Freeze your body in place for everyone else");

            new QMToggleButton(Exploits, 4, 0, "World Triggers", delegate
            {
                Patching.Toggles.SDK2WorldTriggers = true;
            }, delegate
            {
                Patching.Toggles.SDK2WorldTriggers = false;
            }, "Force all items you interact with to be networked <color=red>(SDK2 Worlds Only)</color>");

            new QMToggleButton(Exploits, 1, 1, "God Mode", delegate
            {
                Patching.Toggles.GodMode = true;
            }, delegate
            {
                Patching.Toggles.GodMode = false;
            }, "Toggle being unable to die in gamemodes <color=red><b><i>(if it doesn't work in a specific world let Blaze know in the discord server)</i></b></color>");

            HeadFlipper = new QMToggleButton(Exploits, 2, 1, "Head Flipper", delegate
            {
                SavedNeckRange = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0;
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = new NeckRange(float.MinValue, float.MaxValue, 0f);
            }, delegate
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = SavedNeckRange;
            }, "Allow your head to freely rotate");

            new QMToggleButton(Exploits, 3, 1, "Parrot Mode", delegate
            {
                Patching.Toggles.ParrotMode = true;
            }, delegate
            {
                Patching.Toggles.ParrotMode = false;
            }, "Mimic everything your target says through their mic");
            #endregion

            #region Mic
            Mic = new QMNestedButton(MainMenu, "Mic", 1, 1, "Microphone Functions", "Microphone");
            //Mic.GetMainButton().SetBackgroundImage(AssetBundleManager.MicIcon);

            new QMSingleButton(Mic, 1, 0.5f, "Gain <color=red><b>MAX</b></color>", delegate
            {
                USpeakerUtils.SetGain(float.MaxValue);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>MAX</b></color>");
            }, "Set your microphone gain to the maximum value", null, true);


            new QMSingleButton(Mic, 1, 1f, "Gain <color=yellow><b>++</b></color>", delegate
            {
                Functions.IncreaseGain(10);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Increase your mic gain by 10", null, true);

            new QMSingleButton(Mic, 1, 1.5f, "Gain <color=green><b>+</b></color>", delegate
            {
                Functions.IncreaseGain(1);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Increase your mic gain by 1", null, true);

            new QMSingleButton(Mic, 1, 2f, "Gain <color=green><b>-</b></color>", delegate
            {
                Functions.DecreaseGain(1);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Decrease your mic gain by 1", null, true);

            new QMSingleButton(Mic, 1, 2.5f, "Gain <color=yellow><b>--</b></color>", delegate
            {
                Functions.DecreaseGain(10);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Decrease your mic gain by 10", null, true);

            new QMSingleButton(Mic, 1, 3f, "Gain <color=red><b>MIN</b></color>", delegate
            {
                USpeakerUtils.SetGain(float.MinValue);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>MIN</b></color>");
            }, "Set your microphone gain to the minimum value", null, true);

            new QMSingleButton(Mic, 2, 3f, "Reset Gain", delegate
            {
                USpeakerUtils.SetGain(1);
                GainLabel.SetButtonText($"Gain: <color=yellow><b>1</b></color>");
            }, "Reset your mic gain back to the default value", null, true);

            GainLabel = new QMSingleButton(Mic, 2, 1.5f, "Gain: <color=yellow><b>1</b></color>", delegate { }, "Your current Microphone Gain Value");
            BitRateLabel = new QMSingleButton(Mic, 3, 1.5f, "BitRate: <color=yellow><b>20K</b></color>", delegate { }, "Your current Microphone BitRate Value");

            new QMSingleButton(Mic, 3, 3f, "Reset BitRate", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_24K);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>24K</b></color>");
            }, "Reset your bitrate back to the default value", null, true);

            new QMSingleButton(Mic, 4, 0.5f, "BitRate <color=red><b>MAX</b></color>", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_24K);
                BitRateLabel.SetButtonText("BitRate: <color=yellow><b>MAX</b></color>");
            }, "Increase your bitrate to the maximum setting", null, true);

            new QMSingleButton(Mic, 4, 1f, "BitRate <color=yellow><b>++</b></color>", delegate
            {
                Functions.IncreaseBitRate(5);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 5 settings", null, true);

            new QMSingleButton(Mic, 4, 1.5f, "BitRate <color=green><b>+</b></color>", delegate
            {
                Functions.IncreaseBitRate(1);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 1 settings", null, true);

            new QMSingleButton(Mic, 4, 2f, "BitRate <color=green><b>-</b></color>", delegate
            {
                Functions.DecreaseBitRate(1);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 1 settings", null, true);

            new QMSingleButton(Mic, 4, 2.5f, "BitRate <color=yellow><b>--</b></color>", delegate
            {
                Functions.DecreaseBitRate(5);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 5 settings", null, true);

            new QMSingleButton(Mic, 4, 3f, "BitRate <color=red><b>MIN</b></color>", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_8K);
                BitRateLabel.SetButtonText($"BitRate: <color=yellow><b>MIN</b></color>");
            }, "Increase your bitrate by 10 settings", null, true);
            #endregion

            #region Security
            Security = new QMNestedButton(MainMenu, "Security", 2, 1, "Security Functions", "Security");

            new QMToggleButton(Security, 1, 0, "Anti Instance Lock", delegate
            {
                Config.Main.AntiInstanceLock = true;
            }, delegate
            {
                Config.Main.AntiInstanceLock = false;
            }, "Prevent getting kicked to other worlds because dickbags are locking the instance from being able to be joined.", Config.Main.AntiInstanceLock);

            new QMToggleButton(Security, 2, 0, "Anti Portal", delegate
            {
                Config.Main.AntiPortal = true;
            }, delegate
            {
                Config.Main.AntiPortal = false;
            }, "Prevent entering portals entirely", Config.Main.AntiPortal);

            new QMToggleButton(Security, 3, 0, "Portal Prompt", delegate
            {
                Config.Main.PortalPrompt = true;
            }, delegate
            {
                Config.Main.PortalPrompt = false;
            }, "Toggle a confirmation popup whenever you are about to enter a portal", Config.Main.PortalPrompt);

            new QMToggleButton(Security, 4, 0, "Anti Udon", delegate
            {
                Config.Main.AntiUdon = true;
            }, delegate
            {
                Config.Main.AntiUdon = false;
            }, "Toggle preventing udon scripts from affecting you", Config.Main.AntiUdon);

            new QMToggleButton(Security, 1, 1, "Anti Teleport", delegate
            {
                Config.Main.AntiTeleport = true;
            }, delegate
            {
                Config.Main.AntiTeleport = false;
            }, "Toggle blocking being teleported <color=red>(SDK2 Worlds Only)</color> <color=yellow>[Use Anti Udon for SDK3]</color>", Config.Main.AntiTeleport);

            new QMToggleButton(Security, 2, 1, "Anti-World Triggers", delegate
            {
                Config.Main.AntiWorldTriggers = true;
            }, delegate
            {
                Config.Main.AntiWorldTriggers = false;
            }, "Toggle blocking other people from affecting you with world triggers <color=red>(SDK2 Worlds Only)</color>", Config.Main.AntiWorldTriggers);

            new QMToggleButton(Security, 3, 1, "Anti Desync", delegate
            {
                Config.Main.AntiDesync = true;
            }, delegate
            {
                Config.Main.AntiDesync = false;
            }, "Toggle forcing all malicious rpc's to be rewritten to safe values", Config.Main.AntiDesync);

            #region Moderations
            var moderations = new QMNestedButton(Security, "<size=26>Moderations</size>", 4, 1, "", "Moderations");

            new QMToggleButton(moderations, 1, 0, "Show\nModerations", delegate
            {
                Config.Main.ShowModerations = true;
            }, delegate
            {
                Config.Main.ShowModerations = false;
            }, "Show moderations against you", Config.Main.ShowModerations);

            new QMToggleButton(moderations, 2, 0, "Mutes", delegate
            {
                Config.Main.ModerationMutes = true;
            }, delegate
            {
                Config.Main.ModerationMutes = false;
            }, "Show Mutes against you", Config.Main.ModerationMutes);

            new QMToggleButton(moderations, 3, 0, "UnMutes", delegate
            {
                Config.Main.ModerationUnmutes = true;
            }, delegate
            {
                Config.Main.ModerationUnmutes = false;
            }, "Show UnMutes against you", Config.Main.ModerationUnmutes);

            new QMToggleButton(moderations, 4, 0, "Blocks", delegate
            {
                Config.Main.ModerationBlocks = true;
            }, delegate
            {
                Config.Main.ModerationBlocks = false;
            }, "Show Blocks against you", Config.Main.ModerationBlocks);

            new QMToggleButton(moderations, 1, 1, "UnBlocks", delegate
            {
                Config.Main.ModerationUnblocks = true;
            }, delegate
            {
                Config.Main.ModerationUnblocks = false;
            }, "Show UnBlocks against you", Config.Main.ModerationUnblocks);

            new QMToggleButton(moderations, 2, 1, "Warns", delegate
            {
                Config.Main.ModerationWarns = true;
            }, delegate
            {
                Config.Main.ModerationWarns = false;
            }, "Show Warns against you", Config.Main.ModerationWarns);

            new QMToggleButton(moderations, 3, 1, "Kicks", delegate
            {
                Config.Main.ModerationKicks = true;
            }, delegate
            {
                Config.Main.ModerationKicks = false;
            }, "Show Vote Kicks", Config.Main.ModerationKicks);

            new QMToggleButton(moderations, 4, 1, "Mic Offs", delegate
            {
                Config.Main.ModerationMicOff = true;
            }, delegate
            {
                Config.Main.ModerationMicOff = false;
            }, "Show when instance owners mic off you", Config.Main.ModerationMicOff);

            new QMToggleButton(moderations, 1, 2, "Anti Warns", delegate
            {
                Config.Main.ModerationAntiWarns = true;
            }, delegate
            {
                Config.Main.ModerationAntiWarns = false;
            }, "Block the annoying 'Youve been warned' alerts", Config.Main.ModerationAntiWarns);

            new QMToggleButton(moderations, 2, 2, "Anti Mic Off", delegate
            {
                Config.Main.ModerationAntiMicOff = true;
            }, delegate
            {
                Config.Main.ModerationAntiMicOff = false;
            }, "Block the instance owner from forcing your mic off", Config.Main.ModerationAntiMicOff);
            #endregion

            #endregion

            #region Spoofs
            Spoofs = new QMNestedButton(MainMenu, "Spoofs", 3, 1, "Spoofing Functions", "Spoofs");

            new QMToggleButton(Spoofs, 1, 0, "HWID", delegate
            {
                Config.Main.HWIDSpoofer = true;
            }, delegate
            {
                Config.Main.HWIDSpoofer = false;
            }, "Toggle spoofing your hwid to make bypassing bans easier incase they happen (If you login to a banned account delete the HWID.txt file)", Config.Main.HWIDSpoofer);

            new QMToggleButton(Spoofs, 2, 0, "Ping", delegate
            {
                Config.Main.PingSpoof = true;
            }, delegate
            {
                Config.Main.PingSpoof = false;
            }, "Spoof your ping as whatever numbers you want", Config.Main.PingSpoof);

            new QMSingleButton(Spoofs, 3, 0, "Set\nPing", delegate
            {
                PopupUtils.NumericPopup("Set Ping", "Enter New Ping Value...", delegate (string s)
                {
                    Config.Main.PingSpoofValue = int.Parse(s);
                });
            }, "Set the value of what ping spoof should show as");

            new QMToggleButton(Spoofs, 4, 0, "Frames", delegate
            {
                Config.Main.FramesSpoof = true;
            }, delegate
            {
                Config.Main.FramesSpoof = false;
            }, "Spoof your Frames / FPS as whatever numbers you want", Config.Main.FramesSpoof);

            new QMSingleButton(Spoofs, 1, 1, "Set\nFrames", delegate
            {
                PopupUtils.NumericPopup("Set Frames", "Enter New Frames Value...", delegate (string s)
                {
                    Config.Main.FramesSpoofValue = int.Parse(s);
                });
            }, "Set the value of what frames spoof should show as");
            #endregion

            #region Renders
            Renders = new QMNestedButton(MainMenu, "Renders", 4, 1, "Rendering Functions", "Renders");
            #endregion

            #region World Specific
            WorldSpecific = new QMNestedButton(MainMenu, "World\nSpecific", 1, 2, "World Specific Functions", "World Specific");
            #endregion

            #region Settings
            Settings = new QMNestedButton(MainMenu, "Settings", 2, 2, "Client Settings", "Settings");

            #region Log Settings
            LogSettings = new QMNestedButton(Settings, "Log\nSettings", 1, 0, "Modify what the client should log and where to", "Log Settings");

            new QMToggleButton(LogSettings, 1, 0, "Log To HUD", delegate
            {
                Config.Main.LogToHud = true;
            }, delegate
            {
                Config.Main.LogToHud = false;
            }, "Toggle showing hud messages", Config.Main.LogToHud);

            new QMToggleButton(LogSettings, 2, 0, "Friends Only On Hud", delegate
            {
                Config.Main.LogOnlyFriendsToHud = true;
            }, delegate
            {
                Config.Main.LogOnlyFriendsToHud = false;
            }, "Toggle only showing logs to your hud that are involving friends", Config.Main.LogOnlyFriendsToHud);

            new QMSingleButton(LogSettings, 4, 0, "Clear\nHUD", Logs.ClearHUD, "Clear all Blaze's Client HUD Messages incase some of the messages get stuck (unlikely to happen)");

            new QMToggleButton(LogSettings, 1, 1, "Player Joins", delegate
            {
                Config.Main.LogPlayersJoin = true;
            }, delegate
            {
                Config.Main.LogPlayersJoin = false;
            }, "Toggle logging when players join the same room", Config.Main.LogPlayersJoin);

            new QMToggleButton(LogSettings, 2, 1, "Player Leaves", delegate
            {
                Config.Main.LogPlayersLeave = true;
            }, delegate
            {
                Config.Main.LogPlayersLeave = false;
            }, "Toggle logging when players leave the same room", Config.Main.LogPlayersLeave);

            new QMToggleButton(LogSettings, 3, 1, "Photon Joins", delegate
            {
                Config.Main.LogPhotonsJoin = true;
            }, delegate
            {
                Config.Main.LogPhotonsJoin = false;
            }, "Toggle logging when photons connect to the same room", Config.Main.LogPhotonsJoin);

            new QMToggleButton(LogSettings, 4, 1, "Photon Leaves", delegate
            {
                Config.Main.LogPhotonsLeave = true;
            }, delegate
            {
                Config.Main.LogPhotonsLeave = false;
            }, "Toggle logging when photons disconnect from the same room", Config.Main.LogPhotonsLeave);

            #region RPC Log Settings
            var rpcLogs = new QMNestedButton(LogSettings, "RPC Logs", 1, 2, "RPC Log Settings", "RPC Logs");

            new QMToggleButton(rpcLogs, 1, 0, "Object Destroyed", delegate
            {
                Config.Main.LogRPCObjectDestroyed = true;
            }, delegate
            {
                Config.Main.LogRPCObjectDestroyed = false;
            }, "Toggle showing when an object gets destroyed", Config.Main.LogRPCObjectDestroyed);

            new QMToggleButton(rpcLogs, 2, 0, "Camera Toggled", delegate
            {
                Config.Main.LogRPCCameraToggled = true;
            }, delegate
            {
                Config.Main.LogRPCCameraToggled = false;
            }, "Toggle showing whenever someone toggles their camera on or off", Config.Main.LogRPCCameraToggled);

            new QMToggleButton(rpcLogs, 3, 0, "Camera Timer", delegate
            {
                Config.Main.LogRPCCameraTimer = true;
            }, delegate
            {
                Config.Main.LogRPCCameraTimer = false;
            }, "Toggle showing whenever someone runs the countdown timer on their camera (yes it counts EACH INDIVIDUAL Timer Beep)", Config.Main.LogRPCCameraTimer);

            new QMToggleButton(rpcLogs, 4, 0, "Portal Dropped", delegate
            {
                Config.Main.LogRPCPortalDropped = true;
            }, delegate
            {
                Config.Main.LogRPCPortalDropped = false;
            }, "Toggle showing whenever someone drops a portal", Config.Main.LogRPCPortalDropped);

            new QMToggleButton(rpcLogs, 1, 1, "Entered Portal", delegate
            {
                Config.Main.LogRPCEnteredPortal = true;
            }, delegate
            {
                Config.Main.LogRPCEnteredPortal = false;
            }, "Toggle showing whenever someone enters a portal", Config.Main.LogRPCEnteredPortal);

            new QMToggleButton(rpcLogs, 2, 1, "Spawned Emoji", delegate
            {
                Config.Main.LogRPCSpawnedEmoji = true;
            }, delegate
            {
                Config.Main.LogRPCSpawnedEmoji = false;
            }, "Toggle showing whenever someone spawns an emoji", Config.Main.LogRPCSpawnedEmoji);

            new QMToggleButton(rpcLogs, 3, 1, "Played Emote", delegate
            {
                Config.Main.LogRPCPlayedEmote = true;
            }, delegate
            {
                Config.Main.LogRPCPlayedEmote = false;
            }, "Toggle showing whenever someone plays an emote <color=red>(SDK2 Avatars Only)</color>", Config.Main.LogRPCPlayedEmote);

            new QMToggleButton(rpcLogs, 4, 1, "Stopped Emote", delegate
            {
                Config.Main.LogRPCStoppedEmote = true;
            }, delegate
            {
                Config.Main.LogRPCStoppedEmote = false;
            }, "Toggle showing whenever someone stops an emote from playing early <color=red>(SDK2 Avatars Only)</color>", Config.Main.LogRPCStoppedEmote);

            new QMToggleButton(rpcLogs, 1, 2, "Reloaded Avatar", delegate
            {
                Config.Main.LogRPCReloadedAvatar = true;
            }, delegate
            {
                Config.Main.LogRPCReloadedAvatar = false;
            }, "Toggle showing whenever someone reloads their avatar <color=red>(SDK3 Avatars Only)</color>", Config.Main.LogRPCReloadedAvatar);

            new QMToggleButton(rpcLogs, 2, 2, "Photo Taken", delegate
            {
                Config.Main.LogRPCPhotoTaken = true;
            }, delegate
            {
                Config.Main.LogRPCPhotoTaken = false;
            }, "Toggles showing whenever someone takes a picture with their camera", Config.Main.LogRPCPhotoTaken);

            new QMToggleButton(rpcLogs, 3, 2, "Object Instantiated", delegate
            {
                Config.Main.LogRPCObjectInstantiated = true;
            }, delegate
            {
                Config.Main.LogRPCObjectInstantiated = false;
            }, "Toggle showing whenever someone instantiates an object in the world", Config.Main.LogRPCObjectInstantiated);
            #endregion

            new QMToggleButton(LogSettings, 2, 2, "Log Avatar\nChanges", delegate
            {
                Config.Main.LogAvatarsSwitched = true;
            }, delegate
            {
                Config.Main.LogAvatarsSwitched = false;
            }, "Log whenever someone switches avatars (works if they are hidden)", Config.Main.LogAvatarsSwitched);

            new QMToggleButton(LogSettings, 3, 2, "Log RPCS", delegate
            {
                Config.Main.LogRPCS = true;
            }, delegate
            {
                Config.Main.LogRPCS = false;
            }, "Paste all RPCS received into your console", Config.Main.LogRPCS);

            new QMToggleButton(LogSettings, 4, 2, "Log Photon\nEvents", delegate
            {
                Config.Main.LogPhotonEvents = true;
            }, delegate
            {
                Config.Main.LogPhotonEvents = false;
            }, "Log Photon Events to your console", Config.Main.LogPhotonEvents);

            new QMToggleButton(LogSettings, 1, 3, "Log Notifications", delegate
            {
                Config.Main.LogNotifications = true;
            }, delegate
            {
                Config.Main.LogNotifications = false;
            }, "Log Notifications that are sent or recieved", Config.Main.LogNotifications);
            #endregion

            #region Client UI
            ClientUI = new QMNestedButton(Settings, "Client UI", 2, 0, "Blaze's Client UI Settings", "Client UI");

            new QMToggleButton(ClientUI, 1, 0, "Tab Button", delegate
            {
                Config.Main.UseTabButton = true;
                TabButton.SetActive(true);
                mainGroupHeader.SetActive(false);
                mainGroup.SetActive(false);
            }, delegate
            {
                Config.Main.UseTabButton = false;
                TabButton.SetActive(false);
                mainGroupHeader.SetActive(true);
                mainGroup.SetActive(true);
            }, "Changes the Main Opening button for Blaze's Client to be in the tabs menu below", Config.Main.UseTabButton);

            new QMToggleButton(ClientUI, 2, 0, "Confirm Quit", delegate
            {
                Config.Main.ConfirmQuit = true;
            }, delegate
            {
                Config.Main.ConfirmQuit = false;
            }, "Confirmation Popups will show up before hitting restart or quit asking you to confirm your decision", Config.Main.ConfirmQuit);

            #region Nameplates
            Nameplates = new QMNestedButton(ClientUI, "<size=28>Nameplates</size>", 3, 0, "Client Nameplate Settings", "Nameplates");

            new QMToggleButton(Nameplates, 1, 0, "Better\nNameplates", delegate
            {
                Config.Main.BetterNameplates = true;
                foreach (var p in Main.Players)
                {
                    if (Config.Main.CustomNameplateColor)
                    {
                        p.Value.SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
                    }
                    if (Config.Main.UseBelowNameplates)
                    {
                        p.Value.SetupLowerNameplate();
                    }
                }
            }, delegate
            {
                Config.Main.BetterNameplates = false;
                foreach (var p in Main.Players)
                {
                    p.Value.ResetNameplates();
                    p.Value.DestroyLowerNameplate();
                }
            }, "Use better custom nameplates instead of vrchat's boring ones", Config.Main.BetterNameplates);

            new QMToggleButton(Nameplates, 2, 0, "Custom Color", delegate
            {
                Config.Main.CustomNameplateColor = true;
                if (Config.Main.BetterNameplates)
                {
                    foreach (var p in Main.Players)
                    {
                        p.Value.SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
                    }
                }
            }, delegate
            {
                Config.Main.CustomNameplateColor = false;
                foreach (var p in Main.Players)
                {
                    p.Value.ResetNameplateColors();
                }
            }, "Use a custom background color on nameplates", Config.Main.CustomNameplateColor);

            NameplatesColor = new QMSingleButton(Nameplates, 3, 0, $"<color={Config.Main.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>", delegate
            {
                var input = Clipboard.GetText();
                if (!RegexManager.IsValidHexCode(input))
                {
                    PopupUtils.InformationAlert("That is not a valid hexcolor code! Please copy a valid hex color code to your clipboard and click the button again!");
                    return;
                }
                Config.Main.NameplateColor = input;
                NameplatesColor.SetButtonText($"<color={Config.Main.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>");
                PopupUtils.InformationAlert("Nameplate colors have been set! Please rejoin the world to have them take affect!");
            }, "Change the color of the nameplates");

            new QMToggleButton(Nameplates, 4, 0, "Below\n<size=28>Nameplates</size>", delegate
            {
                Config.Main.UseBelowNameplates = true;
                if (Config.Main.BetterNameplates)
                {
                    foreach (var p in Main.Players)
                    {
                        if (p.Value.lowerNamePlate == null)
                        {
                            p.Value.SetupLowerNameplate();
                        }
                    }
                }
            }, delegate
            {
                Config.Main.UseBelowNameplates = false;
                foreach (var p in Main.Players)
                {
                    if (p.Value.lowerNamePlate != null)
                    {
                        p.Value.DestroyLowerNameplate();
                    }
                }
            }, "Show the below nameplates that show ping and frames and such", Config.Main.UseBelowNameplates);

            new QMToggleButton(Nameplates, 1, 1, "Moderations", delegate
            {
                Config.Main.NameplateModerations = true;
            }, delegate
            {
                Config.Main.NameplateModerations = false;
            }, "Show Moderations on the nameplate", Config.Main.NameplateModerations);

            new QMToggleButton(Nameplates, 2, 1, "Trust Rank", delegate
            {
                Config.Main.NamePlateTrustRank = true;
            }, delegate
            {
                Config.Main.NamePlateTrustRank = false;
            }, "Show Trust Rank on the nameplate", Config.Main.NamePlateTrustRank);

            new QMToggleButton(Nameplates, 3, 1, "Platform", delegate
            {
                Config.Main.NamePlatePlatform = true;
            }, delegate
            {
                Config.Main.NamePlatePlatform = false;
            }, "Show Current Platform on the nameplate", Config.Main.NamePlatePlatform);

            new QMToggleButton(Nameplates, 4, 1, "Ping", delegate
            {
                Config.Main.NamePlatePing = true;
            }, delegate
            {
                Config.Main.NamePlatePing = false;
            }, "Show Ping on the nameplate", Config.Main.NamePlatePing);

            new QMToggleButton(Nameplates, 1, 2, "Frames", delegate
            {
                Config.Main.NamePlateFrames = true;
            }, delegate
            {
                Config.Main.NamePlateFrames = false;
            }, "Show Frames", Config.Main.NamePlateFrames);

            new QMToggleButton(Nameplates, 2, 2, "Master Tag", delegate
            {
                Config.Main.NamePlateMasterTag = true;
            }, delegate
            {
                Config.Main.NamePlateMasterTag = false;
            }, "Show Master Tag on nameplate", Config.Main.NamePlateMasterTag);

            new QMToggleButton(Nameplates, 3, 2, "Friends Tag", delegate
            {
                Config.Main.NamePlateFriendsTag = true;
            }, delegate
            {
                Config.Main.NamePlateFriendsTag = false;
            }, "Show Friends Tag on nameplate", Config.Main.NamePlateFriendsTag);

            new QMToggleButton(Nameplates, 4, 2, "Crash State", delegate
            {
                Config.Main.NamePlateCrashState = true;
            }, delegate
            {
                Config.Main.NamePlateCrashState = false;
            }, "Show Crash State on nameplate", Config.Main.NamePlateCrashState);
            #endregion

            new QMToggleButton(ClientUI, 4, 0, "VR Player List", delegate
            {
                Config.Main.VRPlayerList = true;
                playerList.SetActive(true);
            }, delegate
            {
                Config.Main.VRPlayerList = false;
                playerList.SetActive(false);
            }, "Toggles the in menu playerlist", Config.Main.VRPlayerList);

            new QMToggleButton(ClientUI, 1, 1, "VR Debug Panel", delegate
            {
                Config.Main.VRDebugPanel = true;
                DebugPanel.SetActive(true);
            }, delegate
            {
                Config.Main.VRDebugPanel = false;
                DebugPanel.SetActive(false);
            }, "Toggles the in menu debug panel", Config.Main.VRDebugPanel);

            new QMToggleButton(ClientUI, 2, 1, "IRL Clock", delegate
            {
                Config.Main.IRLClock = true;
                ClockPanel.SetActive(true);
            }, delegate
            {
                Config.Main.IRLClock = false;
                ClockPanel.SetActive(false);
            }, "Toggle the in menu irl clock", Config.Main.IRLClock);

            new QMToggleButton(ClientUI, 3, 1, "Desktop Player List", delegate
            {
                Config.Main.DesktopPlayerList = true;
            }, delegate
            {
                Config.Main.DesktopPlayerList = false;
            }, "Toggle the On Screen Desktop Player List", Config.Main.DesktopPlayerList);

            new QMToggleButton(ClientUI, 4, 1, "Desktop Debug Panel", delegate
            {
                Config.Main.DesktopDebugPanel = true;
            }, delegate
            {
                Config.Main.DesktopDebugPanel = false;
            }, "Toggle the On Screen Desktop Debug Panel", Config.Main.DesktopDebugPanel);

            new QMToggleButton(ClientUI, 1, 2, "<size=22>Disable Avatar Preview</size>", delegate
            {
                Config.Main.DisableAvatarPreview = true;
                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(false);
            }, delegate
            {
                Config.Main.DisableAvatarPreview = false;
                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(true);
            }, "Toggle showing the avatar preview in the avatars menu", Config.Main.DisableAvatarPreview);
            if (Config.Main.DisableAvatarPreview) GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(false);

            new QMToggleButton(ClientUI, 2, 2, "Hud FPS", delegate
            {
                Config.Main.HudFPS = true;
                HudFPSObj.SetActive(true);
            }, delegate
            {
                Config.Main.HudFPS = false;
                HudFPSObj.SetActive(false);
            }, "Toggle an FPS Label above the mute icon", Config.Main.HudFPS);

            new QMToggleButton(ClientUI, 3, 2, "Disable Carousel", delegate
            {
                Config.Main.DisableCarousel = true;
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").SetActive(false);
            }, delegate
            {
                Config.Main.DisableCarousel = false;
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").SetActive(true);
            }, "Disables the VRChat Ad Carousel", Config.Main.DisableCarousel);
            #endregion

            new QMToggleButton(Settings, 3, 0, "ComfyVR Menu", delegate
            {
                Config.Main.ComfyVRMenu = true;
            }, delegate
            {
                Config.Main.ComfyVRMenu = false;
            }, "Place the Big Social Menu infront of your face no matter where you are looking", Config.Main.ComfyVRMenu);

            #region Pipeline Settings
            var pipeline = new QMNestedButton(Settings, "Pipeline\nSettings", 2, 1,  "Click to toggle Blaze's Client Pipeline Logger Settings", "Pipeline Settings");

            new QMToggleButton(pipeline, 1, 0, "Use Pipeline", delegate 
            {
                Config.Main.UsePipeline = true;
            }, delegate 
            {
                Config.Main.UsePipeline = false;
            }, "Toggle using pipeline logger", Config.Main.UsePipeline);

            new QMToggleButton(pipeline, 2, 0, "Online", delegate
            {
                Config.Main.PipelineOnline = true;
            }, delegate
            {
                Config.Main.PipelineOnline = false;
            }, "Toggle showing when friends come online", Config.Main.PipelineOnline);

            new QMToggleButton(pipeline, 3, 0, "Offline", delegate
            {
                Config.Main.PipelineOffline = true;
            }, delegate
            {
                Config.Main.PipelineOffline = false;
            }, "Toggle showing when friends go offline", Config.Main.PipelineOffline);

            new QMToggleButton(pipeline, 4, 0, "Locations", delegate
            {
                Config.Main.PipelineLocations = true;
            }, delegate
            {
                Config.Main.PipelineLocations = false;
            }, "Toggle showing when friends change locations", Config.Main.PipelineLocations);

            new QMToggleButton(pipeline, 1, 1, "Added", delegate
            {
                Config.Main.PipelineAdded = true;
            }, delegate
            {
                Config.Main.PipelineAdded = false;
            }, "Toggle showing when people add you as their friend", Config.Main.PipelineAdded);

            new QMToggleButton(pipeline, 2, 1, "Removed", delegate
            {
                Config.Main.PipelineRemoved = true;
            }, delegate
            {
                Config.Main.PipelineRemoved = false;
            }, "Toggle showing when people remove you from their friends list", Config.Main.PipelineRemoved);

            new QMToggleButton(pipeline, 3, 1, "Website", delegate
            {
                Config.Main.PipelineWebsite = true;
            }, delegate
            {
                Config.Main.PipelineWebsite = false;
            }, "Toggle showing when friends login via the VRC website", Config.Main.PipelineWebsite);

            new QMToggleButton(pipeline, 4, 1, "Hud Only\nFavorites", delegate
            {
                Config.Main.PipelineHudOnlyFavs = true;
            }, delegate
            {
                Config.Main.PipelineHudOnlyFavs = false;
            }, "Toggle only showing users in your first favorite list on the hud notifications", Config.Main.PipelineHudOnlyFavs);
            #endregion

            #endregion

            #region Selected
            var groupHeader = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup"));
            var selectedGroup = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup"));
            groupHeader.GetComponentInChildren<TextMeshProUGUI>(true).text = "Blaze's Client";
            selectedGroup.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            groupHeader.transform.SetSiblingIndex(8);
            selectedGroup.transform.SetSiblingIndex(9);
            for (var i = selectedGroup.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(selectedGroup.transform.GetChild(i).gameObject);
            }

            var groupHeader2 = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Remote/ScrollRect/Viewport/VerticalLayoutGroup"));
            var selectedGroup2 = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject, GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Remote/ScrollRect/Viewport/VerticalLayoutGroup"));
            groupHeader2.GetComponentInChildren<TextMeshProUGUI>(true).text = "Blaze's Client";
            selectedGroup2.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            groupHeader2.transform.SetSiblingIndex(8);
            selectedGroup2.transform.SetSiblingIndex(9);
            for (var i = selectedGroup2.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(selectedGroup2.transform.GetChild(i).gameObject);
            }

            Selected = new QMNestedButton("Menu_SelectedUser_Local", "", 0, 0, "Target functions for Blaze's Client", "Blaze's Client");
            Selected.GetMainButton().SetBackgroundImage(AssetBundleManager.Logo);
            Selected.GetMainButton().GetGameObject().transform.SetParent(selectedGroup.transform);
            Selected.GetMainButton().SetAction(delegate
            {
                Main.SelectedPlayer = PlayerUtils.GetSelectedUser()._player;
                Selected.OpenMe();
            });

            var otherSelectedButton = new QMSingleButton("Menu_SelectedUser_Remote", 0, 0, "", delegate
            {
                Main.SelectedPlayer = PlayerUtils.GetSelectedUser()._player;
                Selected.OpenMe();
            }, "Target functions for Blaze's Client");

            new QMSingleButton(Selected, 1, 0, "Teleport To", delegate
            {
                Functions.TeleportByUserID(Main.SelectedPlayer.GetUserID());
            }, "Teleports to the selected user");

            new QMSingleButton(Selected, 2, 0, "Copy\nInfo", delegate
            {
                var sb = new StringBuilder();
                var user = SocialMenuUtils.GetAPIUser();
                sb.AppendLine("======[Blaze's Client]======");
                sb.AppendLine("Registered Name: " + user.username);
                sb.AppendLine("Display Name: " + user.displayName);
                sb.AppendLine("ID: " + user.id);
                sb.AppendLine("============================");
                Clipboard.SetText(sb.ToString());
                PopupUtils.InformationAlert("Sucessfully copied user's information to your clipboard!");
            }, "Copies the selected user's info to your clipboard");

            new QMSingleButton(Selected, 3, 0, "Set As\nTarget", delegate
            {
                Main.Target = Main.SelectedPlayer;
            }, "Sets the Client Target as the selected user");

            new QMSingleButton(Selected, 4, 0, "Force\nClone", delegate
            {
                if (Main.SelectedPlayer.GetUserID() == PlayerUtils.CurrentUser().GetUserID())
                {
                    PopupUtils.InformationAlert("You cannot clone yourself!");
                }
                else
                {
                    if (Main.SelectedPlayer.GetPCAvatar().releaseStatus == "private")
                    {
                        PopupUtils.InformationAlert("You cannot clone that avatar it is private!");
                    }
                    else
                    {
                        AvatarUtils.ChangeToAvatar(Main.SelectedPlayer.GetPCAvatar().id);
                    }
                }
            }, "Forcefully clone a user if their avatar is public");

            new QMSingleButton(Selected, 1, 1, "Download\nVRCA", delegate
            {
                DownloadManager.DownloadVRCA(Main.SelectedPlayer.GetPCAvatar());
            }, "Download the target's currenty avatar to your VRCA's Folder");

            /*new QMSingleButton(Selected, 4, 2.5f, "Portal Drop", delegate
            {
                //Functions.DropPortal("wrld_1668a4c1-15f7-419a-adcf-f238dc30224a", Main.Target._vrcplayer);
            }, "Drops a regular portal on the user", null, true);*/
            #endregion

            // QM Addons
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
            playerList.InfoObject.AddComponent<BlazeVRPlayerList>();
            playerList.SetActive(Config.Main.VRPlayerList);
            #endregion

            #region VR Debug
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
            Logs.Debug($"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> Debugger is now active! <color=red><3</color>");
            DebugPanel.SetActive(Config.Main.VRDebugPanel);
            #endregion

            #region Local Clock
            var obj = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel");
            ClockPanel = UnityEngine.Object.Instantiate(obj, obj.transform.parent, false);
            ClockPanel.name = "ApolloClockPanel";
            ClockPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(614, 0);
            ClockText = ClockPanel.transform.Find("Panel/Text_FPS").GetComponent<TextMeshProUGUI>();
            ClockText.color = Color.white;
            ClockPanel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().sizeDelta = new Vector2(350, 0);
            ClockPanel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().anchoredPosition = new Vector2(-165, 0);
            ClockPanel.transform.Find("Panel/Text_FPS").name = "Text Label";
            UnityEngine.Object.Destroy(ClockPanel.transform.Find("Panel/Text_Ping"));
            UnityEngine.Object.Destroy(ClockPanel.GetComponent<DebugInfoPanel>());
            ClockPanel.transform.SetSiblingIndex(1);
            ClockPanel.AddComponent<BlazeIRLClock>();
            ClockPanel.SetActive(Config.Main.IRLClock);
            #endregion

            var QMListener = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/MicButton").AddComponent<EnableDisableListener>();
            QMListener.OnEnabled += () =>
            {
                Main.QMIsOpened = true;
            };
            QMListener.OnDisabled += () =>
            {
                Main.QMIsOpened = false;
            };
        }
    }

    public class BlazeHudFPS : MonoBehaviour
    {
        public BlazeHudFPS(IntPtr id) : base(id) { }
        private float timer = 0.5f;

        public void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                try
                {
                    if (BlazeQM.HudFPSObj == null) return;
                    BlazeQM.HudFPSTxt.text = $"{Math.Truncate(1.0f / Time.deltaTime)}";
                }
                catch { }
                timer = 0.5f;
            }
        }
    }

    public class BlazeVRPlayerList : MonoBehaviour
    {
        public BlazeVRPlayerList(IntPtr id) : base(id) { }
        private float timer = 0.5f;

        public void Awake() => timer = 0;

        public void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                try
                {
                    string finalResult = string.Empty;
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
                            Main.Players.TryGetValue(pl.GetUserID(), out BlazePlayerInfo comp);

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

                            if (comp.isCrashed && pl != PlayerUtils.CurrentUser()._player)
                            {
                                currentPlayer += "<b><color=red>CRASHED</color></b> <color=grey>|</color> ";
                            }
                            else if (comp.isGhosting && pl != PlayerUtils.CurrentUser()._player)
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
                    finalResult += mainLine;
                    finalResult += players;
                    BlazeQM.playerList.SetText(finalResult);
                }
                catch { }
                timer = 0.5f;
            }
        }

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
    }

    public class BlazeIRLClock : MonoBehaviour
    {
        public BlazeIRLClock(IntPtr id) : base(id) { }
        private string Month;
        private string Day;

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
                BlazeQM.ClockText.text = $"<b>{Month} <color=white>{Day} :</color> <color=yellow>{DateTime.Now:h:mm:ss tt}</color></b>";
            }
            catch { }
        }

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
    }
}
