using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;

namespace Blaze.Utils.VRChat
{
    internal static class DownloadManager
    {
        internal static void DownloadVRCA(ApiAvatar avi)
        {
            if (File.Exists($"{ModFiles.VRCADir}\\{avi.name}-{avi.authorName}-{avi.version}.vrca"))
            {
                Logs.Warning("[DOWNLOADS] Avatar already downloaded");
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
                        Logs.Log("[DOWNLOADS] Downloading Avatar...");
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
                }
            }
        }

        private static void AviCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Success("[DOWNLOADS] Finished Downloading Avatar!");
        }

        internal static void DownloadVRCW(ApiWorld world)
        {
            if (File.Exists($"{ModFiles.VRCWDir}\\{world.name}-{world.authorName}-{world.version}.vrcw"))
            {
                Logs.Warning("[DOWNLOADS] World already downloaded");
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
                        Logs.Log("[DOWNLOADS] Downloading World...");
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
                }
            }
        }

        private static void WorldCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Success("[DOWNLOADS] Finished Downloading World!");
        }
    }
}
