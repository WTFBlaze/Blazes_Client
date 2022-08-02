using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace Blaze.Modules
{
    class HWIDSpoofer : BModule
    {
        private static Il2CppSystem.Object ourGeneratedHwidString;

        public override void Start()
        {
            try
            {
                var OriginalHWID = SystemInfo.deviceUniqueIdentifier;
                var random = new System.Random();

                if (!File.Exists(ModFiles.HWIDFile))
                {
                    FileManager.CreateFile(ModFiles.HWIDFile);
                }

                if (new FileInfo(ModFiles.HWIDFile).Length == 0)
                {
                    FileManager.WriteAllToFile(ModFiles.HWIDFile, KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                    {
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9),
                        random.Next(1, 9)
                    }))).Select((byte x) =>
                    {
                        return x.ToString("x2");
                    }).Aggregate((string x, string y) => x + y));
                }

                if (!Config.Main.HWIDSpoofer) return;
                var newId = FileManager.ReadAllOfFile(ModFiles.HWIDFile);

                ourGeneratedHwidString = new Il2CppSystem.Object(IL2CPP.ManagedStringToIl2Cpp(newId));

                var icallName = "UnityEngine.SystemInfo::GetDeviceUniqueIdentifier";
                var icallAddress = IL2CPP.il2cpp_resolve_icall(icallName);
                if (icallAddress == IntPtr.Zero)
                {
                    Logs.Error("[Security] Can't resolve the icall, not patching HWID");
                    return;
                }

                unsafe
                {
                    CompatHook((IntPtr)(&icallAddress),
                            typeof(HWIDSpoofer).GetMethod(nameof(GetDeviceIdPatch),
                                BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                }

                Logs.Log("========[HWID Spoofer]========", ConsoleColor.White);
                Logs.Log($"OLD HWID: {OriginalHWID}", ConsoleColor.Magenta);
                Logs.Log($"NEW HWID: {newId}", ConsoleColor.Cyan);
                if (SystemInfo.deviceUniqueIdentifier == newId)
                    Logs.Log("UNITY: Spoofed!", ConsoleColor.Green);
                else
                    Logs.Log("UNITY: Failed!", ConsoleColor.Red);
                if (VRC.Core.API.DeviceID == newId)
                    Logs.Log("VRC: Spoofed!", ConsoleColor.Green);
                else
                    Logs.Log("VRC: Failed!", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.White);
            }
            catch (Exception e)
            {
                Logs.Error("[Security] Error Spoofing HWID! | " + e.Message);
            }
        }

        private static IntPtr GetDeviceIdPatch() => ourGeneratedHwidString.Pointer;

        private static void CompatHook(IntPtr first, IntPtr second)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            typeof(Imports).GetMethod("Hook", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!
#pragma warning restore CS0618 // Type or member is obsolete
                .Invoke(null, new object[] { first, second });
        }
    }
}
