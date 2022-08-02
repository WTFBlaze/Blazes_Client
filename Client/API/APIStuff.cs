using Blaze.API.Wings;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API
{
    internal static class APIStuff
    {
        private static VRC.UI.Elements.QuickMenu QuickMenuInstance;
        private static GameObject SingleButtonReference;
        private static GameObject TabButtonReference;
        private static GameObject MenuPageReference;
        private static GameObject QMInfoReference;
        private static GameObject SliderReference;
        private static GameObject SliderLabelReference;
        private static GameObject SocialMenuInstance;
        private static GameObject AvatarPreviewReference;
        private static GameObject PopupMenuReference;
        private static Sprite OnIconReference;
        private static Sprite OffIconReference;
        private static System.Random rnd = new();
        internal static BaseWing Left = new();
        internal static BaseWing Right = new();
        internal static Action<BaseWing> OnWingInit = _ => { };

        internal static Action Init_L = () =>
        {
            Init_L = () => { };
            OnWingInit(Left);
        };

        internal static Action Init_R = () =>
        {
            Init_R = () => { };
            OnWingInit(Right);
        };

        internal static VRC.UI.Elements.QuickMenu GetQuickMenuInstance()
        {
            if (QuickMenuInstance == null)
                QuickMenuInstance = Resources.FindObjectsOfTypeAll<VRC.UI.Elements.QuickMenu>()[0];
            return QuickMenuInstance;
        }

        internal static GameObject GetSocialMenuInstance()
        {
            if (SocialMenuInstance == null)
            {
                SocialMenuInstance = GameObject.Find("UserInterface/MenuContent/Screens");
            }
            return SocialMenuInstance;
        }

        // Templates

        internal static GameObject SingleButtonTemplate()
        {
            if (SingleButtonReference == null)
            {
                var Buttons = GetQuickMenuInstance().GetComponentsInChildren<Button>(true);
                foreach (var button in Buttons)
                {
                    if (button.name == "Button_Screenshot")
                    {
                        SingleButtonReference = button.gameObject;
                    }
                };
            }
            return SingleButtonReference;
        }

        internal static GameObject GetMenuPageTemplate()
        {
            if (MenuPageReference == null)
            {
                MenuPageReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard").gameObject;
            }
            return MenuPageReference;
        }

        internal static GameObject GetQMInfoTemplate()
        {
            if (QMInfoReference == null)
            {
                QMInfoReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/PerformanceSettingsPopup/Popup/Pages/Page_LimitAvatarPerformance/Tooltip_Details").gameObject;
            }
            return QMInfoReference;
        }

        internal static GameObject GetSliderTemplate()
        {
            if (SliderReference == null)
            {
                SliderReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/VolumeSlider").gameObject;
            }
            return SliderReference;
        }

        internal static GameObject GetSliderLabelTemplate()
        {
            if (SliderLabelReference == null)
            {
                SliderLabelReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/LevelText").gameObject;
            }
            return SliderLabelReference;
        }

        internal static GameObject GetAvatarPreviewBase()
        {
            if (AvatarPreviewReference == null)
            {
                AvatarPreviewReference = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase");
            }

            return AvatarPreviewReference;
        }

        public static GameObject GetTabButtonTemplate()
        {
            if (TabButtonReference == null)
            {
                TabButtonReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
            }
            return TabButtonReference;
        }

        public static GameObject GetPopupMenuReference()
        {
            if (PopupMenuReference == null)
            {
                PopupMenuReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/UserInfo/ModerateDialog").gameObject;
            }
            return PopupMenuReference;
        }

        // Icon Sprites

        public static Sprite GetOnIconSprite()
        {
            if (OnIconReference == null)
            {
                OnIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<Image>().sprite;
            }
            return OnIconReference;
        }

        public static Sprite GetOffIconSprite()
        {
            if (OffIconReference == null)
            {
                OffIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UI_Elements_Row_1/Button_ToggleQMInfo/Icon_Off").GetComponent<Image>().sprite;
            }
            return OffIconReference;
        }

        public enum SMLocations
        {
            Worlds,
            Avatars,
            Social,
            Settings,
            Safety,
            UserInfo
        }

        // Functions

        internal static int RandomNumbers()
        {
            return rnd.Next(100000, 999999);
        }

        internal static void DestroyChildren(this Transform transform)
        {
            transform.DestroyChildren(null);
        }

        internal static void DestroyChildren(this Transform transform, Func<Transform, bool> exclude)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if (exclude == null || exclude(transform.GetChild(i)))
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
