using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Blaze.Utils.Managers
{
    internal static class AssetBundleManager
    {
        private static AssetBundle BlazeBundle;
        internal static Sprite Logo;
        internal static Sprite DebugBackground;
        internal static Sprite MenuBackground;
        internal static Sprite BackArrow;
        internal static Sprite NextArrow;
        internal static Sprite Pause;
        internal static Sprite FallbackThumbnail;
        internal static Sprite ExploitsIcon;
        internal static Sprite MicIcon;
        internal static Sprite MovementIcon;
        internal static Sprite QuitIcon;
        internal static Sprite RendersIcon;
        internal static Sprite RestartIcon;
        internal static Sprite SecurityIcon;
        internal static Sprite SettingsIcon;
        internal static Sprite SpoofsIcon;
        internal static Sprite WorldsIcon;
        internal static Sprite WorldSpecificIcon;
        internal static Sprite HelpIcon;
        internal static AudioClip LoadingSong;

        internal static void Initialize()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Blaze.Resources.blaze"); //String is MainNamespace.assetbundlename
            using var tempStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(tempStream);
            BlazeBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
            BlazeBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            Logo = LoadSprite("Assets/Blaze/logos/main.png");
            LoadingSong = LoadAudioClip("Assets/Blaze/audios/LoadingMain.mp3");
            DebugBackground = LoadSprite("Assets/Blaze/others/debug.png");
            MenuBackground = LoadSprite("Assets/Blaze/others/background.png");
            BackArrow = LoadSprite("Assets/Blaze/musicIcons/back.png");
            NextArrow = LoadSprite("Assets/Blaze/musicIcons/next.png");
            Pause = LoadSprite("Assets/Blaze/musicIcons/pause.png");
            FallbackThumbnail = LoadSprite("Assets/Blaze/musicIcons/fallback.png");
            ExploitsIcon = LoadSprite("Assets/Blaze/buttonIcons/exploits.png");
            MicIcon = LoadSprite("Assets/Blaze/buttonIcons/mic.png");
            MovementIcon = LoadSprite("Assets/Blaze/buttonIcons/movement.png");
            QuitIcon = LoadSprite("Assets/Blaze/buttonIcons/quit.png");
            RendersIcon = LoadSprite("Assets/Blaze/buttonIcons/Renders.png");
            RestartIcon = LoadSprite("Assets/Blaze/buttonIcons/restart.png");
            SecurityIcon = LoadSprite("Assets/Blaze/buttonIcons/security.png");
            SettingsIcon = LoadSprite("Assets/Blaze/buttonIcons/settings.png");
            SpoofsIcon = LoadSprite("Assets/Blaze/buttonIcons/spoofs.png");
            WorldsIcon = LoadSprite("Assets/Blaze/buttonIcons/worlds.png");
            WorldSpecificIcon = LoadSprite("Assets/Blaze/buttonIcons/worldSpecific.png");
            HelpIcon = LoadSprite("Assets/Blaze/buttonIcons/help.png");
        }

        internal static Font LoadFont(string file)
        {
            Font font2 = BlazeBundle.LoadAsset(file, Il2CppType.Of<Font>()).Cast<Font>();
            font2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return font2;
        }

        internal static Sprite LoadSprite(string file)
        {
            Sprite sprite2 = BlazeBundle.LoadAsset(file, Il2CppType.Of<Sprite>()).Cast<Sprite>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite2;
        }

        internal static AudioClip LoadAudioClip(string file)
        {
            AudioClip AudioClip = BlazeBundle.LoadAsset(file, Il2CppType.Of<AudioClip>()).Cast<AudioClip>();
            AudioClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return AudioClip;
        }

        internal static Shader LoadShader(string file)
        {
            Shader shader = BlazeBundle.LoadAsset(file, Il2CppType.Of<Shader>()).Cast<Shader>();
            shader.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return shader;
        }

        internal static Texture2D LoadTexture(string file)
        {
            Texture2D texture2D = BlazeBundle.LoadAsset(file, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return texture2D;
        }
    }
}
