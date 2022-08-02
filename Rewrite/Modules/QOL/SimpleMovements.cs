using Blaze.API.QM;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class SimpleMovements : BModule
    {
        public override void UI()
        {
            new QMToggleButton(BlazeQM.Movement, 3, 0, "Mouse TP", delegate
            {
                Config.Main.MouseTP = true;
            }, delegate
            {
                Config.Main.MouseTP = false;
            }, "Toggle allowing yourself to teleport to where your mouse cursor is looking [Left Control + Left Mouse Click]", Config.Main.MouseTP);

            new QMToggleButton(BlazeQM.Movement, 4, 0, "Infinite Jump", delegate
            {
                Config.Main.InfiniteJump = true;
            }, delegate
            {
                Config.Main.InfiniteJump = false;
            }, "Toggle being able to jump infinitely without having to touch the ground again", Config.Main.InfiniteJump);
        }

        //private Vector3 pos = Vector3.zero;
        //private bool BHop = true;

        public override void Update()
        {
            if (!WorldUtils.IsInRoom()) return;
            if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0 && !Networking.LocalPlayer.IsPlayerGrounded() && Config.Main.InfiniteJump)
            {
                var Jump = Networking.LocalPlayer.GetVelocity();
                Jump.y = Networking.LocalPlayer.GetJumpImpulse();
                Networking.LocalPlayer.SetVelocity(Jump);
            }

            /*if (Input.GetKey(KeyCode.Space))
            {
                //Vector3 pos = Vector3.zero;
                //pos.y += PlayerUtils.CurrentUser().transform.position.y + Math.Abs(Physics.gravity.y) * Time.deltaTime;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(new Vector3(0, (Physics.gravity.y + 50f * Time.deltaTime), 0) * Time.deltaTime);
            }*/

            /*if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0 && Networking.LocalPlayer.IsPlayerGrounded() && BHop)
            {
                pos.y += Physics.gravity.y * Time.deltaTime;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
                return;
            }
            if (pos.y > 0f)
            {
                pos.y += Physics.gravity.y * Time.deltaTime;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
                return;
            }
            if (pos.y < 0f)
            {
                pos.y = 0f;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
            }*/
            /*if (Networking.LocalPlayer.IsPlayerGrounded() && !Flight.FlightState)
            {
                Vector3 pos = Vector3.zero;
                pos.y = 6f + Physics.gravity.y * Time.deltaTime;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
            }
            *//*if (pos.y > 0f)
            {
                pos.y += Physics.gravity.y * Time.deltaTime;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
                return;
            }
            if (pos .y < 0f)
            {
                pos.y = 0f;
                PlayerUtils.CurrentUser().GetComponent<CharacterController>().Move(pos * Time.deltaTime);
            }*/

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0) && Config.Main.MouseTP)
            {
                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                if (Physics.Raycast(ray, out var raycastHit)) PlayerUtils.CurrentUser().transform.position = raycastHit.point;
            }
        }

        /*private IEnumerator MovePlayer(Vector3 target)
        {
            while (PlayerUtils.CurrentUser().transform.position != target)
            {
                PlayerUtils.CurrentUser().transform.position = Vector3.Lerp(PlayerUtils.CurrentUser().transform.position, target, Time.deltaTime * 1f);
                yield return null;
            }
            yield break;
        }*/
    }
}
