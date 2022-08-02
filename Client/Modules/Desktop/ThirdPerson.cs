using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Modules
{
    class ThirdPerson : BModule
    {
		internal static CameraMode Mode { get; set; } = CameraMode.Normal;
		internal static Camera FrontCamera { get; private set; }
		internal static Camera BackCamera { get; private set; }
		private static GameObject _cameraFrontObj;
		private static GameObject _cameraBackObj;
		private static GameObject _referenceCamera;
		private static float _offset = 1.5f;
		internal static bool ThirdPersonState;

		public override void QuickMenuUI()
        {
			Setup();
        }

        public override void Update()
        {
			if (PlayerUtils.CurrentUser() != null)
			{
				if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha1) && _cameraBackObj != null && _cameraFrontObj != null)
				{
					if (Mode == CameraMode.Normal)
					{
						Mode = CameraMode.ThirdPersonBack;
						CurrentCamera = BackCamera;
						EyeCameraObject.GetComponent<Camera>().enabled = false;
						BackCamera.enabled = true;
						FrontCamera.enabled = false;
						ThirdPersonState = true;
					}
					else
					{
						if (Mode == CameraMode.ThirdPersonBack)
						{
							Mode = CameraMode.ThirdPersonFront;
							CurrentCamera = FrontCamera;
							EyeCameraObject.GetComponent<Camera>().enabled = false;
							BackCamera.enabled = false;
							FrontCamera.enabled = true;
							ThirdPersonState = true;
						}
						else
						{
							Mode = CameraMode.Normal;
							CurrentCamera = MainCamera;
							EyeCameraObject.GetComponent<Camera>().enabled = true;
							BackCamera.enabled = false;
							FrontCamera.enabled = false;
							ThirdPersonState = false;
						}
					}
					BlazeInfo.CurrentCamera = CurrentCamera;
				}
				if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) < 0f && Mode > CameraMode.Normal)
				{
					_offset += 0.1f;
				}
				if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) > 0f && Mode > CameraMode.Normal)
				{
					_offset -= 0.1f;
				}
				if (InputUtils.GetMouseButtonDown(2, true, false) && Mode > CameraMode.Normal)
				{
					_offset = 1.5f;
				}
				if (_cameraBackObj != null && _cameraFrontObj != null)
				{
					_cameraBackObj.transform.position = MainCamera.transform.position - MainCamera.transform.forward * _offset;
					_cameraFrontObj.transform.position = MainCamera.transform.position + MainCamera.transform.forward * _offset;
				}
			}
		}

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
			Mode = CameraMode.Normal;
			if (EyeCameraObject != null && EyeCameraObject.GetComponent<Camera>() != null)
			{
				EyeCameraObject.GetComponent<Camera>().enabled = true;
			}
			if (BackCamera != null)
			{
				BackCamera.enabled = false;
			}
			if (FrontCamera != null)
			{
				FrontCamera.enabled = false;
			}
		}

        private void Setup()
		{
			GameObject backCamera = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(backCamera.GetComponent<MeshRenderer>());
			_referenceCamera = GameObject.Find("Camera (eye)");
			if (_referenceCamera != null)
			{
				backCamera.transform.localScale = _referenceCamera.transform.localScale;
				Rigidbody rigidbody = backCamera.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
				bool flag2 = backCamera.GetComponent<Collider>();
				if (flag2)
				{
					backCamera.GetComponent<Collider>().enabled = false;
				}
				backCamera.GetComponent<Renderer>().enabled = false;
				backCamera.AddComponent<Camera>();
				GameObject referenceCamera = _referenceCamera;
				backCamera.transform.parent = referenceCamera.transform;
				backCamera.transform.rotation = referenceCamera.transform.rotation;
				backCamera.transform.position = referenceCamera.transform.position;
				backCamera.transform.position -= backCamera.transform.forward * 2f;
				referenceCamera.GetComponent<Camera>().enabled = false;
				backCamera.GetComponent<Camera>().fieldOfView = 75f;
				backCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
				backCamera.AddComponent<HighlightsFXStandalone>();
				backCamera.name = "Blaze.BackCamera";
				_cameraBackObj = backCamera;
				BackCamera = _cameraBackObj.GetComponent<Camera>();
				GameObject frontCamera = GameObject.CreatePrimitive(PrimitiveType.Cube);
				UnityEngine.Object.Destroy(frontCamera.GetComponent<MeshRenderer>());
				frontCamera.transform.localScale = _referenceCamera.transform.localScale;
				Rigidbody rigidbody2 = frontCamera.AddComponent<Rigidbody>();
				rigidbody2.isKinematic = true;
				rigidbody2.useGravity = false;
				if (frontCamera.GetComponent<Collider>())
				{
					frontCamera.GetComponent<Collider>().enabled = false;
				}
				frontCamera.GetComponent<Renderer>().enabled = false;
				frontCamera.AddComponent<Camera>();
				frontCamera.transform.parent = referenceCamera.transform;
				frontCamera.transform.rotation = referenceCamera.transform.rotation;
				frontCamera.transform.Rotate(0f, 180f, 0f);
				frontCamera.transform.position = referenceCamera.transform.position;
				frontCamera.transform.position += -frontCamera.transform.forward * 2f;
				referenceCamera.GetComponent<Camera>().enabled = false;
				frontCamera.GetComponent<Camera>().fieldOfView = 75f;
				frontCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
				frontCamera.AddComponent<HighlightsFXStandalone>();
				frontCamera.name = "Blaze.FrontCamera";
				_cameraFrontObj = frontCamera;
				FrontCamera = _cameraFrontObj.GetComponent<Camera>();
				_cameraBackObj.GetComponent<Camera>().enabled = false;
				_cameraFrontObj.GetComponent<Camera>().enabled = false;
				GameObject.Find("Camera (eye)").GetComponent<Camera>().enabled = true;
				rigidbody = null;
				referenceCamera = null;
				frontCamera = null;
				rigidbody2 = null;
			}
		}

		internal enum CameraMode
		{
			Normal,
			ThirdPersonBack,
			ThirdPersonFront
		}

		private static Camera _currentCamera;
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

		private static GameObject _eyeCameraObject;
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

		private static Camera _mainCamera;
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
	}
}
