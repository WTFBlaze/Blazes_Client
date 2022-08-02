using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.SM
{
    public class SMPopup
    {
        protected SMButton openButton;
        protected Button backButton;
        protected GameObject mainObject;
        protected GameObject headerObject;
        protected Text titleText;
        protected Image headerImage;
        protected VRCUiPage vrcUiPage;

        public SMPopup(Transform location, float btnPosX, float btnPosY, string btnText, string labelText, float btnSizeX = 1f, float btnSizeY = 1f, Color? panelColor = null)
        {
            Initialize(location, btnPosX, btnPosY, btnText, labelText, btnSizeX, btnSizeY, panelColor);
        }

        private void Initialize(Transform location, float btnPosX, float btnPosY, string btnText, string labelText, float btnSizeX = 1f, float btnSizeY = 1f, Color? panelColor = null)
        {
            mainObject = UnityEngine.Object.Instantiate(APIStuff.GetPopupMenuReference(), location, false);
            string identifier = $"{BlazesAPI.Identifier}-SMPopup-{APIStuff.RandomNumbers()}";
            mainObject.name = identifier;
            headerObject = mainObject.transform.Find("Panel/PanelHeaderBackground").gameObject;
            titleText = mainObject.transform.Find("Panel/TitleText").GetComponent<Text>();
            headerImage = headerObject.GetComponent<Image>();

            // Handle VRCUiPage Component
            vrcUiPage = mainObject.GetComponent<VRCUiPage>();
            vrcUiPage.field_Public_String_0 = "SCREEN";
            vrcUiPage.field_Public_String_1 = identifier;


            // Remove unwanted objects
            Il2CppSystem.Collections.IEnumerator enumerator = headerObject.transform.parent.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Il2CppSystem.Object obj = enumerator.Current;
                Transform btnEnum = obj.Cast<Transform>();
                if (btnEnum.name != "BorderImage" && btnEnum.name != "PanelHeaderBackground" && btnEnum.name != "TitleText" && btnEnum.name != "ExitButton")
                {
                    UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }

            backButton = titleText.transform.parent.Find("ExitButton").gameObject.GetComponent<Button>();
            backButton.onClick = new Button.ButtonClickedEvent();
            backButton.onClick.AddListener(new Action(() =>
            {
                vrcUiPage.Method_Public_Virtual_New_Void_Boolean_1(false);
            }));

            openButton = new(SMButton.SMButtonType.ChangeAvatar, location, btnPosX, btnPosY, btnText, delegate
            {
                //VRCUiManager.prop_VRCUiManager_0.ShowScreen(vrcUiPage);
                vrcUiPage.Method_Public_Virtual_New_Void_Boolean_1(true);
            }, btnSizeX, btnSizeY);

            SetTitleText(labelText);
            if (panelColor != null) SetPanelColor((Color)panelColor);
            BlazesAPI.allSMPopups.Add(this);
        }

        public void OpenMe()
        {
            vrcUiPage.Method_Public_Virtual_New_Void_Boolean_1(true);
        }

        public void CloseMe()
        {
            vrcUiPage.Method_Public_Virtual_New_Void_Boolean_1(false);
        }

        public void SetTitleText(string newText)
        {
            titleText.text = newText;
        }

        public void SetPanelColor(Color newColor)
        {
            headerImage.color = newColor;
        }

        public Transform GetTransform()
        {
            return mainObject.transform.Find("Panel");
        }

        public GameObject GetPanelObject()
        {
            return mainObject;
        }

        public SMButton GetOpenButton()
        {
            return openButton;
        }

        public Button GetExitButton()
        {
            return backButton;
        }
    }
}
