using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Blaze.Modules
{
    public class Flight : BModule
    {
        public static QMToggleButton ToggleButton;
        public static bool FlightState;
        public static Vector3 CachedGravity;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeFlight>();
        }

        public override void UI()
        {
            ToggleButton = new QMToggleButton(BlazeQM.Movement, 1, 0, "Flight", delegate
            {
                FlightState = true;
                CachedGravity = Physics.gravity;
                //AW.flightButton.SetButtonText("Flight: On");
                if (PlayerUtils.CurrentUser().gameObject.GetComponent<BlazeFlight>() == null)
                    PlayerUtils.CurrentUser().gameObject.AddComponent<BlazeFlight>();
                PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
            }, delegate
            {
                FlightState = false;
                //AW.flightButton.SetButtonText("Flight: Off");
                if (PlayerUtils.CurrentUser().gameObject.GetComponent<BlazeFlight>() != null)
                    UnityEngine.Object.Destroy(PlayerUtils.CurrentUser().gameObject.GetComponent<BlazeFlight>());
                PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
            }, "Toggle flying for yourself");

            new QMToggleButton(BlazeQM.Movement, 2, 0, "Directional Fly", delegate
            {
                Config.Main.DirectionalFly = true;
            }, delegate
            {
                Config.Main.DirectionalFly = false;
            }, "Change how flight works", Config.Main.DirectionalFly);
        }
    }

    public class BlazeFlight : MonoBehaviour
    {
        public BlazeFlight(IntPtr id) : base(id) { }
        private static VRCPlayer currentPlayer;
        private static Transform camTransform;
        private static bool isInVR;

        public void Awake()
        {
            Physics.gravity = Vector3.zero;
        }

        public void OnDestroy()
        {
            Physics.gravity = Flight.CachedGravity;
            Flight.ToggleButton.SetToggleState(false);
        }

        public void FixedUpdate()
        {
            if (currentPlayer == null || transform == null)
            {
                currentPlayer = PlayerUtils.CurrentUser();
                isInVR = XRDevice.isPresent;
                camTransform = Camera.main.transform;
            }

            if (Input.GetKeyDown((KeyCode)304))
            {
                Config.Main.DesktopFlySpeed *= 2f;
            }

            if (Input.GetKeyUp((KeyCode)304))
            {
                Config.Main.DesktopFlySpeed /= 2f;
            }
            if (!Main.QMIsOpened && !Main.SMIsOpened && !Main.AMIsOpened)
            {
                if (!Config.Main.DirectionalFly)
                {
                    if (isInVR)
                    {
                        {
                            if (Math.Abs(Input.GetAxis("Vertical")) != 0f)
                                currentPlayer.transform.position += currentPlayer.transform.forward *
                                                                    Config.Main.VRFlySpeed * Time.deltaTime *
                                                                    Input.GetAxis("Vertical");

                            if (Math.Abs(Input.GetAxis("Horizontal")) != 0f)
                                currentPlayer.transform.position += currentPlayer.transform.right *
                                                                    Config.Main.VRFlySpeed * Time.deltaTime *
                                                                    Input.GetAxis("Horizontal");

                            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f)
                                currentPlayer.transform.position += currentPlayer.transform.up *
                                                                    Config.Main.VRFlySpeed * Time.deltaTime *
                                                                    Input.GetAxisRaw(
                                                                        "Oculus_CrossPlatform_SecondaryThumbstickVertical");

                            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f)
                                currentPlayer.transform.position += currentPlayer.transform.up *
                                                                    Config.Main.VRFlySpeed * Time.deltaTime *
                                                                    Input.GetAxisRaw(
                                                                        "Oculus_CrossPlatform_SecondaryThumbstickVertical");
                        }
                    }
                    else
                    {
                        if (Input.GetKey((KeyCode)101))
                            currentPlayer.transform.position += currentPlayer.transform.up *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)113))
                            currentPlayer.transform.position += currentPlayer.transform.up * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)119))
                            currentPlayer.transform.position += currentPlayer.transform.forward *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)97))
                            currentPlayer.transform.position += currentPlayer.transform.right * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)100))
                            currentPlayer.transform.position += currentPlayer.transform.right *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)115))
                            currentPlayer.transform.position += currentPlayer.transform.forward * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;
                    }
                }
                else
                {
                    if (isInVR)
                    {
                        if (Math.Abs(Input.GetAxis("Vertical")) != 0f)
                            currentPlayer.transform.position += camTransform.transform.forward *
                                                                Config.Main.VRFlySpeed * Time.deltaTime *
                                                                Input.GetAxis("Vertical");

                        if (Math.Abs(Input.GetAxis("Horizontal")) != 0f)
                            currentPlayer.transform.position += camTransform.transform.right *
                                                                Config.Main.VRFlySpeed * Time.deltaTime *
                                                                Input.GetAxis("Horizontal");

                        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f)
                            currentPlayer.transform.position += camTransform.transform.up *
                                                                Config.Main.VRFlySpeed * Time.deltaTime *
                                                                Input.GetAxisRaw(
                                                                    "Oculus_CrossPlatform_SecondaryThumbstickVertical");

                        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f)
                            currentPlayer.transform.position += camTransform.transform.up *
                                                                Config.Main.VRFlySpeed * Time.deltaTime *
                                                                Input.GetAxisRaw(
                                                                    "Oculus_CrossPlatform_SecondaryThumbstickVertical");
                    }
                    else
                    {
                        if (Input.GetKey((KeyCode)101))
                            currentPlayer.transform.position += camTransform.transform.up *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)113))
                            currentPlayer.transform.position += camTransform.transform.up * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)119))
                            currentPlayer.transform.position += camTransform.transform.forward *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)97))
                            currentPlayer.transform.position += camTransform.transform.right * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)100))
                            currentPlayer.transform.position += camTransform.transform.right *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;

                        if (Input.GetKey((KeyCode)115))
                            currentPlayer.transform.position += camTransform.transform.forward * -1f *
                                                                Config.Main.DesktopFlySpeed * Time.deltaTime;
                    }
                }
            }
        }
    }
}
