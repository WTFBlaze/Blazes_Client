using Blaze.Utils;
using Blaze.Utils.Managers;
using System.IO;
using System.Reflection;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Configs
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public class MainConfig
    {
        public bool DebugPanel = true;
        public bool PlayerList = true;
        public bool LogToHud = true;
        public bool LogOnlyFriendsToHud = true;
        public bool LogPhotonsJoin = true;
        public bool LogPhotonsLeave = true;
        public bool LogPlayersJoin = true;
        public bool LogPlayersLeave = true;
        public bool LogNotifications = true;
        public bool LogAvatarsSwitched = true;
        public bool LogRPCS = false;
        public bool LogPhotonEvents = false;
        public bool LogRPCObjectInstantiated = true;
        public bool LogRPCObjectDestroyed = true;
        public bool LogRPCCameraToggled = true;
        public bool LogRPCPhotoTaken = true;
        public bool LogRPCCameraTimer = true;
        public bool LogRPCPortalDropped = true;
        public bool LogRPCEnteredPortal = true;
        public bool LogRPCSpawnedEmoji = true;
        public bool LogRPCPlayedEmote = true;
        public bool LogRPCStoppedEmote = true;
        public bool LogRPCReloadedAvatar = true;
        public bool AntiPortal = false;
        public bool PortalPrompt = false;
        public bool AntiInstanceLock = false;
        public bool AntiUdon = false;
        public bool AntiTeleport = false;
        public bool AntiWorldTriggers = false;
        public bool AntiDesync = false;
        public bool GodMode = false;
        public float DesktopFlySpeed = 10f;
        public float VRFlySpeed = 6f;
        public bool DirectionalFly = true;
        public bool MouseTP = false;
        public bool InfiniteJump = false;
        public bool BunnyHop = false;
        public bool OrbitAnnoyanceMode = false;
        public float ItemOrbitSpeed = 1f;
        public float ItemOrbitSize = 1f;
        public float PlayerOrbitSpeed = 1f;
        public float PlayerOrbitSize = 1f;
        public bool ShowModerations = true;
        public bool ModerationBlocks = true;
        public bool ModerationUnblocks = true;
        public bool ModerationMutes = true;
        public bool ModerationUnmutes = true;
        public bool ModerationKicks = true;
        public bool ModerationWarns = true;
        public bool ModerationMicOff = true;
        public bool ModerationAntiWarns = true;
        public bool ModerationAntiMicOff = true;
        public bool HWIDSpoofer = false;
        public bool PingSpoof = false;
        public bool FramesSpoof = false;
        public int PingSpoofValue = -666;
        public int FramesSpoofValue = 420;
        public bool WorldSpoof = false;
        public string WorldSpoofID = "wrld_a5e1fddf-7938-42de-b1ee-698a60fc2ddc";
        public bool OfflineSpoof = false;
        //public bool QuestSpoof = false; - Detected
        public bool DisableBloom = false;
        public bool DisablePickups = false;
        public bool OptimizeMirrors = false;
        public bool PCKeybinds = true;
        public bool UseDiscordRPC = true;
        public bool BetterNameplates = true;
        public bool CustomNameplateColor = true;
        public bool UseBelowNameplates = true;
        public string NameplateColor = "#9830d9";
        public bool NamePlateTrustRank = true;
        public bool NamePlatePlatform = true;
        public bool NamePlatePing = true;
        public bool NamePlateFrames = true;
        public bool NamePlateMasterTag = true;
        public bool NamePlateFriendsTag = true;
        public bool NamePlateCrashState = true;
        public bool ThirdPerson = true;
        public bool ThirdPersonRearCameraChangedEnabled = true;
        public float ThirdPersonFOV = 80f;
        public float ThirdPersonNearClipPlane = 0.01f;
        public float MirrorScaleX = 5f;
        public float MirrorScaleY = 3f;
        public bool OptimizedPersonalMirror = false;
        public bool CanPickupPersonalMirror = false;
        public bool ComfyVRMenu = false;
        public float RotationSpeed = 1f;
        public bool UseActionMenu = true;
        public bool EmojiLaughing = false;
        public bool EmojiSmiles = false;
        public bool EmojiGhosts = false;
        public bool EmojiThumbsUp = false;
        public bool EmojiThumbsDown = false;
        public bool EmojiClouds = false;
        public bool EmojiSnowflakes = false;
        public bool EmojiWaterDroplets = false;
        public bool EmojiCandy = false;
        public bool EmojiCandyCorn = false;
        public bool EmojiPizza = false;
        public bool EmojiConfetti = false;
        public bool EmojiGifts = false;
        public bool EmojiMoney = false;
        public bool EmojiZ = false;
        public bool EmojiPineapple = false;
        //public ColorJson UIColor { get; set; } = new ColorJson(new UnityEngine.Color32(161, 52, 235, 255));
        public float UIColorRed = 161;
        public float UIColorGreen = 52;
        public float UIColorBlue = 235;
        public bool UseTabButton = false;
        public bool IRLClock = true;
        public bool PersonalKOS = false;
        public bool KOSUdonNuke = true;
        public bool KOSPortals = true;
        public bool ConfirmQuit = false;
        public bool FriendsPlusHome = false;
        public bool DesktopDebug = true;
        public bool DesktopPlayerList = true;
        public bool LogAvatars = true;
        public bool LogPlayers = true;
        public bool LogWorlds = true;
        public bool DisableAvatarPreview = false;

        public static MainConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.ConfigFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.ConfigFile, new MainConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<MainConfig>(ModFiles.ConfigFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.ConfigFile, Instance);
        }
    }
}
