using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using VRC;

namespace Blaze.Modules
{
    class DesktopPlayerList : BModule
    {
        internal static GameObject BackgroundObject;
        private static GameObject TextObject;
        internal static Text TextText;

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesDesktopPlayerlist>();
        }

        public override void QuickMenuUI()
        {
            new QMToggleButton(BlazeMenu.BCUI, 1, 1, "Desktop Playerlist", delegate
            {
                Config.Main.DesktopPlayerList = true;
                if (!XRDevice.isPresent)
                {
                    BackgroundObject.SetActive(true);
                }
            }, delegate
            {
                Config.Main.DesktopPlayerList = false;
                if (!XRDevice.isPresent)
                {
                    BackgroundObject.SetActive(false);
                }
            }, "Toggles the Desktop Playerlist", Config.Main.DesktopPlayerList);

            if (XRDevice.isPresent) return;
            /*MainObj = new GameObject("Blaze's Desktop Playerlist");
            UnityEngine.Object.DontDestroyOnLoad(MainObj);*/
            BlazeInfo.BlazesComponents.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            BlazeInfo.BlazesComponents.transform.position = Vector3.zero;
            CanvasScaler canvasScaler = BlazeInfo.BlazesComponents.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            BackgroundObject = new GameObject("Background");
            BackgroundObject.transform.SetParent(BlazeInfo.BlazesComponents.transform, false);
            BackgroundObject.AddComponent<CanvasRenderer>();
            BackgroundObject.AddComponent<RectTransform>();
            BackgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 500f);
            BackgroundObject.GetComponent<RectTransform>().position = new Vector2(Screen.currentResolution.width / 2 - 255, Screen.currentResolution.height / 6 + 85);

            TextObject = new GameObject("Text");
            TextObject.AddComponent<CanvasRenderer>();
            TextObject.transform.SetParent(BackgroundObject.transform, false);
            TextText = TextObject.AddComponent<Text>();
            TextObject.AddComponent<RectTransform>();
            TextText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            TextText.supportRichText = true;
            TextText.fontSize = 13;
            TextText.text = "";
            TextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 500f);
            TextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1400f, 10f);

            BackgroundObject.SetActive(Config.Main.DesktopPlayerList);
            BlazeInfo.BlazesComponents.AddComponent<BlazesDesktopPlayerlist>();
        }
    }

    public class BlazesDesktopPlayerlist : MonoBehaviour
    {
        public BlazesDesktopPlayerlist(IntPtr id) : base(id) {}
        private float playerListTimer = 0.5f;

        public void Update()
        {
            try
            {
                playerListTimer -= Time.deltaTime;
                if (playerListTimer <= 0)
                {
                    var PlayerCount = WorldUtils.GetPlayerCount();
                    string Players = string.Empty;
                    string Title = $"<size=20><b><color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color></b></size>\n";
                    foreach (var player in PhotonUtils.GetAllPhotonPlayers())
                    {
                        var returnstring = string.Empty;
                        string playerstring;
                        var p = player.GetPlayer();
                        if (p == null)
                        {
                            playerstring = $"[<color=yellow>{player.GetPhotonID()}</color>] {player.GetDisplayName()} <color=red>[INVIS]</color> ";
                        }
                        else
                        {
                            if (PlayerCount == 1)
                            {
                                playerstring = $" {GetTags(p)} <color=grey><{GetPlatform(p)}></color> <color={p.GetAPIUser().GetTrueRankColor()}>{p.GetAPIUser().displayName}</color>";
                            }
                            else
                            {
                                playerstring = $" {GetTags(p)} <color=grey><{GetPlatform(p)}></color> <color={p.GetAPIUser().GetTrueRankColor()}>{p.GetAPIUser().displayName}</color> <b>| P:{GetPingColored(p)} F:{GetFramesColored(p)}</b>";
                            }
                        }
                        returnstring += playerstring;
                        Players += returnstring + "\n";
                    }
                    Title += Players;
                    DesktopPlayerList.TextText.text = Title;
                    playerListTimer = 0.5f;
                }
            }
            catch {}
        }

        [HideFromIl2Cpp]
        private string GetTags(Player instance)
        {
            string result = string.Empty;

            if (instance.IsMaster())
                result += "<color=orange>[M]</color>";

            if (instance.IsFriend())
                result += "<color=yellow>[F]</color>";

            if (instance.IsTarget())
                result += "<color=red>[T]</color>";

            if (instance.IsKOS())
                result += "<color=#ba3d34>[KOS]</color>";

            float Distance = Vector3.Distance(PlayerUtils.CurrentUser().transform.position, instance.transform.position);
            if (Distance < 26.5f)
            {
                if (instance.GetUserID() != PlayerUtils.CurrentUser().GetUserID())
                {
                    result += $"<color=lime>[H {Mathf.Round(Distance)}]</color>";
                }
            }

            return result;
        }

        [HideFromIl2Cpp]
        private string GetPlatform(Player instance)
        {
            return instance.GetVRCPlayer().GetPlatform().Substring(0, 1);
        }

        [HideFromIl2Cpp]
        private string GetPingColored(VRC.Player instance)
        {
            var ping = instance.GetPing();
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            return ping <= 35 ? $"<color=green>{ping}</color>" : $"<color=yellow>{ping}</color>";
        }

        [HideFromIl2Cpp]
        private string GetFramesColored(VRC.Player instance)
        {
            var frames = instance.GetFrames();
            if (frames >= 65)
            {
                return $"<color=green>{frames}</color>";
            }
            return frames <= 28 ? $"<color=red>{frames}</color>" : $"<color=yellow>{frames}</color>";
        }
    }
}
