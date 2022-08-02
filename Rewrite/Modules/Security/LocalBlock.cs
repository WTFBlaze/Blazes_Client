using Blaze.API.QM;
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
    public class LocalBlock : BModule
    {
        private QMNestedButton Menu;
        private QMNestedButton Selected;
        private QMScrollMenu Scroll;
        private QMInfo Info;
        private ModLocalBlock selectedLB;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Security, "Local Blocks", 4, 2, "View all user's you have locally blocked", "Local Blocks");
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
                            if (Config.Blocks.list.Exists(x => x.UserID == s))
                            {
                                PopupUtils.InformationAlert("That user is already on your local block list!");
                            }
                            else
                            {
                                APIUser.FetchUser(s, new Action<APIUser>(user =>
                                {
                                    Config.Blocks.list.Add(new Utils.Objects.ModObjects.ModLocalBlock
                                    {
                                        UserID = s,
                                        BlockDate = DateTime.Now,
                                        DisplayName = user.displayName
                                    });
                                    Config.Blocks.Save();
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
                    Config.Blocks.list.Clear();
                    Config.InstanceHistory.Save();
                    RefreshPlayers();
                    Scroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Wipes your entire Local Blocks list!");

            new QMSingleButton(Selected, 1, 0, "Remove\nBlock", delegate
            {
                Config.Blocks.list.Remove(selectedLB);
                Config.Blocks.Save();
                RefreshPlayers();
                Selected.CloseMe();
                Scroll.Refresh();
            }, "Click to remove this user from the blocked list!");

            Scroll.SetAction(delegate
            {
                foreach (var b in Config.Blocks.list)
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

            new QMSingleButton(BlazeQM.Selected, 1, 2, "Add To\nLocal Block", delegate
            {
                if (IsOnLocalBlock(Main.SelectedPlayer.GetUserID()))
                {
                    PopupUtils.InformationAlert("That user is already on your local blocks list!");
                }
                else
                {
                    Config.Blocks.list.Add(new ModLocalBlock
                    {
                        BlockDate = DateTime.Now,
                        DisplayName = Main.SelectedPlayer.GetDisplayName(),
                        UserID = Main.SelectedPlayer.GetUserID()
                    });
                    Config.Blocks.Save();
                    PopupUtils.InformationAlert($"Successfully added [{Main.SelectedPlayer.GetDisplayName()}] to your local block list!");
                    RefreshPlayers();
                }
            }, "Add this user to your local block system! (locally hides and mutes them without using VRChat's API meaning it won't affect their trust rank) [Yes they can still see and hear you]");
        }

        public override void PlayerJoined(VRC.Player player)
        {
            if (Config.Blocks.list.Exists(x => x.UserID == player.GetUserID()))
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

        public static bool IsOnLocalBlock(string userid)
        {
            if (Config.Blocks.list.Exists(x => x.UserID == userid)) return true;
            return false;
        }
    }
}
