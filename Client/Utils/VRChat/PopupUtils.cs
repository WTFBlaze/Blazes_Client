 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blaze.Utils.VRChat
{
    internal static class PopupUtils
    {
        internal static void HideCurrentPopUp()
        {
            VRCUiManager.prop_VRCUiManager_0.HideScreen("POPUP");
        }

        internal static void InformationAlert(string Message, float DurationTime = 5)
        {
            VRCUiPopupManager.prop_VRCUiPopupManager_0.Method_Public_Void_String_String_Single_0("Blaze's Client", Message, DurationTime);
        }

        internal static void Alert(string Message, string ButtonText, Action Action, Action<VRCUiPopup> OnPopupShown = null)
        {
            VRCUiPopupManager.prop_VRCUiPopupManager_0.Method_Public_Void_String_String_String_Action_Action_1_VRCUiPopup_1("Blaze's Client", Message, ButtonText, Action, OnPopupShown);
        }

        internal static void AlertV2(string Message, string LeftButtonTXT, Action LeftButtonAction, string RightButtonTXT, Action RightButtonAction, Il2CppSystem.Action<VRCUiPopup> OnPopupShown = null)
        {
            VRCUiPopupManager.prop_VRCUiPopupManager_0.Method_Public_Void_String_String_String_Action_String_Action_Action_1_VRCUiPopup_1("Blaze's Client", Message, LeftButtonTXT, LeftButtonAction, RightButtonTXT, RightButtonAction, OnPopupShown);
        }

        internal static void AlertV2(string Message, string ButtonText, Action onSuccess, Il2CppSystem.Action<VRCUiPopup> OnPopupShown = null)
        {
            AlertV2(Message, ButtonText, onSuccess, "Cancel", delegate { HideCurrentPopUp(); }, OnPopupShown);
        }

        public static void AskConfirmOpenURL(string url, string location)
        {
            //APIStuff.GetQuickMenuInstance().Method_Public_Virtual_Final_New_Void_String_3(url);
            AlertV2($"You are about to be redirected to [{location}], are you sure you want to continue?", "Open URL", delegate { Process.Start(url); }, "Return", HideCurrentPopUp);
        }

        internal static void InputPopup(string AcceptButtonTXT, string DefaultInputBoxTXT, Action<string> AcceptButtonAction, Action CancelButtonAction = null)
        {
            PopupCall(AcceptButtonTXT, DefaultInputBoxTXT, false, AcceptButtonAction, CancelButtonAction);
        }

        internal static void NumericPopup(string AcceptButtonTXT, string DefaultInputBoxTXT, Action<string> AcceptButtonAction, Action CancelButtonAction = null)
        {
            PopupCall(AcceptButtonTXT, DefaultInputBoxTXT, true, AcceptButtonAction, CancelButtonAction);
        }

        private static void PopupCall(string confirm, string placeholder, bool IsNumpad, Action<string> OnAccept, Action OnCancel = null)
        {
            VRCUiPopupManager
                .prop_VRCUiPopupManager_0
                .Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_Boolean_Int32_1(
                    "Blaze's Client",
                    "",
                    InputField.InputType.Standard,
                    IsNumpad,
                    confirm,
                    UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>>(new Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>((a, _, _) =>
                    {
                        OnAccept?.Invoke(a);
                    })),
                    UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(OnCancel),
                    placeholder);
        }
    }
}
