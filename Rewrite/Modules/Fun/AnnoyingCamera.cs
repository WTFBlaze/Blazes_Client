using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class AnnoyingCamera : BModule
    {
        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeAnnoyingCamera>();
            ComponentManager.RegisterIl2Cpp<BlazeAnnoyingCamera2>();
        }

        public override void UI()
        {
            new QMToggleButton(BlazeQM.Exploits, 1, 2, "Annoying Camera", delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeAnnoyingCamera>();
                }
            }, delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera>());
                }
            }, "Cause everyone to hear the camera timer bloop on loop");

            new QMToggleButton(BlazeQM.Exploits, 2, 3, "Annoying Camera 2", delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera2>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeAnnoyingCamera2>();
                }
            }, delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeAnnoyingCamera>());
                }
            }, "Cause everyone to hear the camera timer bloop on loop");
        }
    }

    public class BlazeAnnoyingCamera : MonoBehaviour
    {
        public BlazeAnnoyingCamera(IntPtr id) : base(id) { }
        public static float targetTime = 1.5f;

        public void Awake() => targetTime = 0;

        public void Update()
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0)
            {
                if (WorldUtils.IsInRoom())
                {
                    Networking.RPC(RPC.Destination.All, PlayerUtils.CurrentUser().transform.Find("UserCameraIndicator/Indicator").gameObject, "TimerBloop", new Il2CppSystem.Object[] { });
                }
                targetTime = 1.5f;
            }
        }
    }

    public class BlazeAnnoyingCamera2 : MonoBehaviour
    {
        public BlazeAnnoyingCamera2(IntPtr id) : base(id) { }
        public static float targetTime = 2f;

        public void Awake() => targetTime = 0;

        public void Update()
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0)
            {
                if (WorldUtils.IsInRoom())
                {
                    Networking.RPC(RPC.Destination.All, PlayerUtils.CurrentUser().transform.Find("UserCameraIndicator/Indicator").gameObject, "PhotoCapture", new Il2CppSystem.Object[] { });
                }
                targetTime = 1.5f;
            }
        }
    }
}
