﻿using Blaze.API.QM;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Modules
{
    public class ColliderHider : BModule
    {
        private static Transform head;
        private static VRC_AnimationController animController;
        private static VRCVrIkController ikController;
        private static readonly bool state;
        private static bool changed;
        private static QMToggleButton ToggleButton;

        public override void UI()
        {
            ToggleButton = new QMToggleButton(BlazeQM.Exploits, 2, 2, "Collider Hider", delegate
            {
                ToggleColliderHider(true);
            }, delegate
            {
                ToggleColliderHider(false);
            }, "Enables Flight & moves your collider deep into the ground");
        }

        public override void LocalPlayerLoaded()
        {
            if (ToggleButton != null)
            {
                ToggleButton.SetToggleState(false);
            }
        }

        public static void ToggleColliderHider(bool state)
        {
            VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            if ((field_Internal_Static_VRCPlayer_?.transform) == null)
            {
                return;
            }
            if (head == null)
            {
                head = VRCVrCamera.field_Private_Static_VRCVrCamera_0.transform.parent;
            }
            if (animController == null)
            {
                animController = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponentInChildren<VRC_AnimationController>();
            }
            if (ikController == null)
            {
                ikController = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponentInChildren<VRCVrIkController>();
            }
            VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += new Vector3(0f, (float)(state ? -4 : 4), 0f);
            animController.field_Private_Boolean_0 = !state;
            MelonCoroutines.Start(ToggleIKController());
            if (state)
            {
                if (!Flight.FlightState)
                {
                    Flight.ToggleButton.SetToggleState(true, true);
                    //PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                    changed = true;
                }
                else
                {
                    changed = false;
                }
                head.localPosition += new Vector3(0f, 4f / head.parent.transform.localScale.y, 0f);
                return;
            }
            head.localPosition = Vector3.zero;
            if (changed)
            {
                Flight.ToggleButton.SetToggleState(false, true);
                //PlayerUtils.CurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
            }
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            animController = null;
            ikController = null;
        }

        private static IEnumerator ToggleIKController()
        {
            if (state)
            {
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return null;
            }
            ikController.field_Private_Boolean_0 = !state;
            yield break;
        }
    }
}
