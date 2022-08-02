using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using VRC.Core;

namespace Blaze.Modules
{
    class EZRip : BModule
    {
        private static readonly ProcessStartInfo startInfo = new(ModFiles.EZRipToolFile)
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        private static readonly Process process = new()
        {
            EnableRaisingEvents = true,
            StartInfo = startInfo
        };

        public override void Start()
        {
            if (!File.Exists(ModFiles.EZRipToolFile))
            {
                Logs.Log("[EZRIP] EZRip Tool not found! Retrieving...", ConsoleColor.Yellow);
                try
                {
                    WebClient wc = new();
                    var bytes = wc.DownloadData("https://cdn.wtfblaze.com/downloads/AssetRipperConsole.zip");
                    wc.Dispose();
                    if (bytes.Length > 0)
                    {
                        FileManager.CreateFile(ModFiles.EZRipZipFile);
                        FileManager.WriteAllBytesToFile(ModFiles.EZRipZipFile, bytes);
                        ZipFile.ExtractToDirectory(ModFiles.EZRipZipFile, ModFiles.EZRipDir);
                        FileManager.DeleteFile(ModFiles.EZRipZipFile);
                        Logs.Success("[EZRIP] Successfully downloaded EZRip Tool!");
                    }
                    else
                    {
                        Logs.Error("[EZRIP] There was an error downloading EZRip Tool! If problems persist please report this error to the discord.");
                        return;
                    }
                }
                catch {}
            }
            process.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(Process_ErrorDataReceived);
            process.Exited += Process_Exited;
            
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data + "\n");
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data + "\n");
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            //Logs.Log("Exit Code: " + process.ExitCode);
            Logs.Success("[EZRIP] Successfully extracted avatar!");
        }

        public override void QuickMenuUI()
        {
            new QMSingleButton(BlazeMenu.Selected, 1, 3, "EZRip\nAvatar", delegate
            {
                ProcessRip(BlazeInfo.SelectedPlayer.prop_ApiAvatar_0);
            }, "Click to download & immediately extract the VRCA");
        }

        public static void ProcessRip(ApiAvatar avi)
        {
            var vrcaFolderFile = ModFiles.VRCADir + $"\\{avi.name}-{avi.authorName}-{avi.version}.vrca";
            var fileName = $"{avi.name}-{avi.authorName}-{avi.version}.vrca";
            var ezripFile = ModFiles.EZRImportsDir + $"\\{avi.name}-{avi.authorName}-{avi.version}.vrca";
            var properName = fileName.Replace(' ', '-');
            var properEZRipFile = ModFiles.EZRImportsDir + "\\" + properName; 
            if (File.Exists(vrcaFolderFile))
            {
                // Delete any Proper Named Copies inside the EZRip Folder
                if (File.Exists(properEZRipFile))
                {
                    File.Delete(properEZRipFile);
                }

                // Delete existing rip folder with the name
                if (Directory.Exists($"{ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca"))
                {
                    Directory.Delete($"{ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca");
                }

                // Move the VRCA File to the Imports folder if it doesn't already exist
                if (!File.Exists(ezripFile))
                {
                    FileManager.CopyFile(vrcaFolderFile, ezripFile);
                }

                // If the VRCA File is there but not properly renamed then rename it
                if (File.Exists(ezripFile))
                {
                    FileManager.RenameFile(ezripFile, properEZRipFile);
                }

                // Start the extraction
                //Process.Start(ModFiles.EZRipToolFile, $"{ModFiles.EZRImportsDir}\\{properName} -o Exports\\{avi.name.Replace(' ', '-')}.vrca -q");
                startInfo.Arguments = $"{ModFiles.EZRImportsDir}\\{properName} -o {ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca --quit";
                process.Start();
                Functions.Delay(delegate
                {
                    process.Kill();
                }, 5f);
            }
            else
            {
                DownloadManager.DownloadVRCA(avi);
                Functions.Delay(delegate
                {
                    if (File.Exists(vrcaFolderFile))
                    {
                        // Delete any Proper Named Copies inside the EZRip Folder
                        if (File.Exists(properEZRipFile))
                        {
                            File.Delete(properEZRipFile);
                        }

                        // Delete existing rip folder with the name
                        if (Directory.Exists($"{ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca"))
                        {
                            Directory.Delete($"{ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca");
                        }

                        // Move the VRCA File to the Imports folder if it doesn't already exist
                        if (!File.Exists(ezripFile))
                        {
                            FileManager.CopyFile(vrcaFolderFile, ezripFile);
                        }

                        // If the VRCA File is there but not properly renamed then rename it
                        if (File.Exists(ezripFile))
                        {
                            FileManager.RenameFile(ezripFile, properEZRipFile);
                        }

                        // Start the extraction
                        //Process.Start(ModFiles.EZRipToolFile, $"{ModFiles.EZRImportsDir}\\{properName} -o Exports\\{avi.name.Replace(' ', '-')}.vrca -q");
                        startInfo.Arguments = $"{ModFiles.EZRImportsDir}\\{properName} -o {ModFiles.EZRExportsDir}\\{avi.name.Replace(' ', '-')}.vrca --quit";
                        process.Start();
                        Logs.Log($"[EZRIP] Started Extracting {avi.name} by {avi.authorName}...", ConsoleColor.Yellow);
                        Logs.Debug($"<color=#89CFF0>[EZRIP]</color> Started Extracting <color=yellow>{avi.name}</color> by <color=yellow>{avi.authorName}</color>");
                        if (Config.Main.LogToHud)
                        {
                            Logs.HUD("<color=#89CFF0>[EZRIP]</color> Started Extracting!", 3.5f);
                        }
                        Functions.Delay(delegate
                        {
                            process.Kill();
                        }, 5f);
                    }
                    else
                    {
                        Logs.Error("[EZRIP] There was an error finding the original VRCA File from the DownloadManager! Aborting Extraction.");
                    }
                }, 6f);
            }
        }
    }
}
