using Blaze.API;
using Blaze.API.QM;
using Blaze.API.Wings;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using RealisticEyeMovements;
using RootMotion.FinalIK;
using System;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.DataModel;

namespace Blaze.Modules
{
    class BlazeMenu : BModule
    {
        internal static QMNestedButton MainMenu;
        internal static QMTabButton TabButton;
        internal static QMNestedButton Worlds;
        internal static QMNestedButton Worlds2;
        internal static QMNestedButton Movement;
        internal static QMNestedButton Exploits;
        internal static QMNestedButton Exploits2;
        internal static QMNestedButton Mic;
        internal static QMNestedButton Protections;
        internal static QMNestedButton Spoofs;
        internal static QMNestedButton Renders;
        internal static QMNestedButton WorldSpecific;
        internal static QMNestedButton Settings;
        internal static QMNestedButton LogSettings;
        internal static QMNestedButton BCUI;
        internal static QMNestedButton Selected;

        internal static QMSingleButton NameplateColor;
        internal static QMSingleButton MicGainLabel;
        internal static QMSingleButton MicBitrateLabel;
        internal static QMToggleButton HeadFlipper;

        public override void QuickMenuUI()
        {
            // Delete the banners
            UnityEngine.Object.Destroy(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners"));

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
            MainMenu = new QMNestedButton("Menu_Dashboard", "", 1, 3, $"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color> <color=white>| Created by:</color> <color=red>WTFBlaze</color>", "Blaze's Client");
            MainMenu.GetMainButton().SetBackgroundImage(AssetBundleManager.Logo);
            MainMenu.GetMainButton().GetGameObject().transform.SetParent(mainGroup.transform);
            TabButton = new QMTabButton(delegate
            {
                MainMenu.GetMainButton().ClickMe();
            }, $"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color> <color=white>| Created by:</color> <color=red>WTFBlaze</color>", AssetBundleManager.Logo);

            if (Config.Main.UseTabButton)
            {
                mainGroupHeader.SetActive(false);
                mainGroup.SetActive(false);
            }
            else
            {
                TabButton.SetActive(false);
            }

            var forceQuit = new QMSingleButton(MainMenu, 4, 3, "<color=red>Force\nQuit</color>", delegate
            {
                if (Config.Main.ConfirmQuit)
                {
                    PopupUtils.AlertV2("Are you sure you want to quit?", "Quit", Functions.ForceQuit);
                }
                else
                {
                    Functions.ForceQuit();
                }
            }, "Instantly quit VRChat");
            //forceQuit.SetBackgroundImage(AssetBundleManager.QuitIcon);

            var restart = new QMSingleButton(MainMenu, 3, 3, "<color=yellow>Restart</color>", delegate
            {
                if (Config.Main.ConfirmQuit)
                {
                    PopupUtils.AlertV2("Are you sure you want to restart?", "Restart", Functions.Restart);
                }
                else
                {
                    Functions.Restart();
                }
            }, "Instantly quit VRChat and restart it (if in vr loads in vr)");
            //restart.SetBackgroundImage(AssetBundleManager.RestartIcon);

            new QMSingleButton(MainMenu, 1, 3, "<color=yellow>Save\nConfig</color>", Config.SaveAll, "Save your current Blaze's Client Settings");

            #region Worlds
            Worlds = new QMNestedButton(MainMenu, "Worlds", 1, 0, "World Functions", "Worlds");
            //Worlds.GetMainButton().SetBackgroundImage(AssetBundleManager.WorldsIcon);

            new QMSingleButton(Worlds, 1, 0, "Set Jump\nHeight", delegate
            {
                PopupUtils.NumericPopup("Set Jump", "Use 3 for normal jump height", delegate (string s)
                {
                    PlayerUtils.CurrentUser().GetVRCPlayerApi().SetJumpImpulse(float.Parse(s));
                });
            }, "Forcefully change your jumping power in the current world");

            new QMSingleButton(Worlds, 2, 0, "Copy\nRoom ID", delegate
            {
                Clipboard.SetText(WorldUtils.GetJoinID());
            }, "Copies the current world's id to your clipboard");

            new QMSingleButton(Worlds, 3, 0, "Join\nRoom ID", delegate
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

            new QMSingleButton(Worlds, 4, 0, "Rejoin\nWorld", delegate
            {
                WorldUtils.RejoinInstance();
            }, "Rejoins the same instance you are currently in");

            new QMSingleButton(Worlds, 1, 2, "Locally\nDelete\nPortals", delegate
            {
                Functions.LocallyDestroyPortals();
            }, "Delete portals for only yourself");

            new QMSingleButton(Worlds, 2, 2, "Globally\nDelete\nPortals", delegate
            {
                Functions.GlobalDestroyPortals();
            }, "Delete portals for everyone");

            new QMSingleButton(Worlds, 3, 2, "Reset\nPickups", delegate
            {
                Functions.ResetPickups();
            }, "Reset all pickups back to their default positions and values");

            new QMSingleButton(Worlds, 1, 3, "Download\nVRCW", delegate
            {
                DownloadManager.DownloadVRCW(WorldUtils.CurrentWorld());
            }, "Downloads the Current World to your VRCW's Folder");

            #region Worlds Page 2
            Worlds2 = new QMNestedButton(Worlds, "<color=yellow>Next\nPage</color>", 4, 3, "Click to view more Worlds Functions", "Worlds");

            new QMToggleButton(Worlds2, 1, 0, "Friends+ Home", delegate
            {
                Config.Main.FriendsPlusHome = true;
            }, delegate
            {
                Config.Main.FriendsPlusHome = false;
            }, "Change your homeworld instance type to Friends+ always", Config.Main.FriendsPlusHome);
            #endregion
            #endregion

            #region Movement
            Movement = new QMNestedButton(MainMenu, "Movement", 2, 0, "Movement Functions", "Movement");
            //Movement.GetMainButton().SetBackgroundImage(AssetBundleManager.MovementIcon);

            new QMToggleButton(Movement, 1, 1, "Beyblade Mode", delegate
            {
                PlayerUtils.CurrentUser().transform.position += new Vector3(0f, 1.2f, 0f);
                PlayerUtils.CurrentUser().transform.rotation = new Quaternion(90f, 0f, 0f, 0f);
            }, delegate
            {
                PlayerUtils.CurrentUser().transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                PlayerUtils.CurrentUser().transform.position += new Vector3(0f, 0.6f, 0f);
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
            Exploits = new QMNestedButton(MainMenu, "Exploits", 3, 0, "Exploits and Fun shit", "Exploits");
            //Exploits.GetMainButton().SetBackgroundImage(AssetBundleManager.ExploitsIcon);

            new QMToggleButton(Exploits, 3, 0, "Orbit Annoyance", delegate
            {
                Config.Main.OrbitAnnoyanceMode = true;
            }, delegate
            {
                Config.Main.OrbitAnnoyanceMode = false;
            }, "Toggles orbiting around targets head instead of feet", Config.Main.OrbitAnnoyanceMode);

            #region Orbit Sliders
            var orbitSliders = new QMNestedButton(Exploits, "Orbit Values", 4, 0, "Change how orbits work", "Orbit Values");

            new QMSlider(orbitSliders, -510, -380, "Player Orbit Speed", 0.1f, 2, Config.Main.PlayerOrbitSpeed, delegate (float f)
            {
                Config.Main.PlayerOrbitSpeed = f;
            });

            new QMSlider(orbitSliders, -510, -500, "Player Orbit Size", 0.1f, 2, Config.Main.PlayerOrbitSize, delegate (float f)
            {
                Config.Main.PlayerOrbitSize = f;
            });

            new QMSlider(orbitSliders, -510, -620, "Item Orbit Speed", 0.1f, 2, Config.Main.ItemOrbitSpeed, delegate (float f)
            {
                Config.Main.ItemOrbitSpeed = f;
            });

            new QMSlider(orbitSliders, -510, -740, "Item Orbit Size", 0.1f, 2, Config.Main.ItemOrbitSize, delegate (float f)
            {
                Config.Main.ItemOrbitSize = f;
            });
            #endregion

            new QMToggleButton(Exploits, 2, 1, "Master Lock", delegate
            {
                BlazeInfo.MasterLockInstance = true;
            }, delegate
            {
                BlazeInfo.MasterLockInstance = false;
            }, "Lock the instance so that nobody else can join (except others with the anti for it lol) [most likely needs master idfk I don't remember]");

            new QMToggleButton(Exploits, 3, 1, "Serialization", delegate
            {
                BlazeInfo.Serialization = true;
                var pos = PlayerUtils.CurrentUser().transform.position;
                var rot = PlayerUtils.CurrentUser().transform.rotation;
                BlazeInfo.SerializeClone = UnityEngine.Object.Instantiate(PlayerUtils.CurrentUser().prop_VRCAvatarManager_0.prop_GameObject_0, null, true);
                Animator component = BlazeInfo.SerializeClone.GetComponent<Animator>();
                if (component != null && component.isHuman)
                {
                    Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
                    if (boneTransform != null) boneTransform.localScale = Vector3.one;
                }
                BlazeInfo.SerializeClone.name = "Blaze's Client Serialization Clone";
                component.enabled = false;
                BlazeInfo.SerializeClone.GetComponent<FullBodyBipedIK>().enabled = false;
                BlazeInfo.SerializeClone.GetComponent<LimbIK>().enabled = false;
                BlazeInfo.SerializeClone.GetComponent<VRIK>().enabled = false;
                BlazeInfo.SerializeClone.GetComponent<LookTargetController>().enabled = false;
                BlazeInfo.SerializeClone.transform.position = pos;
                BlazeInfo.SerializeClone.transform.rotation = rot;
            }, delegate
            {
                BlazeInfo.Serialization = false;
                UnityEngine.Object.Destroy(BlazeInfo.SerializeClone);
            }, "Freeze your body in place for everyone else");

            new QMToggleButton(Exploits, 4, 1, "World Triggers", delegate
            {
                BlazeInfo.WorldTriggers = true;
            }, delegate
            {
                BlazeInfo.WorldTriggers = false;
            }, "Force all items you interact with to be networked (SDK2 Worlds Only)");

            new QMToggleButton(Exploits, 1, 2, "God Mode", delegate
            {
                Config.Main.GodMode = true;
            }, delegate
            {
                Config.Main.GodMode = false;
            }, "Toggle being unable to die in gamemodes (if it doesn't work in a specific world let Blaze know in the discord server)", Config.Main.GodMode);

            HeadFlipper = new QMToggleButton(Exploits, 2, 2, "Head Flipper", delegate
            {
                BlazeInfo.SavedNeckRange = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0;
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = new NeckRange(float.MinValue, float.MaxValue, 0f);
            }, delegate
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = BlazeInfo.SavedNeckRange;
            }, "Allow your head to freely rotate (Credits to Spacers for this)");

            new QMToggleButton(Exploits, 3, 2, "Parrot Mode", delegate
            {
                BlazeInfo.ParrotMode = true;
            }, delegate
            {
                BlazeInfo.ParrotMode = false;
            }, "Mimic everything your target says through their mic");

            /*new QMToggleButton(Exploits, 1, 3, "Anti-Portal Deletion", delegate
            {
                BlazeInfo.InfinitePortals = true;
            }, delegate
            {
                BlazeInfo.InfinitePortals = false;
            }, "Prevent portals from being deleted");*/

            new QMToggleButton(Exploits, 2, 3, "Self Hide", delegate
            {
                Functions.SelfHide(true);
            }, delegate
            {
                Functions.SelfHide(false);
            }, "Locally hide your avatar so you can world crash without hitting yourself");

            #region Exploits Page 2
            Exploits2 = new QMNestedButton(Exploits, "<color=yellow>Next\nPage</color>", 4, 3, "Click to view the next page of exploits", "Exploits - Page 2");
            #endregion

            #endregion

            #region Mic
            Mic = new QMNestedButton(MainMenu, "Mic", 4, 0, "Microphone Functions", "Microphone");
            //Mic.GetMainButton().SetBackgroundImage(AssetBundleManager.MicIcon);

            new QMSingleButton(Mic, 1, 0.5f, "Gain <color=red><b>MAX</b></color>", delegate
            {
                USpeakerUtils.SetGain(float.MaxValue);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>MAX</b></color>");
            }, "Set your microphone gain to the maximum value", null, true);


            new QMSingleButton(Mic, 1, 1f, "Gain <color=yellow><b>++</b></color>", delegate
            {
                Functions.IncreaseGain(10);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Increase your mic gain by 10", null, true);

            new QMSingleButton(Mic, 1, 1.5f, "Gain <color=green><b>+</b></color>", delegate
            {
                Functions.IncreaseGain(1);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Increase your mic gain by 1", null, true);

            new QMSingleButton(Mic, 1, 2f, "Gain <color=green><b>-</b></color>", delegate
            {
                Functions.DecreaseGain(1);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Decrease your mic gain by 1", null, true);

            new QMSingleButton(Mic, 1, 2.5f, "Gain <color=yellow><b>--</b></color>", delegate
            {
                Functions.DecreaseGain(10);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>{USpeakerUtils.GetGain()}</b></color>");
            }, "Decrease your mic gain by 10", null, true);

            new QMSingleButton(Mic, 1, 3f, "Gain <color=red><b>MIN</b></color>", delegate
            {
                USpeakerUtils.SetGain(float.MinValue);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>MIN</b></color>");
            }, "Set your microphone gain to the minimum value", null, true);

            new QMSingleButton(Mic, 2, 3f, "Reset Gain", delegate
            {
                USpeakerUtils.SetGain(1);
                MicGainLabel.SetButtonText($"Gain: <color=yellow><b>1</b></color>");
            }, "Reset your mic gain back to the default value", null, true);

            MicGainLabel = new QMSingleButton(Mic, 2, 1.5f, "Gain: <color=yellow><b>1</b></color>", delegate { }, "Your current Microphone Gain Value");
            MicBitrateLabel = new QMSingleButton(Mic, 3, 1.5f, "BitRate: <color=yellow><b>20K</b></color>", delegate { }, "Your current Microphone BitRate Value");

            new QMSingleButton(Mic, 3, 3f, "Reset BitRate", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_24K);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>24K</b></color>");
            }, "Reset your bitrate back to the default value", null, true);

            new QMSingleButton(Mic, 4, 0.5f, "BitRate <color=red><b>MAX</b></color>", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_24K);
                MicBitrateLabel.SetButtonText("BitRate: <color=yellow><b>MAX</b></color>");
            }, "Increase your bitrate to the maximum setting", null, true);

            new QMSingleButton(Mic, 4, 1f, "BitRate <color=yellow><b>++</b></color>", delegate
            {
                Functions.IncreaseBitRate(5);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 5 settings", null, true);

            new QMSingleButton(Mic, 4, 1.5f, "BitRate <color=green><b>+</b></color>", delegate
            {
                Functions.IncreaseBitRate(1);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 1 settings", null, true);

            new QMSingleButton(Mic, 4, 2f, "BitRate <color=green><b>-</b></color>", delegate
            {
                Functions.DecreaseBitRate(1);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 1 settings", null, true);

            new QMSingleButton(Mic, 4, 2.5f, "BitRate <color=yellow><b>--</b></color>", delegate
            {
                Functions.DecreaseBitRate(5);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>{USpeakerUtils.GetBitRate().ToString().Split('_')[1]}</b></color>");
            }, "Increase your bitrate by 5 settings", null, true);

            new QMSingleButton(Mic, 4, 3f, "BitRate <color=red><b>MIN</b></color>", delegate
            {
                USpeakerUtils.SetBitrate(BitRate.BitRate_8K);
                MicBitrateLabel.SetButtonText($"BitRate: <color=yellow><b>MIN</b></color>");
            }, "Increase your bitrate by 10 settings", null, true);
            #endregion

            #region Protections
            Protections = new QMNestedButton(MainMenu, "Security", 1, 1, "Protection options and functions", "Security");
            //Protections.GetMainButton().SetBackgroundImage(AssetBundleManager.SecurityIcon);

            new QMToggleButton(Protections, 1, 0, "Anti Instance Lock", delegate
            {
                Config.Main.AntiInstanceLock = true;
            }, delegate
            {
                Config.Main.AntiInstanceLock = false;
            }, "Prevent getting kicked to other worlds because dickbags are locking the instance from being able to be joined.", Config.Main.AntiInstanceLock);

            new QMToggleButton(Protections, 2, 0, "Anti Portal", delegate
            {
                Config.Main.AntiPortal = true;
            }, delegate
            {
                Config.Main.AntiPortal = false;
            }, "Prevent entering portals entirely", Config.Main.AntiPortal);

            new QMToggleButton(Protections, 3, 0, "Portal Prompt", delegate
            {
                Config.Main.PortalPrompt = true;
            }, delegate
            {
                Config.Main.PortalPrompt = false;
            }, "Toggle a confirmation popup whenever you are about to enter a portal", Config.Main.PortalPrompt);

            new QMToggleButton(Protections, 4, 0, "Anti Udon", delegate
            {
                Config.Main.AntiUdon = true;
            }, delegate
            {
                Config.Main.AntiUdon = false;
            }, "Toggle preventing udon scripts from affecting you", Config.Main.AntiUdon);

            new QMToggleButton(Protections, 1, 1, "Anti Teleport", delegate
            {
                Config.Main.AntiTeleport = true;
            }, delegate
            {
                Config.Main.AntiTeleport = false;
            }, "Toggle blocking being teleported (SDK2 Worlds Only [Use Anti Udon for SDK3])", Config.Main.AntiTeleport);

            new QMToggleButton(Protections, 2, 1, "Anti-World Triggers", delegate
            {
                Config.Main.AntiWorldTriggers = true;
            }, delegate
            {
                Config.Main.AntiWorldTriggers = false;
            }, "Toggle blocking other people from affecting you with world triggers (SDK2 Worlds Only)", Config.Main.AntiWorldTriggers);

            new QMToggleButton(Protections, 3, 1, "Anti Desync", delegate
            {
                Config.Main.AntiDesync = true;
            }, delegate
            {
                Config.Main.AntiDesync = false;
            }, "Toggle forcing all malicious rpc's to be rewritten to safe values", Config.Main.AntiDesync);

            #region Moderations
            var moderations = new QMNestedButton(Protections, "<size=26>Moderations</size>", 4, 1, "", "Moderations");

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
            Spoofs = new QMNestedButton(MainMenu, "Spoofs", 2, 1, "Spoofing Functions", "Spoofs");
            //Spoofs.GetMainButton().SetBackgroundImage(AssetBundleManager.SpoofsIcon);

            new QMToggleButton(Spoofs, 1, 0, "HWID", delegate
            {
                Config.Main.HWIDSpoofer = true;
            }, delegate
            {
                Config.Main.HWIDSpoofer = false;
            }, "Toggle spoofing your hwid to make bypassing bans easier incase they happen (If you login to a banned account delete the HWID.txt file)", Config.Main.HWIDSpoofer);

            new QMToggleButton(Spoofs, 2, 0, "Offline", delegate
            {
                Config.Main.OfflineSpoof = true;
            }, delegate
            {
                Config.Main.OfflineSpoof = false;
            }, "Appear as offline for anyone not in the same instance as you", Config.Main.OfflineSpoof);

            new QMToggleButton(Spoofs, 3, 0, "World", delegate
            {
                Config.Main.WorldSpoof = true;
            }, delegate
            {
                Config.Main.WorldSpoof = false;
            }, "Appear as being in a different world than you actually are", Config.Main.WorldSpoof);

            /*new QMToggleButton(Spoofs, 4, 0, "Quest", delegate
            {
                Config.Main.QuestSpoof = true;
                PopupUtils.InformationAlert("Please restart your game to start spoofing as quest");
                Config.SaveAll();
            }, delegate
            {
                Config.Main.QuestSpoof = false;
                Config.SaveAll();
            }, "Appear as being a quest user", Config.Main.QuestSpoof);*/

            new QMToggleButton(Spoofs, 1, 1, "Ping", delegate
            {
                Config.Main.PingSpoof = true;
            }, delegate
            {
                Config.Main.PingSpoof = false;
            }, "Spoof your ping as whatever numbers you want", Config.Main.PingSpoof);

            new QMSingleButton(Spoofs, 2, 1, "Set\nPing", delegate
            {
                PopupUtils.NumericPopup("Set Ping", "Enter New Ping Value...", delegate (string s)
                {
                    Config.Main.PingSpoofValue = int.Parse(s);
                });
            }, "Set the value of what ping spoof should show as");

            new QMToggleButton(Spoofs, 3, 1, "Frames", delegate
            {
                Config.Main.FramesSpoof = true;
            }, delegate
            {
                Config.Main.FramesSpoof = false;
            }, "Spoof your Frames / FPS as whatever numbers you want", Config.Main.FramesSpoof);

            new QMSingleButton(Spoofs, 4, 1, "Set\nFrames", delegate
            {
                PopupUtils.NumericPopup("Set Frames", "Enter New Frames Value...", delegate (string s)
                {
                    Config.Main.FramesSpoofValue = int.Parse(s);
                });
            }, "Set the value of what frames spoof should show as");
            #endregion

            #region Renders
            Renders = new QMNestedButton(MainMenu, "Renders", 3, 1, "Render Functions", "Renders");
            //Renders.GetMainButton().SetBackgroundImage(AssetBundleManager.RendersIcon);
            #endregion

            #region World Specific
            WorldSpecific = new QMNestedButton(MainMenu, "World\nSpecifics", 4, 1, "Click to view world specific functions", "World Specific");
            //WorldSpecific.GetMainButton().SetBackgroundImage(AssetBundleManager.WorldSpecificIcon);
            #endregion

            #region Settings
            Settings = new QMNestedButton(MainMenu, "Settings", 1, 2, "Change the way the client runs for your personal experience", "Settings");
            //Settings.GetMainButton().SetBackgroundImage(AssetBundleManager.SettingsIcon);

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
            }, "Toggle showing whenever someone plays an emote (SDK2 Avatars Only)", Config.Main.LogRPCPlayedEmote);

            new QMToggleButton(rpcLogs, 4, 1, "Stopped Emote", delegate
            {
                Config.Main.LogRPCStoppedEmote = true;
            }, delegate
            {
                Config.Main.LogRPCStoppedEmote = false;
            }, "Toggle showing whenever someone stops an emote from playing early (SDK2 Avatars Only)", Config.Main.LogRPCStoppedEmote);

            new QMToggleButton(rpcLogs, 1, 2, "Reloaded Avatar", delegate
            {
                Config.Main.LogRPCReloadedAvatar = true;
            }, delegate
            {
                Config.Main.LogRPCReloadedAvatar = false;
            }, "Toggle showing whenever someone reloads their avatar (SDK3 Avatars Only)", Config.Main.LogRPCReloadedAvatar);
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

            #endregion

            new QMToggleButton(Settings, 2, 0, "DiscordRPC", delegate
            {
                Config.Main.UseDiscordRPC = true;
                DiscordRPC.StartClient();
            }, delegate
            {
                Config.Main.UseDiscordRPC = false;
                DiscordRPC.StartClient();
            }, "Show in discord that you are playing Blaze's Client (does not show your username)", Config.Main.UseDiscordRPC);

            #region BC UI
            BCUI = new QMNestedButton(Settings, "Client UI", 3, 0, "Change the settings of the Client's UI", "Client UI");

            new QMToggleButton(BCUI, 1, 0, "Tab Button", delegate
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

            #region Nameplates
            var nameplates = new QMNestedButton(BCUI, "<size=28>Nameplates</size>", 2, 0, "Blaze's Client Nameplate Settings", "Nameplates");

            new QMToggleButton(nameplates, 1, 0, "Better\nNameplates", delegate
            {
                Config.Main.BetterNameplates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    comp.SetupLowerNameplate();
                    if (Config.Main.CustomNameplateColor)
                    {
                        comp.SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
                    }
                }
            }, delegate
            {
                Config.Main.BetterNameplates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    comp.ResetNameplates();
                }
            }, "Use better custom nameplates instead of vrchat's boring ones", Config.Main.BetterNameplates);

            new QMToggleButton(nameplates, 2, 0, "Custom Color", delegate
            {
                Config.Main.CustomNameplateColor = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    comp.SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
                }
            }, delegate
            {
                Config.Main.CustomNameplateColor = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    comp.ResetNameplateColors();
                }
            }, "Use a custom background color on nameplates", Config.Main.CustomNameplateColor);

            NameplateColor = new QMSingleButton(nameplates, 3, 0, $"<color={Config.Main.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>", delegate
            {
                var input = Clipboard.GetText();
                if (!RegexManager.IsValidHexCode(input))
                {
                    PopupUtils.InformationAlert("That is not a valid hexcolor code! Please copy a valid hex color code to your clipboard and click the button again!");
                    return;
                }
                Config.Main.NameplateColor = input;
                NameplateColor.SetButtonText($"<color={Config.Main.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>");
                PopupUtils.InformationAlert("Nameplate colors have been set! Please rejoin the world to have them take affect!");
                /*if (Config.Main.CustomNameplateColor)
                {
                    foreach (var p in WorldUtils.GetPlayers())
                    {
                        var comp = p.GetComponent<BlazesPlayerInfo>();
                        comp.SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
                    }
                }*/
            }, "Change the color of the nameplates");

            new QMToggleButton(nameplates, 4, 0, "Below\n<size=28>Nameplates</size>", delegate
            {
                Config.Main.UseBelowNameplates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    if (comp.lowerNamePlate == null)
                    {
                        comp.SetupLowerNameplate();
                    }
                }
            }, delegate
            {
                Config.Main.UseBelowNameplates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    var comp = p.GetComponent<BlazesPlayerInfo>();
                    if (comp.lowerNamePlate != null)
                    {
                        comp.DestroyLowerNameplate();
                    }
                }
            }, "Show the below nameplates that show ping and frames and such", Config.Main.UseBelowNameplates);

            new QMToggleButton(nameplates, 1, 1, "Trust Rank", delegate
            {
                Config.Main.NamePlateTrustRank = true;
            }, delegate
            {
                Config.Main.NamePlateTrustRank = false;
            }, "Show the user's trust rank on the lower nameplate", Config.Main.NamePlateTrustRank);

            new QMToggleButton(nameplates, 2, 1, "Platform", delegate
            {
                Config.Main.NamePlatePlatform = true;
            }, delegate
            {
                Config.Main.NamePlatePlatform = false;
            }, "Show the user's platform on the lower nameplate", Config.Main.NamePlatePlatform);

            new QMToggleButton(nameplates, 3, 1, "Ping", delegate
            {
                Config.Main.NamePlatePing = true;
            }, delegate
            {
                Config.Main.NamePlatePing = false;
            }, "Show the user's ping on the lower nameplate", Config.Main.NamePlatePing);

            new QMToggleButton(nameplates, 4, 1, "Frames", delegate
            {
                Config.Main.NamePlateFrames = true;
            }, delegate
            {
                Config.Main.NamePlateFrames = false;
            }, "Show the user's frames on the lower nameplate", Config.Main.NamePlateFrames);

            new QMToggleButton(nameplates, 1, 2, "Master Tag", delegate
            {
                Config.Main.NamePlateMasterTag = true;
            }, delegate
            {
                Config.Main.NamePlateMasterTag = false;
            }, "Show if the user is the current Master of the instance on the lower nameplate", Config.Main.NamePlateMasterTag);

            new QMToggleButton(nameplates, 2, 2, "Friends Tag", delegate
            {
                Config.Main.NamePlateFriendsTag = true;
            }, delegate
            {
                Config.Main.NamePlateFriendsTag = false;
            }, "SHow if the user is your friend on the lower nameplate", Config.Main.NamePlateFriendsTag);
            #endregion

            new QMToggleButton(BCUI, 3, 0, "VR Player List", delegate
            {
                Config.Main.PlayerList = true;
                QMAddons.playerList.SetActive(true);
            }, delegate
            {
                Config.Main.PlayerList = false;
                QMAddons.playerList.SetActive(false);
            }, "Toggles the in menu playerlist", Config.Main.PlayerList);

            new QMToggleButton(BCUI, 4, 0, "VR Debug Panel", delegate
            {
                Config.Main.DebugPanel = true;
                QMAddons.DebugPanel.SetActive(true);
            }, delegate
            {
                Config.Main.DebugPanel = false;
                QMAddons.DebugPanel.SetActive(false);
            }, "Toggles the in menu debug panel", Config.Main.DebugPanel);

            new QMToggleButton(BCUI, 3, 1, "IRL Clock", delegate
            {
                Config.Main.IRLClock = true;
                QMAddons.Panel.SetActive(true);
            }, delegate
            {
                Config.Main.IRLClock = false;
                QMAddons.Panel.SetActive(false);
            }, "Toggle the in menu irl clock", Config.Main.IRLClock);

            new QMToggleButton(BCUI, 4, 1, "Disable Avatar Preview", delegate
            {
                Config.Main.DisableAvatarPreview = true;
                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(false);
            }, delegate
            {
                Config.Main.DisableAvatarPreview = false;
                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(true);
            }, "Toggle showing the avatar preview in the avatars menu", Config.Main.DisableAvatarPreview);
            if (Config.Main.DisableAvatarPreview) GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(false);
            #endregion

            new QMToggleButton(Settings, 3, 1, "ComfyVR Menu", delegate
            {
                Config.Main.ComfyVRMenu = true;
            }, delegate
            {
                Config.Main.ComfyVRMenu = false;
            }, "Place the Big Social Menu infront of your face no matter where you are looking", Config.Main.ComfyVRMenu);

            new QMToggleButton(Settings, 1, 2, "Confirm Quit", delegate
            {
                Config.Main.ConfirmQuit = true;
            }, delegate
            {
                Config.Main.ConfirmQuit = false;
            }, "Makes a Confirmation Popup show before trying to force quit or restart", Config.Main.ConfirmQuit);
            #endregion

            #region Selected Menu
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
                BlazeInfo.SelectedPlayer = PlayerUtils.GetSelectedUser()._player;
                Selected.OpenMe();
            });

            var otherSelectedButton = new QMSingleButton("Menu_SelectedUser_Remote", 0, 0, "", delegate
            {
                BlazeInfo.SelectedPlayer = PlayerUtils.GetSelectedUser()._player;
                Selected.OpenMe();
            }, "Target functions for Blaze's Client");

            new QMSingleButton(Selected, 1, 0, "Set As\nTarget", delegate
            {
                BlazeInfo.Target = BlazeInfo.SelectedPlayer;
            }, "Select this user as your target for mod needed mod functions");

            new QMSingleButton(Selected, 2, 0, "Copy\nUser ID", delegate
            {
                Clipboard.SetText(BlazeInfo.SelectedPlayer.GetUserID());
            }, "Copy the targets user id to your clipboard");

            new QMSingleButton(Selected, 3, 1, "Teleport To", delegate
            {
                Functions.TeleportByUserID(BlazeInfo.SelectedPlayer.GetUserID());
            }, "Teleport to the target");

            new QMSingleButton(Selected, 4, 1, "Force\nClone", delegate
            {
                if (BlazeInfo.SelectedPlayer.GetUserID() == PlayerUtils.CurrentUser().GetUserID())
                {
                    PopupUtils.InformationAlert("You cannot clone yourself!");
                }
                else
                {
                    if (BlazeInfo.SelectedPlayer.GetPCAvatar().releaseStatus == "private")
                    {
                        PopupUtils.InformationAlert("You cannot clone that avatar it is private!");
                    }
                    else
                    {
                        AvatarUtils.ChangeToAvatar(BlazeInfo.SelectedPlayer.GetPCAvatar().id);
                    }
                }
            }, "Forcefully clone a user if their avatar is public");

            new QMSingleButton(Selected, 1, 2, "Download\nVRCA", delegate
            {
                DownloadManager.DownloadVRCA(BlazeInfo.SelectedPlayer.GetPCAvatar());
            }, "Download the target's currenty avatar to your VRCA's Folder");

            new QMSingleButton(Selected, 4, 2, "Crash Portal", delegate
            {
                Functions.DropCrashPortal(BlazeInfo.Target._vrcplayer);
            }, "Drops a crash portal on the target", null, true);

            new QMSingleButton(Selected, 4, 2.5f, "Portal Drop", delegate
            {
                Functions.DropPortal("wrld_1668a4c1-15f7-419a-adcf-f238dc30224a", BlazeInfo.Target._vrcplayer);
            }, "Drops a regular portal on the user", null, true);
            #endregion

            #region Wings Menu
            APIStuff.OnWingInit += new Action<BaseWing>(wing => 
            {
                var WMain = new WingPage(wing, "Blaze's Client");

                var WQuickAccess = new WingPage(WMain, "Functions", 0);
                new WingButton(WQuickAccess, "<color=yellow>Restart</color>", 0, delegate
                {
                    if (Config.Main.ConfirmQuit)
                    {
                        PopupUtils.AlertV2("Are you sure you want to restart?", "Restart", Functions.Restart);
                    }
                    else
                    {
                        Functions.Restart();
                    }
                });
                new WingButton(WQuickAccess, "<color=red>Quit</color>", 1, delegate
                {
                    if (Config.Main.ConfirmQuit)
                    {
                        PopupUtils.AlertV2("Are you sure you want to restart?", "Restart", Functions.Restart);
                    }
                    else
                    {
                        Functions.Restart();
                    }
                });
            });
            #endregion
        }
    }
}
