/*using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Modules
{
    public class EventLocker : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;
        private QMNestedButton Selected;
        private string selectedPlayer;
        private QMToggleButton Event1Toggle;
        private QMToggleButton Event6Toggle;
        private QMToggleButton Event7Toggle;
        private QMToggleButton Event9Toggle;
        private QMNestedButton Global;
        public static bool GlobalLockE1;
        public static bool GlobalLockE6;
        public static bool GlobalLockE7;
        public static bool GlobalLockE9;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Security, "Event Locking", 1, 3, "Event Locking", "Event Locking");
            Scroll = new QMScrollMenu(Menu);
            Scroll.SetAction(delegate
            {
                foreach (var p in PhotonUtils.GetAllPhotonPlayers())
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, p.GetDisplayName(), delegate
                    {
                        if (p.GetUserID() == PlayerUtils.CurrentUser().GetUserID())
                        {
                            PopupUtils.InformationAlert("You cannot block events on yourself!", 3.5f);
                            return;
                        }
                        selectedPlayer = p.GetUserID();
                        if (Config.EventLocks.list.Exists(x => x.UserID == selectedPlayer))
                        {
                            var target = Config.EventLocks.list.Find(x => x.UserID == selectedPlayer);
                            Event1Toggle.SetToggleState(target.BlockedEvents.Contains(1));
                            Event6Toggle.SetToggleState(target.BlockedEvents.Contains(6));
                            Event7Toggle.SetToggleState(target.BlockedEvents.Contains(7));
                            Event9Toggle.SetToggleState(target.BlockedEvents.Contains(9));
                        }
                        else
                        {
                            Event1Toggle.SetToggleState(false);
                            Event6Toggle.SetToggleState(false);
                            Event7Toggle.SetToggleState(false);
                            Event9Toggle.SetToggleState(false);
                        }
                        Selected.OpenMe();
                    }, "Click to select this user!"));
                }
            });

            Selected = new QMNestedButton(Menu, "", 0, 0, "", "EL - Selected");
            Selected.GetMainButton().SetActive(false);
            Event1Toggle = new QMToggleButton(Selected, 1, 0, "Event 1", delegate
            {
                ToggleEvent(selectedPlayer, 1);
            }, delegate 
            {
                ToggleEvent(selectedPlayer, 1);
            }, "Click to block Event 1 (Voice Data) from this user");

            Event6Toggle = new QMToggleButton(Selected, 2, 0, "Event 6", delegate
            {
                ToggleEvent(selectedPlayer, 6);
            }, delegate
            {
                ToggleEvent(selectedPlayer, 6);
            }, "Click to block Event 6 (RPC) from this user");

            Event7Toggle = new QMToggleButton(Selected, 3, 0, "Event 7", delegate
            {
                ToggleEvent(selectedPlayer, 7);
            }, delegate
            {
                ToggleEvent(selectedPlayer, 7);
            }, "Click to block Event 7 (Movement) from this user");

            Event9Toggle = new QMToggleButton(Selected, 4, 0, "Event 9", delegate
            {
                ToggleEvent(selectedPlayer, 9);
            }, delegate
            {
                ToggleEvent(selectedPlayer, 9);
            }, "Click to block Event 9 (Player Sync) from this user");

            Global = new QMNestedButton(Menu, "Global\nLocks", 4, 0, "Globally Block Events for everyone (use for emergency situations)", "Global Locks");
            new QMToggleButton(Global, 1, 0, "Event 1", delegate
            {
                GlobalLockE1 = true;
            }, delegate
            {
                GlobalLockE1 = false;
            }, "Globally lock event 1 for everyone");

            new QMToggleButton(Global, 2, 0, "Event 6", delegate
            {
                GlobalLockE6 = true;
            }, delegate
            {
                GlobalLockE6 = false;
            }, "Globally lock event 6 for everyone");

            new QMToggleButton(Global, 3, 0, "Event 7", delegate
            {
                GlobalLockE7 = true;
            }, delegate
            {
                GlobalLockE7 = false;
            }, "Globally lock event 7 for everyone");

            new QMToggleButton(Global, 4, 0, "Event 9", delegate
            {
                GlobalLockE9 = true;
            }, delegate
            {
                GlobalLockE9 = false;
            }, "Globally lock event 9 for everyone");
        }

        private void ToggleEvent(string userID, byte eventNumber)
        {
            if (Config.EventLocks.list.Exists(x => x.UserID == userID))
            {
                var item = Config.EventLocks.list.Find(x => x.UserID == userID);
                if (item.BlockedEvents.Contains(eventNumber))
                {
                    item.BlockedEvents.Remove(eventNumber);
                }
                else
                {
                    item.BlockedEvents.Add(eventNumber);
                }

                if (item.BlockedEvents.Count == 0)
                {
                    Config.EventLocks.list.Remove(item);
                }

                Config.EventLocks.Save();
            }
            else
            {
                var plr = Functions.GetPlayerByUserID(userID);
                var item = new ModEventLocker()
                {
                    DisplayName = plr.GetDisplayName(),
                    UserID = plr.GetUserID(),
                    BlockedEvents = new List<byte>(),
                };
                item.BlockedEvents.Add(eventNumber);
                Config.EventLocks.list.Add(item);
                Config.EventLocks.Save();
            }
        }
    }
}
*/