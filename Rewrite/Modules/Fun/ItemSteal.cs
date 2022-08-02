using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using UnityEngine;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class ItemSteal : BModule
    {
        private QMToggleButton ToggleButton;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeItemStealer>();
        }

        public override void UI()
        {
            ToggleButton = new QMToggleButton(BlazeQM.Exploits, 4, 2, "Item Stealer", delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeItemStealer>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeItemStealer>();
                }
            }, delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeItemStealer>() != null)
                {
                    UnityEngine.Object.DestroyObject(Main.BlazesComponents.GetComponent<BlazeItemStealer>());
                }
            }, "Forcefully steal items from other people so they cannot hold them.");
        }

        public override void LocalPlayerLoaded()
        {
            if (ToggleButton != null)
            {
                ToggleButton.SetToggleState(false, true);
            }
        }
    }

    public class BlazeItemStealer : MonoBehaviour
    {
        public BlazeItemStealer(IntPtr id) : base(id) { }
        
        public void Update()
        {
            try
            {
                if (!WorldUtils.IsInRoom()) return;
                foreach (VRC_Pickup vrc_Pickup in Main.Pickups)
                {
                    if (Networking.GetOwner(vrc_Pickup.gameObject) != Networking.LocalPlayer)
                    {
                        Networking.SetOwner(Networking.LocalPlayer, vrc_Pickup.gameObject);
                        vrc_Pickup.DisallowTheft = true;
                    }
                }
            }
            catch { }
        }
    }
}
