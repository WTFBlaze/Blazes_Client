using Blaze.API.QM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class WorldToggles : BModule
    {
        public static List<OriginalMirror> originalMirrors = new();
        public static LayerMask optimizeMask;
        public static LayerMask beautifyMask;
        public static QMToggleButton OptimizeMirrors;
        public static QMToggleButton BeautifyMirrors;

        public class OriginalMirror
        {
            public VRC_MirrorReflection MirrorParent;
            public LayerMask OriginalLayers;
        }

        static WorldToggles()
        {
            LayerMask layerMask = default;
            layerMask.value = 263680;
            optimizeMask = layerMask;
            layerMask = default;
            layerMask.value = -1025;
            beautifyMask = layerMask;
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            originalMirrors = new List<OriginalMirror>();
            if (buildIndex == -1)
            {
                foreach (VRC_MirrorReflection vrc_MirrorReflection in Resources.FindObjectsOfTypeAll<VRC_MirrorReflection>())
                {
                    originalMirrors.Add(new OriginalMirror
                    {
                        MirrorParent = vrc_MirrorReflection,
                        OriginalLayers = vrc_MirrorReflection.m_ReflectLayers
                    });
                }
            }
        }

        public override void LocalPlayerLoaded()
        {
            if (Config.Main.DisableBloom) ToggleBloom(false);
            if (Config.Main.DisablePickups) TogglePickups(false);
            if (Config.Main.OptimizeMirrors && !Config.Main.BeautifyMirrors) Optimize();
            if (!Config.Main.OptimizeMirrors && Config.Main.BeautifyMirrors) Beautify();
        }

        public override void UI()
        {
            new QMToggleButton(BlazeQM.Worlds, 3, 2, "Disable Bloom", delegate
            {
                Config.Main.DisableBloom = true;
                ToggleBloom(false);
            }, delegate
            {
                Config.Main.DisableBloom = false;
                ToggleBloom(true);
            }, "Disable bloom in all worlds", Config.Main.DisableBloom);

            new QMToggleButton(BlazeQM.Worlds, 4, 2, "Disable Pickups", delegate
            {
                Config.Main.DisablePickups = true;
                TogglePickups(false);
            }, delegate
            {
                Config.Main.DisablePickups = false;
                TogglePickups(true);
            }, "Disable pickups for yourself in any world", Config.Main.DisablePickups);

            OptimizeMirrors = new QMToggleButton(BlazeQM.Worlds, 1, 3, "Optimize Mirrors", delegate
            {
                Config.Main.OptimizeMirrors = true;
                Optimize();
                if (Config.Main.BeautifyMirrors)
                {
                    Config.Main.BeautifyMirrors = false;
                    BeautifyMirrors.SetToggleState(false);
                }
            }, delegate
            {
                Config.Main.OptimizeMirrors = false;
                Revert();
            }, "Force all mirrors to be optimized instead of the default stance", Config.Main.OptimizeMirrors);

            BeautifyMirrors = new QMToggleButton(BlazeQM.Worlds, 2, 3, "Beautify Mirrors", delegate
            {
                Config.Main.BeautifyMirrors = true;
                Beautify();
                if (Config.Main.OptimizeMirrors)
                {
                    Config.Main.OptimizeMirrors = false;
                    OptimizeMirrors.SetToggleState(false);
                }
            }, delegate
            {
                Config.Main.BeautifyMirrors = false;
                Revert();
            }, "Force all mirrors to be the highest quality instead of the default stance", Config.Main.BeautifyMirrors);
        }

        public static void ToggleBloom(bool newState)
        {
            foreach (var b in Main.Blooms)
            {
                b.enabled = newState;
            }
        }

        public static void TogglePickups(bool newState)
        {
            foreach (var p in Main.Pickups)
            {
                p.gameObject.SetActive(newState);
            }
        }

        public static void ToggleSeats(bool newState)
        {
            foreach (var s in Main.Seats)
            {
                s.enabled = false;
            }
        }

        public static void Optimize()
        {
            if (originalMirrors.Count != 0)
            {
                foreach (OriginalMirror originalMirror in originalMirrors)
                {
                    originalMirror.MirrorParent.m_ReflectLayers = optimizeMask;
                }
            }
        }

        public static void Beautify()
        {
            if (originalMirrors.Count != 0)
            {
                foreach (OriginalMirror originalMirror in originalMirrors)
                {
                    originalMirror.MirrorParent.m_ReflectLayers = beautifyMask;
                }
            }
        }

        public static void Revert()
        {
            if (originalMirrors.Count != 0)
            {
                foreach (OriginalMirror originalMirror in originalMirrors)
                {
                    originalMirror.MirrorParent.m_ReflectLayers = originalMirror.OriginalLayers;
                }
            }
        }
    }
}
