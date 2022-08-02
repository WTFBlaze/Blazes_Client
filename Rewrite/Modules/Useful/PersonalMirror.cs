using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.SDK3.Components;

namespace Blaze.Modules
{
    public class PersonalMirror : BModule
    {
        private GameObject _mirror;
        private float _mirrorScaleX;
        private float _mirrorScaleY;
        private float _oldMirrorScaleY;
        private bool _optimizedMirror;
        private bool _canPickupMirror;
        private QMNestedButton Menu;
        private QMToggleButton ToggleButton;
        private QMSingleButton XLabel;
        private QMSingleButton YLabel;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Self, "Personal\nMirror", 2, 0, "Click to view options for personal mirror", "Personal Mirror");
            ToggleButton = new QMToggleButton(Menu, 1, 0, "Personal Mirror", delegate
            {
                ToggleMirror(true);
            }, delegate
            {
                ToggleMirror(false);
            }, "Toggles a personal mirror for yourself that you can move and change the size of");
            new QMSingleButton(Menu, 3, 0, "Size X<color=green>++</color>", delegate
            {
                Config.Main.MirrorScaleX++;
                Refresh();
            }, "Increase the size of your personal mirror's X axis");

            new QMSingleButton(Menu, 4, 0, "Size X<color=red>--</color>", delegate
            {
                Config.Main.MirrorScaleX--;
                Refresh();
            }, "Decrease the size of your personal mirror's X axis");

            new QMSingleButton(Menu, 3, 1, "Size Y<color=green>++</color>", delegate
            {
                Config.Main.MirrorScaleY++;
                Refresh();
            }, "Increase the size of your personal mirror's Y axis");

            new QMSingleButton(Menu, 4, 1, "Size Y<color=red>--</color>", delegate
            {
                Config.Main.MirrorScaleY--;
                Refresh();
            }, "Decrease the size of your personal mirror's Y axis");

            XLabel = new QMSingleButton(Menu, 2, 0, $"X Size: <color=yellow>{Config.Main.MirrorScaleX}</color>", delegate { }, "Your current Personal Mirror X Axis Value");

            YLabel = new QMSingleButton(Menu, 2, 1, $"Y Size: <color=yellow>{Config.Main.MirrorScaleY}</color>", delegate { }, "Your current Personal Mirror Y Axis Value");

            new QMToggleButton(Menu, 1, 1, "Can Pickup", delegate
            {
                Config.Main.CanPickupPersonalMirror = true;
                Refresh();
            }, delegate
            {
                Config.Main.CanPickupPersonalMirror = false;
                Refresh();
            }, "Toggle being able to pickup your personal mirror", Config.Main.CanPickupPersonalMirror);

            new QMToggleButton(Menu, 1, 2, "Optimize Mirror", delegate
            {
                Config.Main.OptimizedPersonalMirror = true;
                Refresh();
            }, delegate
            {
                Config.Main.OptimizedPersonalMirror = false;
                Refresh();
            }, "Toggle optimizing your personal mirror", Config.Main.OptimizedPersonalMirror);
        }

        public override void LocalPlayerLoaded()
        {
            if (ToggleButton != null)
            {
                ToggleButton.SetToggleState(false);
            }
        }

        private void Refresh()
        {
            _oldMirrorScaleY = _mirrorScaleY;
            _mirrorScaleX = Config.Main.MirrorScaleX;
            _mirrorScaleY = Config.Main.MirrorScaleY;
            _optimizedMirror = Config.Main.OptimizedPersonalMirror;
            _canPickupMirror = Config.Main.CanPickupPersonalMirror;
            XLabel.SetButtonText($"X Size: <color=yellow>{Config.Main.MirrorScaleX}</color>");
            YLabel.SetButtonText($"Y Size: <color=yellow>{Config.Main.MirrorScaleY}</color>");

            if (_mirror != null && PlayerUtils.CurrentUser() != null)
            {
                _mirror.transform.localScale = new Vector3(_mirrorScaleX, _mirrorScaleY, 1f);
                _mirror.transform.position = new Vector3(_mirror.transform.position.x, _mirror.transform.position.y + ((_mirrorScaleY - _oldMirrorScaleY) / 2), _mirror.transform.position.z);
                _mirror.GetOrAddComponent<VRCMirrorReflection>().m_ReflectLayers = Config.Main.OptimizedPersonalMirror ? WorldToggles.optimizeMask : WorldToggles.beautifyMask;
                _mirror.GetOrAddComponent<VRCPickup>().pickupable = _canPickupMirror;
            }
        }

        private void ToggleMirror(bool state)
        {
            if (state)
            {
                VRCPlayer player = PlayerUtils.CurrentUser();
                Vector3 pos = player.transform.position + player.transform.forward;
                pos.y += Config.Main.MirrorScaleY / 2;
                _mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _mirror.transform.position = pos;
                _mirror.transform.rotation = player.transform.rotation;
                _mirror.transform.localScale = new Vector3(Config.Main.MirrorScaleX, Config.Main.MirrorScaleY, 1f);
                _mirror.name = "Blaze's Personal Mirror";
                UnityEngine.Object.Destroy(_mirror.GetComponent<Collider>());
                _mirror.GetOrAddComponent<BoxCollider>().size = new Vector3(1f, 1f, 0.05f);
                _mirror.GetOrAddComponent<BoxCollider>().isTrigger = true;
                _mirror.GetOrAddComponent<MeshRenderer>().material.shader = Shader.Find("FX/MirrorReflection");
                _mirror.GetOrAddComponent<VRCMirrorReflection>().m_ReflectLayers = Config.Main.OptimizedPersonalMirror ? WorldToggles.optimizeMask : WorldToggles.beautifyMask;
                _mirror.GetOrAddComponent<VRCPickup>().proximity = 0.3f;
                _mirror.GetOrAddComponent<VRCPickup>().pickupable = Config.Main.CanPickupPersonalMirror;
                _mirror.GetOrAddComponent<VRCPickup>().allowManipulationWhenEquipped = false;
                _mirror.GetOrAddComponent<Rigidbody>().useGravity = false;
                _mirror.GetOrAddComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                if (_mirror != null)
                {
                    UnityEngine.Object.Destroy(_mirror);
                    _mirror = null;
                }
            }
        }
    }
}
