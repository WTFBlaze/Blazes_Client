using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Configs
{
    internal static class Config
    {
        internal static MainConfig Main => MainConfig.Instance;
        internal static AviFavConfig AvatarFavs => AviFavConfig.Instance;
        internal static CrashConfig AntiCrash => CrashConfig.Instance;
        internal static InstanceHistoryConfig InstanceHistory => InstanceHistoryConfig.Instance;
        internal static KOSConfig KOS => KOSConfig.Instance;
        internal static LocalBlockConfig LocalBlock => LocalBlockConfig.Instance;
        internal static WaypointsConfig Waypoints => WaypointsConfig.Instance;
        internal static BonesConfig GlobalBones => BonesConfig.Instance;

        internal static void Load()
        {
            MainConfig.Load();
            AviFavConfig.Load();
            CrashConfig.Load();
            InstanceHistoryConfig.Load();
            KOSConfig.Load();
            LocalBlockConfig.Load();
            WaypointsConfig.Load();
            BonesConfig.Load();
        }

        internal static void SaveAll()
        {
            Main.Save();
            AvatarFavs.Save();
            AntiCrash.Save();
            InstanceHistory.Save();
            KOS.Save();
            LocalBlock.Save();
            Waypoints.Save();
            GlobalBones.Save();
        }
    }
}
