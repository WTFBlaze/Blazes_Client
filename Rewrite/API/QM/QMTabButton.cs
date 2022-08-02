using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Controls;

namespace Blaze.API.QM
{
    public class QMTabButton
    {
        protected GameObject button;
        protected GameObject badge;
        protected TextMeshProUGUI badgeText;

        public QMTabButton(Action btnAction, string toolTipText, Sprite img = null)
        {
            Initialize(btnAction, toolTipText, img);
        }

        private void Initialize(Action btnAction, string toolTipText, Sprite img = null)
        {
            button = UnityEngine.Object.Instantiate(APIStuff.GetTabButtonTemplate(), APIStuff.GetTabButtonTemplate().transform.parent);
            button.name = $"{BlazesAPI.Identifier}-{APIStuff.RandomNumbers()}";
            UnityEngine.Object.Destroy(button.GetComponent<MenuTab>());
            badge = button.transform.GetChild(0).gameObject;
            badgeText = badge.GetComponentInChildren<TextMeshProUGUI>();

            SetAction(btnAction);
            SetToolTip(toolTipText);
            if (img != null)
            {
                SetImage(img);
            }
            BlazesAPI.allQMTabButtons.Add(this);
        }

        public void SetImage(Sprite newImg)
        {
            button.transform.Find("Icon").GetComponent<Image>().sprite = newImg;
            button.transform.Find("Icon").GetComponent<Image>().overrideSprite = newImg;
            button.transform.Find("Icon").GetComponent<Image>().color = Color.white;
        }

        public void SetToolTip(string newText)
        {
            button.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = newText;
        }

        public void SetIndex(int newPosition)
        {
            button.transform.SetSiblingIndex(newPosition);
        }

        public void SetAction(Action newAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            button.GetComponent<Button>().onClick.AddListener(newAction);
        }

        public void SetActive(bool newState)
        {
            button.SetActive(newState);
        }

        public void SetBadge(bool showing = true, string text = "")
        {
            if (badge == null || badgeText == null)
            {
                return;
            }
            badge.SetActive(showing);
            badgeText.text = text;
        }
    }
}
