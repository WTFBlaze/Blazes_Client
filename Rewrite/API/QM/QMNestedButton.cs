using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;

namespace Blaze.API.QM
{
    public class QMNestedButton
    {
        protected string btnQMLoc;
        protected GameObject MenuObject;
        protected TextMeshProUGUI MenuTitleText;
        protected UIPage MenuPage;
        protected bool IsMenuRoot;
        protected GameObject BackButton;
        protected QMSingleButton MainButton;
        protected string MenuName;

        public QMNestedButton(QMNestedButton location, string btnText, float posX, float posY, string toolTipText, string menuTitle)
        {
            btnQMLoc = location.GetMenuName();
            Initialize(false, btnText, posX, posY, toolTipText, menuTitle);
        }

        public QMNestedButton(string location, string btnText, float posX, float posY, string toolTipText, string menuTitle)
        {
            btnQMLoc = location;
            Initialize(location.StartsWith("Menu_"), btnText, posX, posY, toolTipText, menuTitle);
        }

        private void Initialize(bool isRoot, string btnText, float btnPosX, float btnPosY, string btnToolTipText, string menuTitle)
        {
            MenuName = $"{BlazesAPI.Identifier}-Menu-{APIStuff.RandomNumbers()}";
            MenuObject = UnityEngine.Object.Instantiate(APIStuff.GetMenuPageTemplate(), APIStuff.GetMenuPageTemplate().transform.parent);
            MenuObject.name = MenuName;
            MenuObject.SetActive(false);
            UnityEngine.Object.DestroyImmediate(MenuObject.GetComponent<LaunchPadQMMenu>());
            MenuPage = MenuObject.AddComponent<UIPage>();
            MenuPage.field_Public_String_0 = MenuName;
            MenuPage.field_Private_Boolean_1 = true;
            MenuPage.field_Private_MenuStateController_0 = APIStuff.GetQuickMenuInstance().prop_MenuStateController_0;
            MenuPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            MenuPage.field_Private_List_1_UIPage_0.Add(MenuPage);
            APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.field_Private_Dictionary_2_String_UIPage_0.Add(MenuName, MenuPage);

            if (isRoot)
            {
                var list = APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0.ToList();
                list.Add(MenuPage);
                APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0 = list.ToArray();
            }
            MenuObject.transform.Find("ScrollRect/Viewport/VerticalLayoutGroup").DestroyChildren();
            MenuTitleText = MenuObject.GetComponentInChildren<TextMeshProUGUI>(true);
            MenuTitleText.text = menuTitle;
            IsMenuRoot = isRoot;
            BackButton = MenuObject.transform.GetChild(0).Find("LeftItemContainer/Button_Back").gameObject;
            BackButton.SetActive(true);
            BackButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            BackButton.GetComponentInChildren<Button>().onClick.AddListener(new Action(() =>
            {
                if (isRoot)
                {
                    if (btnQMLoc.StartsWith("Menu_"))
                    {
                        APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.Method_Public_Void_String_Boolean_0("QuickMenu" + btnQMLoc.Remove(0, 5));
                        return;
                    }
                    APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.Method_Public_Void_String_Boolean_0(btnQMLoc);
                    return;
                }
                MenuPage.Method_Protected_Virtual_New_Void_0();
            }));
            MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Expand").gameObject.SetActive(false);
            MainButton = new QMSingleButton(btnQMLoc, btnPosX, btnPosY, btnText, OpenMe, btnToolTipText);

            for (int i = 0; i < MenuObject.transform.childCount; i++)
            {
                if (MenuObject.transform.GetChild(i).name != "Header_H1" && MenuObject.transform.GetChild(i).name != "ScrollRect")
                {
                    UnityEngine.Object.Destroy(MenuObject.transform.GetChild(i).gameObject);
                }
            }
            MenuObject.transform.Find("ScrollRect").GetComponent<ScrollRect>().enabled = false;
            //MenuObject.transform.Find("ScrollRect").GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            BlazesAPI.allQMNestedButtons.Add(this);
        }

        public void OpenMe()
        {
            APIStuff.GetQuickMenuInstance().prop_MenuStateController_0.Method_Public_Void_String_UIContext_Boolean_0(MenuPage.field_Public_String_0);
        }

        public void CloseMe()
        {
            MenuPage.Method_Public_Virtual_New_Void_0();
        }

        public string GetMenuName()
        {
            return MenuName;
        }

        public GameObject GetMenuObject()
        {
            return MenuObject;
        }

        public QMSingleButton GetMainButton()
        {
            return MainButton;
        }

        public GameObject GetBackButton()
        {
            return BackButton;
        }
    }
}
