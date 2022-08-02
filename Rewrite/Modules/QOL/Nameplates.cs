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
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

namespace Blaze.Modules
{
    public class Nameplates : BModule
    {
        private static Color DefaultNameplateColor = new(1, 1, 1, 0.8f);

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

        public override void PlayerJoined(Player player)
        {
            if (Config.Main.BetterNameplates)
            {
                Enable(player);
            }

            if (PlayerUtils.CurrentUser() != null)
            {
                BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new
                {
                    payload = new
                    {
                        type = "FindBlazeUser",
                        date = new
                        {
                            world_id = WorldUtils.GetJoinID()
                        }
                    }
                }), null);
            }
        }

        public override void PlayerLeft(Player player)
        {
            Disable(player);
            if (Config.Main.BetterNameplates)
            {
                MelonCoroutines.Start(DelayedRefresh());
            }
        }

        public override void AvatarIsReady(VRCPlayer vrcPlayer, ApiAvatar apiAvatar)
        {
            if (Config.Main.BetterNameplates)
            {
                MelonCoroutines.Start(DelayedRefresh());
            }
        }

        public static IEnumerator DelayedRefresh()
        {
            yield return new WaitForSeconds(1f);
            Refresh();
        }

        public static void Enable(Player player)
        {
            try
            {
                var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                var transform2 = transform.Find("Quick Stats");
                /*player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_0.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_2.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_4.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_6.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_7.color = new Color(1f, 0f, 1f);
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_8.color = new Color(1f, 0f, 1f);*/
                if (Config.Main.CustomNameplateColor)
                {
                    transform2.GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(Config.Main.NameplateColor);
                    transform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(Config.Main.NameplateColor);
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
                // Custom Tags
                foreach (var t in Main.Tags.Where(t => t.userid == player.prop_APIUser_0.id))
                {
                    foreach (var t2 in t.tags)
                    {
                        SetTag(ref num2, transform2, transform, Color.white, t2);
                    }
                }

                // Blaze User Tags
                foreach (var t in Main.OtherUsers.Where(t => t.UserID == player.prop_APIUser_0.id))
                {
                    switch (t.AccessType)
                    {
                        case "Developer":
                            SetTag(ref num2, transform2, transform, Color.white, $"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> <b><color={Colors.PinkHex}>Developer</color></b>! <color=red><3</color>");                            break;

                        case "Staff":
                            SetTag(ref num2, transform2, transform, Color.white, $"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> <color=yellow><b>Staff</b></color>! <color=red><3</color>");
                            break;

                        default:
                            SetTag(ref num2, transform2, transform, Color.white, $"<color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> User! <color=red><3</color>");
                            break;
                    }
                }
                transform2.localPosition = new Vector3(0f, (num2 + 1) * 30, 0f);

                /*// Temmie Image
                if (player.field_Private_APIUser_0.id == "usr_a88ee27e-2569-47c6-aeb1-4914edd51954")
                {
                    
                }*/
            }
            catch { }
        }

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
            if (!Config.Main.BetterNameplates) return;
            var field_Private_Static_PlayerManager_ = PlayerManager.field_Private_Static_PlayerManager_0;
            if (field_Private_Static_PlayerManager_.field_Private_List_1_Player_0 == null) return;
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
            {
                if (player.field_Private_VRCPlayerApi_0 != null && player.field_Private_APIUser_0 != null)
                {
                    if (Config.Main.BetterNameplates)
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
            /*if (content.StartsWith("<rainbow>"))
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
            }*/
            component.text = content;
            stack++;
        }
    }
}
