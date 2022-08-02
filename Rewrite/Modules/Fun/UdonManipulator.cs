﻿using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.SDKBase;
using VRC.Udon;

namespace Blaze.Modules
{
    public class UdonManipulator : BModule
    {
        public static QMNestedButton Menu;
        public static QMNestedButton InvisMenu;
        public static QMNestedButton Settings;
        public static QMScrollMenu Scroll;
        public static QMScrollMenu Scroll2;
        public static bool NetworkEvents;
        public static bool TargetedEvents;
        private static UdonBehaviour selectedScript;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Worlds, "Udon\nIndexer", 3, 3, "View udon scripts and manipulate them how you want", "Udon Indexer");
            InvisMenu = new QMNestedButton(Menu, "", 0, 0, "", "Udon Manipulator");
            InvisMenu.GetMainButton().SetActive(false);
            Scroll = new QMScrollMenu(Menu);
            Scroll2 = new QMScrollMenu(InvisMenu);

            Settings = new QMNestedButton(Menu, "Settings", 4, 0, "Change how the Udon Manipulator works", "Udon Settings");
            new QMToggleButton(Settings, 1, 0, "Network Events", delegate
            {
                NetworkEvents = true;
            }, delegate
            {
                NetworkEvents = false;
            }, "Run any networkable events over the network so it affects everybody");

            new QMToggleButton(Settings, 2, 0, "Target Events", delegate
            {
                TargetedEvents = true;
            }, delegate
            {
                TargetedEvents = false;
            }, "Force any selected networkable events to only network to your target");

            Scroll2.SetAction(delegate
            {
                foreach (var e in selectedScript._eventTable)
                {
                    Scroll2.Add(new QMSingleButton(Scroll2.BaseMenu, 0, 0, e.key.StartsWith("_") ? $"<color=red>{e.key}</color>" : $"<color=green>{e.key}</color>", delegate
                    {
                        Trigger(e.key);
                    }, "Click me to trigger this event."));
                }
            });

            Scroll.SetAction(delegate
            {
                if (WorldUtils.GetSDKType() == "SDK3")
                {
                    int scriptCount = 0;
                    foreach (var u in WorldUtils.GetUdonScripts())
                    {
                        scriptCount++;
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"[<color=yellow>{scriptCount}</color>] {u.name}", delegate
                        {
                            selectedScript = u;
                            InvisMenu.GetMainButton().ClickMe();
                        }, "Click to view options for: " + u.name));
                    }
                }
                else
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "SDK2\nWorld", delegate { }, "This world is made in SDK2! No Udon here."));
                }
            });
        }

        private static void Trigger(string eventName)
        {
            if (NetworkEvents)
            {
                if (eventName.StartsWith("_"))
                {
                    Logs.Log("Cannot Network events that start with _", ConsoleColor.Red);
                    Logs.HUD("<color=red>Cannot Network events that start with _</color>", 2.5f);
                    return;
                }
                selectedScript.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, eventName);
                return;
            }
            if (TargetedEvents)
            {
                if (eventName.StartsWith("_"))
                {
                    Logs.Log("Cannot Network events that start with _", ConsoleColor.Red);
                    Logs.HUD("<color=red>Cannot Network events that start with _</color>", 2.5f);
                    return;
                }
                Networking.SetOwner(Main.Target.field_Private_VRCPlayerApi_0, selectedScript.gameObject);
                selectedScript.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, eventName);
                return;
            }
            selectedScript.SendCustomEvent(eventName);
        }
    }
}
