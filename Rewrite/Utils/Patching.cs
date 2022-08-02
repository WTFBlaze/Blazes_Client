using Blaze.API.AW;
using Blaze.Modules;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Transmtn;
using Transmtn.DTO.Notifications;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.XR;
using VRC.Core;
using VRC.DataModel;
using VRC.SDKBase;
using VRC.UI.Elements.Menus;
using WebSocketSharp;
using static Blaze.Utils.Objects.ModObjects;
using static Blaze.Utils.Objects.VRChatObjects;
using static VRC.SDKBase.VRC_EventHandler;

namespace Blaze.Utils
{
    public static class Patching
    {
        public class Toggles
        {
            public static bool MasterLockInstance = false;
            public static bool Serialization = false;
            public static bool SDK2WorldTriggers = false;
            public static bool GodMode = false;
            public static bool ParrotMode = false;
            public static bool InfinitePortals = false;
            public static bool InvisibleJoin = false;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidDelegate(IntPtr thisPtr, IntPtr nativeMethodInfo);
        public static VRCAvatarManager cachedAvatarManager;
        public static readonly List<object> antiGCList = new();
        public static List<ModPortal> CachedPortals = new();
        private static readonly Dictionary<string, string> StopDuplicatingAviTextFML = new();
        private static string StopDuplicatingBlacklistNoticeslol;
        private static string LastCheckedAvatarIDPosted;
        public static List<string> KnownBlocks = new();
        public static List<string> KnownMutes = new();
        public static List<int> SentInvisibleNotice = new();
        public static List<int> SentBotNotice = new();

        public class Patch
        {
            private static List<Patch> Patches = new List<Patch>();
            private static int Failed = 0;
            public MethodInfo TargetMethod { get; set; }
            public HarmonyMethod PrefixMethod { get; set; }
            public HarmonyMethod PostfixMethod { get; set; }
            public HarmonyLib.Harmony Instance { get; set; }

            public Patch(MethodInfo targetMethod, HarmonyMethod Before = null, HarmonyMethod After = null)
            {
                if (targetMethod == null || Before == null && After == null)
                {
                    Logs.Error("[Patches] TargetMethod is NULL or Pre And PostFix are Null");
                    Failed++;
                    return;
                }
                Instance = new HarmonyLib.Harmony($"Patch:{targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
                TargetMethod = targetMethod;
                PrefixMethod = Before;
                PostfixMethod = After;
                Patches.Add(this);
            }

            public static void DoPatches()
            {
                foreach (var patch in Patches)
                {
                    try
                    {
                        patch.Instance.Patch(patch.TargetMethod, patch.PrefixMethod, patch.PostfixMethod);
                        //Logs.Log($"[Patches] Patched! /*{patch.TargetMethod.DeclaringType.FullName}.{patch.TargetMethod.Name} | with */{(patch.PrefixMethod?.method.Name)}{(patch.PostfixMethod?.method.Name)}");
                    }
                    catch
                    {
                        Failed++;
                        Logs.Error($"[Patches] Failed At {patch.TargetMethod?.Name} | {patch.PrefixMethod?.method.Name} | {patch.PostfixMethod?.method.Name}");
                    }
                }
                Logs.Log($"[Patches] Done! Patched {Patches.Count} Methods! and {Failed} Failed Patches!", ConsoleColor.Green);
            }
        }

        private static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Patching).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
        }

