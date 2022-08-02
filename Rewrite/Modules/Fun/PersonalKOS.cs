using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;
using static Blaze.Config.KOSConfig;

namespace Blaze.Modules
{
    public class PersonalKOS : BModule
    {
        private static QMNestedButton Menu;
        private static QMNestedButton SelectedMenu;
        private static QMScrollMenu Scroll;
        private static QMInfo SelectedInfo;
        private static KosObject SelectedKOS;
        private static QMNestedButton Options;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazePersonalKOS>();
        }

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Exploits, "Personal\nKOS", 4, 1, "View your personal KOS list", "Personal KOS");
            SelectedMenu = new QMNestedButton(Menu, "", 0, 0, "", "Selected KOS");
            SelectedMenu.GetMainButton().SetActive(false);
            Scroll = new QMScrollMenu(Menu);
            SelectedInfo = new QMInfo(SelectedMenu, 0, -150, 950, 250, "")
            {
                InfoText =
                {
                    color = Color.white,
                    fontSize = 35
                },
            };

            new QMSingleButton(SelectedMenu, 1, 0, "Remove\nFrom KOS", delegate
            {
                Config.KOS.list.Remove(SelectedKOS);
                Logs.Log($"[KOS] Removed {SelectedKOS.DisplayName} from your Personal KOS!");
                Logs.Debug($"<color=red>[KOS]</color> Removed <color={Colors.ModUserHex}>{SelectedKOS.DisplayName}</color> from KOS List!");
                SelectedMenu.CloseMe();
                Scroll.Refresh();
            }, "Click to remove this user from your kos list");

            Scroll.SetAction(delegate
            {
                foreach (var i in Config.KOS.list)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, i.DisplayName, delegate
                    {
                        SelectedKOS = i;
                        var sb = new StringBuilder();
                        sb.AppendLine("<color=magenta>Name:</color> " + i.DisplayName);
                        sb.AppendLine("<color=magenta>ID:</color> " + i.UserID);
                        sb.AppendLine("<color=magenta>Date Added:</color> " + i.DateAddedToKos.ToString("MM/dd/yyyy - hh:ss tt"));
                        SelectedInfo.SetText(sb.ToString());
                        SelectedMenu.OpenMe();
                    }, "Click to view more info about this kos user"));
                }
            });

            new QMSingleButton(BlazeQM.Selected, 3, 2, "Add To KOS", delegate
            {
                if (Config.KOS.list.Exists(x => x.UserID == Main.SelectedPlayer.field_Private_APIUser_0.id))
                {
                    PopupUtils.InformationAlert("That user is already on your kos list!");
                    return;
                }
                Config.KOS.list.Add(new KosObject()
                {
                    DateAddedToKos = DateTime.Now,
                    DisplayName = Main.SelectedPlayer.field_Private_APIUser_0.displayName,
                    UserID = Main.SelectedPlayer.field_Private_APIUser_0.id
                });
                Logs.Log($"[KOS] Added {Main.SelectedPlayer.field_Private_APIUser_0.displayName} to your Personal KOS List!");
                Logs.Debug($"<color=red>[KOS]</color> Added <color={Colors.ModUserHex}>{Main.SelectedPlayer.field_Private_APIUser_0.displayName}</color> to KOS List!");
            }, "Click to add this user to your personal kos list");

            Options = new QMNestedButton(Menu, "Options", 4, 0, "Click to change how Personal KOS operates", "KOS - Options");
            new QMToggleButton(Options, 1, 0, "Personal KOS", delegate
            {
                Main.BlazesComponents.AddComponent<BlazePersonalKOS>();
            }, delegate
            {
                UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazePersonalKOS>());
            }, "Click to toggle personal kos on or off", Config.Main.PersonalKOS);
            if (Config.Main.PersonalKOS)
            {
                Main.BlazesComponents.AddComponent<BlazePersonalKOS>();
            }

            new QMToggleButton(Options, 2, 0, "Portals", delegate
            {
                Config.Main.KOSPortals = true;
            }, delegate
            {
                Config.Main.KOSPortals = false;
            }, "Click to enable dropping portals on the KOS user", Config.Main.KOSPortals);

            new QMToggleButton(Options, 3, 0, "Udon Nuke", delegate
            {
                Config.Main.KOSUdonNuke = true;
            }, delegate
            {
                Config.Main.KOSUdonNuke = false;
            }, "Click to enable Targeted Udon Nuking on the KOS user", Config.Main.KOSUdonNuke);
        }
    }

    public class BlazePersonalKOS : MonoBehaviour
    {
        public BlazePersonalKOS(IntPtr id) : base(id) { }
        private float delay = 6f;
        private UdonBehaviour selectedScript;

        public void Awake() => delay = 0f;

        public void Update()
        {
            try
            {
                if (WorldUtils.IsInRoom())
                {
                    delay -= Time.deltaTime;
                    // Enable & Disable Infinite Portals
                    if (KOSInRoom() == 0)
                    {
                        Patching.Toggles.InfinitePortals = true;
                    }
                    else
                    {
                        Patching.Toggles.InfinitePortals = false;
                    }
                    // Run Functions
                    if (delay <= 0f)
                    {
                        if (Config.Main.KOSPortals)
                        {
                            foreach (var p in WorldUtils.GetPlayers())
                            {
                                if (p.IsKOS() && p.GetUserID() != APIUser.CurrentUser.id)
                                {
                                    Functions.DropPortal("wrld_1668a4c1-15f7-419a-adcf-f238dc30224a", "[BLAZESCLIENT] - KOS", -69, PlayerUtils.CurrentUser().transform.position + PlayerUtils.CurrentUser().transform.forward * 2f, PlayerUtils.CurrentUser().transform.rotation);
                                }
                            }
                        }

                        if (Config.Main.KOSUdonNuke && WorldUtils.GetSDKType() == "SDK3")
                        {
                            foreach (var p in WorldUtils.GetPlayers())
                            {
                                if (p.IsKOS() && p.GetUserID() != APIUser.CurrentUser.id)
                                {
                                    foreach (var u in WorldUtils.GetUdonScripts())
                                    {
                                        selectedScript = u;
                                        foreach (var e in selectedScript._eventTable)
                                        {
                                            Trigger(e.key, p);
                                        }
                                    }
                                }
                            }
                        }

                        if (Config.Main.KOSItemSteal)
                        {
                            foreach (var p in WorldUtils.GetPlayers())
                            {
                                if (p.IsKOS())
                                {
                                    foreach (VRC_Pickup vrc_Pickup in Main.Pickups)
                                    {
                                        if (Networking.GetOwner(vrc_Pickup.gameObject) == p.GetVRCPlayerApi())
                                        {
                                            Networking.SetOwner(Networking.LocalPlayer, vrc_Pickup.gameObject);
                                        }
                                        vrc_Pickup.DisallowTheft = true;
                                    }
                                }
                            }
                        }
                        delay = 6f;
                    }
                }
            }
            catch { }
        }

        private int KOSInRoom()
        {
            int result = 0;
            foreach (var p in WorldUtils.GetPlayers())
            {
                if (p.IsKOS())
                {
                    result++;
                }
            }
            return result;
        }

        private void Trigger(string eventName, Player target)
        {
            if (eventName.StartsWith("_") || !Config.Main.KOSUdonNuke) return;
            Networking.SetOwner(target.field_Private_VRCPlayerApi_0, selectedScript.gameObject);
            selectedScript.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, eventName);
        }
    }
}
