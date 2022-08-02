using Blaze.API.QM;
using Blaze.Utils.Managers;
using System;
using UnityEngine;
using VRC;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class ItemOrbit : BModule
    {
        private static QMToggleButton ToggleButton;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeItemOrbit>();
        }

        public override void UI()
        {
            ToggleButton = new QMToggleButton(BlazeQM.Orbits, 2, 0, "Item Orbit", delegate
            {
                Main.BlazesComponents.AddComponent<BlazeItemOrbit>();
            }, delegate
            {
                UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeItemOrbit>());
            }, "Toggle item orbiting around your target");
        }

        public override void PlayerLeft(Player player)
        {
            if (player == Main.Target)
            {
                ToggleButton.SetToggleState(false, true);
            }
        }
    }

    public class BlazeItemOrbit : MonoBehaviour
    {
        public BlazeItemOrbit(IntPtr id) : base(id) { }

        public void Update()
        {
            try
            {
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null || Main.Target == null) return;
                GameObject obj = new();
                if (Config.Main.OrbitAnnoyanceMode)
                {
                    Transform transform = obj.transform;
                    Player targetPlayer = Main.Target;
                    transform.position = ((targetPlayer?.field_Private_VRCPlayerApi_0) ?? Networking.LocalPlayer).GetTrackingData(0).position;
                }
                else
                {
                    Transform transform2 = obj.transform;
                    Player targetPlayer2 = Main.Target;
                    transform2.position = ((targetPlayer2 != null) ? targetPlayer2.transform.position : VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position) + new Vector3(0f, 0.2f, 0f);
                }
                obj.transform.Rotate(new Vector3(0f, 360f * Time.time * Config.Main.ItemOrbitSpeed, 0f));
                foreach (VRC_Pickup vrc_Pickup in Main.Pickups)
                {
                    if (Networking.GetOwner(vrc_Pickup.gameObject) != Networking.LocalPlayer)
                    {
                        Networking.SetOwner(Networking.LocalPlayer, vrc_Pickup.gameObject);
                    }
                    vrc_Pickup.transform.position = obj.transform.position + obj.transform.forward * Config.Main.ItemOrbitSize;
                    obj.transform.Rotate(new Vector3(0f, 360 / Main.Pickups.Length, 0f));
                }
                Destroy(obj);
            }
            catch { }
        }
    }
}
