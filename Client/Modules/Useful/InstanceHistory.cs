using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
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
    class InstanceHistory : BModule
    {
        private static QMNestedButton Menu;
        private static QMScrollMenu Scroll;
        private static QMNestedButton SelectedMenu;
        private static QMInfo SelectedPanel;
        private static ModInstanceHistory Selected;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Worlds, "Instance\nHistory", 4, 2, "View and rejoin previous worlds you have been to", "Instance History");
            Scroll = new QMScrollMenu(Menu);
            SelectedMenu = new QMNestedButton(Menu, "", 0, 0, "", "Selected Instance");
            SelectedMenu.GetMainButton().SetActive(false);

            SelectedPanel = new QMInfo(SelectedMenu, 0, 0, 950, 250, "")
            {
                InfoText =
                {
                    color = Color.white,
                    fontSize = 30,
                    alignment = TextAnchor.MiddleCenter,
                    supportRichText = true,
                }
            };

            new QMSingleButton(Menu, 4, 0, "<color=yellow>Clear\nHistory</color>", delegate
            {
                PopupUtils.AlertV2("Are you sure you want to clear your instance history? This cannot be reversed!", "Clear", delegate
                {
                    Config.InstanceHistory.list.Clear();
                    Config.InstanceHistory.Save();
                    PopupUtils.HideCurrentPopUp();
                    Scroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Clears your Instance History list");

            new QMSingleButton(SelectedMenu, 1, 0, "Join\nInstance", delegate
            {
                WorldUtils.JoinRoom(Selected.JoinID);
            }, "Click to join the instance for yourself");

            new QMSingleButton(SelectedMenu, 2, 0, "Portal To\nInstance", delegate
            {
                Functions.DropPortal(Selected.JoinID);
            }, "Click to drop a portal for others to join the instance");

            new QMSingleButton(SelectedMenu, 4, 0, "Remove\nInstance", delegate
            {
                PopupUtils.AlertV2("Are you sure you would like to remove this instance from your history?", "Remove", delegate
                {
                    Config.InstanceHistory.list.Remove(Selected);
                    Config.InstanceHistory.Save();
                    PopupUtils.HideCurrentPopUp();
                    SelectedMenu.CloseMe();
                    Scroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Click to remove this instance from your history");

            Scroll.SetAction(delegate
            {
                foreach (var item in Config.InstanceHistory.list)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"{FriendlyName(item.Type)}\n{item.Name}", delegate
                    {
                        Selected = item;
                        StringBuilder sb = new();
                        sb.AppendLine("<color=magenta><b>World Name:</b></color> " + item.Name);
                        sb.AppendLine("<color=magenta><b>Instance ID:</b></color> " + item.JoinID);
                        sb.AppendLine("<color=magenta><b>Type:</b></color> " + item.Type.ToString());
                        SelectedPanel.SetText(sb.ToString());
                        SelectedMenu.OpenMe();
                    }, "Click to view options about this instance"));
                }
            });
        }

        private string FriendlyName(InstanceAccessType input)
        {
            return input switch
            {
                InstanceAccessType.Public => "[<color=green>PUB</color>]",
                InstanceAccessType.FriendsOfGuests => "[<color=yellow>FR+</color>]",
                InstanceAccessType.FriendsOnly => "[<color=yellow>FR</color>]",
                InstanceAccessType.InvitePlus => "[<color=orange>INV+</color>]",
                InstanceAccessType.InviteOnly => "[<color=red>INV</color>]",
                _ => "[<color=green>PUB</color>]",
            };
        }

        public override void LocalPlayerLoaded()
        {
            if (!Config.InstanceHistory.list.Exists(x => x.JoinID == WorldUtils.GetJoinID()))
            {
                Config.InstanceHistory.list.Insert(0, new ModInstanceHistory
                {
                    Name = WorldUtils.CurrentWorld().name,
                    JoinID = WorldUtils.GetJoinID(),
                    Type = WorldUtils.CurrentInstance().type
                });
                Config.InstanceHistory.Save();
            }
            else Config.InstanceHistory.list.MoveItemAtIndexToFront(Config.InstanceHistory.list.FindIndex(x => x.JoinID == WorldUtils.GetJoinID()));
        }
    }
}
