using Blaze.API.QM;
using Blaze.Utils.Managers;
using System;
using UnityEngine;

namespace Blaze.Modules
{
    public class PCKeybinds : BModule
    {
        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazesKeybinds>();
        }

        public override void UI()
        {
            new QMToggleButton(BlazeQM.Settings, 4, 0, "PC Keybinds", delegate
            {
                Config.Main.PCKeybinds = true;
                if (Main.BlazesComponents.GetComponent<BlazesKeybinds>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazesKeybinds>();
                }
            }, delegate
            {
                Config.Main.PCKeybinds = false;
                if (Main.BlazesComponents.GetComponent<BlazesKeybinds>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazesKeybinds>());
                }
            }, "Enable pc keybinds for quick toggle", Config.Main.PCKeybinds);

            if (Config.Main.PCKeybinds) Main.BlazesComponents.AddComponent<BlazesKeybinds>();
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

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    BlazeQM.Serialization.ClickMe();
                }
            }
        }
    }
}
