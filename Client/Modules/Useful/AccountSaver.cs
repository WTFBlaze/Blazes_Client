using Blaze.API.SM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace Blaze.Modules
{
    class AccountSaver : BModule
    {
        public static System.Collections.IEnumerator Login(string username, string password)
        {
            if (APIUser.IsLoggedIn)
            {
                Logs.Log($"[AccountSaver] Logging out...");
                APIUser.Logout();
            }
            VRCUiPage page = GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass").GetComponent<VRCUiPage>();
            VRCUiManager.prop_VRCUiManager_0.ShowScreen(page);
            yield return new WaitForSeconds(0.7f);
            foreach (VRCUiPopupInput input in Resources.FindObjectsOfTypeAll<VRCUiPopupInput>())
            {
                input.field_Public_InputField_0.text = username;
                input.OnRightButton();
            }
            yield return new WaitForSeconds(1.3f);
            foreach (VRCUiPopupInput input in Resources.FindObjectsOfTypeAll<VRCUiPopupInput>())
            {
                input.field_Public_InputField_0.text = password;
                input.OnRightButton();
            }
            yield break;
        }

        public override void Start()
        {
            Logs.Log("[AccountSaver] Initializing...", ConsoleColor.Yellow);
            MelonCoroutines.Start(WaitForUI());
        }

        private static IEnumerator WaitForUI()
        {
            while (GameObject.Find("UserInterface/MenuContent/Screens/Authentication") is null) yield return null;
            InitUI();
        }

        private static void InitUI()
        {
            if (!File.Exists(ModFiles.AccountSaverFile))
            {
                FileManager.CreateFile(ModFiles.AccountSaverFile);
            }
            VRCUiPage LoginPromt = GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt").GetComponent<VRCUiPage>();
            SMMenu Accounts = new(LoginPromt.transform, SMButton.SMButtonType.ChangeAvatar, "Accounts", -625.5f, 371.5f, 1.3f, 1);
            new SMButton(SMButton.SMButtonType.ChangeAvatar, Accounts.Page.transform, -625.5f, 371.5f, "Return", new Action(() =>
            {
                VRCUiManager.prop_VRCUiManager_0.ShowScreen(LoginPromt);
            }), 1.3f, 1);
            List<SMButton> accountsButtons = new();
            string[] accounts = File.ReadAllLines(ModFiles.AccountSaverFile);
            float x = -625.5f;
            float y = 271.5f;
            Logs.Log($"[AccountSaver] Found {accounts.Length} Accounts Creating Buttons...", ConsoleColor.Cyan);
            foreach (var line in accounts)
            {
                var split = line.Split(':');
                accountsButtons.Add(new SMButton(SMButton.SMButtonType.ChangeAvatar, Accounts.Page.transform, x, y, split[0], new Action(() =>
                {
                    MelonCoroutines.Start(Login(split[0], split[1]));
                }), 1.3f, 1));
                x += 200f;
                if (x > 650)
                {
                    x = -625.5f;
                    y -= 100;
                }
                if (y < -500)
                {
                    Logs.Error("[AccountSaver] Found To many Accounts in File!");
                    break;
                }
            }
            Logs.Log("[AccountSaver] Done!", ConsoleColor.Green);
        }
    }
}
