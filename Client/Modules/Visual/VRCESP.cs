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
    class VRCESP : BModule
    {
        internal static bool CapsuleState;
        internal static QMToggleButton CapsuleESP;
        internal static List<HighlightsFXStandalone> portalHighlights = new();

        public override void QuickMenuUI()
        {
            CapsuleESP = new QMToggleButton(BlazeMenu.Renders, 2, 0, "Capsule ESP", delegate
            {
                ToggleCapsules(true);
            }, delegate
            {
                ToggleCapsules(false);
            }, "Toggle an ESP Bubble around all player's capsules");

            new QMToggleButton(BlazeMenu.Renders, 3, 0, "Pickups ESP", delegate
            {
                TogglePickupESP(true);
            }, delegate
            {
                TogglePickupESP(false);
            }, "Toggle an ESP Bubble around all pickups");

            new QMToggleButton(BlazeMenu.Renders, 2, 1, "Interactables ESP", delegate
            {
                ToggleInteractables(true);
            }, delegate
            {
                ToggleInteractables(false);
            }, "Toggle an ESP Bubble around all interactable objects");

            new QMToggleButton(BlazeMenu.Renders, 3, 1, "Portals ESP", delegate
            {
                TogglePortals(true);
            }, delegate
            {
                TogglePortals(false);
            }, "Toggle an ESP Bubble around all portals");
        }

        public override void PlayerJoined(VRC.Player player)
        {
            if (player.GetUserID() == PlayerUtils.CurrentUser().GetUserID()) return;
            if (CapsuleState)
            {
                MelonCoroutines.Start(WaitForESP(player.GetComponent<BlazesPlayerInfo>()));
            }
        }

        private static IEnumerator WaitForESP(BlazesPlayerInfo target)
        {
            while (target.highlightOptions == null) yield return null;
            target.highlightOptions.enabled = true;
        }

        private void ToggleCapsules(bool state)
        {
            CapsuleState = state;
            foreach (var p in WorldUtils.GetPlayers())
            {
                var comp = p.GetComponent<BlazesPlayerInfo>();
                if (comp.apiUser.id != PlayerUtils.CurrentUser().GetUserID())
                {
                    comp.highlightOptions.enabled = state;
                }
            }
        }

        private void TogglePickupESP(bool state)
        {
            try
            {
                List<VRC_Pickup> Items = UnityEngine.Object.FindObjectsOfType<VRC_Pickup>().ToList();
                for (int i = 0; i < Items.Count; i++)
                {
                    HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(Items[i].GetComponent<Renderer>(), state);
                }
            }
            catch { }
        }

        private void ToggleInteractables(bool State)
        {
            try
            {
                List<VRC_Interactable> Items = UnityEngine.Object.FindObjectsOfType<VRC_Interactable>().ToList();
                for (int i = 0; i < Items.Count; i++)
                {
                    HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(Items[i].GetComponent<Renderer>(), State);
                }
            }
            catch { }
        }

        private void TogglePortals(bool State)
        {
            try
            {
                List<PortalInternal> Items = UnityEngine.Object.FindObjectsOfType<PortalInternal>().ToList();
                for (int i = 0; i < Items.Count; i++)
                {
                    //HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(Items[i].GetComponent<Renderer>(), State);
                    if (Items[i].gameObject.GetComponent<HighlightsFXStandalone>() == null)
                    {
                        var comp = CameraUtils.MainCamera.gameObject.AddHighlighter();
                        comp.highlightColor = Color.red;
                        var ObjRenderers = Items[i].gameObject.transform.GetComponentsInChildren<Renderer>(true);
                        var ObjMeshRenderers = Items[i].gameObject.transform.GetComponentsInChildren<MeshRenderer>(true);
                        foreach (var ObjRenderer in ObjRenderers)
                        {
                            if (ObjRenderer != null && ObjRenderer.gameObject.name != "PortalFringe")
                            {
                                comp.SetHighLighter(ObjRenderer, true);
                            }
                        }
                        foreach (var ObjMeshRenderer in ObjMeshRenderers)
                        {
                            if (ObjMeshRenderer != null && ObjMeshRenderer.gameObject.name != "PortalFringe")
                            {
                                comp.SetHighLighter(ObjMeshRenderer, true);
                            }
                        }
                        portalHighlights.Add(comp);
                    }
                }

                foreach (var p in portalHighlights)
                {
                    p.enabled = State;
                }
            }
            catch { }
        }
    }
}
