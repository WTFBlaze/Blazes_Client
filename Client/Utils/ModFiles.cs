using System;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;

namespace Blaze.Utils
{
    internal static class ModFiles
    {
        internal static string MainDir = Directory.GetParent(Application.dataPath) + "\\Blaze";
        internal static string MLDir = Directory.GetParent(Application.dataPath) + "\\MelonLoader";
        internal static string LogsDir = MainDir + "\\Logs";
        internal static string ConfigsDir = MainDir + "\\Configs";
        internal static string DependenciesDir = MainDir + "\\Dependencies";
        internal static string ExportsDir = MainDir + "\\Exports";
        internal static string ImportsDir = MainDir + "\\Imports";
        internal static string VRCADir = MainDir + "\\VRCA";
        internal static string VRCWDir = MainDir + "\\VRCW";
        internal static string EZRipDir = MainDir + "\\EZRip";
        internal static string EZRExportsDir = EZRipDir + "\\Exports";
        internal static string EZRImportsDir = EZRipDir + "\\Imports";

        internal static string ConfigFile = ConfigsDir + "\\Config.json";
        internal static string HWIDFile = ConfigsDir + "\\HWID.txt";
        internal static string KOSFile = ConfigsDir + "\\PersonalKOS.json";
        internal static string KeybindsFile = ConfigsDir + "\\Keybinds.json";
        internal static string DiscordRPCFile = DependenciesDir + "\\DiscordRPC.dll";
        internal static string FriendsExportFile = ExportsDir + "\\Exported-Friends.txt";
        internal static string FriendsImportFile = ImportsDir + "\\Exported-Friends.txt";
        internal static string AviFavFile = ConfigsDir + "\\Avatar-Favorites.json";
        internal static string AntiCrashFile = ConfigsDir + "\\Anti-Crash.json";
        internal static string BlockedShadersFile = ConfigsDir + "\\Blacklisted-Shaders.txt";
        internal static string LocalBlockFile = ConfigsDir + "\\LocalBlocks.json";
        internal static string LNCImportFile = ImportsDir + "\\Late-Night-Avi-Favs.json";
        internal static string ApolloImportFile = ImportsDir + "\\Apollo-Avi-Favs.json";
        internal static string BlacklistedAviFile = ConfigsDir + "\\Blacklisted-Avatars.txt";
        internal static string WMCFile = DependenciesDir + "\\MediaControls.exe";
        internal static string WSSFile = Directory.GetParent(Application.dataPath) + "\\websocket-sharp.dll";
        internal static string InstanceHistoryFile = ConfigsDir + "\\Instance-History.json";
        internal static string WaypointsFile = ConfigsDir + "\\Waypoints.json";
        internal static string DynamicBonesFile = ConfigsDir + "\\Global-Dynamic-Bones.json";
        internal static string AccountSaverFile = ConfigsDir + "\\Account-Saver.txt";
        internal static string PlayerLogsFile = LogsDir + "\\PlayerLogs.txt";
        internal static string WorldLogsFile = LogsDir + "\\WorldLogs.txt";
        internal static string AvatarLogsFile = LogsDir + "\\AvatarLogs.txt";
        internal static string EZRipToolFile = EZRipDir + "\\AssetRipperConsole.exe";
        internal static string EZRipZipFile = EZRipDir + "\\AssetRipperConsole.zip";

        internal static void Initialize()
        {
            Directory.CreateDirectory(MainDir);
            Directory.CreateDirectory(LogsDir);
            Directory.CreateDirectory(DependenciesDir);
            Directory.CreateDirectory(ImportsDir);
            Directory.CreateDirectory(ExportsDir);
            Directory.CreateDirectory(VRCADir);
            Directory.CreateDirectory(VRCWDir);
            Directory.CreateDirectory(EZRExportsDir);
            Directory.CreateDirectory(EZRipDir);
            Directory.CreateDirectory(EZRImportsDir);
        }
    }
}
