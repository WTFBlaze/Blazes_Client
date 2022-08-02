using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;

namespace Blaze.Modules
{
    class SimpleMovement : BModule
    {
        /*private static QMSingleButton PlayerSpeedLabel;
        private static float WalkSpeed;
        private static float RunSpeed;
        private static float StrafeSpeed;
        private static float DefaultWalkSpeed;
        private static float DefaultRunSpeed;
        private static float DefaultStrafeSpeed;*/


        public override void QuickMenuUI()
        {
            new QMToggleButton(BlazeMenu.Movement, 3, 0, "Mouse TP", delegate
            {
                Config.Main.MouseTP = true;
            }, delegate
            {
                Config.Main.MouseTP = false;
            }, "Toggle allowing yourself to teleport to where your mouse cursor is looking [Left Control + Left Mouse Click]", Config.Main.MouseTP);

            new QMToggleButton(BlazeMenu.Movement, 4, 0, "Infinite Jump", delegate
            {
                Config.Main.InfiniteJump = true;
            }, delegate
            {
                Config.Main.InfiniteJump = false;
            }, "Toggle being able to jump infinitely without having to touch the ground again", Config.Main.InfiniteJump);

            /*new QMToggleButton(BlazeMenu.Movement, 2, 1, "Bunny Hop", delegate
            {
                Config.Main.BunnyHop = true;
            }, delegate
            {
                Config.Main.BunnyHop = false;
            }, "");*/

            /*new QMSingleButton(BlazeMenu.Movement, 1, 0.9f, "Speed<color=green>+</color>", delegate
            {
                WalkSpeed++;
                RunSpeed++;
                StrafeSpeed++;
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetWalkSpeed(WalkSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetRunSpeed(RunSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetStrafeSpeed(StrafeSpeed);
                PlayerSpeedLabel.SetButtonText($"Speed: <color=yellow>{WalkSpeed}</color>");
            }, "Increase your player speed", null, null, true);

            PlayerSpeedLabel = new QMSingleButton(BlazeMenu.Movement, 1, 1.4f, $"Speed: <color=yellow>{WalkSpeed}</color>", delegate
            {
                WalkSpeed = DefaultWalkSpeed;
                RunSpeed = DefaultRunSpeed;
                StrafeSpeed = DefaultStrafeSpeed;
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetWalkSpeed(WalkSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetRunSpeed(RunSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetStrafeSpeed(StrafeSpeed);
                PlayerSpeedLabel.SetButtonText($"Speed: <color=yellow>{WalkSpeed}</color>");
            }, "Click the label to reset your player speed back to default");*/
        }

        public override void LocalPlayerLoaded()
        {
            /*Functions.Delay(delegate
            {
                DefaultWalkSpeed = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetWalkSpeed();
                DefaultRunSpeed = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetRunSpeed();
                DefaultStrafeSpeed = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetStrafeSpeed();

                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetWalkSpeed(WalkSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetRunSpeed(RunSpeed);
                PlayerUtils.CurrentUser().GetVRCPlayerApi().SetStrafeSpeed(StrafeSpeed);
                if (PlayerSpeedLabel != null)
                    PlayerSpeedLabel.SetButtonText($"Speed: <color=yellow>{WalkSpeed}</color>");
            }, 3f);*/
        }

        //private static Vector3 bhopVector = Vector3.zero;
        //private static float bhopValue = 6f;

        public override void Update()
        {
            if (!WorldUtils.IsInRoom()) return;
            if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0 && !Networking.LocalPlayer.IsPlayerGrounded() && Config.Main.InfiniteJump)
            {
                var Jump = Networking.LocalPlayer.GetVelocity();
                Jump.y = Networking.LocalPlayer.GetJumpImpulse();
                Networking.LocalPlayer.SetVelocity(Jump);
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0) && Config.Main.MouseTP)
            {
                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                if (Physics.Raycast(ray, out var raycastHit)) PlayerUtils.CurrentUser().transform.position = raycastHit.point;
            }

            /*if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Move").prop_Boolean_0  && Networking.LocalPlayer.IsPlayerGrounded() && Config.Main.BunnyHop)
            {

                PlayerUtils.CurrentUser().transform.position += PlayerUtils.CurrentUser().transform.up * bhopValue * Time.deltaTime;
                                                                //bhopValue + Physics.gravity.y * Time.deltaTime;
                return;
            }*/


            /* if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Single_1 == 1 && Config.Instance.RocketJump)
             {
                 var jump = Networking.LocalPlayer.GetVelocity();
                 jump.y = Networking.LocalPlayer.GetJumpImpulse();
                 Networking.LocalPlayer.SetVelocity(jump);
             }*/
        }
    }
}
