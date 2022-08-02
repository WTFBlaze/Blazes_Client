using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Configs
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    class KOSConfig
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
    }

    public class KosObject
    {
        public string DisplayName { get; set; }
        public string UserID { get; set; }
        public DateTime DateAddedToKos { get; set; }
    }
}
