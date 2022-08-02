using VRC.DataModel;

namespace Blaze.Modules
{
    public abstract class BModule
    {
        public virtual void Start() { }
        public virtual void UI() { }
        public virtual void Update() { }
        public virtual void LocalPlayerLoaded() { }
        public virtual void LeftWorld() { }
        public virtual void PlayerJoined(VRC.Player player) { }
        public virtual void PlayerLeft(VRC.Player player) { }
        public virtual void AvatarIsReady(VRCPlayer vrcPlayer, VRC.Core.ApiAvatar apiAvatar) { }
        public virtual void SelectedUser(IUser user, bool isRemote) { }
        public virtual void PhotonJoined(Photon.Realtime.Player photonPlayer) { }
        public virtual void PhotonLeft(Photon.Realtime.Player photonPlayer) { }
        public virtual void SceneInitialized(int buildIndex, string sceneName) { }
        public virtual void SceneLoaded(int buildIndex, string sceneName) { }
    }
}
