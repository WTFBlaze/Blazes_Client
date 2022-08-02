using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.XR;

namespace Blaze.Modules
{
    class Flight : BModule
    {
        internal static QMToggleButton ToggleButton;
        internal static bool FlightState;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesFlight>();
        }

        public override void QuickMenuUI()
        {
            ToggleButton = new QMToggleButton(BlazeMenu.Movement, 1, 0, "Flight", delegate
            {
                FlightState = true;
                BlazeInfo.SavedGravity = Physics.gravity;
                //AW.flightButton.SetButtonText("Flight: On");
                if (PlayerUtils.CurrentUser().gameObject.GetComponent<BlazesFlight>() == null)
                    PlayerUtils.CurrentUser().gameObject.AddComponent<BlazesFlight>();
                PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
            }, delegate
            {
                FlightState = false;
                //AW.flightButton.SetButtonText("Flight: Off");
                if (PlayerUtils.CurrentUser().gameObject.GetComponent<BlazesFlight>() != null)
                    UnityEngine.Object.Destroy(PlayerUtils.CurrentUser().gameObject.GetComponent<BlazesFlight>());
                PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
            }, "Toggle flying for yourself");

            new QMToggleButton(BlazeMenu.Movement, 2, 0, "Directional Fly", delegate
            {
                Config.Main.DirectionalFly = true;
            }, delegate
            {
                Config.Main.DirectionalFly = false;
            }, "Change how flight works", Config.Main.DirectionalFly);
        }
    }

    public class BlazesFlight : MonoBehaviour
    {
        public BlazesFlight(IntPtr id) : base(id) { }
        private static VRCPlayer currentPlayer;
        private static Transform camTransform;
        private static bool isInVR;

        public void Awake()
        {
            Physics.gravity = Vector3.zero;
        }

        public void OnDestroy()
        {
            Physics.gravity = BlazeInfo.SavedGravity;
            Flight.ToggleButton.SetToggleState(false);
        }

        public void Update()
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
            if (!BlazeInfo.QMIsOpened && !BlazeInfo.SMIsOpened && !BlazeInfo.AWIsOpened)
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
