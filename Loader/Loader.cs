using MelonLoader;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
using WebSocketSharp;

[assembly: MelonInfo(typeof(Blaze.Loader), "Blaze's Client", "UwU\tUwU\tUwU\tUwU", "WTFBlaze")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.Blue)]

namespace Blaze
{
    public class Loader : MelonMod
    {
        internal static Type ModType { get; private set; }
        internal static bool IsDev { get; private set; }
        internal static string AuthKey { get; set; }
        internal static string Hash { get; set; }
        internal static Assembly _assembly { get; private set; }
        internal const string LoaderVersion = "1.0.2";
        internal static string ModDir = Directory.GetParent(Application.dataPath) + "\\Blaze";

        public override void OnApplicationStart()
        {
            Directory.CreateDirectory(ModDir);
            IsDev = Utils.IsDevMode();
            RetrieveMod();
        }

        public override void OnUpdate()
        {
            VRChat.OnUpdate();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            VRChat.OnSceneWasInitialized(buildIndex, sceneName);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            VRChat.OnSceneWasLoaded(buildIndex, sceneName);
        }

        private static void RetrieveMod()
        {
            if (Utils.IsForceRun())
            {
                if (!File.Exists(ModDir + "\\Resources\\Blaze's Client.dll"))
                {
                    Utils.WriteToConsole("There was an error finding the developer assembly!", ConsoleColor.Red);
                    return;
                }
                Utils.WriteToConsole("Loading Blaze's Client Developer Edition...", ConsoleColor.Yellow);
                var rawAssembly = File.ReadAllBytes(ModDir + "\\Resources\\Blaze's Client.dll");
                _assembly = Assembly.Load(rawAssembly);
            }
            else
            {
                if (IsDev)
                {
                    if (!File.Exists(ModDir + "\\Resources\\Blaze's Client.dll"))
                    {
                        Utils.WriteToConsole("There was an error finding the developer assembly!", ConsoleColor.Red);
                        return;
                    }
                    Utils.MakeAPICall(Utils.RetrieveKey());
                    Utils.WriteToConsole("Loading Blaze's Client Developer Edition...", ConsoleColor.Yellow);
                    var rawAssembly = File.ReadAllBytes(ModDir + "\\Resources\\Blaze's Client.dll");
                    _assembly = Assembly.Load(rawAssembly);
                }
                else
                {
                    var APIResponse = Utils.MakeAPICall(Utils.RetrieveKey());
                    if (APIResponse.Response != Response.Error)
                    {
                        _assembly = Assembly.Load(APIResponse.Assembly);
                    }
                    else
                    {
                        Console.Beep();
                        Console.Beep();
                        Utils.WriteToConsole("There was an error loading Blaze's Client! - Message: " + APIResponse.Message, ConsoleColor.Red);
                    }
                }
            }

            if (_assembly != null)
            {
                ModType = _assembly.GetType("Blaze.Main");
                if (Utils.IsForceRun())
                {
                    VRChat.ForceOnApplicationStart();
                }
                else
                {
                    VRChat.OnApplicationStart();
                }
            }
            else
            {
                Utils.WriteToConsole("There was an error loading Blaze's Client! - Message: (404) / _assembly is null!", ConsoleColor.Red);
            }
        }
    }
}
