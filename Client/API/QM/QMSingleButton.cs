using Blaze.Utils.API;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blaze.API.QM
{
    internal class QMSingleButton : QMButtonBase
    {
        public QMSingleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, bool halfBtn = false)
        {
            btnQMLoc = btnMenu.GetMenuName();
            if (halfBtn)
            {
                btnYLocation -= 0.21f;
            }
            InitButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnTextColor);
            if (halfBtn)
            {
                // 2.0175f
                button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition = new Vector2(0, 22);
            }
            /*if (btnHalf)
            {
                *//*RectTransform Recto = button.GetComponent<RectTransform>();
                if (IsUp)
                {
                    Recto.sizeDelta = new Vector2(Recto.sizeDelta.x, Recto.sizeDelta.y / 2);
                    button.GetComponentInChildren<Text>().fontSize = 60;
                }
                else
                {
                    Recto.sizeDelta = new Vector2(Recto.sizeDelta.x, Recto.sizeDelta.y / 2);
                    Recto.anchoredPosition -= new Vector2(0, Recto.sizeDelta.y);
                    button.GetComponentInChildren<Text>().fontSize = 60;
                }*//*
                button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2.0175f);
            }*/
        }

        public QMSingleButton(string btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, bool halfBtn = false)
        {
            btnQMLoc = btnMenu;
            if (halfBtn)
            {
                btnYLocation -= 0.21f;
            }
            InitButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnTextColor);
            if (halfBtn)
            {
                // 2.0175f
                button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition = new Vector2(0, 22);
            }
            /*if (btnHalf)
            {
                RectTransform Recto = button.GetComponentInChildren<RectTransform>(true);
                if (IsUp)
                {
                    Recto.sizeDelta = new Vector2(Recto.sizeDelta.x, Recto.sizeDelta.y / 2);
                }
                else
                {
                    Recto.sizeDelta = new Vector2(Recto.sizeDelta.x, Recto.sizeDelta.y / 2);
                    Recto.anchoredPosition -= new Vector2(0, Recto.sizeDelta.y);
                }
            }*/
        }

        private protected void InitButton(float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null)
        {
            btnType = "SingleButton";
            button = UnityEngine.Object.Instantiate(APIStuff.SingleButtonTemplate(), GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/" + btnQMLoc).transform, true);
            RandomNumb = APIStuff.RandomNumbers();
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 176);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68, 796);
            button.transform.Find("Icon").GetComponentInChildren<Image>().gameObject.SetActive(false);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition += new Vector2(0, 50);

            initShift[0] = 0;
            initShift[1] = 0;
            SetLocation(btnXLocation, btnYLocation);
            SetButtonText(btnText);
            SetToolTip(btnToolTip);
            SetAction(btnAction);

            if (btnTextColor != null)
                SetTextColor((Color)btnTextColor);
            else
                OrigText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>().color;

            SetActive(true);
            BlazesAPI.allQMSingleButtons.Add(this);
        }

        public void SetBackgroundImage(Sprite newImg)
        {
            button.transform.Find("Background").GetComponent<Image>().sprite = newImg;
            button.transform.Find("Background").GetComponent<Image>().overrideSprite = newImg;
            RefreshButton();
        }

        public void SetButtonText(string buttonText)
        {
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;
        }

        public void SetAction(Action buttonAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            if (buttonAction != null)
                button.GetComponent<Button>().onClick.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityAction>(buttonAction));
        }

        public void SetInteractable(bool newState)
        {
            button.GetComponent<Button>().interactable = newState;
            RefreshButton();
        }

        public void ClickMe()
        {
            button.GetComponent<Button>().onClick.Invoke();
        }

        public Image GetBackgroundImage()
        {
            return button.transform.Find("Background").GetComponent<Image>();
        }

        internal override void SetTextColor(Color buttonTextColor, bool save = true)
        {
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetOutlineColor(buttonTextColor);
            if (save)
                OrigText = buttonTextColor;
        }

        private void RefreshButton()
        {
            button.SetActive(false);
            button.SetActive(true);
        }
    }
}
