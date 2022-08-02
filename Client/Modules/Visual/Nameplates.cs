/*using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Modules
{
    class Nameplates : BModule
    {
        private static Color DefaultNameplateColor = new(1, 1, 1, 0.8f);
        private static QMSingleButton NameplateColor;
        internal static List<ModTag> Tags = new();
        internal static List<ModBlazeTag> BlazeUsers = new();

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazeNameplate>();
            ClassInjector.RegisterTypeInIl2Cpp<BlazeNameplateEffect>();
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
            {
                BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new 
                {
                    payload = new
                    {
                        type = "FetchTags"
                    }
                }), null);
            }
        }

        public override void QuickMenuUI()
        {
            var menu = new QMNestedButton(BlazeMenu.Settings, "<size=28>Nameplates</size>", 3, 0, "Blaze's Client Nameplate Settings", "Nameplates");

            new QMToggleButton(menu, 1, 0, "Better\nNameplates", delegate
            {
                Config.Instance.BetterNameplates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Enable(p);
                    CreateBelowNameplate(p);
                }
            }, delegate
            {
                Config.Instance.BetterNameplates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Disable(p);
                    DeleteBelowNameplate(p);
                }
            }, "Use better custom nameplates instead of vrchat's boring ones", Config.Instance.BetterNameplates);

            new QMToggleButton(menu, 2, 0, "Custom Color", delegate
            {
                Config.Instance.CustomNameplateColor = true;
                Refresh();
                var color = ColorManager.HexToColor(Config.Instance.NameplateColor);
                foreach (var p in WorldUtils.GetPlayers())
                {
                    UpdateTagColors(p, color);
                    UpdateBelowNameplateColor(p, color);
                }
            }, delegate
            {
                Config.Instance.CustomNameplateColor = false;
                Refresh();
                foreach (var p in WorldUtils.GetPlayers())
                {
                    UpdateTagColors(p, DefaultNameplateColor);
                    UpdateBelowNameplateColor(p, DefaultNameplateColor);
                }
            }, "Use a custom background color on nameplates", Config.Instance.CustomNameplateColor);

            NameplateColor = new QMSingleButton(menu, 3, 0, $"<color={Config.Instance.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>", delegate
            {
                var input = Clipboard.GetText();
                if (!RegexManager.IsValidHexCode(input))
                {
                    PopupUtils.InformationAlert("That is not a valid hexcolor code! Please copy a valid hex color code to your clipboard and click the button again!");
                    return;
                }
                Config.Instance.NameplateColor = input;
                NameplateColor.SetButtonText($"<color={Config.Instance.NameplateColor}>Color Of\n<size=28>Nameplates</size></color>");
                Refresh();
                var color = ColorManager.HexToColor(Config.Instance.NameplateColor);
                foreach (var p in WorldUtils.GetPlayers())
                {
                    UpdateTagColors(p, color);
                    UpdateBelowNameplateColor(p, color);
                }
            }, "Change the color of the nameplates");

            new QMToggleButton(menu, 4, 0, "Below\n<size=28>Nameplates</size>", delegate
            {
                Config.Instance.UseBelowNameplates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    CreateBelowNameplate(p);
                }
            }, delegate
            {
                Config.Instance.UseBelowNameplates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    DeleteBelowNameplate(p);
                }
            }, "Show the below nameplates that show ping and frames and such", Config.Instance.UseBelowNameplates);
        }

        public override void PlayerJoined(Player player)
        {
            if (Config.Instance.BetterNameplates)
            {
                Enable(player);
                CreateBelowNameplate(player);
            }
        }

        public override void PlayerLeft(Player player)
        {
            Disable(player);
            DeleteBelowNameplate(player);
            if (Config.Instance.BetterNameplates)
            {
                MelonCoroutines.Start(DelayedRefresh());
            }
        }

        internal static void UpdateBelowNameplateColor(Player player, Color color)
        {
            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Blaze Nameplate") == null) return;
            var plate = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Blaze Nameplate").gameObject;
            plate.GetComponent<ImageThreeSlice>().color = color;
        }

        internal static void CreateBelowNameplate(Player player)
        {
            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Blaze Nameplate") != null || !Config.Instance.UseBelowNameplates) return;
            var plate = UnityEngine.Object.Instantiate(player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats"), player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents"), false);
            plate.name = "Blaze Nameplate";
            var comp = plate.gameObject.AddComponent<BlazeNameplate>();
            comp.vrcPlayer = player._vrcplayer;
            for (var i = plate.transform.childCount; i > 0; i--)
            {
                var child = plate.transform.GetChild(i - 1);
                if (child.name == "Trust Text") comp.text = child.GetComponent<TextMeshProUGUI>();
                else UnityEngine.Object.Destroy(child.gameObject);
            }
            plate.gameObject.SetActive(true);
            plate.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, 0);
            player.transform.Find("Player Nameplate/Canvas/Avatar Progress").GetComponent<RectTransform>().localPosition = new Vector3(0, -45, 0);
        }

        internal static void DeleteBelowNameplate(Player player)
        {
            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Blaze Nameplate") == null) return;
            UnityEngine.Object.Destroy(player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Blaze Nameplate").gameObject);
            player.transform.Find("Player Nameplate/Canvas/Avatar Progress").localPosition = new Vector3(0, -15, 0);
        }

        internal static IEnumerator DelayedRefresh()
        {
            yield return new WaitForSeconds(1f);
            Refresh();
        }

        public static void UpdateTagColors(Player player, Color color)
        {
            var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            var num = 0;
            for (; ; )
            {
                var transform5 = transform.Find($"BlazeTag{num}");
                if (transform5 == null) break;
                transform5.gameObject.GetComponent<ImageThreeSlice>().color = color;
                num++;
            }
        }

        public static void Enable(Player player)
        {
            try
            {
                var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                var transform2 = transform.Find("Quick Stats");
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_0.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_2.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_4.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_6.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_7.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_8.color = new Color(1f, 0f, 1f);
                if (Config.Instance.CustomNameplateColor)
                {
                    transform2.GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(Config.Instance.NameplateColor);
                    transform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(Config.Instance.NameplateColor);
                }
                else
                {
                    transform2.GetComponent<ImageThreeSlice>().color = DefaultNameplateColor;
                    transform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = DefaultNameplateColor;
                }
                var num = 0;
                for (; ; )
                {
                    var transform3 = transform.Find(string.Format("BlazeTag{0}", num));
                    if (transform3 == null)
                    {
                        break;
                    }
                    transform3.gameObject.active = false;
                    num++;
                }
                var num2 = 0;
                SetTag(ref num2, transform2, transform, ColorManager.HexToColor(player.GetAPIUser().GetRankColor()), player.field_Private_APIUser_0.GetRank());
                // Custom Tags
                foreach (var t in Tags.Where(t => t.userid == player.prop_APIUser_0.id))
                {
                    foreach (var t2 in t.tags)
                    {
                        SetTag(ref num2, transform2, transform, Color.white, t2);
                    }
                }
                // Blaze User Tags
                foreach (var t in BlazeUsers.Where(t => t.UserID == player.prop_APIUser_0.id))
                {
                    SetTag(ref num2, transform2, transform, Color.white, $"<color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color> User! <color=red><3</color>");
                }
                transform2.localPosition = new Vector3(0f, (num2 + 1) * 30, 0f);
                //CheckForCustoms(transform);
            }
            catch {}
        }

        *//*private static void CheckForCustoms(Transform transform)
        {
            var num = 0;
            for (; ; )
            {
                var transform3 = transform.Find(string.Format("BlazeTag{0}", num));
                if (transform3 == null)
                {
                    break;
                }
                var comp = transform3.Find("Trust Text").gameObject.GetComponent<TextMeshProUGUI>();
                foreach (TMP_LinkInfo linkInfo in comp.textInfo.linkInfo)
                {
                    if (linkInfo.GetLinkID() == "rainbow")
                    {
                        transform3.Find("Trust Text").gameObject.AddComponent<BlazeRGBNameplate>();
                    }
                }
            }
        }*//*

        public static void Disable(Player player)
        {
            var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            var transform2 = transform.Find("Main");
            var transform3 = transform.Find("Icon");
            var transform4 = transform.Find("Quick Stats");
            transform2.Find("Background").GetComponent<ImageThreeSlice>().color = Color.white;
            transform2.Find("Pulse").GetComponent<ImageThreeSlice>().color = Color.white;
            transform2.Find("Glow").GetComponent<ImageThreeSlice>().color = Color.white;
            transform3.Find("Background").GetComponent<Image>().color = Color.white;
            transform3.Find("Pulse").GetComponent<Image>().color = Color.white;
            transform3.Find("Glow").GetComponent<Image>().color = Color.white;
            transform4.GetComponent<ImageThreeSlice>().color = Color.white;
            var num = 0;
            for (; ; )
            {
                var transform5 = transform.Find($"BlazeTag{num}");
                if (transform5 == null) break;
                //transform5.gameObject.active = false;
                UnityEngine.Object.Destroy(transform5);
                num++;
            }
            transform4.localPosition = new Vector3(0f, 30f, 0f);
        }

        public static void Refresh()
        {
            if (!Config.Instance.BetterNameplates) return;
            var field_Private_Static_PlayerManager_ = PlayerManager.field_Private_Static_PlayerManager_0;
            if (field_Private_Static_PlayerManager_.field_Private_List_1_Player_0 == null) return;
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
            {
                if (player.field_Private_VRCPlayerApi_0 != null && player.field_Private_APIUser_0 != null)
                {
                    if (Config.Instance.BetterNameplates)
                    {
                        Enable(player);
                    }
                    else
                    {
                        Disable(player);
                    }
                }
            }
        }

        private static Transform MakeTag(Transform stats, int index)
        {
            var transform = UnityEngine.Object.Instantiate(stats, stats.parent, false);
            transform.name = $"BlazeTag{index}";
            transform.localPosition = new Vector3(0f, 30 * (index + 1), 0f);
            transform.gameObject.active = true;
            Transform result = null;
            for (var i = transform.childCount; i > 0; i--)
            {
                var child = transform.GetChild(i - 1);
                if (child.name == "Trust Text") result = child;
                else UnityEngine.Object.Destroy(child.gameObject);
            }
            return result;
        }

        private static void SetTag(ref int stack, Transform stats, Transform contents, Color color, string content)
        {
            var transform = contents.Find($"BlazeTag{stack}");
            Transform transform2;
            if (transform == null) transform2 = MakeTag(stats, stack);
            else
            {
                transform.gameObject.SetActive(true);
                transform2 = transform.Find("Trust Text");
            }
            var component = transform2.GetComponent<TextMeshProUGUI>();
            component.color = color;
            if (content.StartsWith("<rainbow>"))
            {
                var comp = transform2.gameObject.AddComponent<BlazeNameplateEffect>();
                comp.textComp = component;
                comp.effectType = EffectType.Regular;
                content = content.Substring(9);
            }
            if (content.StartsWith("<rainbow2>"))
            {
               content = content.Substring(10);
               component.enableVertexGradient = true;
               component.colorGradient = new VertexGradient(Color.red, Color.green, Color.blue, Color.magenta);
            }
            if (content.StartsWith("<rainbow3>"))
            {
                var comp = transform2.gameObject.AddComponent<BlazeNameplateEffect>();
                comp.textComp = component;
                comp.effectType = EffectType.Gradient;
                content = content.Substring(10);
                comp.OriginalText = content;
            }
            if (content.StartsWith("<typewriter>"))
            {
                var comp = transform2.gameObject.AddComponent<BlazeNameplateEffect>();
                comp.textComp = component;
                comp.effectType = EffectType.TypeWriter;
                content = content.Substring(12);
                comp.OriginalText = content;
            }
            if (content.StartsWith("<codebreaker>"))
            {
                var comp = transform2.gameObject.AddComponent<BlazeNameplateEffect>();
                comp.textComp = component;
                comp.effectType = EffectType.CodeBreaker;
                content = content.Substring(13);
                comp.OriginalText = content;
            }
            component.text = content;
            stack++;
        }
    }

    public enum EffectType
    {
        Regular,
        Gradient,
        TypeWriter,
        CodeBreaker
    }

    public class BlazeNameplateEffect : MonoBehaviour
    {
        public BlazeNameplateEffect(IntPtr id) : base(id) { }
        public float amountToShift = 0.2f * Time.deltaTime;
        public TextMeshProUGUI textComp;
        public EffectType effectType;
        public string OriginalText;
        public string CurrentText;
        public Color currentColor = Color.white;
        private float gradientTimer = 0.15f;
        private float typewriterTimer = 0f;
        private float codebreakerTimer = 0f;

        public void Update()
        {
            switch (effectType)
            {
                case EffectType.Regular:
                    Color newColor = ShiftHueBy(textComp.color, amountToShift);
                    textComp.color = newColor;
                    break;

                case EffectType.Gradient:
                    gradientTimer -= Time.deltaTime;
                    if (gradientTimer <= 0)
                    {
                        textComp.text = ColorManager.TextGradient(OriginalText, ref currentColor);
                        gradientTimer = 0.15f;
                    }
                    break;

                case EffectType.TypeWriter:
                    typewriterTimer -= Time.deltaTime;  
                    if (typewriterTimer <= 0)
                    {
                        MelonCoroutines.Start(DoTypeWriter());
                        typewriterTimer = 10f;
                    }
                    break;

                case EffectType.CodeBreaker:
                    codebreakerTimer -= Time.deltaTime;
                    if (codebreakerTimer <= 0)
                    {
                        MelonCoroutines.Start(DoCodeBreaker());
                        codebreakerTimer = 10f;
                    }
                    break;
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator DoCodeBreaker()
        {
            for (int i = 0; i < OriginalText.Length; i++)
            {
                CurrentText = RandomString(OriginalText.Length);
                textComp.text = CurrentText;
                yield return new WaitForSeconds(0.1f);
            }
            textComp.text = OriginalText;
            yield return new WaitForSeconds(5f);
        }

        [HideFromIl2Cpp]
        private string RandomString(int count)
        {
            string result = string.Empty;
            for (int i = 0; i < count; i++)
            {
                result += Functions.GetRandomCharacter();
            }
            return result;
        }

        [HideFromIl2Cpp]
        private IEnumerator DoTypeWriter()
        {
            for (int i = 0; i < OriginalText.Length; i++)
            {
                CurrentText = OriginalText.Substring(0, i + 1);
                textComp.text = CurrentText;
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }

        [HideFromIl2Cpp]
        private Color ShiftHueBy(Color color, float amount)
        {
            // convert from RGB to HSV
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            // shift hue by amount
            hue += amount;
            sat = 1f;
            val = 1f;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }
    }

    public class BlazeNameplate : MonoBehaviour
    {
        public BlazeNameplate(IntPtr id) : base(id) { }

        public TextMeshProUGUI text;
        public VRCPlayer vrcPlayer;

        public void Update()
        {
            if (text != null && vrcPlayer != null)
                text.text = GetInfo(vrcPlayer);
        }

        [HideFromIl2Cpp]
        private static string GetInfo(VRCPlayer instance)
        {
            var results = string.Empty;

            // Trust Rank
            //results += $"[<color={instance.GetAPIUser().GetRankColor()}>{instance.GetAPIUser().GetRank().Substring(0, 3)}</color>] ";

            // Platform
            results += $"[<color=grey>{instance.GetPlatform().Substring(0, 1)}</color>] ";

            // Ping
            results += $"[P {GetPingColored(instance._player)}] ";

            // Frames
            results += $"[F {GetFramesColored(instance._player)}] ";

            // Master
            if (instance.IsMaster())
                results += "[<color=orange>M</color>] ";

            // Friend
            if (instance.IsFriend())
                results += "[<color=yellow>F</color>] ";

            if (instance.transform.Find("Player Nameplate/Canvas/Avatar Progress"))
                instance.transform.Find("Player Nameplate/Canvas/Avatar Progress").GetComponent<RectTransform>().localPosition = new Vector3(0, -50, 0);

            return results;
        }

        [HideFromIl2Cpp]
        private static string GetPingColored(Player instance)
        {
            var ping = instance.GetPing();
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            return ping <= 35 ? $"<color=green>{ping}</color>" : $"<color=yellow>{ping}</color>";
        }

        [HideFromIl2Cpp]
        private static string GetFramesColored(VRC.Player instance)
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
*/