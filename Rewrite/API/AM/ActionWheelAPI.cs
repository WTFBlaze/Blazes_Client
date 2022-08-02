using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blaze.API.AW
{
	public static class ActionWheelAPI
	{
		private static readonly List<ActionMenuButton> mainMenuButtons = new();
		public static ActionMenu activeActionMenu;

		public enum ActionMenuBaseMenu
		{
			MainMenu = 1
		}

		public static bool IsOpen(this ActionMenuOpener actionMenuOpener)
		{
			return actionMenuOpener.field_Private_Boolean_0;
		}

		private static ActionMenuOpener GetActionMenuOpener()
		{
			if (ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0.IsOpen() == false &&
				ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1.IsOpen())
			{
				return ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1;
			}
			if (ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0.IsOpen() &&
				ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1.IsOpen() == false)
			{
				return ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0;
			}
			return null;
		}

		public static void OpenMainPage(ActionMenu menu)
		{
			activeActionMenu = menu;
			//if (!BlazeVars.ActionWheel) return;
			foreach (var button in mainMenuButtons)
			{
				var pedalOption = activeActionMenu.Method_Private_PedalOption_0();
				pedalOption.prop_String_0 = button.buttonText;
				//pedalOption.prop_String_0 = button.buttonText;
				pedalOption.field_Public_Action_0 = button.buttonAction;
				if (button.buttonIcon != null)
				{
					pedalOption.prop_Texture2D_0 = button.buttonIcon;
				}
				button.currentPedalOption = pedalOption;
			}
		}

		public class ActionMenuPage
		{
			public List<ActionMenuButton> buttons = new();
			public ActionMenuPage previousPage;
			public ActionMenuButton menuEntryButton;

			public ActionMenuPage(ActionMenuBaseMenu baseMenu, string buttonText, Texture2D buttonIcon = null)
			{
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					menuEntryButton = new ActionMenuButton(ActionMenuBaseMenu.MainMenu, buttonText, OpenMenu, buttonIcon);
				}
			}

			public ActionMenuPage(ActionMenuBaseMenu baseMenu, string buttonText, Sprite buttonIcon = null)
			{
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					menuEntryButton = new ActionMenuButton(ActionMenuBaseMenu.MainMenu, buttonText, OpenMenu, buttonIcon.texture);
				}
			}

			public ActionMenuPage(ActionMenuPage basePage, string buttonText, Texture2D buttonIcon = null)
			{
				ActionMenuPage page = this;
				previousPage = basePage;
				menuEntryButton = new ActionMenuButton(previousPage, buttonText, delegate
				{
					page.OpenMenu();
				}, buttonIcon);
			}

			public ActionMenuPage(ActionMenuPage basePage, string buttonText, Sprite buttonIcon = null)
			{
				ActionMenuPage page = this;
				previousPage = basePage;
				menuEntryButton = new ActionMenuButton(previousPage, buttonText, delegate
				{
					page.OpenMenu();
				}, buttonIcon.texture);
			}

			public void OpenMenu()
			{
				GetActionMenuOpener().field_Public_ActionMenu_0.Method_Public_Page_Action_Action_Texture2D_String_0(new Action(() =>
				{
					foreach (ActionMenuButton button in buttons)
					{
						//var puppetMenu = new PuppetMenu();
						var pedalOption = GetActionMenuOpener().field_Public_ActionMenu_0.Method_Private_PedalOption_0();
						//pedalOption.prop_String_0 = button.buttonText;
						pedalOption.prop_String_0 = button.buttonText;
						pedalOption.field_Public_Action_0 = button.buttonAction;
						if (button.buttonIcon != null)
						{
							pedalOption.prop_Texture2D_0 = button.buttonIcon;
						}
						button.currentPedalOption = pedalOption;
					}
				}));
			}
		}

		public class ActionMenuButton
		{
			public string buttonText;
			public Action buttonAction;
			public Texture2D buttonIcon;
			public PedalOption currentPedalOption;

			public ActionMenuButton(ActionMenuBaseMenu baseMenu, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon;
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					mainMenuButtons.Add(this);
				}
			}

			public ActionMenuButton(ActionMenuBaseMenu baseMenu, string text, Action action, Sprite icon)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon.texture;
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					mainMenuButtons.Add(this);
				}
			}

			public ActionMenuButton(ActionMenuPage basePage, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon;
				basePage.buttons.Add(this);
			}

			public ActionMenuButton(ActionMenuPage basePage, string text, Action action, Sprite icon)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon.texture;
				basePage.buttons.Add(this);
			}

			public void SetButtonText(string newText)
			{
				buttonText = newText;
				if (currentPedalOption != null)
				{
					currentPedalOption.field_Public_ActionButton_0.prop_String_0 = newText;
					currentPedalOption.field_Public_ActionButton_0.prop_String_1 = newText;
				}
			}

			public void SetIcon(Texture2D newTexture)
			{
				buttonIcon = newTexture;
				if (currentPedalOption != null)
				{
					currentPedalOption.prop_Texture2D_0 = newTexture;
				}
			}
		}
	}
}
