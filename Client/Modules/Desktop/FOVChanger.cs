using Blaze.Utils.VRChat;
using System;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Blaze.Modules
{
    class FOVChanger : BModule
    {
        /*public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesFOVChanger>();
        }

        public override void QuickMenuUI()
        {
            BlazeInfo.BlazesComponents.AddComponent<BlazesFOVChanger>();
        }*/

        private float _offset = 60f;

        public override void Update()
        {
            try
            {
                if (WorldUtils.IsInRoom())
                {
                    if (ThirdPerson.Mode == ThirdPerson.CameraMode.Normal)
                    {
                        if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) < 0f)
                        {
                            _offset += 5f;
                        }
                        if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) > 0f)
                        {
                            _offset -= 5f;
                        }
                        if (InputUtils.GetMouseButtonDown(2, true, false))
                        {
                            _offset = 60f;
                        }
                        CameraUtils.EyeCameraObject.GetComponent<Camera>().fieldOfView = _offset;
                    }
                }
            }
            catch {}
        }
    }

    /*public class BlazesFOVChanger : MonoBehaviour
    {
        private float _offset = 60f;

        public void Update()
        {
            if (WorldUtils.IsInRoom())
            {
                try
                {
                    if (ThirdPerson.Mode == ThirdPerson.CameraMode.Normal)
                    {
                        if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) < 0f)
                        {
                            _offset += 5f;
                        }
                        if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) > 0f)
                        {
                            _offset -= 5f;
                        }
                        if (InputUtils.GetMouseButtonDown(2, true, false))
                        {
                            _offset = 60f;
                        }
                        CameraUtils.EyeCameraObject.GetComponent<Camera>().fieldOfView = _offset;
                    }
                }
                catch {}
            }
        }
    }*/
}
