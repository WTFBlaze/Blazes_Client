using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze
{
    public static class Config
    {
        public static MainConfig Main => MainConfig.Instance;
        public static AntiCrashConfig AntiCrash => AntiCrashConfig.Instance;
        public static InstanceHistoryConfig InstanceHistory => InstanceHistoryConfig.Instance;
        public static AvatarFavoritesConfig AviFavs => AvatarFavoritesConfig.Instance;
        public static KOSConfig KOS => KOSConfig.Instance;
        public static LocalBlockConfig Blocks => LocalBlockConfig.Instance;
        public static WaypointsConfig Waypoints => WaypointsConfig.Instance;
        //public static EventLockConfig EventLocks => EventLockConfig.Instance;

        public static void SaveAll()
        {
            Main.Save();
            AntiCrash.Save();
            InstanceHistory.Save();
            AviFavs.Save();
            KOS.Save();
            Blocks.Save();
            Waypoints.Save();
            //EventLocks.Save();
        }

        public static void Initialize()
        {
            MainConfig.Load();
            AntiCrashConfig.Load();
            InstanceHistoryConfig.Load();
            AvatarFavoritesConfig.Load();
            KOSConfig.Load();
            LocalBlockConfig.Load();
            WaypointsConfig.Load();
            //EventLockConfig.Load();
        }

        public class MainConfig
        {
            public string CoreLabel = "---==[ CORE ]==---";
            public bool UseTabButton = false;
            public bool ConfirmQuit = false;
            public bool ComfyVRMenu = false;
            public bool PCKeybinds = false;
            public bool UseDiscordRPC = true;

            public string ClientUILabel = "---==[ CLIENT UI ]==---";
            public bool VRPlayerList = false;
            public bool VRDebugPanel = false;
            public bool DesktopPlayerList = false;
            public bool DesktopDebugPanel = false;
            public bool BetterNameplates = false;
            public bool CustomNameplateColor = false;
            public bool UseBelowNameplates = false;
            public string NameplateColor = "#000000";
            public bool NameplateModerations = false;
            public bool NamePlateTrustRank = false;
            public bool NamePlatePlatform = false;
            public bool NamePlatePing = false;
            public bool NamePlateFrames = false;
            public bool NamePlateMasterTag = false;
            public bool NamePlateFriendsTag = false;
            public bool NamePlateCrashState = false;
            public bool IRLClock = false;
            public bool HudFPS = false;
            public bool DisableAvatarPreview = false;
            public bool DisableCarousel = false;

            public string SelfLabel = "---==[ SELF ]==---";
            public bool OptimizedPersonalMirror = false;
            public bool CanPickupPersonalMirror = false;
            public float MirrorScaleX = 5f;
            public float MirrorScaleY = 3f;

            public string WorldLabel = "---==[ WORLD ]==---";
            public bool FriendsPlusHome = false;
            public bool DisableBloom = false;
            public bool DisablePickups = false;
            public bool OptimizeMirrors = false;
            public bool BeautifyMirrors = false;

            public string MovementLabel = "---==[ MOVEMENT ]==---";
            public float DesktopFlySpeed = 10f;
            public float VRFlySpeed = 6f;
            public bool DirectionalFly = true;
            public bool MouseTP = false;
            public bool InfiniteJump = false;

            public string ExploitsLabel = "---==[ EXPLOITS ]==---";
            public float PlayerOrbitSpeed = 1f;
            public float ItemOrbitSpeed = 1f;
            public float PlayerOrbitSize = 0.5f;
            public float ItemOrbitSize = 0.5f;
            public bool OrbitAnnoyanceMode = false;
            public bool PersonalKOS = false;
            public bool KOSPortals = false;
            public bool KOSUdonNuke = false;
            public bool KOSItemSteal = false;
            public bool EmojiLaughing = false;
            public bool EmojiSmiles = false;
            public bool EmojiGhosts = false;
            public bool EmojiThumbsUp = false;
            public bool EmojiThumbsDown = false;
            public bool EmojiClouds = false;
            public bool EmojiSnowflakes = false;
            public bool EmojiCandy = false;
            public bool EmojiCandyCorn = false;
            public bool EmojiPizza = false;
            public bool EmojiConfetti = false;
            public bool EmojiGifts = false;
            public bool EmojiMoney = false;
            public bool EmojiZ = false;
            public bool EmojiPineapple = false;

            public string SecurityLabel = "---==[ SECURITY ]==---";
            public bool AntiInstanceLock = false;
            public bool AntiPortal = false;
            public bool PortalPrompt = false;
            public bool AntiUdon = false;
            public bool AntiTeleport = false;
            public bool AntiWorldTriggers = false;
            public bool AntiDesync = false;

            public string ModerationsLabel = "---==[ MODERATIONS ]==---";
            public bool ShowModerations = false;
            public bool ModerationMutes = false;
            public bool ModerationUnmutes = false;
            public bool ModerationBlocks = false;
            public bool ModerationUnblocks = false;
            public bool ModerationWarns = false;
            public bool ModerationKicks = false;
            public bool ModerationMicOff = false;
            public bool ModerationAntiWarns = false;
            public bool ModerationAntiMicOff = false;

            public string PipelineLabel = "---==[ PIPELINE ]==---";
            public bool UsePipeline = false;
            public bool PipelineOnline = false;
            public bool PipelineOffline = false;
            public bool PipelineLocations = false;
            public bool PipelineAdded = false;
            public bool PipelineRemoved = false;
            public bool PipelineWebsite = false;
            public bool PipelineHudOnlyFavs = false;

            public string SpoofsLabel = "---==[ SPOOFS ]==---";
            public bool HWIDSpoofer = false;
            public bool PingSpoof = false;
            public bool FramesSpoof = false;
            public int PingSpoofValue = -666;
            public int FramesSpoofValue = 420;

            public string LogSettingsLabel = "---==[ LOGGING SETTINGS ]==---";
            public bool LogAvatarsToFile = false;
            public bool LogWorldsToFile = false;
            public bool LogPlayersToFile = false;
            public bool LogToHud = false;
            public bool LogOnlyFriendsToHud = false;
            public bool LogPlayersJoin = false;
            public bool LogPlayersLeave = false;
            public bool LogPhotonsJoin = false;
            public bool LogPhotonsLeave = false;
            public bool LogRPCObjectDestroyed = false;
            public bool LogRPCCameraToggled = false;
            public bool LogRPCCameraTimer = false;
            public bool LogRPCPhotoTaken = false;
            public bool LogRPCPortalDropped = false;
            public bool LogRPCEnteredPortal = false;
            public bool LogRPCSpawnedEmoji = false;
            public bool LogRPCPlayedEmote = false;
            public bool LogRPCStoppedEmote = false;
            public bool LogRPCReloadedAvatar = false;
            public bool LogAvatarsSwitched = false;
            public bool LogRPCS = false;
            public bool LogPhotonEvents = false;
            public bool LogNotifications = false;
            public bool LogRPCObjectInstantiated = false;
            public bool LogAvatars = false;
            public bool LogPlayers = false;
            public bool LogWorlds = false;

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

        public class AntiCrashConfig
        {
            public bool CheckSelf = false;
            public bool CheckFriends = false;
            public bool CheckOthers = false;
            public bool OnlyCheckInPublicLobbies = false;

            public bool AntiPhysicsCrash = false;
            public bool AntiBlendShapeCrash = false;
            public bool AntiAudioCrash = false;
            public bool AntiClothCrash = false;
            public bool AntiParticleSystemCrash = false;
            public bool AntiDynamicBoneCrash = false;
            public bool AntiLightSourceCrash = false;
            public bool AntiMeshCrash = false;
            public bool AntiMaterialCrash = false;
            public bool AntiShaderCrash = false;
            public bool AntiScreenSpace = false;
            public int MaxAllowedAvatarAudioSources = 25;
            public int MaxAllowedAvatarMaterials = 225;
            public int MaxAllowedAvatarClothVertices = 15000;
            public int MaxAllowedAvatarParticleSimulationSpeed = 5;
            public int MaxAllowedAvatarParticleCollisionShapes = 1024;
            public int MaxAllowedAvatarParticleTrails = 64;
            public int MaxAllowedAvatarParticleLimit = 5000;
            public int MaxAllowedAvatarParticleMeshVertices = 1000000;
            public int MaxAllowedAvatarPolygons = 2000000;
            public int MaxAllowedAvatarDynamicBones = 75;
            public int MaxAllowedAvatarDynamicBoneColliders = 50;
            public int MaxAllowedAvatarLightSources = 5;
            public int MaxAllowedAvatarTransformScale = 1000;

            public static AntiCrashConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.AntiCrashFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.AntiCrashFile, new AntiCrashConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<AntiCrashConfig>(ModFiles.AntiCrashFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.AntiCrashFile, Instance);
            }
        }

        public class InstanceHistoryConfig
        {
            public List<ModInstanceHistory> list = new();

            public static InstanceHistoryConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.InstanceHistoryFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.InstanceHistoryFile, new InstanceHistoryConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<InstanceHistoryConfig>(ModFiles.InstanceHistoryFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.InstanceHistoryFile, Instance);
            }
        }

        public class AvatarFavoritesConfig
        {
            public ModAviFavorites AvatarFavorites = new();

            public static AvatarFavoritesConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.AviFavFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.AviFavFile, new AvatarFavoritesConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<AvatarFavoritesConfig>(ModFiles.AviFavFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.AviFavFile, Instance);
            }
        }

        public class KOSConfig
        {
            public List<KosObject> list = new();

            public static KOSConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.KOSFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.KOSFile, new KOSConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<KOSConfig>(ModFiles.KOSFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.KOSFile, Instance);
            }

            public class KosObject
            {
                public string DisplayName { get; set; }
                public string UserID { get; set; }
                public DateTime DateAddedToKos { get; set; }
            }
        }

        public class LocalBlockConfig
        {
            public List<ModLocalBlock> list = new();

            public static LocalBlockConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.LocalBlockFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.LocalBlockFile, new LocalBlockConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<LocalBlockConfig>(ModFiles.LocalBlockFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.LocalBlockFile, Instance);
            }
        }

        public class WaypointsConfig
        {
            public List<WaypointWorldObject> list = new();

            public static WaypointsConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.WaypointsFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.WaypointsFile, new WaypointsConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<WaypointsConfig>(ModFiles.WaypointsFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.WaypointsFile, Instance);
            }

            public class WaypointObject
            {
                public float X { get; set; }
                public float Y { get; set; }
                public float Z { get; set; }
            }

            public class WaypointWorldObject
            {
                public string WorldID { get; set; }
                public List<WaypointObject> Waypoints { get; set; }
            }
        }

        public class EventLockConfig
        {
            public List<ModEventLocker> list = new();

            public static EventLockConfig Instance;
            public static void Load()
            {
                if (!File.Exists(ModFiles.EventLockFile))
                {
                    JsonManager.WriteToJsonFile(ModFiles.EventLockFile, new EventLockConfig());
                }
                Instance = JsonManager.ReadFromJsonFile<EventLockConfig>(ModFiles.EventLockFile);
            }

            public void Save()
            {
                JsonManager.WriteToJsonFile(ModFiles.EventLockFile, Instance);
            }
        }
    }
}
