using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.SM
{
    public class SMText
    {
        protected GameObject textObject;
        protected Text text;

        public SMText(APIStuff.SMLocations location, float PosX, float PosY, float SizeX, float SizeY, string textLabel, Color? textColor = null)
        {
            Initialize(APIStuff.GetSocialMenuInstance().transform.Find(location.ToString()), PosX, PosY, SizeX, SizeY, textLabel, textColor);
        }

        public SMText(Transform location, float PosX, float PosY, float SizeX, float SizeY, string textLabel, Color? textColor = null)
        {
            Initialize(location, PosX, PosY, SizeX, SizeY, textLabel, textColor);
        }

        public SMText(SMPopup location, float PosX, float PosY, float SizeX, float SizeY, string textLabel, Color? textColor = null)
        {
            Initialize(location.GetTransform(), PosX, PosY, SizeX, SizeY, textLabel, textColor);
        }

        private void Initialize(Transform location, float PosX, float PosY, float SizeX, float SizeY, string textLabel, Color? textColor = null)
        {
            textObject = UnityEngine.Object.Instantiate(APIStuff.GetSocialMenuInstance().transform.Find("Settings/AudioDevicePanel/MicDeviceText").gameObject, location, false);
            text = textObject.GetComponent<Text>();
            textObject.name = $"{BlazesAPI.Identifier}-SMText-{APIStuff.RandomNumbers()}";
            SetLocation(new Vector2(PosX, PosY));
            SetSize(new Vector2(SizeX, SizeY));
            SetText(textLabel);
            if (textColor != null) SetColor((Color)textColor);
            BlazesAPI.allSMTexts.Add(this);
        }

        public void SetLocation(Vector2 location)
        {
            textObject.GetComponent<RectTransform>().anchoredPosition = location;
        }

        public void SetSize(Vector2 size)
        {
            textObject.GetComponent<RectTransform>().sizeDelta = size;
        }

        public void SetColor(Color newColor)
        {
            text.color = newColor;
        }

        public void SetText(string message)
        {
            text.supportRichText = true;
            text.text = message;
        }

        public void SetAnchor(TextAnchor alignment)
        {
            text.alignment = alignment;
        }

        public void SetFontSize(int size)
        {
            text.fontSize = size;
        }

        public GameObject GetGameObject()
        {
            return textObject;
        }
    }
}
