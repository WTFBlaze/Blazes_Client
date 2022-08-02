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
    public static class AssetBundleManager
    {
        private static AssetBundle BlazeBundle;
        public static Sprite Logo;
        public static Sprite DebugBackground;
        public static Sprite MenuBackground;
        public static Sprite BackArrow;
        public static Sprite NextArrow;
        public static Sprite Pause;
        public static Sprite FallbackThumbnail;
        public static Sprite ExploitsIcon;
        public static Sprite MicIcon;
        public static Sprite MovementIcon;
        public static Sprite QuitIcon;
        public static Sprite RendersIcon;
        public static Sprite RestartIcon;
        public static Sprite SecurityIcon;
        public static Sprite SettingsIcon;
        public static Sprite SpoofsIcon;
        public static Sprite WorldsIcon;
        public static Sprite WorldSpecificIcon;
        public static Sprite HelpIcon;
        public static AudioClip LoadingSong;

        public static void Initialize()
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

        public static Font LoadFont(string file)
        {
            Font font2 = BlazeBundle.LoadAsset(file, Il2CppType.Of<Font>()).Cast<Font>();
            font2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return font2;
        }

        public static Sprite LoadSprite(string file)
        {
            Sprite sprite2 = BlazeBundle.LoadAsset(file, Il2CppType.Of<Sprite>()).Cast<Sprite>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite2;
        }

        public static AudioClip LoadAudioClip(string file)
        {
            AudioClip AudioClip = BlazeBundle.LoadAsset(file, Il2CppType.Of<AudioClip>()).Cast<AudioClip>();
            AudioClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return AudioClip;
        }

        public static Shader LoadShader(string file)
        {
            Shader shader = BlazeBundle.LoadAsset(file, Il2CppType.Of<Shader>()).Cast<Shader>();
            shader.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return shader;
        }

        public static Texture2D LoadTexture(string file)
        {
            Texture2D texture2D = BlazeBundle.LoadAsset(file, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return texture2D;
        }
    }
}
