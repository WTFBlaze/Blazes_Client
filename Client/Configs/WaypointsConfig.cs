using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Configs
{
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
