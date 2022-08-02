using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using UnityEngine;

namespace Blaze.Modules
{
    public class FOVChanger : BModule
    {
        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeFOVChanger>();
        }

        public override void UI()
        {
            Main.BlazesComponents.AddComponent<BlazeFOVChanger>();
        }
    }

    public class BlazeFOVChanger : MonoBehaviour
    {
        public BlazeFOVChanger(IntPtr id) : base(id) { }
        private float _offset = 60f;

        public void Update()
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
            catch { }
        }
    }
}
