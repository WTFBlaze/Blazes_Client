using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Blaze.API.AW.ActionWheelAPI;

namespace Blaze.Modules
{
    class BlazeActionWheel : BModule
    {
        internal static Sprite blankSprite;
        internal static ActionMenuPage main;
        internal static ActionMenuPage quickActions;
        internal static ActionMenuPage movement;

        internal static ActionMenuButton Flight;

        public override void QuickMenuUI()
        {
            main = new ActionMenuPage(ActionMenuBaseMenu.MainMenu, "Blaze's Client", AssetBundleManager.Logo);
            blankSprite = new();

            #region Quick Actions
            quickActions = new ActionMenuPage(main, "Quick Actions", blankSprite);

            new ActionMenuButton(quickActions, "Respawn", delegate
            {
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Respawn").GetComponent<Button>().onClick.Invoke();
            });

            #endregion

            #region Movement
            movement = new ActionMenuPage(main, "Movement", blankSprite);

            Flight = new ActionMenuButton(movement, "Flight: Off", delegate
            {
                Modules.Flight.ToggleButton.ClickMe();
            });
            #endregion
        }
    }
}