        public static void Initialize()
        {
            new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Update)), GetPatch(nameof(ReturnFalse)), null);
            new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Start)), GetPatch(nameof(ReturnFalse)), null);
            new Patch(typeof(Analytics).GetMethod(nameof(Analytics.OnEnable)), GetPatch(nameof(ReturnFalse)), null);
            new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServer)), GetPatch(nameof(ReturnFalse)), null);
            new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServerDelayed)), GetPatch(nameof(ReturnFalse)), null);

            new Patch(AccessTools.Method(typeof(VRC_EventDispatcherRFC), nameof(VRC_EventDispatcherRFC.Method_Public_Boolean_Player_VrcEvent_VrcBroadcastType_0)), GetPatch(nameof(CaughtEventPatch)));
            new Patch(AccessTools.Method(typeof(LoadBalancingClient), "Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0"), GetPatch(nameof(OpRaiseEvent)));
            new Patch(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.OnEvent)), GetPatch(nameof(OnEvent)));
            new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_1)), GetPatch(nameof(PlayerLeftPatch)), null);
            new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_0)), GetPatch(nameof(PlayerJoinedPatch)), null);
            new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), GetPatch(nameof(LeftRoomPatch)), null);
            new Patch(typeof(PortalTrigger).GetMethod(nameof(PortalTrigger.OnTriggerEnter), BindingFlags.Public | BindingFlags.Instance), GetPatch(nameof(EnterPortalPatch)));
            new Patch(typeof(PortalInternal).GetMethod("Method_Private_Void_0"), GetPatch(nameof(PortalDestroyPatch)), null);
            new Patch(AccessTools.Method(typeof(Transmtn.PostOffice), "Put"), GetPatch(nameof(GetNotificationPatch)));
            new Patch(AccessTools.Method(typeof(Transmtn.PostOffice), "Send"), GetPatch(nameof(SentNotificationPatch)));
            new Patch(typeof(AssetManagement).GetMethods().First(mb => mb.Name.StartsWith("Method_Public_Static_Object_Object_Boolean_Boolean_Boolean_") && XRefManager.CheckUsed(mb, "__Instantiate__UnityEngineGameObject__UnityEngineGameObject")), GetPatch(nameof(OnObjectInstantiated)));
            new Patch(typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake)), null, GetPatch(nameof(OnAwakePatch)));
            new Patch(AccessTools.Property(typeof(PhotonPeer), nameof(PhotonPeer.RoundTripTime)).GetMethod, null, GetPatch(nameof(FramesSpoof)));
            new Patch(AccessTools.Property(typeof(Time), nameof(Time.smoothDeltaTime)).GetMethod, null, GetPatch(nameof(PingSpoof)));
            new Patch(typeof(WebsocketPipeline).GetMethod(nameof(WebsocketPipeline._ProcessPipe_b__21_0)), GetPatch(nameof(PipelinePatch)));
            //VRCWebSocketsManager

            if (BlazesXRefs.FriendNameTargetMethod != null)
                new Patch(BlazesXRefs.FriendNameTargetMethod, new HarmonyMethod(typeof(TrueRanks).GetMethod("GetFriendlyDetailedNameForSocialRank", BindingFlags.NonPublic | BindingFlags.Static)));
            if (BlazesXRefs.ColorForRankMethods != null)
                BlazesXRefs.ColorForRankMethods.ForEach(method => new Patch(method, new HarmonyMethod(typeof(TrueRanks).GetMethod("GetColorForSocialRank", BindingFlags.NonPublic | BindingFlags.Static))));
            if (BlazesXRefs.OnPhotonPlayerJoinMethod != null)
                new Patch(BlazesXRefs.OnPhotonPlayerJoinMethod, GetPatch(nameof(PhotonJoin)));
            if (BlazesXRefs.OnPhotonPlayerLeftMethod != null)
                new Patch(BlazesXRefs.OnPhotonPlayerLeftMethod, GetPatch(nameof(PhotonLeft)));
            if (BlazesXRefs.PlaceUiMethod != null)
                new Patch(BlazesXRefs.PlaceUiMethod, GetPatch(nameof(PlaceUiPatch)));
            if (BlazesXRefs.ActionWheelMethod != null)
                new Patch(BlazesXRefs.ActionWheelMethod, null, GetPatch(nameof(OpenMainPage)));

            /*foreach (var method in typeof(SelectedUserMenuQM).GetMethods())
            {
                if (!method.Name.StartsWith("Method_Private_Void_IUser_PDM_")) continue;
                if (XrefScanner.XrefScan(method).Count() < 3) continue;
                new Patch(method, null, GetPatch(nameof(SetUserPatch)));
            }*/

            foreach (var methodInfo in typeof(VRCFlowManager).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (methodInfo.ReturnType != typeof(void) || methodInfo.GetParameters().Length != 0)
                    continue;
                if (!XrefScanner.XrefScan(methodInfo).Any(it => it.Type == XrefType.Global && it.ReadAsObject()?.ToString() == "Going to Home Location: "))
                    continue;
                new Patch(methodInfo, null, GetPatch(nameof(GoHomePatch)));
            }

            Patch.DoPatches();
            SetupCachedAvatarManager();
        }

        private static void SetupCachedAvatarManager()
        {
            foreach (var nestedType in typeof(VRCAvatarManager).GetNestedTypes())
            {
                var moveNext = nestedType.GetMethod("MoveNext");
                if (moveNext == null)
                {
                    continue;
                }
                var avatarManagerField = nestedType.GetProperties().SingleOrDefault(it => it.PropertyType == typeof(VRCAvatarManager));
                if (avatarManagerField == null)
                {
                    continue;
                }
                var fieldOffset = (int)IL2CPP.il2cpp_field_get_offset((IntPtr)UnhollowerUtils.GetIl2CppFieldInfoPointerFieldForGeneratedFieldAccessor(avatarManagerField.GetMethod).GetValue(null));
                unsafe
                {
                    var originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(moveNext).GetValue(null);
                    originalMethodPointer = XrefScannerLowLevel.JumpTargets(originalMethodPointer).First();
                    VoidDelegate originalDelegate = null;
                    void TaskMoveNextPatch(IntPtr taskPtr, IntPtr nativeMethodInfo)
                    {
                        var avatarManager = *(IntPtr*)(taskPtr + fieldOffset - 16);

                        cachedAvatarManager = new VRCAvatarManager(avatarManager);
                        originalDelegate(taskPtr, nativeMethodInfo);
                        cachedAvatarManager = null;
                    }
                    VoidDelegate patchDelegate = TaskMoveNextPatch;
                    antiGCList.Add(patchDelegate);
                    MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPointer), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                    originalDelegate = Marshal.GetDelegateForFunctionPointer<VoidDelegate>(originalMethodPointer);
                }
            }
        }

        #region Patch Methods
        private static bool ReturnFalse()
        {
            return false;
        }

        private static void PlayerJoinedPatch(VRC.Player __0)
        {
            try
            {
                foreach (var m in Main.Modules) m.PlayerJoined(__0);
            }
            catch {}
        }

        private static void PlayerLeftPatch(VRC.Player __0)
        {
            try
            {
                foreach (var m in Main.Modules) m.PlayerLeft(__0);
            }
            catch {}
        }

        private static void LeftRoomPatch()
        {
            try
            {
                foreach (var m in Main.Modules) m.LeftWorld();
            }
            catch {}
        }

        private static void PhotonJoin(ref Photon.Realtime.Player __0)
        {
            try
            {
                foreach (var m in Main.Modules) m.PhotonJoined(__0);
            }
            catch { }
        }

        private static void PhotonLeft(ref Photon.Realtime.Player __0)
        {
            try
            {
                foreach (var m in Main.Modules) m.PhotonLeft(__0);
            }
            catch { }
        }

        private static bool PlaceUiPatch(VRCUiManager __instance, bool __0)
        {
            if (!XRDevice.isPresent || !Config.Main.ComfyVRMenu) return true;
            float num = TrackingUtils.GetVRCTrackingManager() != null ? TrackingUtils.GetVRCTrackingManager().transform.localScale.x : 1f;
            if (num <= 0f)
            {
                num = 1f;
            }
            var playerTrackingDisplay = __instance.transform;
            var unscaledUIRoot = __instance.transform.Find("UnscaledUI");
            playerTrackingDisplay.position = CameraUtils.GetWorldCameraPosition();
            Vector3 rotation = GameObject.Find("Camera (eye)").transform.rotation.eulerAngles;
            Vector3 euler = new Vector3(rotation.x - 30f, rotation.y, 0f);
            //if (rotation.x > 0f && rotation.x < 300f) rotation.x = 0f;
            if (PlayerUtils.CurrentUser() == null)
            {
                euler.x = euler.z = 0f;
            }
            if (!__0)
            {
                playerTrackingDisplay.rotation = Quaternion.Euler(euler);
            }
            else
            {
                Quaternion quaternion = Quaternion.Euler(euler);
                if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 15f))
                {
                    if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 25f))
                    {
                        playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 5f);
                    }
                    else
                    {
                        playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 1f);
                    }
                }
            }
            if (num >= 0f)
            {
                playerTrackingDisplay.localScale = num * Vector3.one;
            }
            else
            {
                playerTrackingDisplay.localScale = Vector3.one;
            }
            if (num > float.Epsilon)
            {
                unscaledUIRoot.localScale = 1f / num * Vector3.one;
            }
            else
            {
                unscaledUIRoot.localScale = Vector3.one;
            }
            return false;
        }

        private static void OpenMainPage(ActionMenu __instance)
        {
            ActionWheelAPI.OpenMainPage(__instance);
        }

        private static void SetUserPatch(SelectedUserMenuQM __instance, IUser __0)
        {
            if (__0 == null) return;

            foreach (var m in Main.Modules)
            {
                m.SelectedUser(__0, __instance.field_Public_Boolean_0);
            }
        }

        private static void GoHomePatch(VRCFlowManagerVRC __instance)
        {
            StartEnforcingInstanceType(__instance, true);
        }

        private static void StartEnforcingInstanceType(VRCFlowManager flowManager, bool isButton)
        {
            if (Config.Main.FriendsPlusHome)
            {
                flowManager.field_Protected_InstanceAccessType_0 = InstanceAccessType.FriendsOfGuests;
                MelonCoroutines.Start(Functions.EnforceTargetInstanceType(flowManager, InstanceAccessType.FriendsOfGuests, isButton ? 10 : 30));
            }
        }

        private static bool PortalDestroyPatch()
        {
            bool result = true;
            try
            {
                if (Toggles.InfinitePortals) result = false;
            }
            catch { }
            return result;
        }

        private static void FramesSpoof(ref int __result)
        {
            try
            {
                if (Config.Main.FramesSpoof)
                    __result = Config.Main.FramesSpoofValue;
                return;
            }
            catch { }
        }

        private static void PingSpoof(ref float __result)
        {
            try
            {
                if (Config.Main.PingSpoof)
                    __result = Config.Main.PingSpoofValue;
                return;
            }
            catch { }
        }

        private static bool OpRaiseEvent(byte __0, object __1, RaiseEventOptions __2, SendOptions __3)
        {
            try
            {
                switch (__0)
                {
                    // Invisible Join
                    case 202:
                        __2.field_Public_ReceiverGroup_0 = Toggles.InvisibleJoin ? ReceiverGroup.MasterClient : ReceiverGroup.Others;
                        //BlazeVars.InvisibleJoin = false;
                        //QM.InvisibleJoin.setToggleState(false);
                        break;

                    /*// Force Mute
                    case 1: return !GlobalVariables.ForceMute;*/

                    // Master Lock
                    case 4: return !Toggles.MasterLockInstance;
                    case 5: return !Toggles.MasterLockInstance;
                    case 6: return !Toggles.MasterLockInstance;
                }
                if (__1 != null && __2 != null) return __0 != 7 || !Toggles.Serialization;
            }
            catch { }
            return true;
        }

        private static void OnAwakePatch(VRCPlayer __instance)
        {
            try
            {
                if (__instance == null) return;
                __instance.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() =>
                {
                    if (__instance._player != null && __instance.GetAPIUser() != null && __instance.prop_ApiAvatar_0 != null)
                    {
                        var p = __instance.GetAPIUser();
                        var a = __instance.prop_ApiAvatar_0;
                        if (Config.Main.LogAvatarsSwitched && PlayerUtils.CurrentUser() != null)
                        {
                            if (StopDuplicatingAviTextFML.TryGetValue(p.id, out var avatarID))
                            {
                                if (avatarID == a.id) return;
                                StopDuplicatingAviTextFML[p.id] = a.id;
                                Logs.Debug($"[<color=yellow>AVI</color>] <color={p.GetRankColor()}>{p.displayName}</color> -> <color=cyan>{a.name}</color>");
                                Logs.Log($"[AVI] {p.displayName} -> {a.name} - {a.authorName}", ConsoleColor.DarkYellow);
                            }
                            else
                            {
                                StopDuplicatingAviTextFML.Add(p.id, a.id);
                                Logs.Debug($"[<color=yellow>AVI</color>] <color={p.GetRankColor()}>{p.displayName}</color> -> <color=cyan>{a.name}</color>");
                                Logs.Log($"[AVI] {p.displayName} -> {a.name} - {a.authorName}", ConsoleColor.DarkYellow);
                            }
                        }

                        if (p.id == PlayerUtils.CurrentUser().GetUserID())
                        {
                            AvatarFavorites.AddToRecentlyUsed(a);
                        }
                        else
                        {
                            AvatarFavorites.AddToRecentlySeen(a);
                        }

                        if (BlazeNetwork.IsConnected)
                        {
                            BlazeNetwork.ws.Send(JsonConvert.SerializeObject(new
                            {
                                payload = new
                                {
                                    type = "AvatarAdd",
                                    data = new
                                    {
                                        _id = a.id,
                                        AssetURL = a.assetUrl,
                                        AuthorID = a.authorId,
                                        AuthorName = a.authorName,
                                        AvatarName = a.name,
                                        Description = a.description,
                                        Featured = a.featured,
                                        ImageURL = a.imageUrl,
                                        ReleaseStatus = a.releaseStatus,
                                        ThumbnailImageURL = a.thumbnailImageUrl,
                                        Version = a.version,
                                        Tags = a.tags,
                                        TimeDetected = DateTime.Now,
                                    }
                                }
                            }));
                        }

                        foreach (var m in Main.Modules) m.AvatarIsReady(__instance, __instance.GetPCAvatar());
                    }
                }));
            }
            catch { }
        }

        private static bool OnObjectInstantiated(ref UnityEngine.Object __0)
        {
            try
            {
                if (__0 == null) return true;
                var instantiatedGameObject = __0.TryCast<GameObject>();
                if (!instantiatedGameObject.name.ToLower().Contains("avatar")) return true;
                APIUser user = null;
                try
                {
                    user = cachedAvatarManager.field_Private_VRCPlayer_0._player.field_Private_APIUser_0;
                }
                catch { }
                if (user == null) return true;
                var currentAviID = Functions.GetPlayerByUserID(user.id).prop_ApiAvatar_0.id;

                // Blacklisted Avatars
                if (ShaderBlacklist.blockList.Contains(cachedAvatarManager.prop_ApiAvatar_0.id) && user.id != PlayerUtils.CurrentUser().GetAPIUser().id)
                {
                    if (StopDuplicatingBlacklistNoticeslol != user.id)
                    {
                        Logs.Log($"Blocked {user.displayName}'s Avatar. (Avatar Is Blacklisted)");
                        Logs.Debug($"<color=red>[BLACKLIST]</color> Blocked <color=yellow>{user.displayName}</color>'s Avi!");
                        StopDuplicatingBlacklistNoticeslol = user.id;
                    }
                    return false;
                }
                // Public Lobby Check
                if (Config.AntiCrash.OnlyCheckInPublicLobbies && WorldUtils.CurrentInstance().type != InstanceAccessType.Public) return true;
                // Self Check
                if (!Config.AntiCrash.CheckSelf && user.id == PlayerUtils.CurrentUser().GetUserID()) return true;
                // Friends Check
                if (user.isFriend && !Config.AntiCrash.CheckFriends) return true;
                // Others CHeck
                if (!user.isFriend && !Config.AntiCrash.CheckOthers) return true;

                // Start Anti Crash
                if (LastCheckedAvatarIDPosted != currentAviID)
                {
                    Logs.Log($"[AntiCrash] Checking {user.displayName}'s Avatar!", ConsoleColor.DarkCyan);
                    Logs.Debug($"<color=#3A96DD>[AntiCrash]</color> Checking <color=yellow>{user.displayName}</color>'s Avatar!");
                }
                bool LabelToggle = false;
                if (Config.AntiCrash.AntiPhysicsCrash)
                {
                    List<Transform> transforms = GameObjectManager.FindAllComponentsInGameObject<Transform>(instantiatedGameObject);
                    List<Rigidbody> rigidbodies = GameObjectManager.FindAllComponentsInGameObject<Rigidbody>(instantiatedGameObject);
                    List<Collider> colliders = GameObjectManager.FindAllComponentsInGameObject<Collider>(instantiatedGameObject);
                    List<Joint> joints = GameObjectManager.FindAllComponentsInGameObject<Joint>(instantiatedGameObject);

                    //Transform Limiter
                    int limitedTransforms = 0;
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        if (transforms[i] == null)
                        {
                            continue;
                        }
                        limitedTransforms = AntiCrash.ProcessTransform(transforms[i], limitedTransforms);
                    }

                    //Rigidbody
                    for (int i = 0; i < rigidbodies.Count; i++)
                    {
                        if (rigidbodies[i] == null)
                        {
                            continue;
                        }
                        AntiCrash.ProcessRigidbody(rigidbodies[i]);
                    }

                    //Collider
                    int nukedColliders = 0;
                    for (int i = 0; i < colliders.Count; i++)
                    {
                        if (colliders[i] == null)
                        {
                            continue;
                        }
                        if (AntiCrash.ProcessCollider(colliders[i]) == true)
                        {
                            nukedColliders++;
                        }
                    }

                    //SpringJoint
                    int nukedSpringJoints = 0;
                    for (int i = 0; i < joints.Count; i++)
                    {
                        if (joints[i] == null)
                        {
                            continue;
                        }
                        if (AntiCrash.ProcessJoint(joints[i]) == true)
                        {
                            nukedSpringJoints++;
                        }
                    }

                    //Output Reports
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (limitedTransforms > 0)
                        {
                            Logs.Log($"[AntiCrash] Limited Transforms (x{limitedTransforms})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                        if (nukedColliders > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted Colliders (x{nukedColliders})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                        if (nukedSpringJoints > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted SpringJoints (x{limitedTransforms})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti Audio Crash
                if (Config.AntiCrash.AntiAudioCrash == true)
                {
                    List<AudioSource> audioSources = GameObjectManager.FindAllComponentsInGameObject<AudioSource>(instantiatedGameObject);
                    int nukedAudioSources = 0;
                    for (int i = Config.AntiCrash.MaxAllowedAvatarAudioSources; i < audioSources.Count; i++)
                    {
                        if (audioSources[i] == null)
                        {
                            continue;
                        }

                        //Potential Attack Vector if user names it USpeak intentionally
                        if (audioSources[i].name.Contains("USpeak") == true)
                        {
                            continue;
                        }
                        UnityEngine.Object.DestroyImmediate(audioSources[i], true);
                        nukedAudioSources++;
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (nukedAudioSources > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted AudioSources (x{nukedAudioSources})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti Cloth Crash
                if (Config.AntiCrash.AntiClothCrash == true)
                {
                    List<Cloth> clothes = GameObjectManager.FindAllComponentsInGameObject<Cloth>(instantiatedGameObject);
                    AntiCrash.AntiCrashClothPostProcess postProcessReport = new();
                    for (int i = 0; i < clothes.Count; i++)
                    {
                        if (clothes[i] == null)
                        {
                            continue;
                        }
                        postProcessReport = AntiCrash.ProcessCloth(clothes[i], postProcessReport.nukedCloths, postProcessReport.currentVertexCount);
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (postProcessReport.nukedCloths > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted Cloths (x{postProcessReport.nukedCloths})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti ParticleSystem Crash
                if (Config.AntiCrash.AntiParticleSystemCrash == true)
                {
                    List<ParticleSystem> particleSystems = GameObjectManager.FindAllComponentsInGameObject<ParticleSystem>(instantiatedGameObject);
                    AntiCrash.AntiCrashParticleSystemPostProcess postProcessReport = new();
                    for (int i = 0; i < particleSystems.Count; i++)
                    {
                        if (particleSystems[i] == null)
                        {
                            continue;
                        }
                        AntiCrash.ProcessParticleSystem(particleSystems[i], ref postProcessReport);
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (postProcessReport.nukedParticleSystems > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted ParticleSystems (x{postProcessReport.nukedParticleSystems})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti DynamicBone Crash
                if (Config.AntiCrash.AntiDynamicBoneCrash == true)
                {
                    //DynamicBoneCollider
                    List<DynamicBoneCollider> dynamicBoneColliders = GameObjectManager.FindAllComponentsInGameObject<DynamicBoneCollider>(instantiatedGameObject);
                    AntiCrash.AntiCrashDynamicBoneColliderPostProcess postDynamicBoneColliderProcessReport = new();
                    for (int i = 0; i < dynamicBoneColliders.Count; i++)
                    {
                        if (dynamicBoneColliders[i] == null)
                        {
                            continue;
                        }
                        postDynamicBoneColliderProcessReport = AntiCrash.ProcessDynamicBoneCollider(dynamicBoneColliders[i], postDynamicBoneColliderProcessReport.nukedDynamicBoneColliders, postDynamicBoneColliderProcessReport.dynamicBoneColiderCount);
                    }

                    //DynamicBone
                    List<DynamicBone> dynamicBones = GameObjectManager.FindAllComponentsInGameObject<DynamicBone>(instantiatedGameObject);
                    AntiCrash.AntiCrashDynamicBonePostProcess postDynamicBoneProcessReport = new();
                    for (int i = 0; i < dynamicBones.Count; i++)
                    {
                        if (dynamicBones[i] == null)
                        {
                            continue;
                        }
                        postDynamicBoneProcessReport = AntiCrash.ProcessDynamicBone(dynamicBones[i], postDynamicBoneProcessReport.nukedDynamicBones, postDynamicBoneProcessReport.dynamicBoneCount);
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (postDynamicBoneProcessReport.nukedDynamicBones > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted DynamicBones (x{postDynamicBoneProcessReport.nukedDynamicBones})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                        if (postDynamicBoneColliderProcessReport.nukedDynamicBoneColliders > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted DynamicBonesColliders (x{postDynamicBoneColliderProcessReport.nukedDynamicBoneColliders})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti Light Source Crash
                if (Config.AntiCrash.AntiLightSourceCrash == true)
                {
                    List<Light> lightSources = GameObjectManager.FindAllComponentsInGameObject<Light>(instantiatedGameObject);
                    AntiCrash.AntiCrashLightSourcePostProcess postProcessReport = new();
                    for (int i = 0; i < lightSources.Count; i++)
                    {
                        if (lightSources[i] == null)
                        {
                            continue;
                        }
                        postProcessReport = AntiCrash.ProcessLight(lightSources[i], postProcessReport.nukedLightSources, postProcessReport.lightSourceCount);
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (postProcessReport.nukedLightSources > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted LightSources (x{postProcessReport.nukedLightSources})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }

                //Anti Renderer Crash
                bool antiMeshCrash = Config.AntiCrash.AntiMeshCrash;
                bool antiMaterialCrash = Config.AntiCrash.AntiMaterialCrash;
                bool antiShaderCrash = Config.AntiCrash.AntiShaderCrash;

                if (antiMeshCrash == true || antiMaterialCrash == true || antiShaderCrash == true)
                {
                    List<Renderer> renderers = GameObjectManager.FindAllComponentsInGameObject<Renderer>(instantiatedGameObject);
                    AntiCrash.AntiCrashRendererPostProcess postProcessReport = new();
                    for (int i = 0; i < renderers.Count; i++)
                    {
                        if (renderers[i] == null)
                        {
                            continue;
                        }
                        AntiCrash.ProcessRenderer(renderers[i], antiMeshCrash, antiMaterialCrash, antiShaderCrash, ref postProcessReport);
                    }

                    //Output Report
                    if (LastCheckedAvatarIDPosted != currentAviID)
                    {
                        if (postProcessReport.nukedMeshes > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted Meshes (x{postProcessReport.nukedMeshes})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                        if (postProcessReport.nukedMaterials > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted Materials (x{postProcessReport.nukedMaterials})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                        if (postProcessReport.nukedShaders > 0)
                        {
                            Logs.Log($"[AntiCrash] Deleted Shaders (x{postProcessReport.nukedShaders})!", ConsoleColor.DarkCyan);
                            LabelToggle = true;
                        }
                    }
                }
                if (LastCheckedAvatarIDPosted != currentAviID)
                {
                    Main.Players.TryGetValue(user.id, out var comp);
                    comp.SetAntiCrashBool(LabelToggle);
                }
                LastCheckedAvatarIDPosted = currentAviID;
                return true;
            }
            catch { }
            return true;
        }

        private static bool OnEvent(ref EventData __0)
        {
            if (__0.Parameters == null) return true;
            bool result = true;
            try
            {
                var playerSender = Functions.GetPlayerByActorID(__0.sender);
                var photonSender = PhotonUtils.GetPhotonPlayer(__0.sender);
                var senderID = __0.sender;
                if (Config.Main.LogPhotonEvents)
                {
                    object Data = SerializationUtils.FromIL2CPPToManaged<object>(__0.Parameters);
                    if (__0.Code != 7 && __0.Code != 1)
                    {
                        Logs.Log($"[Event {__0.Code}] Event \n{JsonConvert.SerializeObject(Data, Formatting.Indented)}", ConsoleColor.Magenta);
                    }
                }

                /*if (photonSender.IsInvisible() && senderID != PlayerUtils.CurrentUser().GetActorNumber())
                {
                    if (PlayerUtils.CurrentUser() != null)
                    {
                        if (!SentInvisibleNotice.Contains(__0.sender))
                        {
                            Logs.HUD($"Blocking Events from (<color=yellow>{photonSender.GetDisplayName()}</color>) <i>[Invisible User]</i>", 3.5f);
                            SentInvisibleNotice.Add(__0.sender);
                            Functions.Delay(delegate
                            {
                                SentInvisibleNotice.Remove(senderID);
                            }, 10);
                        }
                        return false;
                    }
                }
                else
                {
                    if (playerSender._vrcplayer.IsBot() && senderID != PlayerUtils.CurrentUser().GetActorNumber())
                    {
                        if (PlayerUtils.CurrentUser() != null)
                        {
                            if (!SentBotNotice.Contains(__0.sender))
                            {
                                Logs.HUD($"Blocking Events from (<color=yellow>{photonSender.GetDisplayName()}</color>) <i>[Bot]</i>", 3.5f);
                                SentBotNotice.Add(__0.sender);
                                Functions.Delay(delegate
                                {
                                    SentBotNotice.Remove(senderID);
                                }, 10);
                            }
                            return false;
                        }
                    }
                }*/

                /*if (Config.EventLocks.list.Exists(x => x.UserID == photonSender.GetUserID()))
                {
                    var item = Config.EventLocks.list.Find(x => x.UserID == photonSender.GetUserID());
                    if (item.BlockedEvents.Contains(__0.Code))
                    {
                        return false;
                    }
                }*/

                switch (__0.Code)
                {
                    case 1:
                        if (Toggles.ParrotMode)
                        {
                            if (playerSender.GetUserID() == Main.Target.GetUserID())
                            {
                                PhotonUtils.OpRaiseEvent(1, __0.CustomData, new RaiseEventOptions()
                                {
                                    field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                                    field_Public_Byte_0 = 1,
                                    field_Public_Byte_1 = 1,
                                }, SendOptions.SendUnreliable);
                            }
                        }
                        //return !EventLocker.GlobalLockE1;
                        break;

                    case 6:
                        //return !EventLocker.GlobalLockE6;
                        break;

                    case 7:
                        //return !EventLocker.GlobalLockE7;
                        break;

                    case 9:
                        //return !EventLocker.GlobalLockE9;
                        break;

                    case 33:
                        object RawData = SerializationUtils.FromIL2CPPToManaged<object>(__0.Parameters);
                        //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(RawData, Newtonsoft.Json.Formatting.Indented));
                        var ParsedData = RawData as Dictionary<byte, object>;
                        var InfoData = ParsedData[245] as Dictionary<byte, object>;
                        int EventType = int.Parse(InfoData[0].ToString());
                        switch (EventType)
                        {
                            //World Event
                            case 2:
                                //Message of 2 Event
                                string Message = InfoData[2].ToString();
                                //type (mostly "Moderation")
                                string Type = InfoData[5].ToString();

                                if (Message.Contains("Unable to start a vote to kick"))
                                {
                                    Console.WriteLine("[Moderation] Failed at Votekick");
                                    return false;
                                }
                                else if (Message.Contains("You have been warned for your behavior. If you continue, you may be kicked out of the instance"))
                                {
                                    if (Config.Main.ShowModerations && Config.Main.ModerationWarns)
                                    {
                                        Logs.Log("[Moderation] RoomOwner --> Warn You", ConsoleColor.DarkRed);
                                        if (Config.Main.LogToHud)
                                        {
                                            Logs.HUD("[Moderation] RoomOwner --> Warn You", 3.5f);
                                        }
                                    }
                                    result = !Config.Main.ModerationAntiWarns;
                                }
                                break;

                            //World Owner Mic OFF event
                            case 8:
                                if (Config.Main.ShowModerations && Config.Main.ModerationMicOff)
                                {
                                    Logs.Log("[Moderation] The RoomOwner --> MicOff You", ConsoleColor.DarkRed);
                                    if (Config.Main.LogToHud)
                                    {
                                        Logs.HUD("[Moderation] The RoomOwner --> MicOff You", 3.5f);
                                    }
                                }
                                result = !Config.Main.ModerationAntiMicOff;
                                break;

                            //Votekick Event
                            case 13:
                                string VoteMessage = InfoData[2] as string;
                                string RandomID = InfoData[3] as string;
                                if (Config.Main.ShowModerations && Config.Main.ModerationKicks)
                                {
                                    Logs.Log("[Moderation] [?] VoteKick --> [?]", ConsoleColor.DarkRed);
                                    if (Config.Main.LogToHud)
                                    {
                                        Logs.HUD("[Moderation] [?] VoteKick --> [?]", 3.5f);
                                    }
                                }
                                break;

                            //Player Join / maybe Leave
                            case 20:
                                string[] JoinDataMaybe = InfoData[3] as string[];
                                break;

                            //Moderation event
                            case 21:
                                if (InfoData[10] != null && InfoData[11] != null)
                                {
                                    //If the Key 1 Exists is an direct moderation
                                    if (InfoData.ContainsKey(1))
                                    {
                                        //Photon Players ID
                                        int SenderID = int.Parse(InfoData[1].ToString());
                                        //var VRCPlayer = Utils.PlayerManager.GetPlayerID(SenderID);

                                        var PhotonPlayer = PhotonUtils.LoadBalancingPeer.GetPhotonPlayer(SenderID);
                                        string SenderName = "?";
                                        if (PhotonPlayer != null) SenderName = PhotonPlayer.GetDisplayName();

                                        //10 = Blocked | 11 = Muted
                                        #region FOR LATER OPTIMISATION!!!!!!!!(arion)
                                        //byte i = Convert.ToByte(InfoData[10]);
                                        //i += i;
                                        //i += Convert.ToByte(InfoData[11]);
                                        ////0 = f / f, 1 = f / t , 2 = t / f, 3 = t / t
                                        //switch (i)
                                        //{
                                        //    case 0:
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Unblocked You");
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Unmuted you");
                                        //        break;
                                        //    case 1:
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Unblocked You");
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Muted you");
                                        //        break;
                                        //    case 2:
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Blocked You");
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Unmuted you");
                                        //        break;
                                        //    case 3:
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Blocked You");
                                        //        Console.WriteLine($"[Moderation] {SenderName} --> Muted you");
                                        //        break;
                                        //}
                                        //ModerationHelper.UpdateModeration(PhotonPlayer.GetUserID(), ModerationHelper.ModerationType.BlockU, i == 2 || i == 3 ? true : false,PhotonPlayer.GetDisplayName());
                                        //ModerationHelper.UpdateModeration(PhotonPlayer.GetUserID(), ModerationHelper.ModerationType.MuteU, i == 1 || i == 3 ? true : false, PhotonPlayer.GetDisplayName());
                                        #endregion

                                        #region Bool Non Optimized Shit
                                        bool Blocked = bool.Parse(InfoData[10].ToString());
                                        bool Muted = bool.Parse(InfoData[11].ToString());
                                        if (Blocked)
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationBlocks)
                                            {
                                                KnownBlocks.Add(SenderName);
                                                Logs.Log($"[Moderation] {SenderName} --> Blocked You", ConsoleColor.DarkRed);
                                                if (Config.Main.LogToHud)
                                                {
                                                    Logs.HUD($"[Moderation] {SenderName} --> Blocked You", 3.5f);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationUnblocks)
                                            {
                                                if (KnownBlocks.Contains(SenderName))
                                                {
                                                    KnownBlocks.Remove(SenderName);
                                                    Logs.Log($"[Moderation] {SenderName} --> Unblocked You", ConsoleColor.DarkRed);
                                                    if (Config.Main.LogToHud)
                                                    {
                                                        Logs.HUD($"[Moderation] {SenderName} --> Unblocked You", 3.5f);
                                                    }
                                                }
                                            }
                                        }
                                        if (Muted)
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationMutes)
                                            {
                                                KnownMutes.Add(SenderName);
                                                Logs.Log($"[Moderation] {SenderName} --> Muted You", ConsoleColor.DarkRed);
                                                if (Config.Main.LogToHud)
                                                {
                                                    Logs.HUD($"[Moderation] {SenderName} --> Muted You", 3.5f);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationUnmutes)
                                            {
                                                if (KnownMutes.Contains(SenderName))
                                                {
                                                    KnownMutes.Remove(SenderName);
                                                    Logs.Log($"[Moderation] {SenderName} --> Unmuted You", ConsoleColor.DarkRed);
                                                    if (Config.Main.LogToHud)
                                                    {
                                                        Logs.HUD($"[Moderation] {SenderName} --> Unmuted You", 3.5f);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        return !Blocked;
                                    }
                                    else
                                    {
                                        // It sends the Arrays when the Block and Mute Event happen fast.
                                        // if 10 is an Array it has all the PhotonIds that Blocked You
                                        // if 11 is an Array it has all the PhotonIds that Muted You
                                        var BlockedList = InfoData[10] as int[];
                                        var MuteList = InfoData[11] as int[];

                                        if (BlockedList.Count() == 0)
                                        {
                                            //Console.WriteLine($"[Moderation] No Blocked Users in the Lobby");
                                        }
                                        else
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationBlocks)
                                            {
                                                foreach (var blockid in BlockedList)
                                                {
                                                    var BlockPlayer = PhotonUtils.LoadBalancingPeer.GetPhotonPlayer(blockid);
                                                    Logs.Log($"[Moderation] {BlockPlayer.GetDisplayName()} Has you Blocked!", ConsoleColor.DarkRed);
                                                    if (Config.Main.LogToHud)
                                                    {
                                                        Logs.HUD($"[Moderation] {BlockPlayer.GetDisplayName()} Has you Blocked!", 3.5f);
                                                    }
                                                }
                                            }
                                            // result = false;
                                        }
                                        if (MuteList.Count() == 0)
                                        {
                                            // Console.WriteLine($"[Moderation] No Muted Users in the Lobby");
                                        }
                                        else
                                        {
                                            if (Config.Main.ShowModerations && Config.Main.ModerationMutes)
                                            {
                                                foreach (var muteId in MuteList)
                                                {
                                                    var MutePlayer = PhotonUtils.LoadBalancingPeer.GetPhotonPlayer(muteId);
                                                    Console.WriteLine($"[Moderation] {MutePlayer.GetDisplayName()} Has you Muted!");
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            catch { }
            return result;
        }

        private static bool EnterPortalPatch(PortalTrigger __instance)
        {
            try
            {
                if (Vector3.Distance(PlayerUtils.CurrentUser().transform.position, __instance.transform.position) > 1) return false;
                if (Config.Main.AntiPortal) return false;
                if (Config.Main.PortalPrompt)
                {
                    var portalInternal = __instance.field_Private_PortalInternal_0;
                    PopupUtils.AlertV2($"To: {portalInternal.field_Private_ApiWorld_0.name}", "Yes", () =>
                    {
                        // this is long and fucking annoying but it prevents having to change it whenever vrc is changing shit >:(
                        string instanceID;
                        if (portalInternal.field_Private_String_1 != null)
                        {
                            instanceID = portalInternal.field_Private_String_1;
                        }
                        else
                        {
                            if (portalInternal.field_Private_String_2 != null)
                            {
                                instanceID = portalInternal.field_Private_String_2;
                            }
                            else
                            {
                                if (portalInternal.field_Private_String_3 != null)
                                {
                                    instanceID = portalInternal.field_Private_String_3;
                                }
                                else
                                {
                                    instanceID = portalInternal.field_Private_String_4;
                                }
                            }
                        }
                        WorldUtils.JoinRoom2(portalInternal.field_Private_ApiWorld_0.id, instanceID);
                        Logs.Log($"[Portals] Joining: {portalInternal.field_Private_ApiWorld_0.id}:{portalInternal.field_Private_String_4}", ConsoleColor.Yellow);
                        PopupUtils.HideCurrentPopUp();
                    }, "No", PopupUtils.HideCurrentPopUp);
                    return false;
                }
            }
            catch { }
            return true;
        }

        private static bool GetNotificationPatch(ref Notification __0)
        {
            try
            {
                var username = __0.senderUsername;
                if (__0 != null && Config.Main.LogNotifications)
                {
                    switch (__0.type)
                    {
                        case "friendRequest":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Friend Request");
                                Logs.Log($"{user.displayName} Sent Friend Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Friend Request");
                                    Logs.Log($"{user.displayName} Sent Friend Request");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Friend Request");
                                    Logs.Log($"{username} Sent Friend Request");
                                }));
                            }
                            break;

                        case "invite":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite");
                                Logs.Log($"{user.displayName} Sent Invite");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite");
                                    Logs.Log($"{user.displayName} Sent Invite");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Invite");
                                    Logs.Log($"{username} Sent Invite");
                                }));
                            }
                            break;

                        case "requestInvite":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Request");
                                Logs.Log($"{user.displayName} Sent Invite Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Request");
                                    Logs.Log($"{user.displayName} Sent Invite Request");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Invite Request");
                                    Logs.Log($"{username} Sent Invite Request");
                                }));
                            }
                            break;

                        case "requestInviteResponse":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Response");
                                Logs.Log($"{user.displayName} Sent Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Response");
                                    Logs.Log($"{user.displayName} Sent Invite Response");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Invite Response");
                                    Logs.Log($"{username} Sent Invite Response");
                                }));
                            }
                            break;

                        case "message":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Message");
                                Logs.Log($"{user.displayName} Sent Message");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Message");
                                    Logs.Log($"{user.displayName} Sent Message");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Message");
                                    Logs.Log($"{username} Sent Message");
                                }));
                            }
                            break;

                        case "inviteResponse":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Response");
                                Logs.Log($"{user.displayName} Sent Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.senderUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"<color={user.GetRankColor()}>{user.displayName}</color> Sent Invite Response");
                                    Logs.Log($"{user.displayName} Sent Invite Response");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug($"{username} Sent Invite Response");
                                    Logs.Log($"{username} Sent Invite Response");
                                }));
                            }
                            break;

                        default:
                            Logs.Log($"YOU RECIEVED A NOTIFICATION OF AN UN RECOGNIZED TYPE! Please let Blaze know in the discord server (if you send it via dms your key is getting revoked i swear to fucking god)! | Type: [{__0.type}]", ConsoleColor.DarkYellow);
                            break;
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool SentNotificationPatch(ref Notification __0)
        {
            try
            {
                /*foreach (var item in __0.details)
                {
                    Logs.Log($"Key: {item.Key} | Value: {item.Value}");
                }*/

                if (__0 != null && Config.Main.LogNotifications)
                {
                    switch (__0.type)
                    {
                        case "friendRequest":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Friend Request");
                                Logs.Log($"You Sent {user.displayName} A Friend Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Friend Request");
                                    Logs.Log($"You Sent {user.displayName} A Friend Request");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> A Friend Request");
                                    Logs.Log("You Sent Someone A Friend Request");
                                }));
                            }
                            break;

                        case "invite":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite");
                                Logs.Log($"You Sent {user.displayName} An Invite");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite");
                                    Logs.Log($"You Sent {user.displayName} An Invite");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> An Invite");
                                    Logs.Log("You Sent Someone An Invite");
                                }));
                            }
                            break;

                        case "requestInvite":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Request");
                                Logs.Log($"You Sent {user.displayName} An Invite Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Request");
                                    Logs.Log($"You Sent {user.displayName} An Invite Request");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> An Invite Request");
                                    Logs.Log("You Sent Someone An Invite Request");
                                }));
                            }
                            break;

                        case "requestInviteResponse":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                Logs.Log($"You Sent {user.displayName} An Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                    Logs.Log($"You Sent {user.displayName} An Invite Response");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> An Invite Response");
                                    Logs.Log("You Sent Someone An Invite Response");
                                }));
                            }
                            break;

                        case "message":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Message");
                                Logs.Log($"You Sent {user.displayName} A Message");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Message");
                                    Logs.Log($"You Sent {user.displayName} A Message");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> A Message");
                                    Logs.Log("You Sent Someone A Message");
                                }));
                            }
                            break;

                        case "inviteResponse":
                            if (PlayerUtils.APIUserIsCached(__0.receiverUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.receiverUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                Logs.Log($"You Sent {user.displayName} An Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                    Logs.Log($"You Sent {user.displayName} An Invite Response");
                                }), new Action<string>(_ =>
                                {
                                    Logs.Debug("You Sent <i>Someone</i> An Invite Response");
                                    Logs.Log("You Sent Someone An Invite Response");
                                }));
                            }
                            break;

                        default:
                            Logs.Log($"YOU Sent A NOTIFICATION OF AN UN RECOGNIZED TYPE! Please let Blaze know in the discord server (if you send it via dms your key is getting revoked i swear to fucking god)! | Type: [{__0.type}]", ConsoleColor.DarkYellow);
                            break;
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool CaughtEventPatch(ref VRC.Player __0, ref VrcEvent __1, ref VrcBroadcastType __2)
        {
            bool result = true;
            try
            {
                var player = __0;
                var PlayerName = player.GetAPIUser().displayName;
                var PlayerID = player.GetAPIUser().id;
                var ObjectName = __1.ParameterObject.name;
                var NameColored = $"<color={player.prop_APIUser_0.GetTrueRankColor()}>{PlayerName}</color>";
                Il2CppSystem.Object[] RawParameters = Networking.DecodeParameters(__1.ParameterBytes);
                string ParametersString = "";
                for (int i = 0; i < RawParameters.Length; i++) ParametersString += Il2CppSystem.Convert.ToString(RawParameters[i]) + "|";

                if (ObjectName != "USpeak" && __1.ParameterString != "SanityCheck" && Config.Main.LogRPCS)
                {
                    Logs.Log($"[RPC] Sender: {PlayerName} | OBJ: {ObjectName} | RPC: {__1.ParameterString} | VALUES: [{ParametersString}] | TYPE: {__2}", ConsoleColor.Cyan);
                }

                switch (ObjectName)
                {
                    case "SceneEventHandlerAndInstantiator":
                        switch (__1.ParameterString)
                        {
                            case "_InstantiateObject":
                                if (!ParametersString.ToLower().Contains("portal") && Config.Main.LogRPCObjectInstantiated)
                                {
                                    Logs.Log($"{PlayerName} instantiated an object!");
                                    Logs.Debug($"{NameColored} instantiated an object!");
                                }
                                //[05:45:40.117] [Apollo] [RPC] Sender: Netherrack | OBJ: SceneEventHandlerAndInstantiator | RPC: _InstantiateObject | VALUES: [Portals/PortalInternalDynamic|(0.4, 0.0, 3.0)|(0.0, 1.0, 0.0, 0.0)|100005|] | TYPE: Always
                                break;

                            case "_SendOnSpawn":
                                //[05:45:40.121] [Apollo] [RPC] Sender: Netherrack | OBJ: SceneEventHandlerAndInstantiator | RPC: _SendOnSpawn | VALUES: [100005|] | TYPE: AlwaysUnbuffered
                                break;

                            case "_DestroyObject":
                                if (Config.Main.LogRPCObjectDestroyed)
                                {
                                    if (CachedPortals.Exists(x => x.ObjectID == ParametersString.Split('|')[0]))
                                    {
                                        var portalInfo = CachedPortals.Find(x => x.ObjectID == ParametersString.Split('|')[0]);
                                        Logs.Log($"{PlayerName} destroyed {portalInfo.DisplayName}'s Portal!");
                                        Logs.Debug($"{NameColored} destroyed <color={Colors.ModUserHex}>{portalInfo.DisplayName}</color>'s Portal!");
                                        CachedPortals.Remove(portalInfo);
                                    }
                                    else
                                    {
                                        Logs.Log($"{PlayerName} Destroyed an Object!");
                                        Logs.Debug($"{NameColored} Destroyed an Object!");
                                    }
                                }
                                //[05:46:11.621] [Apollo] [RPC] Sender: Netherrack | OBJ: SceneEventHandlerAndInstantiator | RPC: _DestroyObject | VALUES: [100005|] | TYPE: AlwaysUnbuffered
                                break;
                        }
                        break;

                    case "Indicator":
                        switch (__1.ParameterString)
                        {
                            case "ChangeVisibility":
                                if (Config.Main.LogRPCCameraToggled)
                                {
                                    var camState = Il2CppSystem.Convert.ToBoolean(RawParameters[0]) ? "Enabled" : "Disabled";
                                    Logs.Log($"{PlayerName} {camState} their Camera!");
                                    Logs.Debug($"{NameColored} {camState} their Camera!");
                                    //if (PlayerName == PlayerUtils.CurrentUser().GetDisplayName()) result = false;
                                }
                                //[05:40:15.418] [Apollo] [RPC] Sender: toxicboith1 | OBJ: Indicator | RPC: ChangeVisibility | VALUES: [True|] | TYPE: AlwaysBufferOne
                                break;

                            case "PhotoCapture":
                                if (Config.Main.LogRPCPhotoTaken)
                                {
                                    Logs.Log($"{PlayerName} took a photo!");
                                    Logs.Debug($"{NameColored} took a photo!");
                                }
                                //[05:43:43.252] [Apollo] [RPC] Sender: Netherrack | OBJ: Indicator | RPC: PhotoCapture | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "TimerBloop":
                                if (Config.Main.LogRPCCameraTimer)
                                {
                                    Logs.Log($"{PlayerName} is using camera timer!");
                                    Logs.Debug($"{NameColored} is using camera timer!");
                                }
                                //[05:43:38.207] [Apollo] [RPC] Sender: Netherrack | OBJ: Indicator | RPC: TimerBloop | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;
                        }
                        break;

                    case { } a when a.Contains("portal"):
                        switch (__1.ParameterString)
                        {
                            case "SetTimerRPC":
                                //[05:23:54.436] [Apollo] [RPC] Sender: Netherrack | OBJ: (Clone [100005] Portals/PortalInternalDynamic) | RPC: SetTimerRPC | VALUES: [1.000992|] | TYPE: AlwaysBufferOne
                                break;

                            case "ConfigurePortal":
                                if (Config.Main.LogRPCPortalDropped)
                                {
                                    Logs.Log($"{PlayerName} dropped a portal!");
                                    Logs.Debug($"{NameColored} dropped a portal!");
                                }
                                //[05:23:53.428] [Apollo] [RPC] Sender: Netherrack | OBJ: (Clone [100005] Portals/PortalInternalDynamic) | RPC: ConfigurePortal | VALUES: [wrld_791ebf58-54ce-4d3a-a0a0-39f10e1b20b2|86142|2|] | TYPE: AlwaysBufferOne
                                break;

                            case "PlayEffect":
                                if (Config.Main.LogRPCEnteredPortal)
                                {
                                    Logs.Log($"{PlayerName} entered a portal!");
                                    Logs.Debug($"{NameColored} entered a portal!");
                                }
                                //[05:27:52.730] [Apollo] [RPC] Sender: Dustfennec | OBJ: (Clone [200005] Portals/PortalInternalDynamic) | RPC: PlayEffect | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "IncrementPortalPlayerCountRPC":
                                //[05:26:00.822] [Apollo] [RPC] Sender: Netherrack | OBJ: (Clone [100006] Portals/PortalInternalDynamic) | RPC: IncrementPortalPlayerCountRPC | VALUES: [] | TYPE: Always
                                break;
                        }
                        break;

                    default:
                        switch (__1.ParameterString)
                        {
                            case "SpawnEmojiRPC":
                                if (Config.Main.LogRPCSpawnedEmoji)
                                {
                                    var emojiNumber = Il2CppSystem.Convert.ToInt32(RawParameters[0]);
                                    if (emojiNumber > Functions.EmojiType.Count || emojiNumber < 0)
                                    {
                                        Logs.Log($"{PlayerName} spawned an invalid emoji!");
                                        Logs.Debug($"{NameColored} spawned an <i><color=yellow>invalid emoji</color></i>!");
                                    }
                                    else
                                    {
                                        var emojiType = Functions.EmojiType[emojiNumber];
                                        Logs.Log($"{PlayerName} spawned the emoji [{emojiType}]!");
                                        Logs.Debug($"{NameColored} spawned the emoji [{emojiType}]!");
                                    }
                                }
                                //[05:21:08.040] [Apollo] [RPC] Sender: Netherrack | OBJ: VRCPlayer[Local] 51240259 1 | RPC: SpawnEmojiRPC | VALUES: [1|] | TYPE: AlwaysUnbuffered
                                break;

                            case "PlayEmoteRPC":
                                if (Config.Main.LogRPCPlayedEmote)
                                {
                                    var emoteNumber = Il2CppSystem.Convert.ToInt32(RawParameters[0]);
                                    if (emoteNumber > Functions.EmoteType.Count || emoteNumber < 0)
                                    {
                                        Logs.Log($"{PlayerName} played an invalid emote!");
                                        Logs.Debug($"{NameColored} played an <i><color=yellow>invalid emote</color></i>!");
                                    }
                                    else
                                    {
                                        var emoteType = Functions.EmoteType[emoteNumber];
                                        Logs.Log($"{PlayerName} played the emote [{emoteType}]!");
                                        Logs.Debug($"{NameColored} played the emote [{emoteType}]!");
                                    }
                                }
                                //[05:22:00.069] [Apollo] [RPC] Sender: Netherrack | OBJ: VRCPlayer[Local] 51240259 1 | RPC: PlayEmoteRPC | VALUES: [1|] | TYPE: AlwaysUnbuffered
                                break;

                            case "CancelRPC":
                                if (Config.Main.LogRPCStoppedEmote)
                                {
                                    Logs.Log($"{PlayerName} stopped playing an emote!");
                                    Logs.Debug($"{NameColored} stopped playing an emote!");
                                }
                                //[05:22:00.807] [Apollo] [RPC] Sender: Netherrack | OBJ: AnimationController | RPC: CancelRPC | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "ReloadAvatarNetworkedRPC":
                                if (Config.Main.LogRPCReloadedAvatar)
                                {
                                    Logs.Log($"{PlayerName} reloaded their avatar!");
                                    Logs.Debug($"{NameColored} reloaded their avatar!");
                                }
                                //[05:23:03.337] [Apollo] [RPC] Sender: Netherrack | OBJ: VRCPlayer[Local] 51240259 1 | RPC: ReloadAvatarNetworkedRPC | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "UdonSyncRunProgramAsRPC":
                                //[05:40:16.761] [Apollo] [RPC] Sender: Netherrack | OBJ: ScoreRow (3) | RPC: UdonSyncRunProgramAsRPC | VALUES: [RemoteBehaviorEnabled|] | TYPE: AlwaysUnbuffered
                                result = !Config.Main.AntiUdon;
                                switch (ParametersString.Split('|')[0])
                                {
                                    // Ghost World
                                    case { } a when a.StartsWith("HitDamage"):
                                        result = !Toggles.GodMode;
                                        break;

                                    // Jar Worlds
                                    case "KillLocalPlayer":
                                        result = !Toggles.GodMode;
                                        break;
                                }
                                break;

                            case "damaged":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !Toggles.GodMode;
                                }
                                break;

                            case "Drop":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !Toggles.GodMode;
                                }
                                break;

                            case "DropOnHitCooldown":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !Toggles.GodMode;
                                }
                                break;

                            case "FireCooldown":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !Toggles.GodMode;
                                }
                                break;

                            case "cooled down":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !Toggles.GodMode;
                                }
                                break;
                        }
                        break;
                }

                if (__1.EventType == VrcEventType.TeleportPlayer) result = !Config.Main.AntiTeleport;
                if (__2 == VrcBroadcastType.Always || __2 == VrcBroadcastType.AlwaysUnbuffered || __2 == VrcBroadcastType.AlwaysBufferOne)
                {
                    if (PlayerID != APIUser.CurrentUser.id && Config.Main.AntiWorldTriggers)
                    {
                        result = false;
                    }
                }
                if (__2 == VrcBroadcastType.Local && Toggles.SDK2WorldTriggers) __2 = VrcBroadcastType.AlwaysUnbuffered;

                if (__1.ParameterBytes.Length > 1000 || __1.ParameterString.Length > 60 && Config.Main.AntiDesync)
                {
                    if (!string.IsNullOrEmpty(__1.ParameterString))
                    {
                        //ConsoleLogs.Msg($"Blocked Modified RPC ({__1.ParameterString}) from {PlayerName} - {PlayerID} [{__1.ParameterBytes.Length}]", ConsoleColor.Cyan);
                        __1.ParameterBytes = new byte[0];
                        __1.ParameterString = "";
                        __1.ParameterObject = null;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch { }
            return result;
        }

        private static void PipelinePatch(ref Il2CppSystem.Object __0, ref MessageEventArgs __1)
        {
            try
            {
                var WebSocketRawData = JsonConvert.DeserializeObject<VRCWebSocketObject>(__1.Data);
                var WebSocketData = JsonConvert.DeserializeObject<VRCWebSocketContent>(WebSocketRawData?.content);
                var apiuser = WebSocketData?.user;
                if (WebSocketData.userId == APIUser.CurrentUser.id) return;

                switch (WebSocketRawData?.type)
                {
                    case "user-location": // Self Location Update (called on first world load in)
                        break;

                    case "friend-update": // friend updates personal info
                        break;

                    case "notification": // Whenever any form of a notification is sent or recieved (ignoring this since we manually patch Transmtn Put & Send)
                        break;

                    case "friend-active": // Whenever someone logs into VRChat's Website
                        if (Config.Main.PipelineOnline)
                        {
                            if (Config.Main.LogToHud)
                            {
                                if (Config.Main.PipelineHudOnlyFavs)
                                {
                                    if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                }
                                Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> -> <color=yellow>VRC's Website</color>", 3.5f);
                            }
                            Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> logged into <color=yellow>VRC's Website</color>");
                            Logs.Log($"[PIPELINE] {apiuser.displayName} logged into VRC's Website!", ConsoleColor.DarkCyan);
                        }
                        break;

                    case "friend-online": // Whenever someone comes online
                        if (Config.Main.PipelineOnline)
                        {
                            if (Config.Main.LogToHud)
                            {
                                if (Config.Main.PipelineHudOnlyFavs)
                                {
                                    if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                }
                                Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> -> <color=green>online</color>", 3.5f);
                            }
                            Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> has come <color=green>online</color>");
                            Logs.Log($"[PIPELINE] {apiuser.displayName} has come online!", ConsoleColor.DarkCyan);
                        }
                        break;

                    case "friend-offline": // Whenever someone goes offline
                        if (Config.Main.PipelineOffline)
                        {
                            if (PlayerUtils.APIUserIsCached(WebSocketData.userId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(WebSocketData.userId);
                                if (Config.Main.LogToHud)
                                {
                                    if (Config.Main.PipelineHudOnlyFavs)
                                    {
                                        if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                    }
                                    Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{user.displayName}</color> -> <color=red>offline</color>", 3.5f);
                                }
                                Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{user.displayName}</color> has gone <color=red>offline</color>");
                                Logs.Log($"[PIPELINE] {user.displayName} has gone offline!", ConsoleColor.DarkCyan);
                            }
                            else
                            {
                                APIUser.FetchUser(WebSocketData.userId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRanks.CachedApiUsers.Contains(user))
                                    {
                                        TrueRanks.CachedApiUsers.Add(user);
                                    }
                                    if (Config.Main.LogToHud)
                                    {
                                        if (Config.Main.PipelineHudOnlyFavs)
                                        {
                                            if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                        }
                                        Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{user.displayName}</color> -> <color=red>offline</color>", 3.5f);
                                    }
                                    Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{user.displayName}</color> has gone <color=red>offline</color>");
                                    Logs.Log($"[PIPELINE] {user.displayName} has gone offline!", ConsoleColor.DarkCyan);

                                }), new Action<string>(_ =>
                                {
                                    if (Config.Main.LogToHud)
                                    {
                                        if (Config.Main.PipelineHudOnlyFavs)
                                        {
                                            if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                        }
                                        Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>someone</color> -> <color=red>offline</color>", 3.5f);
                                    }
                                    Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>someone</color> has gone <color=red>offline</color>");
                                    Logs.Log($"[PIPELINE] {WebSocketData.userId} has gone offline!", ConsoleColor.DarkCyan);
                                }));
                            }
                                
                            
                        }
                        break;

                    case "friend-location": // Whenever someone changes worlds
                        if (Config.Main.PipelineLocations)
                        {
                            if (Config.Main.LogToHud)
                            {
                                if (Config.Main.PipelineHudOnlyFavs)
                                {
                                    if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                }
                                Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> -> <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>", 3.5f);
                            }
                            Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> went to <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>");
                            Logs.Log($"[PIPELINE] {apiuser.displayName} went to {(WebSocketData.location == "private" ? "[PRIVATE]" : $"{WebSocketData.world.name} [{WebSocketData.location}]")}!", ConsoleColor.DarkCyan);
                        }
                        break;

                    case "friend-delete": // Whenever someone remove you as a friend
                        if (Config.Main.PipelineRemoved)
                        {
                            if (Config.Main.LogToHud)
                            {
                                Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> -> <color=yellow>REMOVED</color>", 3.5f);
                            }
                            Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> has <color=yellow>removed you</color>");
                            Logs.Log($"[PIPELINE] {apiuser.displayName} has removed you!", ConsoleColor.DarkCyan);
                        }
                        break;

                    case "friend-add": // Whenever someone adds you as a friend
                        if (Config.Main.PipelineAdded)
                        {
                            if (Config.Main.LogToHud)
                            {
                                if (Config.Main.PipelineHudOnlyFavs)
                                {
                                    if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                                }
                                Logs.HUD($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> -> <color=yellow>ADDED</color>", 3.5f);
                            }
                            Logs.Debug($"<color={Colors.GreyHex}>[PIPELINE]</color> <color={Colors.ModUserHex}>{apiuser.displayName}</color> has <color=yellow>added you</color>");
                            Logs.Log($"[PIPELINE] {apiuser.displayName} has added you!", ConsoleColor.DarkCyan);
                        }
                        break;

                    default:
                        Logs.Log($"[PIPELINE] Unrecognized Type: {WebSocketRawData.type}\n{JsonConvert.SerializeObject(WebSocketData, Formatting.Indented)}");
                        break;
                }
            }
            catch (Exception e)
            {
                Logs.Error("Pipeline", e);
                if (!__1.Data.Contains("user-location"))
                {
                    Logs.Log(__1.Data);
                }
            }
        }
        #endregion
    }
}
