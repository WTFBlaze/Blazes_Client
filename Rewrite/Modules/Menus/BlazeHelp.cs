using Blaze.API.QM;
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
    public class BlazeHelp : BModule
    {
        private QMNestedButton Menu;
        private GameObject openObject;
        private GameObject iconObject;
        private Button btnComp;
        private Image iconComp;
        private List<string> slides;
        private QMSingleButton prevBtn;
        private QMSingleButton nextBtn;
        private QMInfo infoPanel;
        private int selectedSlide = 1;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.MainMenu, "", 0, 0, "", "Blaze's Help Menu");
            Menu.GetMainButton().SetActive(false);
            slides = new();
            openObject = BlazeQM.MainMenu.GetMenuObject().transform.Find("Header_H1/RightItemContainer/Button_QM_Expand").gameObject;
            openObject.GetComponent<RectTransform>().sizeDelta = new Vector2(84, 84);
            openObject.SetActive(true);
            iconObject = openObject.transform.Find("Icon").gameObject;
            iconObject.SetActive(true);
            iconComp = iconObject.GetComponent<Image>();
            iconComp.sprite = AssetBundleManager.HelpIcon;
            iconComp.overrideSprite = AssetBundleManager.HelpIcon;
            btnComp = openObject.GetComponent<Button>();
            btnComp.onClick = new Button.ButtonClickedEvent();
            btnComp.onClick.AddListener(new Action(() => { Menu.GetMainButton().ClickMe(); }));

            prevBtn = new QMSingleButton(Menu, 0, 0, "<", delegate { PreviousPage(); }, "Click to go back a page!");
            prevBtn.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            prevBtn.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(815, 960);
            prevBtn.GetGameObject().transform.Find("Text_H4").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 25);
            prevBtn.SetInteractable(false);

            nextBtn = new QMSingleButton(Menu, 1, 0, ">", delegate { NextPage(); }, "Click to go forward a page!");
            nextBtn.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            nextBtn.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(955, 960);
            nextBtn.GetGameObject().transform.Find("Text_H4").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 25);

            #region Credits
            slides.Add(@"<size=45><b><color=yellow>Credits</color></b></size>
<color=red>WTFBlaze</color> - Creating & Maintaining the Client
<color=cyan>Nikei</color> - Creating & Maintaining the Server
<color=yellow>Killer_bigpoint</color> - Helping me fix the Media Controller's Image issue
<color=#3D1E3F>Mr.Null</color> - Giving me tips on how to detect Ghosting
<color=lime>Awooochy</color> & <color=lime>I A G Ф L D</color> - Creating Menu Icons");
            #endregion

            #region Player List Info
            slides.Add(@"<size=45><b><color=yellow>Player List Info</color></b></size>
<color=red>T</color> - Selected Target
<color=red>M</color> - Muted You
<color=red>B</color> - Blocked You
<color=red>K</color> - Is On your Personal KOS
<color=red>INVIS</color> - In the world but is invisible
<color=yellow>Username Color is Trust Rank</color>
<color=magenta>V</color> - VR
<color=lime>Q</color> - Quest
<color=yellow>D</color> - Desktop
P - Ping
F - Frames
AVI - <color=lime>Green</color> means Public Avi. <color=red>Red</color> means Private Avi.
<color=orange>M</color> - Current Instance Master
<color=yellow>F</color> - Friend
<color=green>H</color> - In Range to hear your voice
<color=red>CRASHED</color> - User has crashed
<color=orange>GHOSTING</color> - User froze their body and is currently ghosting around without others seeing them");
            #endregion

            #region Keybinds
            slides.Add(@"<size=45><b><color=yellow>Keybinds</color></b></size>
<b>Left Ctrl + F</b> - <color=green>Flight</color>
<b>Left Ctrl + G</b> - <color=magenta>Capsule ESP</color>
<b>Left Ctrl + 1</b> - <color=red>Third Person</color>
<b>Left Ctrl + Mouse Scroll</b> - <color=red>Third Person FOV</color>
<b>Left Ctrl + Middle Mouse Button</b> - <color=red>Third Person FOV Reset</color>
<b>Left Ctrl + 2</b> - <color=cyan>Serialization</color>
<b>Left Ctrl + Mouse Scroll</b> - <color=yellow>FOV</color>
<b>Left Ctrl + Middle Mouse Button</b> - <color=yellow>Reset FOV</color>");
            #endregion

            infoPanel = new QMInfo(Menu, 0, -60, 1000, 850, slides[0])
            {
                InfoText =
                {
                    color = Color.white,
                    fontSize = 30,
                    alignment = TextAnchor.MiddleCenter,
                    supportRichText = true,
                    fontStyle = FontStyle.Normal,
                },
                InfoBackground =
                {
                    color = new Color(0, 0, 0, 0.85f)
                }
            };
        }

        private void NextPage()
        {
            if (selectedSlide == slides.Count) return;
            selectedSlide++;
            infoPanel.SetText(slides[selectedSlide - 1]);
            prevBtn.SetInteractable(true);
            if (selectedSlide == slides.Count)
            {
                nextBtn.SetInteractable(false);
            }
            //Logs.Log(selectedSlide.ToString());
        }

        private void PreviousPage()
        {
            if (selectedSlide == 1) return;
            selectedSlide--;
            infoPanel.SetText(slides[selectedSlide - 1]);
            nextBtn.SetInteractable(true);
            if (selectedSlide == 1)
            {
                prevBtn.SetInteractable(false);
            }
            //Logs.Log(selectedSlide.ToString());
        }
    }
}
