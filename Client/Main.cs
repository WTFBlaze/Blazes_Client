using Blaze.API;
using Blaze.Configs;
using Blaze.Modules;
using Blaze.Utils;
using Blaze.Utils.Attributes;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using I2.Loc;
using MelonLoader;
using System;
using System.Collections;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Blaze
{
    public static class Main
    {
        private static bool IsVerified;
        internal static string userHash;
        internal static string authKey;

        public static void OnApplicationStart(string loaderGUID = null, string hash = null, string bcAuthKey = null)
        {
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
            if (!IsVerified) return;
            Logs.Log("Initializing Blaze's Client...");
            Console.Title = "Blaze's Client | Created by WTFBlaze...";
            ModFiles.Initialize();
            // Load the assets from the embedded Asset Bundle
            AssetBundleManager.Initialize();
            // Initialize the Patches required to make the mod run
            Patching.Initialize();
            // Load Configs
            Config.Load();
            // Start Searching for UiManager
            MelonCoroutines.Start(WaitForUiManager());

            // Register EnableDisableListener to IL2CPP
            ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>();

			// Start custom loading screen process
			//MelonCoroutines.Start(ProcessLoadingScreen());

            // Add Modules to List for functions to call
            BlazeInfo.Modules.Add(new HWIDSpoofer());
            BlazeInfo.Modules.Add(new BlazeMenu());
            BlazeInfo.Modules.Add(new BlazeSocialMenu());
            BlazeInfo.Modules.Add(new QMAddons());
            BlazeInfo.Modules.Add(new BlazeHelpMenu());
            BlazeInfo.Modules.Add(new DiscordRPC());
            BlazeInfo.Modules.Add(new BlazePlayerInfo());
            BlazeInfo.Modules.Add(new TrueRank());
            BlazeInfo.Modules.Add(new Flight());
            BlazeInfo.Modules.Add(new SimpleMovement());
            BlazeInfo.Modules.Add(new ItemOrbit());
            BlazeInfo.Modules.Add(new PlayerOrbit());
            BlazeInfo.Modules.Add(new EmojiSpammer());
            BlazeInfo.Modules.Add(new WorldOptimizations());
            BlazeInfo.Modules.Add(new UdonManipulator());
            BlazeInfo.Modules.Add(new ImGUIESP());
            BlazeInfo.Modules.Add(new DesktopDebug());
            BlazeInfo.Modules.Add(new VRCESP());
            BlazeInfo.Modules.Add(new PersonalKOS());
            BlazeInfo.Modules.Add(new MediaControls());
            BlazeInfo.Modules.Add(new AvatarFavorites());
            BlazeInfo.Modules.Add(new AvatarSearch());
            BlazeInfo.Modules.Add(new SilentFavoriting());
            BlazeInfo.Modules.Add(new NetworkSanity());
            BlazeInfo.Modules.Add(new EZRip());
            BlazeInfo.Modules.Add(new BlacklistedAvatars());
            BlazeInfo.Modules.Add(new AvatarIndexer());
            BlazeInfo.Modules.Add(new Waypoints());
            BlazeInfo.Modules.Add(new PCKeybinds());
            BlazeInfo.Modules.Add(new AccountSaver());
            BlazeInfo.Modules.Add(new AnnoyingCamera());
            BlazeInfo.Modules.Add(new InstanceHistory());
            BlazeInfo.Modules.Add(new BlacklistedShaders());
            BlazeInfo.Modules.Add(new FOVChanger());
            BlazeInfo.Modules.Add(new BodyAttacher());
            BlazeInfo.Modules.Add(new AmongUs());
            BlazeInfo.Modules.Add(new Ghost());
            BlazeInfo.Modules.Add(new ColliderHider());
            BlazeInfo.Modules.Add(new USpeakSpammer());
            BlazeInfo.Modules.Add(new ItemSteal());
            BlazeInfo.Modules.Add(new ExtraAviLists());
            BlazeInfo.Modules.Add(new PickupsList());
            BlazeInfo.Modules.Add(new AntiCrash());
            BlazeInfo.Modules.Add(new GlobalBones());
            BlazeInfo.Modules.Add(new PersonalMirror());
            BlazeInfo.Modules.Add(new ThirdPerson());
            BlazeInfo.Modules.Add(new GameLogs());
            BlazeInfo.Modules.Add(new LocalBlock());
            BlazeInfo.Modules.Add(new DesktopPlayerList());
            BlazeInfo.Modules.Add(new IKManipulation());
            BlazeInfo.Modules.Add(new BlazeNetwork());

			Application.targetFrameRate = 120;
            foreach (var m in BlazeInfo.Modules) m.Start();
        }

        public static void OnUpdate()
        {
            if (!IsVerified) return;
            foreach (var m in BlazeInfo.Modules) m.Update();
        }

		public static void OnFixedUpdate()
        {
			if (!IsVerified) return;
			foreach (var m in BlazeInfo.Modules) m.FixedUpdate();
        }

        private static void OnSMInit()
        {
            if (!IsVerified) return;
            Console.Title = "Blaze's Client | Created by WTFBlaze...";
            foreach (var m in BlazeInfo.Modules) m.SocialMenuUI();
        }

        private static void OnQMInit()
        {
            if (!IsVerified) return;
            Console.Title = "Blaze's Client | Created by WTFBlaze...";
			BlazeInfo.BlazesComponents = new GameObject("Blaze's Components");
			UnityEngine.Object.DontDestroyOnLoad(BlazeInfo.BlazesComponents);
			BlazeInfo.CurrentCamera = Camera.main;
            foreach (var m in BlazeInfo.Modules) m.QuickMenuUI();
        }

        private static void OnHUDInit()
        {
            if (!IsVerified) return;
			QMAddons.InitializeHUD();
        }

        public static void OnSceneWasInitialized(int buildIndex, string sceneName)  
        {
            if (!IsVerified) return;
            Patching.AntiLockInstance(-1, null);
            if (buildIndex != -1)
            {
                Patching.CachedPortals.Clear();
            }
            foreach (var m in BlazeInfo.Modules) m.SceneInitialized(buildIndex, sceneName);
        }

        public static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!IsVerified) return;
            if (buildIndex == -1)
            {
                MelonCoroutines.Start(WaitForPlayer());
            }
            foreach (var m in BlazeInfo.Modules) m.SceneLoaded(buildIndex, sceneName);
        }

        private static IEnumerator WaitForUiManager()
        {
            //while (UIManager.prop_UIManager_0 == null) yield return null;
            while (GameObject.Find("UserInterface/MenuContent/Backdrop") == null) yield return null;
            OnSMInit();

            //while (GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music") == null && GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup") == null) yield return null;
            //OnLoadingScreenInit();

            while (GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud") == null) yield return null;
            OnHUDInit();

            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;
            APIStuff.Left.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left").transform);
            APIStuff.Right.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right").transform);
            APIStuff.Left.WingOpen.GetComponent<Button>().onClick.AddListener(new Action(() => APIStuff.Init_L()));
            APIStuff.Right.WingOpen.GetComponent<Button>().onClick.AddListener(new Action(() => APIStuff.Init_R()));
            OnQMInit();
        }

		public static IEnumerator ProcessLoadingScreen()
		{
			if (!VRCPlayer.field_Internal_Static_VRCPlayer_0)
			{
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/TitleText").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/Rectangle").active = false;
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/Rectangle/Panel").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonLeft").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonLeft/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				//GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Background").active = false;
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/ButtonAboutUs (1)").active = false;
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/VRChatButtonLogin").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/StoreButtonLogin (1)").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/VRChatButtonLogin/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/StoreButtonLogin (1)/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/ButtonCreate").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/VRChat_LOGO (1)").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextLoginWith").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Text>().m_Text = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Text>().text = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Localize>().prop_String_1 = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Localize>().field_Public_String_2 = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Localize>().field_Public_String_0 = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextWelcome").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/TextWelcome").GetComponent<Text>().text = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/TextWelcome").GetComponent<Text>().m_Text = "Blaze's Client";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/TextWelcome").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/VRChat_LOGO (1)").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/ButtonAboutUs").active = false;
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/Panel").active = false;
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/Text").GetComponent<Text>().m_Text = "Login (VRCHAT)";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/Text").GetComponent<Text>().text = "Login (VRCHAT)";
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldPassword").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldUser").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldPassword/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldUser/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/ButtonBack (1)").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/ButtonDone (1)").GetComponent<Image>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/ButtonBack (1)/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/ButtonDone (1)/Text").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Image").active = false;
				GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/TextOr").GetComponent<Text>().color = new Color(0.4157f, 0, 1, 1);
				GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/SkyCube_Baked").active = false;
				MeshRenderer Border = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponent<MeshRenderer>();
				Light PointLight = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Point light").GetComponent<Light>();
				ReflectionProbe ReflectionProbe1 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Reflection Probe").GetComponent<ReflectionProbe>();
				while (Border == null)
				{
					yield return null;
				}
				while (PointLight == null)
				{
					yield return null;
				}
				while (ReflectionProbe1 == null)
				{
					yield return null;
				}
				ReflectionProbe1.mode = ReflectionProbeMode.Realtime;
				ReflectionProbe1.backgroundColor = new Color(0.4006691f, 0f, 1f, 0f);
				Material material2 = (Border.material = new Material(Shader.Find("Standard")));
				Border.material.color = Color.black;
				Border.material.SetFloat("_Metallic", 1f);
				Border.material.SetFloat("_SmoothnessTextureChar", 1f);
				PointLight.color = Color.white;
				ParticleSystem Snow = GameObject.Find("/UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
				ParticleSystemRenderer Snow2 = GameObject.Find("/UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
				while (Snow == null)
				{
					yield return null;
				}
				while (Snow2 == null)
				{
					yield return null;
				}
				var tealBackground = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked");
				while (tealBackground == null)
                {
					yield return null;
                }
				tealBackground.SetActive(false);
				Snow.gameObject.SetActive(value: false);
				Snow.gameObject.transform.position -= new Vector3(0f, 5f, 0f);
				Material TrailMaterial = (Snow2.trailMaterial = new Material(Shader.Find("UI/Default"))
				{
					color = Color.white
				});
				Snow2.material.color = Color.white;
				Snow.trails.enabled = true;
				Snow.trails.mode = ParticleSystemTrailMode.PerParticle;
				Snow.trails.ratio = 1f;
				Snow.trails.lifetime = 0.04f;
				Snow.trails.minVertexDistance = 0f;
				Snow.trails.worldSpace = false;
				Snow.trails.dieWithParticles = true;
				Snow.trails.textureMode = ParticleSystemTrailTextureMode.Stretch;
				Snow.trails.sizeAffectsWidth = true;
				Snow.trails.sizeAffectsLifetime = false;
				Snow.trails.inheritParticleColor = false;
				Snow.trails.colorOverLifetime = Color.white;
				Snow.trails.widthOverTrail = 0.1f;
				Snow.trails.colorOverTrail = new Color(0.02987278f, 0f, 0.3962264f, 0.5f);
				Snow.shape.scale = new Vector3(1f, 1f, 1.82f);
				Snow.main.startColor.mode = ParticleSystemGradientMode.Color;
				Snow.colorOverLifetime.enabled = false;
				Snow.main.prewarm = false;
				Snow.playOnAwake = true;
				Snow.startColor = new Color(1f, 0f, 0.282353f, 1f);
				Snow.noise.frequency = 1f;
				Snow.noise.strength = 0.5f;
				Snow.maxParticles = 250;
				Snow.gameObject.SetActive(value: true);
				GameObject CloseParticles = GameObject.Find("/UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_CloseParticles");
				while (CloseParticles == null)
				{
					yield return null;
				}
				CloseParticles.GetComponent<ParticleSystem>().startColor = Color.red;
				GameObject Floor = GameObject.Find("/UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_floor");
				while (Floor == null)
				{
					yield return null;
				}
				Floor.GetComponent<ParticleSystem>().startColor = Color.red;
				ParticleSystem Snow3 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
				ParticleSystemRenderer Snow4 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
				while (Snow3 == null)
				{
					yield return null;
				}
				while (Snow4 == null)
				{
					yield return null;
				}
				Snow3.gameObject.SetActive(value: false);
				Snow3.gameObject.transform.position -= new Vector3(0f, 5f, 0f);
				TrailMaterial.color = Color.white;
				Snow4.trailMaterial = TrailMaterial;
				Snow4.material.color = Color.white;
				new Sprite();
				//UnityWebRequest request1 = UnityWebRequestTexture.GetTexture("https://i.imgur.com/79KdVND.png");
				UnityWebRequest request1 = UnityWebRequestTexture.GetTexture("https://imgur.com/ZNnxhnc.png");
				yield return request1.SendWebRequest();
				if (request1.isNetworkError || request1.isHttpError)
				{
					Logs.Error("Custom Loading Screen - " + request1.error);

				}
				else
				{
					Texture2D text = DownloadHandlerTexture.GetContent(request1);
					//Rect rect = new Rect(0.0f, 0.0f, text.width, text.height);
					//Vector2 pivot = new Vector2(0.5f, 0.5f);
					// Vector4 border = Vector4.zero;
					//Sprite mfsprite = Sprite.CreateSprite_Injected(text, ref rect, ref pivot, 100.0f, 0, SpriteMeshType.Tight, ref border, false);
					Snow.GetComponent<ParticleSystemRenderer>().material.mainTexture = text;
					Snow2.GetComponent<ParticleSystemRenderer>().material.mainTexture = text;
					Snow3.GetComponent<ParticleSystemRenderer>().material.mainTexture = text;
					Snow4.GetComponent<ParticleSystemRenderer>().material.mainTexture = text;
					Snow.startSize = 1;
					Snow3.startSize = 1;

				}
				Snow3.trails.enabled = true;
				Snow3.trails.mode = ParticleSystemTrailMode.PerParticle;
				Snow3.trails.ratio = 1f;
				Snow3.trails.lifetime = 0.04f;
				Snow3.trails.minVertexDistance = 0f;
				Snow3.trails.worldSpace = false;
				Snow3.trails.dieWithParticles = true;
				Snow3.trails.textureMode = ParticleSystemTrailTextureMode.Stretch;
				Snow3.trails.sizeAffectsWidth = true;
				Snow3.trails.sizeAffectsLifetime = false;
				Snow3.trails.inheritParticleColor = false;
				Snow3.trails.colorOverLifetime = Color.white;
				Snow3.trails.widthOverTrail = 0.1f;
				//Snow3.trails.colorOverTrail = new Color(0.02987278f, 0f, 0.3962264f, 0.5f);
				Snow3.trails.colorOverTrail = new Color(0.2298728f, 0f, 0.09622642f, 0.5f);
				Snow3.shape.scale = new Vector3(1f, 1f, 1.82f);
				Snow3.main.startColor.mode = ParticleSystemGradientMode.Color;
				Snow3.colorOverLifetime.enabled = false;
				Snow3.main.prewarm = false;
				Snow3.playOnAwake = true;
				//Snow3.startColor = new Color(1f, 0f, 0.282353f, 1f);
				Snow3.startColor = Color.red;
				Snow3.noise.frequency = 1f;
				Snow3.noise.strength = 0.5f;
				Snow3.maxParticles = 250;
				Snow3.gameObject.SetActive(value: true);
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE/titleFrame").GetComponent<MeshRenderer>().material.color = Color.red;
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponent<MeshRenderer>().material.color = Color.red;
				//GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON/iconFrame").GetComponent<MeshRenderer>().material.color = Color.red;
				//GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON").active = false;
				//GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE").active = false;
				GameObject CloseParticles2 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_CloseParticles");
				while (CloseParticles2 == null)
				{
					yield return null;
				}
				CloseParticles2.GetComponent<ParticleSystem>().startColor = Color.red;
				GameObject Floor2 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_floor");
				while (Floor2 == null)
				{
					yield return null;
				}
				Floor2.GetComponent<ParticleSystem>().startColor = Color.red;
				Image LoadingBar = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/LOADING_BAR_BG").GetComponent<Image>();
				while (LoadingBar == null)
				{
					yield return null;
				}
				LoadingBar.color = Color.red;
				Image LoadingBar2 = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/LOADING_BAR").GetComponent<Image>();
				while (LoadingBar2 == null)
				{
					yield return null;
				}
				LoadingBar2.color = Color.red * 5f;
				Image LoadingBarPanel = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>();
				while (LoadingBarPanel == null)
				{
					yield return null;
				}
				LoadingBarPanel.color = Color.red * 5f;
				Image LoadingBarPanelRight = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>();
				while (LoadingBarPanelRight == null)
				{
					yield return null;
				}
				LoadingBarPanelRight.color = Color.red * 5f;
				Image LoadingBarPanelLeft = GameObject.Find("/UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>();
				while (LoadingBarPanelLeft == null)
				{
					yield return null;
				}
				LoadingBarPanelLeft.color = Color.red * 5f;
				//GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM").GetComponent<Animator>().enabled = false;
			}
		}

		private static void OnLoadingScreenInit()
        {
			/*GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().clip = AssetBundleManager.LoadingSong;
            GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().Play();
            GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>().volume = 0.06f;
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().clip = AssetBundleManager.LoadingSong;
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>().volume = 0.06f;
            var snowPart = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
            snowPart.startColor = Color.magenta;
            snowPart.trails.enabled = true;
            snowPart.trails.dieWithParticles = true;
            snowPart.trails.widthOverTrail = 0.35f;
            snowPart.trails.lifetimeMultiplier = 0.05f;
            snowPart.trails.inheritParticleColor = false;
            snowPart.noise.strengthMultiplier = 0.5f;
            var snowPart2 = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
            snowPart2.trailMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"))
            {
                color = Color.magenta,
            };
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").SetActive(false);
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE/titleFrame").GetComponent<MeshRenderer>().material.color = Color.magenta;
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponent<MeshRenderer>().material.color = Color.magenta;
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON/iconFrame").GetComponent<MeshRenderer>().material.color = Color.magenta;

            var snowPart3 = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystem>();
            snowPart3.startColor = Color.magenta;
            snowPart3.trails.enabled = true;
            snowPart3.trails.dieWithParticles = true;
            snowPart3.trails.widthOverTrail = 0.35f;
            snowPart3.trails.lifetimeMultiplier = 0.05f;
            snowPart3.trails.inheritParticleColor = false;
            snowPart3.noise.strengthMultiplier = 0.5f;
            var snowPart4 = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/_FX_ParticleBubbles/FX_snow").GetComponent<ParticleSystemRenderer>();
            snowPart4.trailMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"))
            {
                color = Color.magenta,
            };
            GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/SkyCube_Baked").SetActive(false);*/
		}

        private static IEnumerator WaitForPlayer()
        {
            while (PlayerUtils.CurrentUser() == null) yield return null;
            while (PlayerUtils.CurrentUser().GetAPIUser() == null) yield return null;
            foreach (var m in BlazeInfo.Modules) m.LocalPlayerLoaded();
        }
    }
}
