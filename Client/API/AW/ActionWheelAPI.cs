using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.API.AW
{
	public static class ActionWheelAPI
	{
		private static readonly List<ActionMenuButton> mainMenuButtons = new();
		internal static ActionMenu activeActionMenu;

		internal enum ActionMenuBaseMenu
		{
			MainMenu = 1
		}

		internal static bool IsOpen(this ActionMenuOpener actionMenuOpener)
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

		internal static void OpenMainPage(ActionMenu menu)
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

		internal class ActionMenuPage
		{
			internal List<ActionMenuButton> buttons = new();
			internal ActionMenuPage previousPage;
			internal ActionMenuButton menuEntryButton;

			internal ActionMenuPage(ActionMenuBaseMenu baseMenu, string buttonText, Texture2D buttonIcon = null)
			{
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					menuEntryButton = new ActionMenuButton(ActionMenuBaseMenu.MainMenu, buttonText, OpenMenu, buttonIcon);
				}
			}

			internal ActionMenuPage(ActionMenuBaseMenu baseMenu, string buttonText, Sprite buttonIcon = null)
			{
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					menuEntryButton = new ActionMenuButton(ActionMenuBaseMenu.MainMenu, buttonText, OpenMenu, buttonIcon.texture);
				}
			}

			internal ActionMenuPage(ActionMenuPage basePage, string buttonText, Texture2D buttonIcon = null)
			{
				ActionMenuPage page = this;
				previousPage = basePage;
				menuEntryButton = new ActionMenuButton(previousPage, buttonText, delegate
				{
					page.OpenMenu();
				}, buttonIcon);
			}

			internal ActionMenuPage(ActionMenuPage basePage, string buttonText, Sprite buttonIcon = null)
			{
				ActionMenuPage page = this;
				previousPage = basePage;
				menuEntryButton = new ActionMenuButton(previousPage, buttonText, delegate
				{
					page.OpenMenu();
				}, buttonIcon.texture);
			}

			internal void OpenMenu()
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

		internal class ActionMenuButton
		{
			internal string buttonText;
			internal Action buttonAction;
			internal Texture2D buttonIcon;
			internal PedalOption currentPedalOption;

			internal ActionMenuButton(ActionMenuBaseMenu baseMenu, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon;
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					mainMenuButtons.Add(this);
				}
			}

			internal ActionMenuButton(ActionMenuBaseMenu baseMenu, string text, Action action, Sprite icon)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon.texture;
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					mainMenuButtons.Add(this);
				}
			}

			internal ActionMenuButton(ActionMenuPage basePage, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon;
				basePage.buttons.Add(this);
			}

			internal ActionMenuButton(ActionMenuPage basePage, string text, Action action, Sprite icon)
			{
				buttonText = text;
				buttonAction = action;
				buttonIcon = icon.texture;
				basePage.buttons.Add(this);
			}

			internal void SetButtonText(string newText)
			{
				buttonText = newText;
				if (currentPedalOption != null)
				{
					currentPedalOption.field_Public_ActionButton_0.prop_String_0 = newText;
					currentPedalOption.field_Public_ActionButton_0.prop_String_1 = newText;
				}
			}

			internal void SetIcon(Texture2D newTexture)
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
