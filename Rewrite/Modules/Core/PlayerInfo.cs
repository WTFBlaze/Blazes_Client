using Blaze.API;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Blaze.Modules
{
    public class PlayerInfo : BModule
    {
        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazePlayerInfo>();
        }

        public override void PlayerJoined(Player player)
        {
            Main.Players.Add(player.GetUserID(), player.gameObject.AddComponent<BlazePlayerInfo>());
        }
    }

    public class BlazePlayerInfo : MonoBehaviour
    {
        public BlazePlayerInfo(IntPtr id) : base(id) { }

        public Player player;
        public VRCPlayer vrcPlayer;
        public PlayerNet playerNet;
        public VRCPlayerApi vrcPlayerApi;
        public APIUser apiUser;

        public Transform tagsTransform;
        public GameObject lowerNamePlate;
        public TextMeshProUGUI namePlateText;
        public GameObject funnyImage;

        public HighlightsFXStandalone highlightOptions;
        public Transform SelectRegion;
        public UnhollowerBaseLib.Il2CppArrayBase<Renderer> ObjRenderers;
        public UnhollowerBaseLib.Il2CppArrayBase<MeshRenderer> ObjMeshRenderers;

        public string platform;
        public string displayName;
        public string userID;
        public bool isGhosting;
        public bool isCrashed;
        public bool isLagging;
        private float nameplateTimer = 1f;

        public int lastNetworkedUpdatePacketNumber;
        public float lastNetworkedUpdateTime;
        public int lagBarrier;
        public bool BlockedByAntiCrash;
        public int crashedTime;
        public bool currentlyCountingCrashed;

        private void Start()
        {
            SetupPlayerInfo();
            tagsTransform = gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            SetupFunnyImage();
            if (Config.Main.UseBelowNameplates)
            {
                SetupLowerNameplate();
            }
            if (Config.Main.CustomNameplateColor)
            {
                SetNameplateColor(ColorManager.HexToColor(Config.Main.NameplateColor));
            }
            if (apiUser.id != PlayerUtils.CurrentUser().GetUserID())
            {
                SetupESP();
            }
            
        }

        private void Update()
        {
            if (player == null || vrcPlayer == null || playerNet == null || vrcPlayerApi == null || apiUser == null || displayName == null || platform == null || userID == null)
            {
                SetupPlayerInfo();
            }
            
            // Process Crash Info Shit
            int networkedUpdatePacketNumber = vrcPlayer.prop_PlayerNet_0.field_Private_Int32_0;
            float lastNetworkedUpdateDelta = Time.realtimeSinceStartup - lastNetworkedUpdateTime;
            short ping = MelonUtils.Clamp<short>(vrcPlayer.prop_PlayerNet_0.prop_Int16_0, 0, short.MaxValue);
            float lagThreshold = 0.5f + (Mathf.Min(ping, 500f) / 1000f);
            if (lastNetworkedUpdateDelta > lagThreshold && lagBarrier < 5)
            {
                lagBarrier++;
            }
            if (lastNetworkedUpdatePacketNumber != networkedUpdatePacketNumber)
            {
                lastNetworkedUpdatePacketNumber = networkedUpdatePacketNumber;
                lastNetworkedUpdateTime = Time.realtimeSinceStartup;
                lagBarrier--;
            }

            if (lagBarrier == 5 && apiUser.id != PlayerUtils.CurrentUser().GetUserID())
            {
                if (!currentlyCountingCrashed)
                {
                    currentlyCountingCrashed = true;
                    crashedTime = 0;
                    MelonCoroutines.Start(CountCrashedTime());
                }
            }
            else
            {
                if (currentlyCountingCrashed)
                {
                    currentlyCountingCrashed = false;
                    crashedTime = 0;
                }
            }

            if (lastNetworkedUpdateDelta > 10f)
            {
                isCrashed = true;
                isGhosting = false;
                isLagging = false;
            }
            else if (lagBarrier > 0)
            {
                isCrashed = false;
                isGhosting = false;
                isLagging = true;
            }
            else
            {
                isCrashed = false;
                isGhosting = false;
                isLagging = false;
            }
            lagBarrier = MelonUtils.Clamp(lagBarrier, 0, 15);

            nameplateTimer -= Time.deltaTime;
            if (nameplateTimer >= 0)
            {
                // Update Lower Name Plate Text
                if (lowerNamePlate != null)
                {
                    namePlateText.text = GetNameplateTags(lastNetworkedUpdateDelta);
                }
                nameplateTimer = 1f;
            }

            // Process Anti Crash Label
            if (BlockedByAntiCrash)
            {
                tagsTransform.Find("Main/Text Container/Name").gameObject.GetComponent<TextMeshProUGUI>().richText = true;
                tagsTransform.Find("Main/Text Container/Name").gameObject.GetComponent<TextMeshProUGUI>().text = $"<b><color=red>[X]</color></b>{apiUser.displayName}";
            }

            // Determine Crashed & Ghosting
            if (crashedTime >= 45f)
            {
                isCrashed = false;
                isGhosting = true;
            }
            else if (lastNetworkedUpdateDelta > 10f)
            {
                isCrashed = true;
                isGhosting = false;
            }
            else
            {
                isCrashed = false;
                isGhosting = false;
            }
        }

        private void OnDestroy()
        {
            Main.Players.Remove(userID);
        }

        [HideFromIl2Cpp]
        private IEnumerator CountCrashedTime()
        {
            while (currentlyCountingCrashed)
            {
                crashedTime++;
                yield return new WaitForSecondsRealtime(1);
            }
        }

        private void SetupPlayerInfo()
        {
            player = gameObject.GetComponent<VRC.Player>();
            vrcPlayer = gameObject.GetComponent<VRCPlayer>();
            playerNet = gameObject.GetComponent<PlayerNet>();
            vrcPlayerApi = player.prop_VRCPlayerApi_0;
            apiUser = player.prop_APIUser_0;
            displayName = apiUser.displayName;
            userID = apiUser.id;
            platform = vrcPlayer.GetPlatform();
        }

        public void SetNameplateColor(Color newColor)
        {
            tagsTransform.Find("Quick Stats").GetComponent<ImageThreeSlice>().color = newColor;
            tagsTransform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = newColor;
            if (lowerNamePlate != null)
            {
                lowerNamePlate.GetComponent<ImageThreeSlice>().color = newColor;
            }
        }

        private void SetupFunnyImage()
        {
            /*if (funnyImage != null)
            {
                Logs.Log("Funny Image isn't null!");
                return;
            }*/

            funnyImage = Instantiate(tagsTransform.Find("Quick Stats/Trust Icon").gameObject, tagsTransform, false);
            funnyImage.name = "Blaze Funny Image";
            funnyImage.SetActive(true);
            funnyImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            funnyImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 25);
            if (userID == null)
            {
                Logs.Log("Funny Images | userID is null!", ConsoleColor.Yellow);
            }
            switch (userID)
            {
                case "usr_a88ee27e-2569-47c6-aeb1-4914edd51954": // Temmie
                    funnyImage.GetComponent<Image>().sprite = Main.LeanSprite;
                    funnyImage.GetComponent<Image>().overrideSprite = Main.LeanSprite;
                    break;

                case "usr_3add7949-07bb-47dc-838f-3c2e65eb6a14": // Blaze
                    funnyImage.GetComponent<Image>().sprite = Main.LeanSprite;
                    funnyImage.GetComponent<Image>().overrideSprite = Main.LeanSprite;
                    break;

                case "usr_560474e9-fb27-47ec-9efc-2a441232d277": // Wolfie
                    funnyImage.GetComponent<Image>().sprite = Main.NikeiSprite;
                    funnyImage.GetComponent<Image>().overrideSprite = Main.NikeiSprite;
                    break;

                default:
                    funnyImage.SetActive(false);
                    break;
            }
        }

        public void SetupLowerNameplate()
        {
            if (lowerNamePlate != null) return;
            lowerNamePlate = Instantiate(tagsTransform.Find("Quick Stats").gameObject, tagsTransform, false);
            lowerNamePlate.name = "Blaze Nameplate";
            for (var i = lowerNamePlate.transform.childCount; i > 0; i--)
            {
                var child = lowerNamePlate.transform.GetChild(i - 1);
                if (child.name == "Trust Text")
                {
                    namePlateText = child.GetComponent<TextMeshProUGUI>();
                    namePlateText.color = Color.white;
                }
                else Destroy(child.gameObject);
            }
            lowerNamePlate.gameObject.SetActive(true);
            lowerNamePlate.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);
            player.transform.Find("Player Nameplate/Canvas/Avatar Progress").GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -46, 0);

            /*try
            {
                // le epic snaxytags compatibility B)
                var snaxyTag = tagsTransform.Find("SnaxyTag").gameObject;
                if (snaxyTag != null)
                {
                    //snaxyTag.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120);
                    lowerNamePlate.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -120, 0);
                }
            }
            catch { }*/
        }

        public void SetAntiCrashBool(bool state)
        {
            BlockedByAntiCrash = state;
        }

        public void DestroyLowerNameplate()
        {
            if (lowerNamePlate == null) return;
            Destroy(lowerNamePlate);
            lowerNamePlate = null;
        }

        public void ResetNameplates()
        {
            if (lowerNamePlate != null)
            {
                Destroy(lowerNamePlate);
            }
            tagsTransform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Main/Pulse").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Main/Glow").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Icon/Background").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Icon/Pulse").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Icon/Glow").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Quick Stats").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Quick Stats").localPosition = new Vector3(0f, 30f, 0f);
        }

        public void ResetNameplateColors()
        {
            if (lowerNamePlate != null)
            {
                lowerNamePlate.GetComponent<ImageThreeSlice>().color = Color.white;
            }
            tagsTransform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Main/Pulse").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Main/Glow").GetComponent<ImageThreeSlice>().color = Color.white;
            tagsTransform.Find("Icon/Background").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Icon/Pulse").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Icon/Glow").GetComponent<Image>().color = Color.white;
            tagsTransform.Find("Quick Stats").GetComponent<ImageThreeSlice>().color = Color.white;
        }

        private void SetupESP()
        {
            SelectRegion = player.transform.Find("SelectRegion");
            if (SelectRegion == null)
            {
                Logs.Log("[CapsuleESP] Failed to find SelectRegion!", ConsoleColor.Red);
            }
            else
            {
                ObjRenderers = SelectRegion.GetComponentsInChildren<Renderer>(true);
                ObjMeshRenderers = SelectRegion.GetComponentsInChildren<MeshRenderer>(true);
                if (ObjMeshRenderers == null && ObjRenderers == null && ObjMeshRenderers.Count == 0 && ObjRenderers.Count == 0)
                {
                    Destroy(this);
                    return;
                }
                else
                {
                    if (highlightOptions == null)
                    {
                        highlightOptions = CameraUtils.MainCamera.gameObject.AddHighlighter();
                    }
                    foreach (var ObjRenderer in ObjRenderers)
                    {
                        if (ObjRenderer != null)
                        {
                            highlightOptions.SetHighLighter(ObjRenderer, true);
                        }
                    }
                    foreach (var ObjMeshRenderer in ObjMeshRenderers)
                    {
                        if (ObjMeshRenderer != null)
                        {
                            highlightOptions.SetHighLighter(ObjMeshRenderer, true);
                        }
                    }
                }
                highlightOptions.highlightColor = apiUser.GetTrueRankUnityColor(); /*ColorManager.HexToColor(apiUser.GetTrueRankColor());*/
                highlightOptions.enabled = false;
            }
        }

        [HideFromIl2Cpp]
        private string GetNameplateTags(float lastNetworkedUpdateDelta)
        {
            var results = string.Empty;

            if (Config.Main.NameplateModerations)
            {
                if (player.IsBlockedBy())
                {
                    results += "[<b><color=red>B</color></b>]";
                }

                if (player.IsMutedBy())
                {
                    results += "[<b><color=red>M</color></b>]";
                }
            }

            // Trust Rank
            if (Config.Main.NamePlateTrustRank)
            {
                results += $"[<color={vrcPlayer.GetAPIUser().GetTrueRankColor()}>{vrcPlayer.GetAPIUser().GetTrueRank()}</color>] ";
            }

            // Platform
            if (Config.Main.NamePlatePlatform)
            {
                results += $"[<color=grey>{vrcPlayer.GetPlatform().Substring(0, 1)}</color>] ";
            }

            if (WorldUtils.GetPlayerCount() != 1)
            {
                // Ping
                if (Config.Main.NamePlatePing)
                {
                    results += $"[P {GetPingColored()}] ";
                }

                // Frames
                if (Config.Main.NamePlateFrames)
                {
                    results += $"[F {GetFramesColored()}] ";
                }
            }

            // Master
            if (Config.Main.NamePlateMasterTag)
            {
                if (vrcPlayer.IsMaster())
                {
                    results += "[<color=orange>M</color>] ";
                }
            }

            // Friend
            if (Config.Main.NamePlateFriendsTag)
            {
                if (vrcPlayer.IsFriend())
                {
                    results += "[<color=yellow>F</color>] ";
                }
            }

            // Crash State
            if (vrcPlayer.GetUserID() != PlayerUtils.CurrentUser().GetUserID())
            {
                if (Config.Main.NamePlateCrashState)
                {
                    if (crashedTime >= 45f)
                    {
                        results += "[<color=#ff8800><b>GHOSTING</b></color>]";
                    }
                    else if (lastNetworkedUpdateDelta > 10f)
                    {
                        results += "[<color=red><b>CRASHED</b></color>]";
                    }
                    else if (lagBarrier > 0)
                    {
                        results += "[<color=yellow>Lagging</color>]";
                    }
                    else
                    {
                        results += "[<color=green>Stable</color>]";
                    }
                }
            }

            return results;
        }

        [HideFromIl2Cpp]
        public string GetPingColored()
        {
            var ping = player.GetPing();
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            return ping <= 35 ? $"<color=green>{ping}</color>" : $"<color=yellow>{ping}</color>";
        }

        [HideFromIl2Cpp]
        public string GetFramesColored()
        {
            var frames = player.GetFrames();
            if (frames >= 65)
            {
                return $"<color=green>{frames}</color>";
            }
            return frames <= 28 ? $"<color=red>{frames}</color>" : $"<color=yellow>{frames}</color>";
        }

        [HideFromIl2Cpp]
        public string GetPlatformColored()
        {
            return platform switch
            {
                "Quest" => "<color=green>Q</color>",
                "Desktop" => "<color=yellow>D</color>",
                "VR" => "<color=magenta>V</color>",
                _ => "<color=yellow>D</color>"
            };
        }
    }
}
