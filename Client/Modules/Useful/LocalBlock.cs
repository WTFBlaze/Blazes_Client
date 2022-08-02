using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Modules
{
    class LocalBlock : BModule
    {
        private QMNestedButton Menu;
        private QMNestedButton Selected;
        private QMScrollMenu Scroll;
        private QMInfo Info;
        private ModLocalBlock selectedLB;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Protections, "Local Blocks", 1, 2, "View all user's you have locally blocked", "Local Blocks");
            Selected = new QMNestedButton(Menu, "", 0, 0, "", "LB - Selected");
            Selected.GetMainButton().SetActive(false);
            Scroll = new QMScrollMenu(Menu);
            Info = new QMInfo(Selected, 0, 0, 950, 250, "")
            {
                InfoText =
                {
                    color = Color.white,
                    fontSize = 30,
                    alignment = TextAnchor.MiddleCenter,
                    supportRichText = true,
                }
            };

            new QMSingleButton(Menu, 4, 0, "Add User\nBy ID", delegate
            {
                PopupUtils.InputPopup("Block", "Enter VRC User ID...", delegate (string s)
                {
                    if (!RegexManager.IsValidUserID(s))
                    {
                        PopupUtils.HideCurrentPopUp();
                        PopupUtils.InformationAlert("Please provide a valid VRChat User ID to add to the local block list!");
                    }
                    else
                    {
                        if (s == PlayerUtils.CurrentUser().GetUserID())
                        {
                            PopupUtils.InformationAlert("You cannot add yourself to the local block list!");
                        }
                        else
                        {
                            if (Config.LocalBlock.list.Exists(x => x.UserID == s))
                            {
                                PopupUtils.InformationAlert("That user is already on your local block list!");
                            }
                            else
                            {
                                APIUser.FetchUser(s, new Action<APIUser>(user =>
                                {
                                    Config.LocalBlock.list.Add(new Utils.Objects.ModObjects.ModLocalBlock
                                    {
                                        UserID = s,
                                        BlockDate = DateTime.Now,
                                        DisplayName = user.displayName
                                    });
                                    Config.LocalBlock.Save();
                                    RefreshPlayers();
                                    Scroll.Refresh();
                                }), new Action<string>(error =>
                                {
                                    PopupUtils.InformationAlert("There was a problem adding that user to your local blocks list! Perhaps they don't exist?");
                                }));
                            }
                        }
                    }
                });
            }, "Adds a user to your local block via their user id");

            new QMSingleButton(Menu, 4, 3, "<color=red>WIPE\nBLOCKS</color>", delegate
            {
                PopupUtils.AlertV2("Are you sure you want to wipe all users off your local block list? This CANNOT be undone!", "Wipe", delegate
                {
                    Config.LocalBlock.list.Clear();
                    Config.InstanceHistory.Save();
                    RefreshPlayers();
                    Scroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Wipes your entire Local Blocks list!");

            new QMSingleButton(Selected, 1, 0, "Remove\nBlock", delegate
            {
                Config.LocalBlock.list.Remove(selectedLB);
                Config.LocalBlock.Save();
                RefreshPlayers();
                Selected.CloseMe();
                Scroll.Refresh();
            }, "Click to remove this user from the blocked list!");

            Scroll.SetAction(delegate
            {
                foreach (var b in Config.LocalBlock.list)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, b.DisplayName, delegate
                    {
                        selectedLB = b;
                        StringBuilder sb = new();
                        sb.AppendLine("<color=magenta>Display Name:</color> " + b.DisplayName);
                        sb.AppendLine("<color=magenta>User ID:</color>" + b.UserID);
                        sb.AppendLine("<color=magenta>Date Blocked:</color>" + b.BlockDate.ToString("MM/dd/yyyy - hh:mm tt"));
                        Info.SetText(sb.ToString());
                        Selected.OpenMe();
                    }, "Click to view more info about this user!"));
                }
            });

            new QMSingleButton(BlazeMenu.Selected, 2, 2, "Add To\nLocal Block", delegate
            {
                if (IsOnLocalBlock(BlazeInfo.SelectedPlayer.GetUserID()))
                {
                    PopupUtils.InformationAlert("That user is already on your local blocks list!");
                }
                else
                {
                    Config.LocalBlock.list.Add(new ModLocalBlock
                    {
                        BlockDate = DateTime.Now,
                        DisplayName = BlazeInfo.SelectedPlayer.GetDisplayName(),
                        UserID = BlazeInfo.SelectedPlayer.GetUserID()
                    });
                    Config.LocalBlock.Save();
                    PopupUtils.InformationAlert($"Successfully added [{BlazeInfo.SelectedPlayer.GetDisplayName()}] to your local block list!");
                    RefreshPlayers();
                }
            }, "Add this user to your local block system! (locally hides and mutes them without using VRChat's API meaning it won't affect their trust rank) [Yes they can still see and hear you]");
        }

        public override void PlayerJoined(VRC.Player player)
        {
            if (Config.LocalBlock.list.Exists(x => x.UserID == player.GetUserID()))
            {
                player.gameObject.SetActive(false);
            }
        }

        private void RefreshPlayers()
        {
            foreach (var p in WorldUtils.GetPlayers())
            {
                if (IsOnLocalBlock(p.GetUserID()))
                {
                    p.gameObject.SetActive(false);
                }
                else
                {
                    p.gameObject.SetActive(true);
                }
            }
        }

        internal static bool IsOnLocalBlock(string userid)
        {
            if (Config.LocalBlock.list.Exists(x => x.UserID == userid)) return true;
            return false;
        }
    }
}
