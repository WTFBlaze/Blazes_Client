using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Utils.VRChat
{
    internal static class CameraUtils
    {
        private static Camera _mainCamera;
        private static GameObject _eyeCameraObject;
        private static Camera _currentCamera;

        internal static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = MiscUtils.GetVRCVrCamera().field_Public_Camera_0;
                }
                return _mainCamera;
            }
        }

        internal static GameObject EyeCameraObject
        {
            get
            {
                if (_eyeCameraObject == null)
                {
                    _eyeCameraObject = GameObject.Find("Camera (eye)");
                }
                return _eyeCameraObject;
            }
        }

        internal static Camera CurrentCamera
        {
            get
            {
                return _currentCamera;
            }
            set
            {
                _currentCamera = value;
                Camera currentCamera = _currentCamera;
                //CameraUtils.HighlightsFX = ((currentCamera != null) ? currentCamera.GetComponent<HighlightsFXStandalone>() : null);
            }
        }
    }
}
