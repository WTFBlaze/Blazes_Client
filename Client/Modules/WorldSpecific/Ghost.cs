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
    class Ghost : BModule
    {
        private string GhostWorldID = "wrld_0ec97c4f-1e84-4a3a-9e3a-fa3075b6c56d";
        private QMNestedButton Menu;

        public override void QuickMenuUI()
        {
            //Local_HumanWin

            Menu = new QMNestedButton(BlazeMenu.WorldSpecific, "Ghost", 2, 0, "Ghost Game Specific Functions", "WS - Ghost");

            new QMSingleButton(Menu, 1, 0, "<color=green>Start\nGame</color>", delegate
            {
                GhostCommand("Local_ReadyStartGame");
            }, "Forcefully start the game");

            new QMSingleButton(Menu, 2, 0, $"<color={BlazeInfo.ModColor1}>Humans\nWin</color>", delegate
            {
                //GhostCommand("Local_GhostWin");
                GhostCommand("Local_HumanWin");
            }, "Force the Humans to win the round");

            new QMSingleButton(Menu, 3, 0, "<color=red>Ghosts\nWin</color>", delegate
            {
                //GhostCommand("Local_HumanWin");
                GhostCommand("Local_GhostWin");
            }, "Force the Ghosts to win the round");

            new QMSingleButton(Menu, 4, 0, "<color=#3a088a>Kill\nEveryone</color>", KillEverybody, "Kill's Everybody alive in the round");

            //new QMSingleButton(Menu, 1, 1, "<color=#fcba03>Give\nMoney</color>", GiveMoney, "Gives Yourself money");
            //new QMSingleButton(Menu, 1, 1, "<color=#fcba03>Unlock\nLocks</color>", UnlockAllLocks, "Unlocks all locks");
            //new QMSingleButton(Menu, 2, 1, "<color=#fc0377>Craft All\nWeapons</color>", CraftAllWeapons, "Force start crafting all weapons");
        }

        private void GhostCommand(string command)
        {
            if (WorldUtils.CurrentWorld().id != GhostWorldID)
            {
                Logs.HUD("<color=red>You are not in a Ghost World!</color>", 3);
                Logs.Log("[WORLD SPECIFIC] You are not in a Ghost World!", ConsoleColor.Red);
                return;
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.Contains("LobbyManager"))
                {
                    gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, command);
                }
            }
        }

        private void UnlockAllLocks()
        {
            if (WorldUtils.CurrentWorld().id != GhostWorldID)
            {
                Logs.HUD("<color=red>You are not in a Ghost World!</color>", 3);
                Logs.Log("[WORLD SPECIFIC] You are not in a Ghost World!", ConsoleColor.Red);
                return;
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith("Lock"))
                {
                    gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "Local_Unlock");
                }
            }
        }

        private void KillEverybody()
        {
            if (WorldUtils.CurrentWorld().id != GhostWorldID)
            {
                Logs.HUD("<color=red>You are not in a Ghost World!</color>", 3);
                Logs.Log("[WORLD SPECIFIC] You are not in a Ghost World!", ConsoleColor.Red);
                return;
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith("PlayerCharacterObject"))
                {
                    gameObject.transform.Find("DamageSync").gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "HitDamage52");
                    gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "Net_Killer0");
                }
            }
        }

        private void CraftAllWeapons()
        {
            var obj = GameObject.Find("Weapons-InGame");
            for (int i = 0; i > GameObject.Find("Weapons-InGame").transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "Local_StartCraft");
            }

            //child.gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "Local_StartCraft");

            /*foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith("PlayerCharacterObject"))
                {
                    gameObject.transform.Find("DamageSync").gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "HitDamage52");
                    gameObject.GetComponent<VRC.Udon.UdonBehaviour>().SendCustomNetworkEvent(0, "Net_Killer0");
                }
            }*/
        }

        private void GiveMoney()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.Contains("GameManager"))
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "ForceAltCurrency");
                }
            }
        }
    }
}
