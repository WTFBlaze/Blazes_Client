using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using VRC.Core;
using VRC.UI;

namespace Blaze.Utils.VRChat
{
    internal static class SocialMenuUtils
    {
        internal static void CloseUI()
        {
            VRCUiManager.prop_VRCUiManager_0.CloseUI();
        }

        internal static void CloseUI(this VRCUiManager Instance, bool withFade = false)
        {
            try
            {
                Instance.Method_Public_Void_Boolean_Boolean_0();
                //Instance.Method_Public_Void_Boolean_Boolean_1();
            }
            catch { }
        }

        internal static void OpenUI()
        {
            VRCUiManager.prop_VRCUiManager_0.OpenUI(true);
        }

        internal static void OpenUI(this VRCUiManager Instance, bool showDefaultScreen = false, bool showBackdrop = true)
        {
            Instance.Method_Public_Void_Boolean_Boolean_2(showDefaultScreen, showBackdrop);
        }

        internal static void ShowScreen(this VRCUiManager Instance, VRCUiPage page)
        {
            ShowScreenActionAction(page);
        }

        internal static void ShowScreen(this VRCUiManager Instance, string pageName)
        {
            var vrcuiPage = Instance.GetPage(pageName);
            if (vrcuiPage != null)
            {
                Instance.ShowScreen(vrcuiPage);
            }
        }

        internal static VRCUiPage GetPage(this VRCUiManager Instance, string screenPath)
        {
            var gameObject = GameObject.Find(screenPath);
            VRCUiPage vrcuiPage = null;
            if (gameObject != null)
            {
                vrcuiPage = gameObject.GetComponent<VRCUiPage>();
                if (vrcuiPage == null)
                {
                    //MelonLogger.Error("Screen Not Found - " + screenPath);
                }
            }
            else
            {
                //MelonLogger.Warning("Screen Not Found - " + screenPath);
            }
            return vrcuiPage;
        }

        internal static ShowScreenAction ShowScreenActionAction
        {
            get
            {
                if (ourShowScreenAction != null)
                {
                    return ourShowScreenAction;
                }
                MethodInfo method = typeof(VRCUiManager).GetMethods(BindingFlags.Instance | BindingFlags.Public).Single(delegate (MethodInfo it)
                {
                    if (it.ReturnType == typeof(VRCUiPage) && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(VRCUiPage))
                    {
                        return XrefScanner.XrefScan(it).Any(jt =>
                        {
                            if (jt.Type == XrefType.Global)
                            {
                                var @object = jt.ReadAsObject();
                                return @object?.ToString() == "Screen Not Found - ";
                            }
                            return false;
                        });
                    }
                    return false;
                });
                ourShowScreenAction = (ShowScreenAction)Delegate.CreateDelegate(typeof(ShowScreenAction), VRCUiManager.prop_VRCUiManager_0, method);
                return ourShowScreenAction;
            }
        }

        private static ShowScreenAction ourShowScreenAction;

        public delegate VRCUiPage ShowScreenAction(VRCUiPage page);

        internal static void RefreshUser()
        {
            APIUser user = VRCUiManager.prop_VRCUiManager_0.field_Public_GameObject_0.GetComponentInChildren<PageUserInfo>().GetUser();

            if (user == null)
            {
                Console.WriteLine("user null");
                return;
            }
            APIUser.FetchUser(user.id, new Action<APIUser>((userapi) =>
            {
                PageUserInfo pageUserInfo = VRCUiManager.prop_VRCUiManager_0.prop_VRCUiPopupManager_0.GetComponentInChildren<PageUserInfo>();
                if (pageUserInfo != null)
                {
                    pageUserInfo.Method_Private_Void_APIUser_PDM_0(userapi);
                    //pageUserInfo.APIUser(userapi);
                    //pageUserInfo.Method_Private_Void_EnumNPublicSealedvaNoGeBlMuVoFrInFrReUnique_APIUser_0(userapi);
                    //LogHandler.Log("Refreshed user: " + userapi.id);
                }
            }),
                new Action<string>(_ =>
                {
                    //LogHandler.Log("Error Couldn't Fetch User\n" + Error);
                }));
        }

        internal static APIUser GetUser(this PageUserInfo Instance)
        {
            return Instance.field_Private_APIUser_0;
        }

        internal static APIUser GetAPIUser()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").GetComponent<PageUserInfo>().field_Private_APIUser_0;
        }

        internal static PageUserInfo GetPageUserInfo()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").GetComponent<PageUserInfo>();
        }

        internal static ApiWorld GetUserWorld()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").GetComponent<PageUserInfo>().field_Private_ApiWorld_0;
        }

        internal static ApiWorld GetSelectedWorld()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo").GetComponent<PageWorldInfo>().field_Private_ApiWorld_0;
        }

        internal static ApiWorldInstance GetSelectedInstance()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo").GetComponent<PageWorldInfo>().field_Public_ApiWorldInstance_0;
        }

        internal static void SelectAPIUser(APIUser user)
        {
            QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0 = user;
            QuickMenu.prop_QuickMenu_0.Method_Public_Void_EnumNPublicSealedvaUnShEmUsEmNoCaMo_nUnique_0(QuickMenu.EnumNPublicSealedvaUnShEmUsEmNoCaMo_nUnique.NotificationsMenu_obsolete);
        }

        internal static void SelectAPIUser(this VRCUiManager instance, string userid)
        {
            APIUser.FetchUser(userid, new Action<APIUser>(SelectAPIUser), new Action<string>(Error =>
            {
                PopupUtils.InformationAlert("Unable to Fetch User\n" + Error);
            }));
        }
    }
}
