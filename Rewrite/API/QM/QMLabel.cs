using TMPro;
using UnityEngine;

namespace Blaze.API.QM
{
    public class QMLabel
    {
        protected GameObject gameObject;
        protected TextMeshProUGUI text;

        public QMLabel(QMNestedButton location, float posX, float posY, string labelText, Color? textColor)
        {
            Initialize(location.GetMenuObject().transform.parent, posX, posY, labelText, textColor);
        }

        public QMLabel(Transform location, float posX, float posY, string labelText, Color? textColor)
        {
            Initialize(location, posX, posY, labelText, textColor);
        }

        private void Initialize(Transform location, float posX, float posY, string labelText, Color? textColor)
        {
            gameObject = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickLinks/LeftItemContainer/Text_Title"), location, false);
            gameObject.name = $"{BlazesAPI.Identifier}-QMLabel-{APIStuff.RandomNumbers()}";
            text = gameObject.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.autoSizeTextContainer = true;
            text.enableWordWrapping = false;
            text.fontSize = 32;
            text.richText = true;
            SetPosition(new Vector2(posX, posY));
            SetText(labelText);
            if (textColor != null) SetTextColor((Color)textColor);
            BlazesAPI.allQMLabels.Add(this);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public TextMeshProUGUI GetText()
        {
            return text;
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }

        public void SetTextColor(Color newColor)
        {
            text.color = newColor;
        }

        public void SetPosition(Vector2 newPosition)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(gameObject);
                BlazesAPI.allQMLabels.Remove(this);
            }
            catch { }
        }
    }
}
