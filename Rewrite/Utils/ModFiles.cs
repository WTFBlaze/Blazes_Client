using System.IO;
using UnityEngine;

namespace Blaze.Utils
{
    public static class ModFiles
    {
        // Folders
        public static string MainDir = Directory.GetParent(Application.dataPath) + "\\Blaze";
        public static string MLDir = Directory.GetParent(Application.dataPath) + "\\MelonLoader";
        public static string LogsDir = MainDir + "\\Logs";
        public static string ConfigsDir = MainDir + "\\Configs";
        public static string DependenciesDir = MainDir + "\\Dependencies";
        public static string ExportsDir = MainDir + "\\Exports";
        public static string ImportsDir = MainDir + "\\Imports";
        public static string VRCADir = MainDir + "\\VRCA";
        public static string VRCWDir = MainDir + "\\VRCW";
        public static string EZRipDir = MainDir + "\\EZRip";
        public static string EZRExportsDir = EZRipDir + "\\Exports";
        public static string EZRImportsDir = EZRipDir + "\\Imports";

        // Files
        public static string ConfigFile = ConfigsDir + "\\Config.json";
        public static string HWIDFile = ConfigsDir + "\\HWID.txt";
        public static string KOSFile = ConfigsDir + "\\PersonalKOS.json";
        public static string KeybindsFile = ConfigsDir + "\\Keybinds.json";
        public static string DiscordRPCFile = DependenciesDir + "\\DiscordRPC.dll";
        public static string FriendsExportFile = ExportsDir + "\\Exported-Friends.txt";
        public static string FriendsImportFile = ImportsDir + "\\Exported-Friends.txt";
        public static string AviFavFile = ConfigsDir + "\\Avatar-Favorites.json";
        public static string AntiCrashFile = ConfigsDir + "\\Anti-Crash.json";
        public static string BlockedShadersFile = ConfigsDir + "\\Blacklisted-Shaders.txt";
        public static string LocalBlockFile = ConfigsDir + "\\LocalBlocks.json";
        public static string LNCImportFile = ImportsDir + "\\Late-Night-Avi-Favs.json";
        public static string ApolloImportFile = ImportsDir + "\\Apollo-Avi-Favs.json";
        public static string BlacklistedAviFile = ConfigsDir + "\\Blacklisted-Avatars.txt";
        public static string WMCFile = DependenciesDir + "\\MediaControls.exe";
        public static string WSSFile = Directory.GetParent(Application.dataPath) + "\\websocket-sharp.dll";
        public static string InstanceHistoryFile = ConfigsDir + "\\Instance-History.json";
        public static string WaypointsFile = ConfigsDir + "\\Waypoints.json";
        public static string DynamicBonesFile = ConfigsDir + "\\Global-Dynamic-Bones.json";
        public static string AccountSaverFile = ConfigsDir + "\\Account-Saver.txt";
        public static string PlayerLogsFile = LogsDir + "\\PlayerLogs.txt";
        public static string WorldLogsFile = LogsDir + "\\WorldLogs.txt";
        public static string AvatarLogsFile = LogsDir + "\\AvatarLogs.txt";
        public static string EZRipToolFile = EZRipDir + "\\AssetRipperConsole.exe";
        public static string EZRipZipFile = EZRipDir + "\\AssetRipperConsole.zip";
        public static string EventLockFile = ConfigsDir + "\\EventLocks.json";

        public static void Initialize()
        {
            Directory.CreateDirectory(MainDir);
            Directory.CreateDirectory(LogsDir);
            Directory.CreateDirectory(ConfigsDir);
            Directory.CreateDirectory(DependenciesDir);
            Directory.CreateDirectory(ExportsDir);
            Directory.CreateDirectory(ImportsDir);
            Directory.CreateDirectory(VRCADir);
            Directory.CreateDirectory(VRCWDir);
            Directory.CreateDirectory(EZRipDir);
            Directory.CreateDirectory(EZRImportsDir);
            Directory.CreateDirectory(EZRExportsDir);
        }
    }
}
