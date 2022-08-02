using UnityEngine.Rendering.PostProcessing;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class ClientCaching : BModule
    {
        public override void LocalPlayerLoaded()
        {
            Main.Pickups = UnityEngine.Object.FindObjectsOfType<VRC_Pickup>();
            Main.Blooms = UnityEngine.Object.FindObjectsOfType<PostProcessVolume>();
            Main.Seats = UnityEngine.Object.FindObjectsOfType<VRCStation>();
        }
    }
}
