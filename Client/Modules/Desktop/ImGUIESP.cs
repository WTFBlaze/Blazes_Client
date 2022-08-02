using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.XR;

namespace Blaze.Modules
{
    class ImGUIESP : BModule
    {
        internal static bool TracersState = false;
        internal static bool BoxState = false;
        internal static bool NameState = false;

        internal static QMToggleButton BoxESP;
        internal static QMToggleButton Tracers;
        internal static QMToggleButton NameESP;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazeIMGUIESP>();
        }

        public override void QuickMenuUI()
        {
            BoxESP = new QMToggleButton(BlazeMenu.Renders, 1, 0, "Box ESP", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    BoxESP.SetToggleState(false);
                    return;
                }
                BoxState = true;
                if (BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    BlazeInfo.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                BoxState = false;
                if (!BoxState && !TracersState && !NameState && BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "Draw Boxes around all player");

            Tracers = new QMToggleButton(BlazeMenu.Renders, 4, 0, "Tracers", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    Tracers.SetToggleState(false);
                    return;
                }
                TracersState = true;
                if (BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    BlazeInfo.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                TracersState = false;
                if (!BoxState && !TracersState && !NameState && BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "Draw lines to all players");

            NameESP = new QMToggleButton(BlazeMenu.Renders, 1, 1, "Name ESP", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    NameESP.SetToggleState(false);
                    return;
                }
                NameState = true;
                if (BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    BlazeInfo.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                NameState = false;
                if (!BoxState && !TracersState && !NameState && BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(BlazeInfo.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "show user's display names through walls");
        }
    }

    public class BlazeIMGUIESP : MonoBehaviour
    {
        public BlazeIMGUIESP(IntPtr id) : base(id) {}

        public void OnGUI()
        {
            try
            {
                if (WorldUtils.IsInRoom() && !BlazeInfo.QMIsOpened && !BlazeInfo.SMIsOpened)
                {
                    var list = WorldUtils.GetPlayers2();
                    list.Remove(PlayerUtils.CurrentUser()._player);
                    foreach (var player in list)
                    {
                        // Box ESP
                        if (ImGUIESP.BoxState)
                        {
                            Vector3 boxPivot = player._vrcplayer.transform.position; //Pivot point NOT at the feet, at the center
                            Vector3 boxFootPos; boxFootPos.x = boxPivot.x; boxFootPos.z = boxPivot.z; boxFootPos.y = boxPivot.y; //At the feet
                            Vector3 boxHeadPos = player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Head);
                            boxHeadPos.y += 0.15f;
                            Vector3 box_w2s_footpos = BlazeInfo.CurrentCamera.WorldToScreenPoint(boxFootPos);
                            Vector3 box_w2s_headpos = BlazeInfo.CurrentCamera.WorldToScreenPoint(boxHeadPos);
                            if (box_w2s_footpos.z > 0f)
                            {
                                DrawBox(box_w2s_footpos, box_w2s_headpos, PlayerUtils.GetRankUnityColor(player.GetAPIUser().GetTrueRank()));
                            }
                        }

                        // Tracers
                        if (ImGUIESP.TracersState)
                        {
                            Vector3 trace_w2s_footpos = BlazeInfo.CurrentCamera.WorldToScreenPoint(player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Hips));
                            if (trace_w2s_footpos.z > 0f) 
                            {
                                DrawTracers(trace_w2s_footpos, PlayerUtils.GetRankUnityColor(player.GetAPIUser().GetTrueRank()));
                            }
                        }

                        // Name ESP
                        if (ImGUIESP.NameState)
                        {
                            Vector3 playerHead = player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Head);
                            playerHead.y += 0.45f;
                            Vector3 vector = BlazeInfo.CurrentCamera.WorldToScreenPoint(playerHead);
                            if (vector.z > 0.0)
                            {
                                Vector3 vector2 = GUIUtility.ScreenToGUIPoint(vector);
                                vector2.y = Screen.height - vector2.y;
                                GUI.Label(new Rect(vector2.x, vector2.y, 250f, 25f), $"<size=18><b><color={player.GetAPIUser().GetTrueRankColor()}>" + player.GetDisplayName() + "</color></b></size>");
                            }
                        }
                    }
                }
            }
            catch { }
        }

        [HideFromIl2Cpp]
        public void DrawTracers(Vector3 pointerPos, Color color)
        {
            RenderManager.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(pointerPos.x, Screen.height - pointerPos.y), color, 2f);
        }

        [HideFromIl2Cpp]
        public void DrawBox(Vector3 footpos, Vector3 headpos, Color color)
        {
            float height = headpos.y - footpos.y;
            float widthOffset = 2f;
            float width = height / widthOffset;
            RenderManager.DrawBox(footpos.x - (width / 2), Screen.height - footpos.y - height, width, height, color, 2f);
        }
    }
}
