using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Blaze.Modules
{
    class BlacklistedShaders : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;
        internal static string[] blockFile;
        internal static List<string> blockList;

        public override void Start()
        {
            if (!File.Exists(ModFiles.BlockedShadersFile))
            {
                FileManager.CreateFile(ModFiles.BlockedShadersFile);
            }
            UpdateFiles();
        }

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Protections, "Shader\nBlacklist", 2, 2, "Click to view all shaders you have blacklisted", "Shader Blacklist");
            Scroll = new QMScrollMenu(Menu);

            Scroll.SetAction(delegate
            {
                foreach (var s in blockList)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, s, delegate
                    {
                        blockList.Remove(s);
                        string tempFileName = Path.GetTempFileName();
                        using (StreamReader streamReader = new(ModFiles.BlockedShadersFile))
                        {
                            using StreamWriter streamWriter = new(tempFileName);
                            string text;
                            while ((text = streamReader.ReadLine()) != null)
                            {
                                if (text != s)
                                {
                                    streamWriter.WriteLine(text);
                                }
                            }
                        }
                        File.Delete(ModFiles.BlockedShadersFile);
                        File.Move(tempFileName, ModFiles.BlockedShadersFile);
                        UpdateFiles();
                        Scroll.Refresh();
                    }, "Click to remove this shader from your blacklist"));
                }
            });

            new QMSingleButton(Menu, 4, 0, "Add\nShader", delegate
            {
                PopupUtils.InputPopup("Blacklist", "Enter Shader Name Here...", delegate (string s)
                {
                    if (blockList.Contains(s.ToLower()))
                    {
                        PopupUtils.InformationAlert("That shader is already blacklisted!");
                        return;
                    }
                    blockList.Add(s);
                    FileManager.AppendLineToFile(ModFiles.BlockedShadersFile, s);
                    Logs.Log($"[BLACKLIST] Successfully added ({s}) to your shader blacklist!", ConsoleColor.Green);
                    Logs.Debug($"<color=red>[BLACKLIST]</color> Added (<color=yellow>{s}</color>) to shaders blacklist!");
                    UpdateFiles();
                    Scroll.Refresh();
                });
            }, "Click to input a shader name");
        }

        internal static void AddOrRemoveFromList(string shader)
        {
            if (!blockList.Contains(shader))
            {
                blockList.Add(shader);
                FileManager.AppendLineToFile(ModFiles.BlockedShadersFile, shader);
                Logs.Log($"[BLACKLIST] Successfully added ({shader}) to your shader blacklist!", ConsoleColor.Green);
                Logs.Debug($"<color=red>[BLACKLIST]</color> Added (<color=yellow>{shader}</color>) to shaders blacklist!");
                UpdateFiles();
            }
            else
            {
                blockList.Remove(shader);
                string tempFileName = Path.GetTempFileName();
                using (StreamReader streamReader = new(ModFiles.BlockedShadersFile))
                {
                    using StreamWriter streamWriter = new(tempFileName);
                    string text;
                    while ((text = streamReader.ReadLine()) != null)
                    {
                        if (text != shader)
                        {
                            streamWriter.WriteLine(text);
                        }
                    }
                }
                File.Delete(ModFiles.BlockedShadersFile);
                File.Move(tempFileName, ModFiles.BlockedShadersFile);
                UpdateFiles();
            }
        }

        internal static void UpdateFiles()
        {
            blockFile = File.ReadAllLines(ModFiles.BlockedShadersFile);
            blockList = new(blockFile);
        }
    }
}
