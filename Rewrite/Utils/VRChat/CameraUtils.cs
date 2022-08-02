using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.XR;

namespace Blaze.Utils.VRChat
{
    public static class CameraUtils
    {
        private static Camera _mainCamera;
        private static GameObject _eyeCameraObject;
        private static Camera _currentCamera;

        public static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = GetVRCVrCamera().field_Public_Camera_0;
                }
                return _mainCamera;
            }
        }

        public static GameObject EyeCameraObject
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

        public static Camera CurrentCamera
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

        public static VRCVrCamera GetVRCVrCamera()
        {
            return VRCVrCamera.field_Private_Static_VRCVrCamera_0;
        }

        public static Vector3 GetWorldCameraPosition()
        {
            VRCVrCamera camera = GetVRCVrCamera();
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return transform1.position;
                }
                else if (transform2.name == "Camera (eye)")
                {
                    return transform2.position;
                }
            }
            else if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return unity.field_Public_Camera_0.transform.position;
            }
            else if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.transform.position;
            }
            return camera.transform.parent.TransformPoint(GetLocalCameraPosition());
        }

        public static Vector3 GetLocalCameraPosition()
        {
            VRCVrCamera camera = GetVRCVrCamera();
            var type = camera.GetIl2CppType();
            if (type == Il2CppType.Of<VRCVrCameraSteam>())
            {
                VRCVrCameraSteam steam = camera.Cast<VRCVrCameraSteam>();
                Transform transform1 = steam.field_Private_Transform_0;
                Transform transform2 = steam.field_Private_Transform_1;
                if (transform1.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform1.position);
                }
                else if (transform2.name == "Camera (eye)")
                {
                    return camera.transform.parent.InverseTransformPoint(transform2.position);
                }
                else
                {
                    return Vector3.zero;
                }
            }
            else if (type == Il2CppType.Of<VRCVrCameraUnity>())
            {
                if (XRDevice.isPresent)
                {
                    return camera.transform.localPosition + InputTracking.GetLocalPosition(XRNode.CenterEye);
                }
                VRCVrCameraUnity unity = camera.Cast<VRCVrCameraUnity>();
                return camera.transform.parent.InverseTransformPoint(unity.field_Public_Camera_0.transform.position);
            }
            else if (type == Il2CppType.Of<VRCVrCameraWave>())
            {
                VRCVrCameraWave wave = camera.Cast<VRCVrCameraWave>();
                return wave.field_Public_Transform_0.InverseTransformPoint(camera.transform.position);
            }
            return camera.transform.localPosition;
        }
    }
}
