using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Blaze.Modules
{
    public class IMGUIESP : BModule
    {
        public static bool TracersState = false;
        public static bool BoxState = false;
        public static bool NameState = false;
        public static QMToggleButton BoxESP;
        public static QMToggleButton Tracers;
        public static QMToggleButton NameESP;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeIMGUIESP>();
        }

        public override void UI()
        {
            BoxESP = new QMToggleButton(BlazeQM.Renders, 1, 1, "Box ESP", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    BoxESP.SetToggleState(false);
                    return;
                }
                BoxState = true;
                if (Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                BoxState = false;
                if (!BoxState && !TracersState && !NameState && Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "Draw Boxes around all player");

            Tracers = new QMToggleButton(BlazeQM.Renders, 2, 1, "Tracers", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    Tracers.SetToggleState(false);
                    return;
                }
                TracersState = true;
                if (Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                TracersState = false;
                if (!BoxState && !TracersState && !NameState && Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "Draw lines to all players");

            NameESP = new QMToggleButton(BlazeQM.Renders, 3, 1, "Name ESP", delegate
            {
                if (XRDevice.isPresent)
                {
                    PopupUtils.InformationAlert("This is a desktop only feature!");
                    NameESP.SetToggleState(false);
                    return;
                }
                NameState = true;
                if (Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeIMGUIESP>();
                }
            }, delegate
            {
                NameState = false;
                if (!BoxState && !TracersState && !NameState && Main.BlazesComponents.GetComponent<BlazeIMGUIESP>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeIMGUIESP>());
                }
            }, "show user's display names through walls");
        }
    }

    public class BlazeIMGUIESP : MonoBehaviour
    {
        public BlazeIMGUIESP(IntPtr id) : base(id) { }

        public void OnGUI()
        {
            try
            {
                if (WorldUtils.IsInRoom())
                {
                    var list = WorldUtils.GetPlayers2();
                    list.Remove(PlayerUtils.CurrentUser()._player);
                    foreach (var player in list)
                    {
                        // Box ESP
                        if (IMGUIESP.BoxState)
                        {
                            Vector3 boxPivot = player._vrcplayer.transform.position; //Pivot point NOT at the feet, at the center
                            Vector3 boxFootPos; boxFootPos.x = boxPivot.x; boxFootPos.z = boxPivot.z; boxFootPos.y = boxPivot.y; //At the feet
                            Vector3 boxHeadPos = player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Head);
                            boxHeadPos.y += 0.15f;
                            Vector3 box_w2s_footpos = Main.CurrentCamera.WorldToScreenPoint(boxFootPos);
                            Vector3 box_w2s_headpos = Main.CurrentCamera.WorldToScreenPoint(boxHeadPos);
                            if (box_w2s_footpos.z > 0f)
                            {
                                DrawBox(box_w2s_footpos, box_w2s_headpos, PlayerUtils.GetRankUnityColor(player.GetAPIUser().GetTrueRank()));
                            }
                        }

                        // Tracers
                        if (IMGUIESP.TracersState)
                        {
                            Vector3 trace_w2s_footpos = Main.CurrentCamera.WorldToScreenPoint(player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Hips));
                            if (trace_w2s_footpos.z > 0f)
                            {
                                DrawTracers(trace_w2s_footpos, PlayerUtils.GetRankUnityColor(player.GetAPIUser().GetTrueRank()));
                            }
                        }

                        // Name ESP
                        if (IMGUIESP.NameState)
                        {
                            Vector3 playerHead = player.GetVRCPlayerApi().GetBonePosition(HumanBodyBones.Head);
                            playerHead.y += 0.45f;
                            Vector3 vector = Main.CurrentCamera.WorldToScreenPoint(playerHead);
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
