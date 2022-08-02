using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Modules
{
    public class PlayerOrbit : BModule
    {
        public static QMToggleButton ToggleButton;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazePlayerOrbit>();
        }

        public override void UI()
        {
            ToggleButton = new QMToggleButton(BlazeQM.Orbits, 3, 0, "Player Orbit", delegate
            {
                Main.BlazesComponents.AddComponent<BlazePlayerOrbit>();
            }, delegate
            {
                UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazePlayerOrbit>());
                if (ToggleButton != null && !Flight.FlightState && PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>() != null)
                {
                    PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                }
            }, "Toggle oribiting yourself around the target");
        }

        public override void SceneInitialized(int buildIndex, string sceneName)
        {
            if (ToggleButton != null && buildIndex == -1)
            {
                ToggleButton.SetToggleState(false, true);
            }
        }
    }

    public class BlazePlayerOrbit : MonoBehaviour
    {
        public BlazePlayerOrbit(IntPtr id) : base(id) { }

        public void Update()
        {
            try
            {
                if (Main.Target == null || Main.Target != null && Main.Target.GetUserID() == PlayerUtils.CurrentUser().GetUserID()) return;
                if (PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled != false)
                {
                    PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                }
                float axis = Input.GetAxis("Horizontal");
                float axis2 = Input.GetAxis("Vertical");
                if (axis < -0.1f || axis > 0.1f || axis2 < -0.1f || axis2 > 0.1f || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    PlayerOrbit.ToggleButton.SetToggleState(false, true);
                    return;
                }
                GameObject obj = new();
                Vector3 vector = Config.Main.OrbitAnnoyanceMode ? Main.Target.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Head) : Main.Target.transform.position;
                obj.transform.position = vector;
                obj.transform.Rotate(new Vector3(0f, 1f, 0f), Time.time * Config.Main.PlayerOrbitSpeed * 90f);
                VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = obj.transform.position + obj.transform.forward * Config.Main.PlayerOrbitSize;
                Destroy(obj);
            }
            catch { }
        }
    }
}
