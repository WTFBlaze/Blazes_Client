using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using VRC.Core;

namespace Blaze.Modules
{
    class GameLogs : BModule
    {
        private QMNestedButton Menu;

        public override void Start()
        {
            if (!File.Exists(ModFiles.AvatarLogsFile))
            {
                FileManager.CreateFile(ModFiles.AvatarLogsFile);
                FileManager.AppendLineToFile(ModFiles.AvatarLogsFile, "Avatar Logger by WTFBlaze <3" + Environment.NewLine + Environment.NewLine);
            }

            if (!File.Exists(ModFiles.PlayerLogsFile))
            {
                FileManager.CreateFile(ModFiles.PlayerLogsFile);
                FileManager.AppendLineToFile(ModFiles.PlayerLogsFile, "Player Logger by WTFBlaze <3" + Environment.NewLine + Environment.NewLine);
            }

            if (!File.Exists(ModFiles.WorldLogsFile))
            {
                FileManager.CreateFile(ModFiles.WorldLogsFile);
                FileManager.AppendLineToFile(ModFiles.WorldLogsFile, "World Logger by WTFBlaze <3" + Environment.NewLine + Environment.NewLine);
            }
        }

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.LogSettings, "Log\nFiles", 3, 0, "Toggle what logs to the log files", "Log Files");

            new QMToggleButton(Menu, 1, 0, "Log Avatars", delegate
            {
                Config.Main.LogAvatars = true;
            }, delegate
            {
                Config.Main.LogAvatars = false;
            }, "Log Avatars you come into contact with to the AvatarLogs.txt file", Config.Main.LogAvatars);

            new QMToggleButton(Menu, 2, 0, "Log Players", delegate
            {
                Config.Main.LogPlayers = true;
            }, delegate
            {
                Config.Main.LogPlayers = false;
            }, "Log Avatars you come into contact with to the PlayerLogs.txt file", Config.Main.LogPlayers);

