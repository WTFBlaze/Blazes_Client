using Blaze.API.QM;
using Blaze.Configs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Blaze.Modules
{
    class WorldOptimizations : BModule
    {
        internal static List<OriginalMirror> originalMirrors = new();

        public class OriginalMirror
        {
            public VRC_MirrorReflection MirrorParent;
            public LayerMask OriginalLayers;
        }

        static WorldOptimizations()
        {
            LayerMask layerMask = default;
            layerMask.value = 263680;
            BlazeInfo.optimizeMask = layerMask;
            layerMask = default;
            layerMask.value = -1025;
            BlazeInfo.beautifyMask = layerMask;
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            originalMirrors = new List<OriginalMirror>();
            foreach (VRC_MirrorReflection vrc_MirrorReflection in Resources.FindObjectsOfTypeAll<VRC_MirrorReflection>())
            {
                originalMirrors.Add(new OriginalMirror
                {
                    MirrorParent = vrc_MirrorReflection,
                    OriginalLayers = vrc_MirrorReflection.m_ReflectLayers
                });
            }
        }

        public override void QuickMenuUI()
        {
            new QMToggleButton(BlazeMenu.Worlds, 1, 1, "Disable Bloom", delegate
            {
                Config.Main.DisableBloom = true;
                ToggleBloom(false);
            }, delegate
            {
                Config.Main.DisableBloom = false;
                ToggleBloom(true);
            }, "Disable bloom in all worlds", Config.Main.DisableBloom);

            new QMToggleButton(BlazeMenu.Worlds, 2, 1, "Disable Pickups", delegate
            {
                Config.Main.DisablePickups = true;
                TogglePickups(false);
            }, delegate
            {
                Config.Main.DisablePickups = false;
                TogglePickups(true);
            }, "Disable pickups for yourself in any world", Config.Main.DisablePickups);

            new QMToggleButton(BlazeMenu.Worlds, 3, 1, "Optimize Mirrors", delegate
            {
                Config.Main.OptimizeMirrors = true;
                Optimize();
            }, delegate
            {
                Config.Main.OptimizeMirrors = false;
                Revert();
            }, "Force all mirrors to be optimized instead of the default stance", Config.Main.OptimizeMirrors);
        }

        public override void LocalPlayerLoaded()
        {
            BlazeInfo.CachedBloomComponents = Object.FindObjectsOfType<PostProcessVolume>();
            if (Config.Main.DisableBloom) ToggleBloom(false);
            if (Config.Main.DisablePickups) TogglePickups(false);
            if (Config.Main.OptimizeMirrors) Optimize();
        }

        internal static void ToggleBloom(bool newState)
        {
            foreach (var b in BlazeInfo.CachedBloomComponents)
            {
                b.enabled = newState;
            }
        }

        internal static void TogglePickups(bool newState)
        {
            foreach (var p in BlazeInfo.CachedPickups)
            {
                p.gameObject.SetActive(newState);
            }
        }

        public static void Optimize()
        {
            if (originalMirrors.Count != 0)
            {
                foreach (OriginalMirror originalMirror in originalMirrors)
                {
                    originalMirror.MirrorParent.m_ReflectLayers = BlazeInfo.optimizeMask;
                }
            }
        }

        public static void Beautify()
        {
            if (originalMirrors.Count != 0)
            {
                foreach (OriginalMirror originalMirror in originalMirrors)
                {
                    originalMirror.MirrorParent.m_ReflectLayers = BlazeInfo.beautifyMask;
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
