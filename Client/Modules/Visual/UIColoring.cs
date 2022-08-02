using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.Modules
{
    class UIColoring : BModule
    {
        private Image QM;
        private Image SM;
        //private QMNestedButton Menu;
        //private QMSingleButton PreviewColor;

        public override void QuickMenuUI()
        {
            var QMParent = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent");
            QMParent.transform.Find("BackgroundLayer02").gameObject.SetActive(false);
            QM = QMParent.transform.Find("BackgroundLayer01").gameObject.GetComponent<Image>();
            QM.sprite = AssetBundleManager.MenuBackground;
            QM.overrideSprite = AssetBundleManager.DebugBackground;
            SM = GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Background").GetComponent<Image>();
            SM.sprite = AssetBundleManager.MenuBackground;
            SM.overrideSprite = AssetBundleManager.DebugBackground;

            /*Menu = new QMNestedButton(BlazeMenu.Settings, "UI Color", 4, 1, "Click to change the VRChat UI Color", "UI Coloring");
            new QMSlider(Menu, -700, -300, "Red", 0, 255, float.Parse(Config.Instance.UIColorRed.ToString()), delegate (float f)
            {
                Config.Instance.UIColorRed = Convert.ToByte(f);
            }, Color.red);

            new QMSlider(Menu, -700, -450, "Green", 0, 255, float.Parse(Config.Instance.UIColorGreen.ToString()), delegate (float f)
            {
                Config.Instance.UIColorGreen = Convert.ToByte(f);
            }, Color.green);

            new QMSlider(Menu, -700, -600, "Blue", 0, 255, float.Parse(Config.Instance.UIColorBlue.ToString()), delegate (float f)
            {
                Config.Instance.UIColorBlue = Convert.ToByte(f);
            }, Color.blue);

            PreviewColor = new QMSingleButton(Menu, 4, 0, $"<color=#{ColorManager.ColorToHex(new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255))}>Preview\nColor</color>", delegate
            {
                PreviewColor.SetButtonText($"<color=#{ColorManager.ColorToHex(new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255))}>Preview\nColor</color>");
            }, "Click to update the preview color of your new current ui color slider values");

            new QMSingleButton(Menu, 4, 1, "Set UI\nColor", delegate
            {
                QM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
                SM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
            }, "Click to set your UI Color to your slider values");

            new QMSingleButton(Menu, 4, 2, "Reset\nColor", delegate
            {
                //Config.Instance.UIColor = new Utils.Objects.ModObjects.ColorJson(new Color32(161, 52, 235, 255));
                Config.Instance.UIColorRed = 161;
                Config.Instance.UIColorGreen = 52;
                Config.Instance.UIColorBlue = 235;
                QM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
                SM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
            }, "Resets the UI Color back to the default purple");

            Functions.Delay(delegate
            {
                QM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
                SM.color = new Color(Config.Instance.UIColorRed, Config.Instance.UIColorGreen, Config.Instance.UIColorRed, 255);
            }, 1.5f);*/
        }
    }
}
