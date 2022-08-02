using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using VRC.Core;

namespace Blaze.Utils.Managers
{
    public static class DownloadManager
    {
        public static void DownloadVRCA(ApiAvatar avi)
        {
            if (File.Exists($"{ModFiles.VRCADir}\\{avi.name}-{avi.authorName}-{avi.version}.vrca"))
            {
                Logs.Warning("[DOWNLOADS] Avatar already downloaded");
                Logs.Debug("<color=yellow>[DOWNLOADS]</color> Avatar Already Downloaded!");
            }
            else
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept", "application/zip");
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        webClient.DownloadFileCompleted += AviCompleted;
                        Logs.Log("[DOWNLOADS] Downloading Avatar...", ConsoleColor.Yellow);
                        Logs.Debug("<color=yellow>[DOWNLOADS]</color> Downloading Avatar...");
                        webClient.DownloadFileAsync(new Uri(avi.assetUrl), $"{ModFiles.VRCADir}\\{avi.name}-{avi.authorName}-{avi.version}.vrca");
                    }

                    using (var webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept", "application/zip");
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        webClient.DownloadFileAsync(new Uri(avi.imageUrl), $"{ModFiles.VRCADir}\\{avi.name}-{avi.authorName}-{avi.version}.png");
                    }
                }
                catch (Exception e)
                {
                    Logs.Error("Avatar Download Failed! | Error Message: " + e.Message);
                    Logs.Debug("<color=yellow>[DOWNLOADS]</color> Avatar Download Failed! Check Console For More Info!");
                }
            }
        }

        private static void AviCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Success("[DOWNLOADS] Finished Downloading Avatar!");
            Logs.Debug("<color=yellow>[DOWNLOADS]</color> Finished Downloading Avatar!");
        }

        public static void DownloadVRCW(ApiWorld world)
        {
            if (File.Exists($"{ModFiles.VRCWDir}\\{world.name}-{world.authorName}-{world.version}.vrcw"))
            {
                Logs.Warning("[DOWNLOADS] World already downloaded");
                Logs.Debug("<color=yellow>[DOWNLOADS]</color> World Already Downloaded!");
            }
            else
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept", "application/zip");
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        webClient.DownloadFileCompleted += WorldCompleted;
                        Logs.Log("[DOWNLOADS] Downloading World...", ConsoleColor.Yellow);
                        Logs.Debug("<color=yellow>[DOWNLOADS]</color> Downloading World...");
                        webClient.DownloadFileAsync(new Uri(world.assetUrl), $"{ModFiles.VRCWDir}\\{world.name}-{world.authorName}-{world.version}.vrcw");
                    }

                    using (var webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept", "application/zip");
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        webClient.DownloadFileAsync(new Uri(world.imageUrl), $"{ModFiles.VRCWDir}\\{world.name}-{world.authorName}-{world.version}.png");
                    }
                }
                catch (Exception e)
                {
                    Logs.Error("World Download Failed! | Error Message: " + e.Message);
                    Logs.Debug("<color=yellow>[DOWNLOADS]</color> World Download Failed! Check Console For More Info!");
                }
            }
        }

        private static void WorldCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Success("[DOWNLOADS] Finished Downloading World!");
            Logs.Debug("<color=yellow>[DOWNLOADS]</color> Finished Downloading World!");
        }
    }
}
