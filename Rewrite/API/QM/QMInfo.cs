using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.API.QM
{
    public class QMInfo
    {
        public GameObject InfoObject;
        public Text InfoText;
        public Image InfoBackground;

        public QMInfo(Transform location, float PosX, float PosY, float SizeX, float SizeY, string PanelText)
        {
            Initialize(location, PosX, PosY, SizeX, SizeY, PanelText);
        }

        public QMInfo(QMNestedButton location, float PosX, float PosY, float SizeX, float SizeY, string PanelText)
        {
            Initialize(location.GetMenuObject().transform, PosX, PosY, SizeX, SizeY, PanelText);
        }

        private void Initialize(Transform location, float PosX, float PosY, float SizeX, float SizeY, string panelText)
        {
            InfoObject = UnityEngine.Object.Instantiate(APIStuff.GetQMInfoTemplate(), location);
            InfoObject.name = $"{BlazesAPI.Identifier}-InfoPanel-{APIStuff.RandomNumbers()}";
            InfoText = InfoObject.GetComponentInChildren<Text>();
            InfoBackground = InfoObject.GetComponentInChildren<Image>();
            SetSize(new Vector2(SizeX, SizeY));
            SetLocation(new Vector3(PosX, PosY, 1));
            SetText(panelText);
            BlazesAPI.allQMInfos.Add(this);
        }

        public void SetSize(Vector2 size)
        {
            InfoObject.GetComponent<RectTransform>().sizeDelta = size;
            InfoObject.transform.Find("Text").GetComponent<RectTransform>().sizeDelta = new Vector2(-25, 0);
        }

        public void SetLocation(Vector3 location)
        {
            InfoObject.GetComponent<RectTransform>().anchoredPosition = location;
        }

        public void SetText(string text)
        {
            InfoText.text = text;
        }

        public void SetTextColor(Color newColor)
        {
            InfoText.color = newColor;
        }

        public void ToggleBackground(bool state)
        {
            InfoObject.GetComponentInChildren<Image>().enabled = state;
        }

        public void SetActive(bool state)
        {
            InfoObject.SetActive(state);
        }
    }
}
