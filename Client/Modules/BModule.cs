using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.DataModel;

namespace Blaze.Modules
{
    public class BModule
    {
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void QuickMenuUI() { }
        public virtual void SocialMenuUI() { }
        public virtual void HudMenuUI() { }
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
        public virtual void BlockedPlayer(Photon.Realtime.Player photonPlayer) { }
        public virtual void UnBlockedPlayer(Photon.Realtime.Player photonPlayer) { }
        public virtual void MutedPlayer(Photon.Realtime.Player photonPlayer) { }
        public virtual void UnMutedPlayer(Photon.Realtime.Player photonPlayer) { }
    }
}
