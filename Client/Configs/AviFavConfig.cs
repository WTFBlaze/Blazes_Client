using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.Objects;
using System.IO;
using System.Reflection;

namespace Blaze.Configs
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    class AviFavConfig
    {
        public ModObjects.ModAviFavorites AvatarFavorites = new();

        public static AviFavConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.AviFavFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.AviFavFile, new AviFavConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<AviFavConfig>(ModFiles.AviFavFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.AviFavFile, Instance);
        }
    }
}
