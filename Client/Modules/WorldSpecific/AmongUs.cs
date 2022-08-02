using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Udon;

namespace Blaze.Modules
{
    class AmongUs : BModule
    {
        private string AmongUsID = "wrld_dd036610-a246-4f52-bf01-9d7cea3405d7";
        private QMNestedButton Menu;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.WorldSpecific, "Among Us", 1, 0, "Click to view functions for Among Us", "WS - Among Us");

            new QMSingleButton(Menu, 1, 0, "<color=green>Start\nGame</color>", delegate
            {
                AmongUsCommand("Btn_Start");
            }, "Force Start the round");

            new QMSingleButton(Menu, 2, 0, "<color=red>Stop\nGame</color>", delegate
            {
                AmongUsCommand("SyncAbort");
            }, "Force Stop the round");

            new QMSingleButton(Menu, 3, 0, "<color=yellow>Imposters\nWin</color>", delegate
            {
                AmongUsCommand("SyncVictoryM");
            }, "Force Imposters to win the round");

            new QMSingleButton(Menu, 4, 0, $"<color={BlazeInfo.ModColor1}>Crewmates\nWin</color>", delegate
            {
                AmongUsCommand("SyncVictoryB");
            }, "Force Crewmates to win the round");

            new QMSingleButton(Menu, 1, 1, "<color=blue>Kill\nAll</color>", delegate
            {
                AmongUsCommand("KillLocalPlayer");
            }, "Kill All players in the round");

            new QMSingleButton(Menu, 2, 1, "<color=green>Call\nMeeting</color>", delegate
            {
                AmongUsCommand("StartMeeting");
            }, "Force Start a meeting");

            new QMSingleButton(Menu, 3, 1, "<color=yellow>Stop\nVoting</color>", delegate
            {
                AmongUsCommand("SyncCloseVoting");
            }, "Force Stop the voting");

            new QMSingleButton(Menu, 4, 1, $"<color={BlazeInfo.ModColor2}>All\nSkip", delegate
            {
                AmongUsCommand("Btn_SkipVoting");
            }, "Force everyone to vote skip");

            new QMSingleButton(Menu, 1, 2, "<color=red>Body\nFound</color>", delegate
            {
                AmongUsCommand("OnBodyWasFound");
            }, "Force a body to be found");

            new QMSingleButton(Menu, 2, 2, "<color=green>Complete\nTask</color>", delegate
            {
                AmongUsCommand("OnLocalPlayerCompletedTask");
            }, "Force complete task");
        }

        private void AmongUsCommand(string command)
        {
            if (WorldUtils.CurrentWorld().id != AmongUsID)
            {
                Logs.HUD("<color=red>You are not in an Among Us World!</color>", 3);
                Logs.Log("[WORLD SPECIFIC] You are not in an Among Us World!", ConsoleColor.Red);
                return;
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.Contains("Game Logic"))
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, command);
                }
            }
        }
    }
}
