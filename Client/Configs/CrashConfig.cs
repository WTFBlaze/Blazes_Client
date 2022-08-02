using Blaze.Utils;
using Blaze.Utils.Managers;
using System.IO;

namespace Blaze.Configs
{
    class CrashConfig
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

        public static CrashConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.AntiCrashFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.AntiCrashFile, new CrashConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<CrashConfig>(ModFiles.AntiCrashFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.AntiCrashFile, Instance);
        }
    }
}
