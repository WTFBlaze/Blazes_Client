using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Configs
{
    class LocalBlockConfig
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
}
