using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Blaze
{
    internal static class Utils
    {
        internal static IEnumerable<string> GetCommandLineArgs()
        {
            var text = Environment.CommandLine.ToLower();
            return text.Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static bool IsDevMode()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower() == "--wtfblaze.devmode");
        }

        internal static bool IsForceRun()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower() == "--wtfblaze.forcerun");
        }

        internal static void WriteToConsole(string input, ConsoleColor color)
        {
            Console.ResetColor();
            WritePrefix();
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        private static void WritePrefix()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Blaze's ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Client");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
        }

        internal static string RetrieveKey()
        {
            if (!File.Exists(Loader.ModDir + "\\AuthKey.txt"))
            {
                Console.Beep();
                Console.Beep();
                WriteToConsole("Please enter your Blaze's Client Authorization Key...", ConsoleColor.Yellow);
                var input = Console.ReadLine();
                File.Create(Loader.ModDir + "\\AuthKey.txt").Close();
                File.WriteAllText(Loader.ModDir + "\\AuthKey.txt", input.Trim());
            }
            return File.ReadAllText(Loader.ModDir + "\\AuthKey.txt").Trim();
        }

        internal static string CreateMD5(string input)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        internal static AssemblyResponse MakeAPICall(string key)
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.wtfblaze.com/v3/user/auth", JsonConvert.SerializeObject(new
            {
                HWID = CheckPermissions()
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("Authorization", key.Trim());
            unityWebRequest.SetRequestHeader("LoaderVersion", Loader.LoaderVersion);
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SetRequestHeader("User-Agent", $"BlazeClient ({Loader.LoaderVersion}, {CreateMD5($"{key}_{CheckPermissions()}")})");
            unityWebRequest.SendWebRequest();
            //WriteToConsole("Key: " + key, ConsoleColor.DarkYellow);
            //WriteToConsole("HWID: " + CheckPermissions(), ConsoleColor.DarkYellow);
            //WriteToConsole($"Plain: {key}_{CheckPermissions()}", ConsoleColor.Yellow);
            //WriteToConsole("MD5: " + CreateMD5($"{key}_{CheckPermissions()}"), ConsoleColor.Yellow);
            while (!unityWebRequest.isDone) { }

            if (string.IsNullOrWhiteSpace(unityWebRequest.error))
            {
                //Console.WriteLine(unityWebRequest.downloadHandler.text + "\n");
                var result = JsonConvert.DeserializeObject<JToken>(unityWebRequest.downloadHandler.text);
                var apiInfo = result["api_info"];
                var response = result["response"];

                if ((bool)apiInfo["Valid"])
                {
                    unityWebRequest.Dispose();
                    //Loader.AccessLevel = (string)response["level"];
                    Loader.Hash = (string)response["hash"];
                    Loader.AuthKey = key;
                    return AssemblyResponse.OK(Convert.FromBase64String((string)response["mod"]), (string)response["message"]);
                }
                unityWebRequest.Dispose();
                return AssemblyResponse.Error((string)response["message"]);
            }
            var result2 = JsonConvert.DeserializeObject<JToken>(unityWebRequest.downloadHandler.text);
            var response2 = result2["response"];
            return AssemblyResponse.Error((string)response2["message"]);
        }

        internal static string CheckPermissions()
        {
            const string location = @"SOFTWARE\Microsoft\Cryptography";
            const string name = "MachineGuid";
            using var localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var rk = localMachineX64View.OpenSubKey(location);
            if (rk == null)
            {
                throw new KeyNotFoundException($"Key Not Found: {location}");
            }
            var machineGuid = rk.GetValue(name);
            if (machineGuid == null)
            {
                throw new IndexOutOfRangeException($"Index Not Found: {name}");
            }
            return machineGuid.ToString();
        }
    }
}
