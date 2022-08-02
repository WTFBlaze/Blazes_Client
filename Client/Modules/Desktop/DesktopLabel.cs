using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.Modules
{
    class DesktopLabel : BModule
    {
        public static GameObject MainObj;
        public static Outline HudLeftColor;
        public static Text HudLeftText;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazeDesktopHud>();
        }

        public override void SocialMenuUI()
        {
            MainObj = new GameObject("BlazeHack");
            UnityEngine.Object.DontDestroyOnLoad(MainObj);
            MainObj.AddComponent<Canvas>().renderMode = 0;
            MainObj.transform.position = Vector3.zero;
            CanvasScaler canvasScaler = MainObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = (CanvasScaler.ScaleMode)1;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

            var BHLabelGO = new GameObject("BH LH Panel");
            BHLabelGO.transform.SetParent(MainObj.transform, false);
            BHLabelGO.AddComponent<CanvasRenderer>();
            BHLabelGO.AddComponent<RectTransform>();
            BHLabelGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(-860, 520);
            BHLabelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 20);
            var BHLImg = BHLabelGO.AddComponent<Image>();
            BHLImg.color = Color.black;
            HudLeftColor = BHLabelGO.AddComponent<Outline>();
            HudLeftColor.effectColor = Color.white;
            HudLeftColor.effectDistance = new Vector2(2, -2);

            var BHLabel = new GameObject("BlazeHack Text");
            BHLabel.transform.SetParent(BHLabelGO.transform, false);
            BHLabel.AddComponent<CanvasRenderer>();
            BHLabel.AddComponent<RectTransform>();
            BHLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(4, -43);
            BHLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 100);
            HudLeftText = BHLabel.AddComponent<Text>();
            HudLeftText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            HudLeftText.supportRichText = true;
            HudLeftText.fontSize = 12;
            HudLeftText.lineSpacing = 1;
            HudLeftText.fontStyle = FontStyle.Bold;
            HudLeftText.text = $"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color>";

            MainObj.AddComponent<BlazeDesktopHud>();
            MainObj.SetActive(true);
        }
    }

    public class BlazeDesktopHud : MonoBehaviour
    {
        public BlazeDesktopHud(IntPtr id) : base(id) {}

        public void Update()
        {
            try
            {
                float amountToShift = 0.2f * Time.deltaTime;
                Color newColor = ShiftHueBy(DesktopLabel.HudLeftColor.effectColor, amountToShift);
                DesktopLabel.HudLeftColor.effectColor = newColor;
            }
            catch { }
        }

        [HideFromIl2Cpp]
        private Color ShiftHueBy(Color color, float amount)
        {
            // convert from RGB to HSV
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            // shift hue by amount
            hue += amount;
            sat = 1f;
            val = 1f;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }
    }
}