            new QMToggleButton(Menu, 3, 0, "Log Worlds", delegate
            {
                Config.Main.LogWorlds = true;
            }, delegate
            {
                Config.Main.LogWorlds = false;
            }, "Log Avatars you come into contact with to the WorldLogs.txt file", Config.Main.LogWorlds);
        }

        public override void AvatarIsReady(VRCPlayer vrcPlayer, ApiAvatar a)
        {
            if (Config.Main.LogAvatars)
            {
                if (!File.ReadAllText(ModFiles.AvatarLogsFile).Contains(a.id))
                {
                    StringBuilder log = new();
                    log.AppendLine("========================");
                    log.AppendLine($"Author Name: {a.authorName}");
                    log.AppendLine($"Author ID: {a.authorId}");
                    log.AppendLine("========================");
                    log.AppendLine($"Name: {a.name}");
                    log.AppendLine($"ID: {a.id}");
                    log.AppendLine($"Version: {a.id}");
                    log.AppendLine($"Release Status: {a.releaseStatus}");
                    log.AppendLine($"Featured: {a.featured}");
                    log.AppendLine($"Creation Date: {a.created_at.ToString()}");
                    log.AppendLine($"Last Updated Date: {a.updated_at.ToString()}");
                    log.AppendLine("========================");
                    log.AppendLine($"Description: {a.description}");
                    log.AppendLine("========================");
                    log.AppendLine($"Image URL: {a.imageUrl}");
                    log.AppendLine($"Download URL: {a.assetUrl}");
                    log.AppendLine($"Thumbnail URL: {a.thumbnailImageUrl}");
                    log.AppendLine("========================");
                    if (a.tags.Count != 0)
                    {
                        foreach (var t in a.tags)
                        {
                            log.AppendLine(t);
                        }
                    }
                    else
                    {
                        log.AppendLine($"No Tags!");
                    }
                    log.AppendLine("========================");
                    log.AppendLine("\n");
                    log.AppendLine("\n");
                    Task.Factory.StartNew(delegate
                    {
                        FileManager.AppendTextToFile(ModFiles.AvatarLogsFile, log.ToString());
                    });
                }
            }
        }

        public override void LocalPlayerLoaded()
        {
            if (Config.Main.LogWorlds)
            {
                var w = WorldUtils.CurrentWorld();
                if (!File.ReadAllText(ModFiles.WorldLogsFile).Contains(w.id))
                {
                    StringBuilder log = new();
                    log.AppendLine("========================");
                    log.AppendLine($"Author Name: {w.authorName}");
                    log.AppendLine($"Author ID: {w.authorId}");
                    log.AppendLine("========================");
                    log.AppendLine($"Name: {w.name}");
                    log.AppendLine($"ID: {w.id}");
                    log.AppendLine($"Capacity: {w.capacity}");
                    log.AppendLine($"Release Status: {w.releaseStatus}");
                    log.AppendLine($"Version: {w.version}");
                    log.AppendLine($"Creation Date: {w.createdAt.ToString()}");
                    log.AppendLine($"Last Update Date: {w.updated_at.ToString()}");
                    log.AppendLine("========================");
                    log.AppendLine($"Description: {(string.IsNullOrEmpty(w.description) ? "Not Set!" : w.description)}");
                    log.AppendLine("========================");
                    log.AppendLine($"Image URL: {w.imageUrl}");
                    log.AppendLine($"Download URL: {w.assetUrl}");
                    log.AppendLine($"Thumbnail URL: {w.thumbnailImageUrl}");
                    log.AppendLine("========================");
                    if (w.tags.Count != 0)
                    {
                        foreach (var t in w.tags)
                        {
                            log.AppendLine(t);
                        }
                    }
                    else
                    {
                        log.AppendLine($"No Tags!");
                    }
                    log.AppendLine("========================");
                    log.AppendLine("\n");
                    log.AppendLine("\n");
                    Task.Factory.StartNew(delegate
                    {
                        FileManager.AppendTextToFile(ModFiles.WorldLogsFile, log.ToString());
                    });
                }
            }
        }

        public override void PlayerJoined(Player player)
        {
            LogPlayer(player);
        }

        public override void PlayerLeft(Player player)
        {
            LogPlayer(player);
        }

        private void LogPlayer(Player player)
        {
            if (Config.Main.LogPlayers)
            {
                if (!File.ReadAllText(ModFiles.PlayerLogsFile).Contains(player.GetUserID()))
                {
                    var p = player.GetAPIUser();
                    StringBuilder log = new();
                    log.AppendLine("========================");
                    log.AppendLine($"Display Name: {p.displayName}");
                    log.AppendLine($"Registered Name: {p.username}");
                    log.AppendLine($"User Icon: {(string.IsNullOrEmpty(p.userIcon) ? "Not Set!" : p.userIcon)}");
                    log.AppendLine($"Developer Type: {p.developerType}");
                    log.AppendLine($"Allow Avatar Cloning: {p.allowAvatarCopying}");
                    log.AppendLine($"Status: {p.status}");
                    log.AppendLine($"Last Platform: {p.last_platform}");
                    log.AppendLine($"Is Friend: {p.isFriend}");
                    log.AppendLine("========================");
                    log.AppendLine($"Creation Date: {p.date_joined}");
                    log.AppendLine("========================");
                    foreach (var n in p.pastDisplayNames)
                    {
                        log.AppendLine(n);
                    }
                    log.AppendLine("========================");
                    if (p.bioLinks.Count != 0)
                    {
                        foreach (var l in p.bioLinks)
                        {
                            log.AppendLine(l);
                        }
                    }
                    else
                    {
                        log.AppendLine("No Links Set!");
                    }
                    log.AppendLine("========================");
                    log.AppendLine($"Bio: {(string.IsNullOrEmpty(p.bio) ? "Not Set!" : p.bio)}");
                    log.AppendLine("========================");
                    log.AppendLine($"Description: {(string.IsNullOrEmpty(p.statusDescription) ? "Not Set!" : p.statusDescription)}");
                    log.AppendLine("========================");
                    log.AppendLine($"Status: {(string.IsNullOrEmpty(p.status) ? "Not Set!" : p.status)}");
                    log.AppendLine("========================");
                    log.AppendLine("\n");
                    log.AppendLine("\n");
                    Task.Factory.StartNew(delegate
                    {
                        FileManager.AppendTextToFile(ModFiles.PlayerLogsFile, log.ToString());
                    });
                }
            }
        }
    }
}
