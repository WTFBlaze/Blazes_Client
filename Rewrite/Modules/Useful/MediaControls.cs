using Blaze.API;
using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using static Blaze.Utils.Objects.MediaObjects;

namespace Blaze.Modules
{
    public class MediaControls : BModule
    {
        private static string LastLoggedSong = string.Empty;

        #region WMC Functions
        private static readonly ProcessStartInfo startInfo = new(ModFiles.WMCFile)
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8
        };

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte keybd_byte1, byte keybd_byte2, uint keybd_uint, IntPtr keybd_intptr);
        private static readonly Process wmcProcess = new() { StartInfo = startInfo };
        public static Thread wmcThread;
        private static readonly ConcurrentQueue<int> queue = new();
        private static readonly ConcurrentQueue<string> queue2 = new();
        private static Dictionary<Source, float> CachedSources = new();
        public static List<CurrentStatus> currentSources = new() { new CurrentStatus() };
        public static int currentlyViewing = 0;
        private static readonly float defaultValue = 1f;

        private static readonly Dictionary<Commands, byte> CommandsToBytes = new()
        {
            {
                Commands.Play,
                179
            },
            {
                Commands.Pause,
                179
            },
            {
                Commands.Stop,
                178
            },
            {
                Commands.SkipPrevious,
                177
            },
            {
                Commands.SkipNext,
                176
            }
        };

        public override void Update()
        {
            if (Thumbnail != null)
            {
                try
                {
                    ProcessThumbnail();
                }
                catch (Exception ex)
                {
                    Logs.Error("ImageProcessor.ProcessMainThreadJobs() - 1", ex);
                }
            }
            /*QMSingleButton pictureForeground = Thumbnail;
            bool flag;
            if (pictureForeground == null) { flag = false; }
            else
            {
                UnityEngine.UI.Image uiImage = pictureForeground.GetBackgroundImage();
                bool? flag2;
                if (uiImage == null)
                {
                    flag2 = null;
                }
                else
                {
                    GameObject gameObject = uiImage.gameObject;
                    flag2 = (gameObject != null) ? new bool?(gameObject.active) : null;
                }
                bool? flag3 = flag2;
                flag = flag3.GetValueOrDefault() & flag3 != null;
            }
            if (flag)
            {
                try
                {
                    ProcessThumbnail();
                }
                catch (Exception ex)
                {
                    Logs.Error("ImageProcessor.ProcessMainThreadJobs() - 1", ex);
                }
                try
                {
                    //_TextTitle.UpdateScrolling();
                    //_TextArtist.UpdateScrolling();
                    //RMCQuickMenuProvider._TextSource.UpdateScrolling();
                }
                catch (Exception ex3)
                {
                    Logs.Error("ImageProcessor.ProcessMainThreadJobs()", ex3);
                }
            }*/
            Dequeue();
            CheckWMCProcess();
            if (CachedSources.Count > 0)
            {
                List<Source> list = new();
                float deltaTime = Time.deltaTime;
                foreach (Source source in CachedSources.Keys.ToList())
                {
                    CachedSources[source] = CachedSources[source] - deltaTime;
                    if (CachedSources[source] < 0f)
                    {
                        list.Add(source);
                    }
                }
                foreach (Source source2 in list)
                {
                    RefreshSources(source2);
                    CheckIfSourceIsAdded(source2);
                }
            }
        }

        public static void StartWMC()
        {
            try
            {
                if (File.Exists(ModFiles.WMCFile))
                {
                    Logs.Log("[MediaController] Started!", ConsoleColor.Green);
                    wmcProcess.Start();
                    wmcThread = new Thread(new ThreadStart(ProcessLines));
                    wmcThread.Start();
                }
            }
            catch (Exception arg_InvalidOperationException)
            {
                Logs.Error("WMC Start", arg_InvalidOperationException);
            }
        }

        public static void CheckWMCProcess()
        {
            if (wmcThread != null && !wmcThread.IsAlive && queue.Count != 0 && queue.TryDequeue(out int num))
            {
                Logs.Warning("[MediaController] WMC Closed/Crashed. Please use the button in the QuickMenu to restart.");
            }
        }

        private static void ProcessLines()
        {
            IntPtr setCurrentToken = IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
            while (!wmcProcess.HasExited)
            {
                try
                {
                    string item = wmcProcess.StandardOutput.ReadLine();
                    queue2.Enqueue(item);
                }
                catch (Exception ex)
                {
                    if (!wmcProcess.HasExited)
                    {
                        Logs.Error("WMC Read", ex);
                        Thread.Sleep(1000);
                    }
                }
            }
            Logs.Warning("[MediaController] WMC Exited! Exit Code: " + wmcProcess.ExitCode.ToString());
            queue.Enqueue(wmcProcess.ExitCode);
            IL2CPP.il2cpp_thread_detach(setCurrentToken);
        }

        public static void SendRequest(PlaybackChangeRequest Win32GetHelper)
        {
            wmcProcess.StandardInput.WriteLine(Win32GetHelper.ToString());
        }

        public static void Dequeue()
        {
            if (queue2.Count != 0 && queue2.TryDequeue(out string message))
            {
                ProcessInputs(message);
            }
        }

        private static void RefreshSources(Source RestoreLock)
        {
            int num = CheckForSource(RestoreLock);
            if (num != -1)
            {
                currentSources.RemoveAt(num);
                bool flag = false;
                if (num <= currentlyViewing || num == currentlyViewing)
                {
                    currentlyViewing--;
                    flag = num == currentlyViewing + 1;
                }
                UpdateUI(true, flag, flag);
            }
        }

        private static void ProcessInputs(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            string[] array = message.Split(new char[] { '|' }, 2);
            if (array.Length != 2)
            {
                Logs.Warning("Unkown Communication From WMC\n" + message);
                return;
            }
            string text = array[0];
            if (text != null)
            {
                if (text == "SourceChange")
                {
                    ProcessSourceChange(JsonConvert.DeserializeObject<SourceChange>(array[1]));
                    return;
                }
                if (text == "SongChange")
                {
                    //Logs.Log(message);
                    ProcessSongChange(JsonConvert.DeserializeObject<SongChange>(array[1]));
                    return;
                }
                if (text != "PlaybackChange") return;
                ProcessPlaybackChanged(JsonConvert.DeserializeObject<PlaybackChange>(array[1]));
            }
        }

        public static void ProcessSourceChange(SourceChange sourceChanged)
        {
            try
            {
                /*string str = "SRC:\t";
                string str2;
                if (sourceChanged == null)
                {
                    str2 = null;
                }
                else
                {
                    Source source = sourceChanged.Source;
                    str2 = (source?.Name);
                }
                Logs.Log(str + str2 + "\t" + (sourceChanged?.SourceState));*/
                int num = CheckForSource(sourceChanged.Source);
                if (num == -1)
                {
                    if (sourceChanged.SourceState == "New")
                    {
                        currentSources.Add(new CurrentStatus(sourceChanged));
                        UpdateUI(true, false, false);
                    }
                }
                else if (sourceChanged.SourceState == "New")
                {
                    CheckIfSourceIsAdded(sourceChanged.Source);
                    currentSources[num].SourceChange(sourceChanged);
                    if (currentlyViewing == num)
                    {
                        UpdateUI(false, true, true);
                    }
                }
                else
                {
                    CachedSources.Add(sourceChanged.Source, defaultValue);
                }
            }
            catch (Exception ex)
            {
                Logs.Error("OnSourceChange", ex);
            }
        }

        public static void ProcessSongChange(SongChange song)
        {
            try
            {
                //string str = "SNG:\t";
                string str2;
                if (song == null)
                {
                    str2 = null;
                }
                else
                {
                    Source source = song.Source;
                    str2 = (source?.Name);
                }
                //Logs.Log(str + str2 + "\t" + (song?.Title));
                if (LastLoggedSong != song.Title)
                {
                    LastLoggedSong = song.Title;
                    var title = str2;
                    if (title.EndsWith(".exe"))
                    {
                        title = str2.Substring(0, str2.LastIndexOf(".exe"));
                    }
                    else
                    {
                        title = str2;
                    }
                    Logs.Debug($"<color=#e134eb>[Media Controls]</color> {title} now playing <color=yellow>{song.Title}</color>");
                    if (Config.Main.LogToHud)
                    {
                        Logs.HUD($"<color=#e134eb>[MC]</color> {title} -> <color=yellow>{song.Title}</color>", 3.5f);
                    }
                }
                int num = CheckForSource(song.Source);
                if (num == -1)
                {
                    Logs.Warning("OnSongChange Source wasn't communicated");
                }
                else
                {
                    CheckIfSourceIsAdded(song.Source);
                    currentSources[num].SongChange(song);
                    if (currentlyViewing == num)
                    {
                        UpdateUI(false, true, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Error("OnSongChange", ex);
            }
        }

        public static void ProcessPlaybackChanged(PlaybackChange playback)
        {
            try
            {
                //string str = "PLY:\t";
                string str2;
                if (playback == null) { str2 = null; }
                else
                {
                    Source source = playback.Source;
                    str2 = source?.Name;
                }
                //Logs.Log(str + str2 + "\t" + (playback?.PlaybackState));
                var title = str2;
                if (title.EndsWith(".exe"))
                {
                    title = str2.Substring(0, str2.LastIndexOf(".exe"));
                }
                else
                {
                    title = str2;
                }
                Logs.Debug($"<color=#e134eb>[Media Controls]</color> {title} is now <color=yellow>{playback.PlaybackState}</color>");
                if (Config.Main.LogToHud)
                {
                    Logs.HUD($"<color=#e134eb>[MC]</color> {title} -> <color=yellow>{playback.PlaybackState}</color>", 3.5f);
                }
                int num = CheckForSource(playback.Source);
                if (num == -1)
                {
                    Logs.Warning("OnPlaybackChange Source wasn't communicated");
                }
                else
                {
                    CheckIfSourceIsAdded(playback.Source);
                    currentSources[num].PlaybackChange(playback);
                    if (currentlyViewing == num)
                    {
                        UpdateUI(false, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Error("OnPlaybackChange", ex);
            }
        }

        private static void CheckIfSourceIsAdded(Source source)
        {
            if (CachedSources.ContainsKey(source))
            {
                CachedSources.Remove(source);
            }
        }

        private static int CheckForSource(Source source)
        {
            for (int i = 0; i < currentSources.Count; i++)
            {
                if (source.Name == currentSources[i].Source.Name)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void MaxWindowSize()
        {
            for (int i = currentSources.Count - 1; i > 0; i--)
            {
                currentSources.RemoveAt(i);
            }
            currentlyViewing = 0;
            UpdateUI(true, true, true);
        }

        public static void ChangeSource(int value)
        {
            try
            {
                int num = currentlyViewing + value;
                if (num < currentSources.Count && num >= 0)
                {
                    currentlyViewing = num;
                    UpdateUI(true, true, true);
                }
                else
                {
                    Logs.Warning("ChangeSources invalid move");
                }
            }
            catch (Exception ex)
            {
                Logs.Error("ChangeSources", ex);
            }
        }

        public static void ProcessPlayerCommands(Commands command)
        {
            try
            {
                if (currentlyViewing == 0)
                {
                    keybd_event(CommandsToBytes[command], 0, 1U, IntPtr.Zero);
                }
                else
                {
                    SendRequest(new PlaybackChangeRequest(currentSources[currentlyViewing].Source.Name, command));
                }
            }
            catch (Exception ex)
            {
                Logs.Error("ChangePlayback", ex);
            }
        }
        #endregion

        #region WMC UI
        private static GameObject Panel;
        private static GameObject SourceNameObj;
        private static TextMeshProUGUI SourceNameText;
        private static GameObject TitleObj;
        private static TextMeshProUGUI TitleText;
        private static GameObject ArtistObj;
        private static TextMeshProUGUI ArtistText;

        private static QMSingleButton PrevSource;
        private static QMSingleButton NextSource;
        private static QMSingleButton PrevSong;
        private static QMSingleButton NextSong;
        private static QMSingleButton PauseSong;
        private static QMSingleButton PlaySong;
        private static QMSingleButton Thumbnail;
        private static readonly Dictionary<string, string> OtherMusicSoftware = new()
        {
            {
                "Microsoft.ZuneMusic",
                "Groove Music"
            }
        };

        public override void Start()
        {
            if (!File.Exists(ModFiles.WMCFile))
            {
                Logs.Log("[MediaControls] WMC File not found! Retrieving...", ConsoleColor.Yellow);
                try
                {
                    WebClient wc = new();
                    var bytes = wc.DownloadData("https://cdn.wtfblaze.com/downloads/MediaControls.exe");
                    wc.Dispose();
                    if (bytes.Length > 0)
                    {
                        FileManager.CreateFile(ModFiles.WMCFile);
                        FileManager.WriteAllBytesToFile(ModFiles.WMCFile, bytes);
                        Logs.Success("[MediaControls] Successfully downloaded Blaze's Media Controller Tool!");
                    }
                    else
                    {
                        Logs.Error("[MediaControls] There was an error downloading Blaze's Media Controller Tool! If problems persist please report this error to the discord.");
                        return;
                    }
                }
                catch { }
            }
        }

        public override void UI()
        {
            InitializeUI();
            UpdateUI(true, true, true);
            StartWMC();
        }

        public static void InitializeUI()
        {
            if (!File.Exists(ModFiles.WMCFile)) return;
            APIStuff.GetQuickMenuInstance().gameObject.transform.Find("Container/Window").gameObject.GetComponent<BoxCollider>().size = new Vector3(1024, 1500, 1);
            var obj = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel");
            Panel = UnityEngine.Object.Instantiate(obj, obj.transform.parent.parent, false);
            Panel.name = "Blaze's Media Controller";
            Panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(420, 1120);
            UnityEngine.Object.Destroy(Panel.GetComponent<DebugInfoPanel>());
            var background = Panel.transform.Find("Panel").gameObject;
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 170);

            // Source Name
            SourceNameObj = background.transform.Find("Text_FPS").gameObject;
            SourceNameObj.name = "Source Title";
            SourceNameObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-280, 105);
            SourceNameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 0);
            SourceNameText = SourceNameObj.GetComponent<TextMeshProUGUI>();
            SourceNameText.alignment = TextAlignmentOptions.Left;
            SourceNameText.text = "<color=white><b>Source Name</b></color>";

            // Song Title
            TitleObj = background.transform.Find("Text_Ping").gameObject;
            TitleObj.name = "Song Title";
            TitleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 55);
            //TitleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-266, 55);
            //TitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 0);
            TitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 0);
            TitleText = TitleObj.GetComponent<TextMeshProUGUI>();
            TitleText.alignment = TextAlignmentOptions.Left;
            TitleText.text = "<color=white><b>Song Title</b></color>";
            TitleText.fontSize = 30;
            TitleText.fontSizeMin = 5;
            TitleText.fontSizeMax = 30;
            TitleText.enableAutoSizing = true;
            TitleText.enableWordWrapping = false;

            // Song Artist
            ArtistObj = UnityEngine.Object.Instantiate(TitleObj, TitleObj.transform.parent, false);
            ArtistObj.name = "Song Artist";
            ArtistObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-117, 15);
            //ArtistObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-265, 15);
            //ArtistObj.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 0);
            ArtistObj.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 0);
            ArtistText = ArtistObj.GetComponent<TextMeshProUGUI>();
            ArtistText.alignment = TextAlignmentOptions.Left;
            ArtistText.text = "<color=white><b>Song Artist</b></color>";
            ArtistText.fontSize = 25;
            ArtistText.fontSizeMin = 5;
            ArtistText.fontSizeMax = 25;
            ArtistText.enableAutoSizing = true;
            ArtistText.enableWordWrapping = false;

            Thumbnail = new QMSingleButton("Menu_Dashboard", 0, 0, "", delegate { }, "");
            Thumbnail.GetGameObject().transform.SetParent(background.transform);
            Thumbnail.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
            Thumbnail.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(85, 85);
            Thumbnail.SetBackgroundImage(AssetBundleManager.FallbackThumbnail);
            Thumbnail.GetGameObject().GetComponent<StyleElement>().enabled = false;
            Thumbnail.GetGameObject().GetComponent<Button>().enabled = false;

            PrevSource = new QMSingleButton("Menu_Dashboard", 0, 0, "<", delegate { ChangeSource(-1); }, "Click to go to the previous source");
            PrevSource.GetGameObject().transform.SetParent(background.transform);
            PrevSource.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
            PrevSource.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(405, 205);
            PrevSource.GetGameObject().transform.Find("Text_H4").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);

            NextSource = new QMSingleButton("Menu_Dashboard", 0, 0, ">", delegate { ChangeSource(1); }, "Click to go to the next source");
            NextSource.GetGameObject().transform.SetParent(background.transform);
            NextSource.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
            NextSource.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(535, 205);
            NextSource.GetGameObject().transform.Find("Text_H4").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);

            PrevSong = new QMSingleButton("Menu_Dashboard", 0, 0, "", delegate { ProcessPlayerCommands(Commands.SkipPrevious); }, "Click to go to the previous song");
            PrevSong.SetBackgroundImage(AssetBundleManager.BackArrow);
            PrevSong.GetGameObject().transform.SetParent(background.transform);
            PrevSong.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(75, 73);
            PrevSong.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 40);

            NextSong = new QMSingleButton("Menu_Dashboard", 0, 0, "", delegate { ProcessPlayerCommands(Commands.SkipNext); }, "Click to go to the next song");
            NextSong.SetBackgroundImage(AssetBundleManager.NextArrow);
            NextSong.GetGameObject().transform.SetParent(background.transform);
            NextSong.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(75, 73);
            NextSong.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(370, 40);

            PauseSong = new QMSingleButton("Menu_Dashboard", 0, 0, "", delegate { ProcessPlayerCommands(Commands.Pause); }, "Click to pause the current song");
            PauseSong.SetBackgroundImage(AssetBundleManager.Pause);
            PauseSong.GetGameObject().transform.SetParent(background.transform);
            PauseSong.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(75, 73);
            PauseSong.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(285, 40);

            PlaySong = new QMSingleButton("Menu_Dashboard", 0, 0, "", delegate { ProcessPlayerCommands(Commands.Play); }, "Click to play the current song");
            PlaySong.SetBackgroundImage(AssetBundleManager.NextArrow);
            PlaySong.GetGameObject().transform.SetParent(background.transform);
            PlaySong.GetGameObject().GetComponent<RectTransform>().sizeDelta = new Vector2(75, 73);
            PlaySong.GetGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(285, 40);
        }

        public static void UpdateUI(bool AddMetadata, bool ReadBuffer, bool UpdateIOPadBuffers)
        {
            if (AddMetadata)
            {
                PrevSource.SetInteractable(currentlyViewing != 0);
                NextSource.SetInteractable(currentlyViewing != currentSources.Count - 1);
            }
            if (ReadBuffer || UpdateIOPadBuffers)
            {
                CurrentStatus currentStatus = currentSources[currentlyViewing];
                if (ReadBuffer)
                {
                    TitleText.text = currentStatus.Title.ToUpper();
                    ArtistText.text = currentStatus.Artist.ToUpper();
                    string text = null;
                    foreach (KeyValuePair<string, string> keyValuePair in OtherMusicSoftware)
                    {
                        if (currentStatus.Source.Name.Contains(keyValuePair.Key))
                        {
                            text = keyValuePair.Value;
                            break;
                        }
                    }
                    if (text == null)
                    {
                        text = currentStatus.Source.Name.ToUpper();
                        if (text.EndsWith(".EXE"))
                        {
                            text = currentStatus.Source.Name.Substring(0, text.LastIndexOf(".EXE"));
                        }
                        else
                        {
                            text = currentStatus.Source.Name;
                        }
                    }
                    SourceNameText.text = text.ToUpper();
                    ConvertImage(currentStatus.Thumbnail);
                }
                if (UpdateIOPadBuffers)
                {
                    if (currentStatus.Source.Controls != null)
                    {
                        if (currentStatus.Source.Controls.IsPlayEnabled)
                        {
                            PlaySong.SetActive(true);
                            PauseSong.SetActive(false);
                        }
                        else if (currentStatus.Source.Controls.IsPauseEnabled)
                        {
                            PauseSong.SetActive(true);
                            PlaySong.SetActive(false);
                        }
                        else
                        {
                            PlaySong.SetActive(true);
                            PauseSong.SetActive(false);
                        }
                        return;
                    }
                    PlaySong.SetActive(false);
                    PauseSong.SetActive(false);
                }
            }
        }
        #endregion

        #region Other Shit

        private static MultiStageLoadingJob currentJob;
        private static byte[] cachedBytes;
        public static int[] imageSize = new int[2] { 200, 200 };

        public enum LoadingStep
        {
            CREATE_TEXTURE,
            COPY_PIXELS,
            APPLY,
            SET_IMAGE,
            COMPLETE
        }

        public class MultiStageLoadingJob
        {
            public Texture2D texture2D;
            public LoadingStep nextStep;
            public Color32[,] pixels;
            public int Height;
            public int Width;
        }

        public static Texture2D GeneratePixelMap(UnityEngine.Color color)
        {
            Texture2D texture2D = new(1, 1);
            texture2D.SetPixels(new UnityEngine.Color[]
            {
                color
            });
            texture2D.Apply();
            return texture2D;
        }

        public static void ProcessThumbnail()
        {
            if (currentJob != null && currentJob.nextStep != LoadingStep.COMPLETE && currentJob.nextStep == LoadingStep.CREATE_TEXTURE)
            {
                currentJob.texture2D = new Texture2D(currentJob.Width, currentJob.Height, TextureFormat.ARGB32, false);
                currentJob.nextStep = LoadingStep.COPY_PIXELS;
                for (int i = 0; i < currentJob.Height; i++)
                {
                    for (int j = 0; j < currentJob.Width; j++)
                    {
                        if (currentJob.texture2D != null)
                        {
                            currentJob.texture2D.SetPixel(j, i, currentJob.pixels[i, j]);
                        }
                    }
                }
                currentJob.nextStep = LoadingStep.APPLY;
                currentJob.texture2D.Apply();
                currentJob.texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                currentJob.nextStep = LoadingStep.SET_IMAGE;
                var rect = new Rect(0f, 0f, currentJob.Width, currentJob.Height);
                var vector = Vector2.zero;
                var border = Vector4.zero;
                var sprite = Sprite.CreateSprite_Injected(currentJob.texture2D, ref rect, ref vector, 100, 0, SpriteMeshType.Tight, ref border, false);
                sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                Thumbnail.SetBackgroundImage(sprite);
                currentJob.nextStep = LoadingStep.COMPLETE;
                return;
            }
        }

        public static void ConvertImage(byte[] input)
        {
            if (input == cachedBytes) { return; }
            if (input == null)
            {
                cachedBytes = null;
                Thumbnail.SetBackgroundImage(AssetBundleManager.FallbackThumbnail);
                return;
            }
            if (input != null && cachedBytes != null && cachedBytes.SequenceEqual(input)) { return; }
            try
            {
                // From Bytes to Drawing.Image
                System.Drawing.Image image;
                using var ms = new MemoryStream(input);
                image = System.Drawing.Image.FromStream(ms);
                // Force Square Image
                int num;
                int num2;
                if (image.Width == image.Height)
                {
                    num = imageSize[0];
                    num2 = imageSize[1];
                }
                else if (image.Width > image.Height)
                {
                    num = imageSize[0];
                    num2 = image.Height / image.Width * imageSize[1];
                }
                else
                {
                    num = image.Width / image.Height * imageSize[0];
                    num2 = imageSize[1];
                }
                // Convert Drawing.Image to Bitmap
                Bitmap bmp = new(image);
                bmp.SetResolution(num, num2);
                Color32[,] array = new Color32[bmp.Height, bmp.Width];
                for (int i = 1; i <= bmp.Height; i++)
                {
                    for (int j = 1; j <= bmp.Width; j++)
                    {
                        var item = bmp.GetPixel(j - 1, bmp.Height - i);
                        array[i - 1, j - 1] = new Color32(item.R, item.G, item.B, item.A);
                    }
                }
                currentJob = new MultiStageLoadingJob
                {
                    pixels = array,
                    Height = bmp.Height,
                    Width = bmp.Width
                };
                cachedBytes = input;
            }
            catch (Exception ex)
            {
                if (cachedBytes != null)
                {
                    cachedBytes = null;
                    Thumbnail.SetBackgroundImage(AssetBundleManager.FallbackThumbnail);
                }
                Logs.Error("Image Render", ex);
            }
        }
        #endregion
    }
}
