using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Configs
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    class KeybindsConfig
    {
        public List<ModKeybind> Keybinds = new();

        public static KeybindsConfig Instance;
        public static void Load()
        {
            var addBinds = false;
            if (!File.Exists(ModFiles.KeybindsFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.KeybindsFile, new KeybindsConfig());
                addBinds = true;
            }
            Instance = JsonManager.ReadFromJsonFile<KeybindsConfig>(ModFiles.KeybindsFile);
            if (addBinds)
            {
                Instance.Keybinds.Add(new ModKeybind(ModFeature.Flight, KeyCode.LeftControl, KeyCode.F, true));
                Instance.Keybinds.Add(new ModKeybind(ModFeature.ESP, KeyCode.LeftControl, KeyCode.G, true));
                Instance.Save();
            }
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.KeybindsFile, Instance);
        }
    }
}
