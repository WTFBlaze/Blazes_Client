using Blaze.API.QM;
using Blaze.Utils.VRChat;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Modules
{
    class IKManipulation : BModule
    {
        private QMNestedButton Menu;
        public static bool LeftHandUp = false;
        public static bool RightHandUp = false;
        public static bool TwistHead = false;
        public static bool slingy = false;
        public static bool Tpose = false;
        public static bool BrokenBones = false;

        public override void QuickMenuUI()
        {
			Menu = new QMNestedButton(BlazeMenu.Exploits2, "IK\nManipulation", 4, 0, "Manipulate your own IK", "IK Manipulation");

			new QMToggleButton(Menu, 1, 0, "Left Hand Up", delegate
			{
				LeftHandUp = true;
			}, delegate
			{
				LeftHandUp = false;
			}, "Put your left hand up");

			new QMToggleButton(Menu, 2, 0, "Right Hand Up", delegate
			{
				RightHandUp = true;
			}, delegate
			{
				RightHandUp = false;
			}, "Put your right hand up");

			new QMToggleButton(Menu, 3, 0, "Twist Head", delegate
			{
				TwistHead = true;
			}, delegate
			{
				TwistHead = false;
			}, "twist your head around");

			new QMToggleButton(Menu, 4, 0, "Stringy", delegate
			{
				slingy = true;
			}, delegate
			{
				slingy = false;
			}, "Stretch your body like a slinky");

			/*new QMToggleButton(Menu, 1, 1, "T-Pose", delegate
			{
				Tpose = true;
			}, delegate
			{
				Tpose = false;
			}, "");*/

			new QMToggleButton(Menu, 1, 1, "Broken Bones", delegate
			{
				BrokenBones = true;
			}, delegate
			{
				BrokenBones = false;
			}, "Break your bones");
        }

        public override void Update()
        {
			try
			{
				if (LeftHandUp && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.leftArm.positionWeight = 1f;
				}

				if (RightHandUp && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.rightArm.positionWeight = 1f;
				}

				if (TwistHead && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasNeck = false;
				}
				else
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasNeck = true;
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.locomotion.blockingEnabled = true;
				}

				if (slingy && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasChest = false;
				}
				else
				{
					PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>().solver.hasChest = true;
				}

				/*if (Tpose)
				{
					VRIK componentInChildren1 = PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren1.animator.enabled = false;
				}
				else
				{
					VRIK componentInChildren2 = PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren2.animator.enabled = true;
				}*/

				if (BrokenBones && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
				{
					VRIK componentInChildren2 = PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren2.fixTransforms = false;
					componentInChildren2.animator.enabled = false;
				}
				else
				{
					VRIK componentInChildren2 = PlayerUtils.CurrentUser().gameObject.GetComponentInChildren<VRIK>();
					componentInChildren2.fixTransforms = true;
					componentInChildren2.animator.enabled = true;
				}
			}
			catch
			{
			}
		}
    }
}
