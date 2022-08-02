using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.IO;

namespace Blaze.Modules
{
    public class AvatarBlacklist : BModule
    {
        public static string[] blockFile;
        public static List<string> blockList;

        public override void Start()
        {
            if (!File.Exists(ModFiles.BlacklistedAviFile))
            {
                FileManager.CreateFile(ModFiles.BlacklistedAviFile);
            }
            UpdateFiles();
        }

        public override void UI()
        {
            new QMSingleButton(BlazeQM.Selected, 2, 1, "Blacklist\nAvatar", delegate
            {
                AddOrRemoveFromList(Main.Target.GetPCAvatar().id);
            }, "Add the user's avatar to your avatar blacklist");

            new QMSingleButton(BlazeQM.Security, 1, 2, "Add Avi\nTo <size=28>Blacklist</size>", delegate
            {
                PopupUtils.InputPopup("Blacklist Avi", "Enter Avatar ID...", delegate (string s)
                {
                    if (!RegexManager.IsValidAvatarID(s))
                    {
                        PopupUtils.InformationAlert("Please provide a valid avatar id!");
                        return;
                    }
                    if (blockList.Contains(s))
                    {
                        PopupUtils.InformationAlert("That avatar is already on your avatar blacklist!");
                    }
                    else
                    {
                        blockList.Add(s);
                        FileManager.AppendLineToFile(ModFiles.BlacklistedAviFile, s);
                        Logs.Log($"[BLACKLIST] Successfully added ({s}) to your avatar blacklist!", ConsoleColor.Green);
                        Logs.Debug($"<color=red>[BLACKLIST]</color> Added (<color=yellow>{s}</color>) to avatar blacklist!");
                        UpdateFiles();
                    }
                });
            }, "Add an avatar to your personal blacklist by its id");
        }

        public static void AddOrRemoveFromList(string avatarID)
        {
            if (!blockList.Contains(avatarID))
            {
                blockList.Add(avatarID);
                FileManager.AppendLineToFile(ModFiles.BlacklistedAviFile, avatarID);
                Logs.Log($"[BLACKLIST] Successfully added ({avatarID}) to your avatar blacklist!", ConsoleColor.Green);
                Logs.Debug($"<color=red>[BLACKLIST]</color> Added (<color=yellow>{avatarID}</color>) to avatar blacklist!");
                UpdateFiles();
            }
            else
            {
                blockList.Remove(avatarID);
                string tempFileName = Path.GetTempFileName();
                using (StreamReader streamReader = new(ModFiles.BlacklistedAviFile))
                {
                    using StreamWriter streamWriter = new(tempFileName);
                    string text;
                    while ((text = streamReader.ReadLine()) != null)
                    {
                        if (text != avatarID)
                        {
                            streamWriter.WriteLine(text);
                        }
                    }
                }
                File.Delete(ModFiles.BlacklistedAviFile);
                File.Move(tempFileName, ModFiles.BlacklistedAviFile);
                UpdateFiles();
            }
        }

        public static void UpdateFiles()
        {
            blockFile = File.ReadAllLines(ModFiles.BlacklistedAviFile);
            blockList = new(blockFile);
        }
    }
}
