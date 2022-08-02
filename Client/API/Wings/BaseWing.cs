using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.API.Wings
{
    public class BaseWing
    {
        public readonly List<WingPage> openedPages = new();
        internal void Setup(Transform wing)
        {
            Wing = wing;
            WingOpen = wing.Find("Button");
            WingPages = wing.Find("Container/InnerContainer");
            WingMenu = WingPages.Find("WingMenu");
            WingButtons = WingPages.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup");

            ProfilePage = WingPages.Find("Profile");
            ProfileButton = WingButtons.Find("Button_Profile");
        }

        public Transform Wing; //        UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left
        public Transform WingOpen; //    UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Button
        public Transform WingPages; //   UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer
        public Transform WingMenu; //    UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer/WingMenu
        public Transform WingButtons; // UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup

        public Transform ProfilePage;
        public Transform ProfileButton;
    }
}
