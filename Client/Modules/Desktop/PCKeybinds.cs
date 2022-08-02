using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using System;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Blaze.Modules
{
    class PCKeybinds : BModule
    {
        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesKeybinds>();
        }

        public override void QuickMenuUI()
        {
            new QMToggleButton(BlazeMenu.Settings, 2, 1, "PC Keybinds", delegate
            {
                MainConfig.Instance.PCKeybinds = true;
                if (BlazeInfo.BlazesComponents.GetComponent<BlazesKeybinds>() == null)
                {
                    BlazeInfo.BlazesComponents.AddComponent<BlazesKeybinds>();
                }
            }, delegate
            {
                MainConfig.Instance.PCKeybinds = false;
                if (BlazeInfo.BlazesComponents.GetComponent<BlazesKeybinds>() != null)
                {
                    UnityEngine.Object.Destroy(BlazeInfo.BlazesComponents.GetComponent<BlazesKeybinds>());
                }
            }, "Enable pc keybinds for quick toggle", MainConfig.Instance.PCKeybinds);

            if (MainConfig.Instance.PCKeybinds) BlazeInfo.BlazesComponents.AddComponent<BlazesKeybinds>();
        }
    }

    public class BlazesKeybinds : MonoBehaviour
    {
        public BlazesKeybinds(IntPtr id) : base(id) { }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Flight.ToggleButton.ClickMe();
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    VRCESP.CapsuleESP.ClickMe();
                }
            }
            /*foreach (var keyBind in KeybindsConfig.Instance.Keybinds.Where(keyBind => Input.GetKey(keyBind.FirstKey) && Input.GetKeyDown(keyBind.SecondKey)))
            {
                switch (keyBind.Target)
                {
                    case Utils.Objects.ModObjects.ModFeature.Flight:
                        Flight.ToggleButton.ClickMe();
                        break;

                    case Utils.Objects.ModObjects.ModFeature.ESP:
                        VRCESP.CapsuleESP.ClickMe();
                        break;
                }
            }*/
        }
    }
}
