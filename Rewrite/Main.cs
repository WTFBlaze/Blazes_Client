using Blaze.API;
using Blaze.Modules;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDKBase;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze
{
    public static class Main
    {
        private static bool IsVerified;
        public static string userHash;
        public static string authKey;
        public static List<BModule> Modules = new();
        public static List<ModBlazeTag> OtherUsers = new();
        public static List<ModTag> Tags = new();
        public static GameObject BlazesComponents;
        public static VRC_Pickup[] Pickups;
        public static PostProcessVolume[] Blooms;
        public static VRCStation[] Seats;
        public static Dictionary<string, BlazePlayerInfo> Players = new();
        public static VRC.Player SelectedPlayer;
        public static VRC.Player Target;
        public static bool QMIsOpened;
        public static bool SMIsOpened;
        public static bool AMIsOpened;
        public static Camera CurrentCamera;
        public static ModUser CurrentUser;
        public static Sprite LeanSprite;
        public static Sprite NikeiSprite;

        public static void OnApplicationStart(string loaderGUID = null, string hash = null, string bcAuthKey = null)
        {
            // Check if loader used is valid loader version
            if (Functions.CheckLoaderVersion(loaderGUID))
            {
                IsVerified = true;
                userHash = hash;
                authKey = bcAuthKey;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Loader you tried to run this mod with is an invalid loader! Please use the official Blaze's Client Loader file!");
            }

            // MelonLoader version check
            if (int.TryParse(((string)typeof(BuildInfo).GetField("Version").GetValue(null)).Replace(".", ""), out int num) && int.TryParse("0.5.2".Replace(".", ""), out int num2))
            {
                if (num2 > num)
                {
                    Logs.Warning("You are currently using an outdated MelonLoader Version than what Blaze's Client is built for! [Recommended Version: 0.5.2]");
                    Logs.Warning("Any support requests submitted while using this ML Version will be ignored. Please Update your ML to 0.5.2");
                }
                else if (num > num2)
                {
                    Logs.Warning("You are currently using a MelonLoader Version that is newer than what Blaze's Client is built for! [Recommended Version: 0.5.2]");
                    Logs.Warning("Any support requests submitted while using this ML Version will be ignored. Please Downgrade your ML to 0.5.2");
                }
            }

            if (!IsVerified) return;
            Logs.Log("Initializing Blaze's Client...");
            Console.Title = "Blaze's Client | Created by WTFBlaze...";
            ModFiles.Initialize();
            AssetBundleManager.Initialize();
            Patching.Initialize();
            Config.Initialize();
            MelonCoroutines.Start(WaitForUI());

            #region Download Nameplate Images
            MelonCoroutines.Start(DownloadLean());
            MelonCoroutines.Start(DownloadNikei());
            #endregion

            #region Register Modules
            Modules.Add(new BlazeQM());
            Modules.Add(new BlazeSM());
            Modules.Add(new BlazeHelp());
            //Modules.Add(new BlazeStaff());
            Modules.Add(new BlazeNetwork());
            Modules.Add(new ClientCaching());
            Modules.Add(new PlayerInfo());
            Modules.Add(new GameLogs());
            Modules.Add(new DiscordRPC());
            Modules.Add(new Nameplates());
            Modules.Add(new MediaControls());
            Modules.Add(new EZRip());
            Modules.Add(new TrueRanks());
            Modules.Add(new HWIDSpoofer());
            Modules.Add(new AntiCrash());
            Modules.Add(new Flight());
            Modules.Add(new SimpleMovements());
            Modules.Add(new AvatarFavorites());
            Modules.Add(new AvatarSearch());
            Modules.Add(new VRCESP());
            Modules.Add(new IMGUIESP());
            Modules.Add(new PCKeybinds());
            Modules.Add(new LocalBlock());
            Modules.Add(new PickupsList());
            Modules.Add(new UdonManipulator());
            Modules.Add(new WorldToggles());
            Modules.Add(new SelectUser());
            //Modules.Add(new EventLocker());
            Modules.Add(new InstanceHistory());
            Modules.Add(new AvatarIndexer());
            Modules.Add(new ThirdPerson());
            Modules.Add(new FOVChanger());
            Modules.Add(new Waypoints());
            Modules.Add(new AccountSaver());
            Modules.Add(new AvatarBlacklist());
            Modules.Add(new ShaderBlacklist());
            Modules.Add(new PersonalMirror());
            Modules.Add(new PersonalKOS());
            Modules.Add(new AnnoyingCamera());
            Modules.Add(new SilentFavoriting());
            Modules.Add(new ColliderHider());
            Modules.Add(new EmojiSpammer());
            Modules.Add(new ItemOrbit());
            Modules.Add(new ItemSteal());
            Modules.Add(new PlayerOrbit());
            Modules.Add(new USpeakSpammer());
            Modules.Add(new AmongUs());
            Modules.Add(new Ghost());
            #endregion

            foreach (var m in Modules) m.Start();
        }

        public static void OnUpdate()
        {
            if (!IsVerified) return;
            foreach (var m in Modules) m.Update();
        }

        public static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!IsVerified) return;
            Functions.AntiLockInstance(buildIndex);
            foreach (var m in Modules) m.SceneInitialized(buildIndex, sceneName);
        }

        public static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!IsVerified) return;
            if (buildIndex == -1)
            {
                MelonCoroutines.Start(WaitForPlayer());
            }
            foreach (var m in Modules) m.SceneLoaded(buildIndex, sceneName);
        }

        private static void OnHUDInit()
        {
            if (!IsVerified) return;
            BlazeQM.InitiliazeHud();
        }

        private static IEnumerator WaitForUI()
        {
            while (GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud") == null) yield return null;
            OnHUDInit();

            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            APIStuff.Left.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left").transform);
            APIStuff.Right.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right").transform);
            APIStuff.Left.WingOpen.GetComponent<Button>().onClick.AddListener(new Action(() => APIStuff.Init_L()));
            APIStuff.Right.WingOpen.GetComponent<Button>().onClick.AddListener(new Action(() => APIStuff.Init_R()));
            BlazesComponents = new GameObject("Blaze's Components");
            UnityEngine.Object.DontDestroyOnLoad(BlazesComponents);
            CurrentCamera = Camera.main;
            foreach (var m in Modules) m.UI();
        }

        private static IEnumerator WaitForPlayer()
        {
            while (PlayerUtils.CurrentUser() == null) yield return null;
            while (PlayerUtils.CurrentUser().GetAPIUser() == null) yield return null;
            foreach (var m in Modules) m.LocalPlayerLoaded();
        }

        private static IEnumerator DownloadLean()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://cdn.wtfblaze.com/downloads/lean.png");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                var tex = DownloadHandlerTexture.GetContent(request);
                tex.Apply();
                tex.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var rect = new Rect(0f, 0f, tex.width, tex.height);
                var vector = Vector2.zero;
                var border = Vector4.zero;
                LeanSprite = Sprite.CreateSprite_Injected(tex, ref rect, ref vector, 100, 0, SpriteMeshType.Tight, ref border, false);
                LeanSprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
        }

        private static IEnumerator DownloadNikei()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://cdn.wtfblaze.com/downloads/nikei.png");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                var tex = DownloadHandlerTexture.GetContent(request);
                tex.Apply();
                tex.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var rect = new Rect(0f, 0f, tex.width, tex.height);
                var vector = Vector2.zero;
                var border = Vector4.zero;
                NikeiSprite = Sprite.CreateSprite_Injected(tex, ref rect, ref vector, 100, 0, SpriteMeshType.Tight, ref border, false);
                NikeiSprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
        }
    }
}
