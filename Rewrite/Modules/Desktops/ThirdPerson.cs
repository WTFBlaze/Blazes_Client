using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using UnityEngine;

namespace Blaze.Modules
{
    public class ThirdPerson : BModule
    {
		public static CameraMode Mode { get; set; } = CameraMode.Normal;
		public static Camera FrontCamera { get; private set; }
		public static Camera BackCamera { get; private set; }
		public static GameObject _cameraFrontObj;
		public static GameObject _cameraBackObj;
		public static GameObject _referenceCamera;
		public static float _offset = 1.5f;
		public static bool ThirdPersonState;

        public override void Start()
        {
			ComponentManager.RegisterIl2Cpp<BlazeThirdPerson>();
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

		public override void UI()
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
				Main.BlazesComponents.AddComponent<BlazeThirdPerson>();
			}
		}

		public enum CameraMode
		{
			Normal,
			ThirdPersonBack,
			ThirdPersonFront
		}

		private static Camera _currentCamera;
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
			}
		}

		private static GameObject _eyeCameraObject;
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

		private static Camera _mainCamera;
		public static Camera MainCamera
		{
			get
			{
				if (_mainCamera == null)
				{
					_mainCamera = CameraUtils.GetVRCVrCamera().field_Public_Camera_0;
				}
				return _mainCamera;
			}
		}
	}

	public class BlazeThirdPerson : MonoBehaviour
    {
		public BlazeThirdPerson(IntPtr id) : base(id) { }

		public void Update()
        {
			if (PlayerUtils.CurrentUser() != null)
			{
				if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha1) && ThirdPerson._cameraBackObj != null && ThirdPerson._cameraFrontObj != null)
				{
					if (ThirdPerson.Mode == ThirdPerson.CameraMode.Normal)
					{
						ThirdPerson.Mode = ThirdPerson.CameraMode.ThirdPersonBack;
						ThirdPerson.CurrentCamera = ThirdPerson.BackCamera;
						ThirdPerson.EyeCameraObject.GetComponent<Camera>().enabled = false;
						ThirdPerson.BackCamera.enabled = true;
						ThirdPerson.FrontCamera.enabled = false;
						ThirdPerson.ThirdPersonState = true;
					}
					else
					{
						if (ThirdPerson.Mode == ThirdPerson.CameraMode.ThirdPersonBack)
						{
							ThirdPerson.Mode = ThirdPerson.CameraMode.ThirdPersonFront;
							ThirdPerson.CurrentCamera = ThirdPerson.FrontCamera;
							ThirdPerson.EyeCameraObject.GetComponent<Camera>().enabled = false;
							ThirdPerson.BackCamera.enabled = false;
							ThirdPerson.FrontCamera.enabled = true;
							ThirdPerson.ThirdPersonState = true;
						}
						else
						{
							ThirdPerson.Mode = ThirdPerson.CameraMode.Normal;
							ThirdPerson.CurrentCamera = ThirdPerson.MainCamera;
							ThirdPerson.EyeCameraObject.GetComponent<Camera>().enabled = true;
							ThirdPerson.BackCamera.enabled = false;
							ThirdPerson.FrontCamera.enabled = false;
							ThirdPerson.ThirdPersonState = false;
						}
					}
					Main.CurrentCamera = ThirdPerson.CurrentCamera;
				}
				if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) < 0f && ThirdPerson.Mode > ThirdPerson.CameraMode.Normal)
				{
					ThirdPerson._offset += 0.1f;
				}
				if (InputUtils.GetAxis("Mouse ScrollWheel", true, false) > 0f && ThirdPerson.Mode > ThirdPerson.CameraMode.Normal)
				{
					ThirdPerson._offset -= 0.1f;
				}
				if (InputUtils.GetMouseButtonDown(2, true, false) && ThirdPerson.Mode > ThirdPerson.CameraMode.Normal)
				{
					ThirdPerson._offset = 1.5f;
				}
				if (ThirdPerson._cameraBackObj != null && ThirdPerson._cameraFrontObj != null)
				{
					ThirdPerson._cameraBackObj.transform.position = ThirdPerson.MainCamera.transform.position - ThirdPerson.MainCamera.transform.forward * ThirdPerson._offset;
					ThirdPerson._cameraFrontObj.transform.position = ThirdPerson.MainCamera.transform.position + ThirdPerson.MainCamera.transform.forward * ThirdPerson._offset;
				}
			}
		}
    }
}
