using Blaze.API.QM;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace Blaze.Modules
{
    public class USpeakSpammer : BModule
    {
        private QMToggleButton ToggleButton;

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeUSpeakRape>();
        }

        public override void UI()
        {
            ToggleButton = new(BlazeQM.Exploits, 1, 3, "USpeaker Rape", delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeUSpeakRape>() == null)
                {
                    Main.BlazesComponents.AddComponent<BlazeUSpeakRape>();
                }
            }, delegate
            {
                if (Main.BlazesComponents.GetComponent<BlazeUSpeakRape>() != null)
                {
                    UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeUSpeakRape>());
                }
            }, "Spam Earrape obnoxious noises to anyone who doesn't have you muted");
        }

        public override void LocalPlayerLoaded()
        {
            if (ToggleButton != null)
            {
                ToggleButton.SetToggleState(false, true);
            }
        }
    }

    public class BlazeUSpeakRape : MonoBehaviour
    {
        public BlazeUSpeakRape(IntPtr id) : base(id) { }
        private byte[] uSpeakData = { 86, 0, 0, 0, 201, 60, 74, 145, 187, 134, 59,
                0, 248, 125, 232, 192, 92, 160, 82, 254, 48,
                228, 30, 187, 149, 196, 177, 215, 140, 223,
                127, 209, 66, 60, 0, 226, 53, 180, 176, 97,
                104, 4, 248, 238, 195, 125, 44, 185, 182, 68,
                94, 114, 205, 181, 150, 56, 232, 126, 247, 155,
                123, 172, 108, 98, 80, 56, 113, 89, 160, 125, 221 };

        public void Update()
        {
            try
            {
                if (!WorldUtils.IsInRoom()) return;
                byte[] serverTime = BitConverter.GetBytes(PhotonNetwork.field_Public_Static_LoadBalancingClient_0.field_Private_LoadBalancingPeer_0.ServerTimeInMilliSeconds);
                byte[] Actor = BitConverter.GetBytes(PlayerUtils.CurrentUser().GetActorNumber());
                Buffer.BlockCopy(serverTime, 0, uSpeakData, 4, 4);
                Buffer.BlockCopy(Actor, 0, uSpeakData, 0, 4);
                var il2CppArray = new Il2CppStructArray<byte>(uSpeakData.Length);
                for (var i = 0; i < uSpeakData.Length; i++)
                {
                    il2CppArray[i] = uSpeakData[i];
                }
                var __1 = il2CppArray.Cast<Il2CppSystem.Object>();
                for (var i = 0; i < uSpeakData.Length; i++)
                {
                    il2CppArray[i] = uSpeakData[i];
                }
                PhotonUtils.OpRaiseEvent(1, __1,
                new RaiseEventOptions
                {
                    field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                    field_Public_EventCaching_0 = EventCaching.DoNotCache,
                }, default);
            }
            catch { }
        }
    }
}
