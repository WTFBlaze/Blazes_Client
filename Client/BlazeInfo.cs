using Blaze.Modules;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC;
using VRC.SDKBase;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze
{
    internal class BlazeInfo
    {
        internal static List<BModule> Modules = new();
        internal static string ModColor1 = "#349beb";
        internal static string ModColor2 = "#9830d9";
        internal static string UserColor = "#e1add3";
        internal static Dictionary<string, BlazesPlayerInfo> CachedPlayers = new();
        internal static ModUser CurrentUser;
        internal static GameObject BlazesComponents;
        internal static GameObject SerializeClone;
        internal static List<string> KnownBlocks = new();
        internal static List<string> KnownMutes = new();
        internal static VRC_Pickup[] CachedPickups;
        public static PostProcessVolume[] CachedBloomComponents;
        internal static LayerMask optimizeMask;
        internal static LayerMask beautifyMask;
        internal static VRC.DataModel.NeckRange SavedNeckRange;
        internal static Vector3 SavedGravity;
        internal static Camera CurrentCamera;
        internal static Player SelectedPlayer;
        internal static Player Target;
        internal static bool MasterLockInstance;
        internal static bool Serialization;
        internal static bool WorldTriggers;
        internal static bool ParrotMode;
        internal static bool InfinitePortals;
        internal static bool QMIsOpened;
        internal static bool SMIsOpened;
        internal static bool AWIsOpened;
    }
}
