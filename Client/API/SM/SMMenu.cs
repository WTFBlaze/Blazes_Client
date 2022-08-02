using Blaze.Utils;
using Blaze.Utils.API;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Blaze.API.SM.SMButton;

namespace Blaze.API.SM
{
    internal class SMMenu
    {
        public SMMenu(Transform Parent, SMButtonType buttonType, string name, float x, float y, float sizeX = 1f, float sizeY = 1f, Action OnPageShown = null, Action OnPageClose = null)
        {
            try
            {
                Instance = this;
                GameObject SettingsPage = GameObject.Find("UserInterface/MenuContent/Screens/Settings");
                Page = UnityEngine.Object.Instantiate(SettingsPage, SettingsPage.transform.parent);
                Page.name = $"{BlazesAPI.Identifier}-SMMenu-{APIStuff.RandomNumbers()}";
                VRCUiPage = Page.GetComponent<VRCUiPageSettings>();
                Page.GetComponent<VRCUiPageSettings>().enabled = true;
                VRCUiPage.field_Public_Action_0 = new Action(() =>
                {
                    Page.active = true;
                    Page.SetActive(true);
                    Page.GetComponent<VRCUiPageSettings>().enabled = true;
                    Page.GetComponent<VRCUiPageSettings>().field_Protected_Boolean_0 = true;
                    VRCUiPage.enabled = true;
                    VRCUiPage.field_Protected_Boolean_0 = true;
                    OnPageShown?.Invoke();
                });
                VRCUiPage.field_Public_Action_1 = new Action(() =>
                {
                });
                Il2CppSystem.Collections.IEnumerator enumerator = Page.transform.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Il2CppSystem.Object obj = enumerator.Current;
                    Transform btnEnum = obj.Cast<Transform>();
                    if (!btnEnum.name.ToLower().Contains("depth"))
                        UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }
            catch (Exception e) { Logs.Error("SMMenu init", e); }
            try
            {
                MenuButton = new SMButton(buttonType, Parent, x, y, name, new Action(() =>
                {
                    if (Instance.VRCUiPage != null)
                    {
                        VRCUiManager.prop_VRCUiManager_0.ShowScreen(Instance.VRCUiPage);
                    }
                    else
                    {
                        Console.WriteLine("Day Your Fucked mate");
                    }
                }), sizeX, sizeY);
            }
            catch (Exception e) { Logs.Error("VRCMenu 2 init", e); }
        }

        public SMMenu(string name, Action OnPageShown = null, Action OnPageClose = null)
        {
            try
            {
                Instance = this;
                GameObject SettingsPage = GameObject.Find("UserInterface/MenuContent/Screens/Settings");
                Page = GameObject.Instantiate(SettingsPage, SettingsPage.transform.parent);
                Page.name = $"{BlazesAPI.Identifier}-SMMenu-{APIStuff.RandomNumbers()}";
                //UnityEngine.Object.Destroy(Page.GetComponent<PageAvatar>());
                //VRCUiPage = Page.AddComponent<VRCUiPage>();
                VRCUiPage = Page.GetComponent<VRCUiPageSettings>();
                Page.GetComponent<VRCUiPageSettings>().enabled = true;
                //VRCUiPage.field_Protected_CanvasGroup_0 = Page.GetComponent<CanvasGroup>();
                //VRCUiPage.screenType = "SCREEN";
                VRCUiPage.field_Public_Action_0 = new Action(() =>
                {
                    Page.active = true;
                    Page.SetActive(true);
                    Page.GetComponent<VRCUiPageSettings>().enabled = true;
                    Page.GetComponent<VRCUiPageSettings>().field_Protected_Boolean_0 = true;
                    VRCUiPage.enabled = true;
                    VRCUiPage.field_Protected_Boolean_0 = true;
                    OnPageShown?.Invoke();
                    //VRCUiCursorManager.Method_Public_Static_Void_Boolean_0(true);
                });
                VRCUiPage.field_Public_Action_1 = new Action(() =>
                {
                });
                Il2CppSystem.Collections.IEnumerator enumerator = Page.transform.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Il2CppSystem.Object obj = enumerator.Current;
                    Transform btnEnum = obj.Cast<Transform>();
                    if (!btnEnum.name.ToLower().Contains("depth"))
                        UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }
            catch (Exception e) { Logs.Error("VRCMenu Init", e); }
        }

        public string ScreenPath { get; set; }
        public SMButton MenuButton { get; set; }
        public GameObject Page { get; set; }
        public VRCUiPage VRCUiPage { get; set; }
        public SMMenu Instance { get; set; }
    }
}
