using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Configs
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    class InstanceHistoryConfig
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
}
