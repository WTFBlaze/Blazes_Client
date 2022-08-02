using Blaze.API.AW;
using Blaze.Configs;
using Blaze.Modules;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Transmtn.DTO.Notifications;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.Networking;
using VRC.SDKBase;
using VRC.UI.Elements.Menus;
using static Blaze.Modules.AntiCrash;
using static Blaze.Utils.Objects.ModObjects;
using static VRC.SDKBase.VRC_EventHandler;

namespace Blaze
{
    internal class Patching
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidDelegate(IntPtr thisPtr, IntPtr nativeMethodInfo);
        internal static VRCAvatarManager cachedAvatarManager;
        internal static readonly List<object> antiGCList = new();
        //private static Patch QuestPatch;
        internal static List<ModPortal> CachedPortals = new();
        private static readonly Dictionary<string, string> StopDuplicatingAviTextFML = new();

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

        public static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Patching).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
        }

        internal static void Initialize()
        {
            try
            {
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Update)), GetPatch(nameof(ReturnFalse)), null);
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Start)), GetPatch(nameof(ReturnFalse)), null);
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.OnEnable)), GetPatch(nameof(ReturnFalse)), null);
                new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServer)), GetPatch(nameof(ReturnFalse)), null);
                new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServerDelayed)), GetPatch(nameof(ReturnFalse)), null);

                new Patch(AccessTools.Method(typeof(VRC_EventDispatcherRFC), nameof(VRC_EventDispatcherRFC.Method_Public_Boolean_Player_VrcEvent_VrcBroadcastType_0)), GetPatch(nameof(CaughtEventPatch)));
                new Patch(AccessTools.Method(typeof(LoadBalancingClient), "Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0"), GetPatch(nameof(OpRaiseEvent)));
                new Patch(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.OnEvent)), GetPatch(nameof(OnEvent)));
                new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_1)), GetPatch(nameof(PlayerLeft)), null);
                new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_0)), GetPatch(nameof(PlayerJoin)), null);
                new Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), GetPatch(nameof(LeftRoom)), null);
                new Patch(typeof(PortalTrigger).GetMethod(nameof(PortalTrigger.OnTriggerEnter), BindingFlags.Public | BindingFlags.Instance), GetPatch(nameof(EnterPortalPatch)));
                new Patch(typeof(PortalInternal).GetMethod("Method_Private_Void_0"), GetPatch(nameof(PortalDestroyPatch)), null);
                new Patch(AccessTools.Method(typeof(Transmtn.PostOffice), "Put"), GetPatch(nameof(GetNotificationPatch)));
                new Patch(AccessTools.Method(typeof(Transmtn.PostOffice), "Send"), GetPatch(nameof(SentNotificationPatch)));
                new Patch(typeof(FlatBufferNetworkSerializer).GetMethod(nameof(FlatBufferNetworkSerializer.Method_Public_Void_EventData_0)), new HarmonyMethod(typeof(FlatBufferSanitizer).GetMethod("FlatBufferNetworkSerializeReceivePatch", BindingFlags.NonPublic | BindingFlags.Static)));
                new Patch(typeof(AssetManagement).GetMethods().First(mb => mb.Name.StartsWith("Method_Public_Static_Object_Object_Boolean_Boolean_Boolean_") && XRefManager.CheckUsed(mb, "__Instantiate__UnityEngineGameObject__UnityEngineGameObject")), GetPatch(nameof(OnObjectInstantiated)));
                new Patch(typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake)), null, GetPatch(nameof(OnAwakePatch)));
                new Patch(AccessTools.Property(typeof(PhotonPeer), nameof(PhotonPeer.RoundTripTime)).GetMethod, null, GetPatch(nameof(FramesSpoof)));
                new Patch(AccessTools.Property(typeof(Time), nameof(Time.smoothDeltaTime)).GetMethod, null, GetPatch(nameof(PingSpoof)));
                //new Patch(AccessTools.Method(typeof(VRC.Core.API), "SendPutRequest"), GetPatch(nameof(OnlineLocationPatch))); - Detected!
                //QuestPatch = new Patch(AccessTools.Property(typeof(Tools), nameof(Tools.Platform)).GetMethod, null, GetPatch(nameof(QuestSpoof))); - Detected!

                if (BlazesXRefs.FriendNameTargetMethod != null)
                    new Patch(BlazesXRefs.FriendNameTargetMethod, new HarmonyMethod(typeof(TrueRank).GetMethod("GetFriendlyDetailedNameForSocialRank", BindingFlags.NonPublic | BindingFlags.Static)));
                if (BlazesXRefs.ColorForRankMethods != null)
                    BlazesXRefs.ColorForRankMethods.ForEach(method => new Patch(method, new HarmonyMethod(typeof(TrueRank).GetMethod("GetColorForSocialRank", BindingFlags.NonPublic | BindingFlags.Static))));
                if (BlazesXRefs.OnPhotonPlayerJoinMethod != null)
                    new Patch(BlazesXRefs.OnPhotonPlayerJoinMethod, GetPatch(nameof(PhotonJoin)));
                if (BlazesXRefs.OnPhotonPlayerLeftMethod != null)
                    new Patch(BlazesXRefs.OnPhotonPlayerLeftMethod, GetPatch(nameof(PhotonLeft)));
                if (BlazesXRefs.PlaceUiMethod != null)
                    new Patch(BlazesXRefs.PlaceUiMethod, GetPatch(nameof(PlaceUiPatch)));
                if (BlazesXRefs.ActionWheelMethod != null)
                    new Patch(BlazesXRefs.ActionWheelMethod, null, GetPatch(nameof(OpenMainPage)));
                //if (BlazesXRefs.ApplyPlayerMotionMethod != null)
                //    new Patch(BlazesXRefs.ApplyPlayerMotionMethod, GetPatch(nameof(ApplyPlayerMotionPatch)));

                foreach (var method in typeof(SelectedUserMenuQM).GetMethods())
                {
                    if (!method.Name.StartsWith("Method_Private_Void_IUser_PDM_")) continue;
                    if (XrefScanner.XrefScan(method).Count() < 3) continue;
                    new Patch(method, null, GetPatch(nameof(SetUserPatch)));
                }

                foreach (var methodInfo in typeof(VRCFlowManager).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (methodInfo.ReturnType != typeof(void) || methodInfo.GetParameters().Length != 0)
                        continue;

                    if (!XrefScanner.XrefScan(methodInfo).Any(it => it.Type == XrefType.Global && it.ReadAsObject()?.ToString() == "Going to Home Location: "))
                        continue;

                    new Patch(methodInfo, null, GetPatch(nameof(GoHomePatch)));
                }
            }
            catch (Exception e)
            {
                Logs.Error("Hey uhh... if you are seeing this then something is seriously wrong. Please contact Blaze immediately and report this error... | Error Message: " + e.Message);
            }
            finally 
            { 
                Patch.DoPatches();
                SetupCachedAvatarManager();
            }
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

                if (ObjectName != "USpeak" && __1.ParameterString != "SanityCheck" && MainConfig.Instance.LogRPCS)
                {
                    Logs.Log($"[RPC] Sender: {PlayerName} | OBJ: {ObjectName} | RPC: {__1.ParameterString} | VALUES: [{ParametersString}] | TYPE: {__2}", ConsoleColor.Cyan);
                }

                switch (ObjectName)
                {
                    case "SceneEventHandlerAndInstantiator":
                        switch (__1.ParameterString)
                        {
                            case "_InstantiateObject":
                                if (!ParametersString.ToLower().Contains("portal") && MainConfig.Instance.LogRPCObjectInstantiated)
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
                                if (MainConfig.Instance.LogRPCObjectDestroyed)
                                {
                                    if (CachedPortals.Exists(x => x.ObjectID == ParametersString.Split('|')[0]))
                                    {
                                        var portalInfo = CachedPortals.Find(x => x.ObjectID == ParametersString.Split('|')[0]);
                                        Logs.Log($"{PlayerName} destroyed {portalInfo.DisplayName}'s Portal!");
                                        Logs.Debug($"{NameColored} destroyed <color={BlazeInfo.UserColor}>{portalInfo.DisplayName}</color>'s Portal!");
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
                                if (MainConfig.Instance.LogRPCCameraToggled)
                                {
                                    var camState = Il2CppSystem.Convert.ToBoolean(RawParameters[0]) ? "Enabled" : "Disabled";
                                    Logs.Log($"{PlayerName} {camState} their Camera!");
                                    Logs.Debug($"{NameColored} {camState} their Camera!");
                                }
                                //[05:40:15.418] [Apollo] [RPC] Sender: toxicboith1 | OBJ: Indicator | RPC: ChangeVisibility | VALUES: [True|] | TYPE: AlwaysBufferOne
                                break;

                            case "PhotoCapture":
                                if (MainConfig.Instance.LogRPCPhotoTaken)
                                {
                                    Logs.Log($"{PlayerName} took a photo!");
                                    Logs.Debug($"{NameColored} took a photo!");
                                }
                                //[05:43:43.252] [Apollo] [RPC] Sender: Netherrack | OBJ: Indicator | RPC: PhotoCapture | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "TimerBloop":
                                if (MainConfig.Instance.LogRPCCameraTimer)
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
                                if (MainConfig.Instance.LogRPCPortalDropped)
                                {
                                    Logs.Log($"{PlayerName} dropped a portal!");
                                    Logs.Debug($"{NameColored} dropped a portal!");
                                }
                                //[05:23:53.428] [Apollo] [RPC] Sender: Netherrack | OBJ: (Clone [100005] Portals/PortalInternalDynamic) | RPC: ConfigurePortal | VALUES: [wrld_791ebf58-54ce-4d3a-a0a0-39f10e1b20b2|86142|2|] | TYPE: AlwaysBufferOne
                                break;

                            case "PlayEffect":
                                if (MainConfig.Instance.LogRPCEnteredPortal)
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
                                if (MainConfig.Instance.LogRPCSpawnedEmoji)
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
                                if (MainConfig.Instance.LogRPCPlayedEmote)
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
                                if (MainConfig.Instance.LogRPCStoppedEmote)
                                {
                                    Logs.Log($"{PlayerName} stopped playing an emote!");
                                    Logs.Debug($"{NameColored} stopped playing an emote!");
                                }
                                //[05:22:00.807] [Apollo] [RPC] Sender: Netherrack | OBJ: AnimationController | RPC: CancelRPC | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "ReloadAvatarNetworkedRPC":
                                if (MainConfig.Instance.LogRPCReloadedAvatar)
                                {
                                    Logs.Log($"{PlayerName} reloaded their avatar!");
                                    Logs.Debug($"{NameColored} reloaded their avatar!");
                                }
                                //[05:23:03.337] [Apollo] [RPC] Sender: Netherrack | OBJ: VRCPlayer[Local] 51240259 1 | RPC: ReloadAvatarNetworkedRPC | VALUES: [] | TYPE: AlwaysUnbuffered
                                break;

                            case "UdonSyncRunProgramAsRPC":
                                //[05:40:16.761] [Apollo] [RPC] Sender: Netherrack | OBJ: ScoreRow (3) | RPC: UdonSyncRunProgramAsRPC | VALUES: [RemoteBehaviorEnabled|] | TYPE: AlwaysUnbuffered
                                result = !MainConfig.Instance.AntiUdon;
                                switch (ParametersString.Split('|')[0])
                                {
                                    // Ghost World
                                    case { } a when a.StartsWith("HitDamage"):
                                        result = !MainConfig.Instance.GodMode;
                                        break;

                                    // Jar Worlds
                                    case "KillLocalPlayer":
                                        result = !MainConfig.Instance.GodMode;
                                        break;
                                }
                                break;

                            case "damaged":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !MainConfig.Instance.GodMode;
                                }
                                break;

                            case "Drop":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !MainConfig.Instance.GodMode;
                                }
                                break;

                            case "DropOnHitCooldown":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !MainConfig.Instance.GodMode;
                                }
                                break;

                            case "FireCooldown":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !MainConfig.Instance.GodMode;
                                }
                                break;

                            case "cooled down":
                                if (PlayerID == APIUser.CurrentUser.id)
                                {
                                    result = !MainConfig.Instance.GodMode;
                                }
                                break;
                        }
                        break;
                }

                if (__1.EventType == VrcEventType.TeleportPlayer) result = !MainConfig.Instance.AntiTeleport;
                if (__2 == VrcBroadcastType.Always || __2 == VrcBroadcastType.AlwaysUnbuffered || __2 == VrcBroadcastType.AlwaysBufferOne)
                {
                    if (PlayerID != APIUser.CurrentUser.id && MainConfig.Instance.AntiWorldTriggers)
                    {
                        result = false;
                    }
                }
                if (__2 == VrcBroadcastType.Local && BlazeInfo.WorldTriggers) __2 = VrcBroadcastType.AlwaysUnbuffered;

                if (__1.ParameterBytes.Length > 1000 || __1.ParameterString.Length > 60 && MainConfig.Instance.AntiDesync)
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

        private static bool OpRaiseEvent(byte __0, object __1, RaiseEventOptions __2)
        {
            try
            {
                switch (__0)
                {
                    /*// Invisible Join
                    case 202:
                        __2.field_Public_ReceiverGroup_0 = BlazeVars.InvisibleJoin ? ReceiverGroup.MasterClient : ReceiverGroup.Others;
                        BlazeVars.InvisibleJoin = false;
                        QM.InvisibleJoin.setToggleState(false);
                        break;*/

                    /*// Force Mute
                    case 1: return !GlobalVariables.ForceMute;*/

                    // Master Lock
                    case 4: return !BlazeInfo.MasterLockInstance;
                    case 5: return !BlazeInfo.MasterLockInstance;
                    case 6: return !BlazeInfo.MasterLockInstance;
                }
                if (__1 != null && __2 != null) return __0 != 7 || !BlazeInfo.Serialization;
            }
            catch { }
            return true;
        }

        private static void PlayerJoin(ref VRC.Player __0)
        {
            try
            {
                if (MainConfig.Instance.LogPlayersJoin && __0.GetUserID() != PlayerUtils.CurrentUser().GetUserID() && PlayerUtils.CurrentUser().field_Private_VRCAvatarManager_0.field_Private_GameObject_0 != null)
                {
                    if (MainConfig.Instance.LogToHud)
                    {
                        if (MainConfig.Instance.LogOnlyFriendsToHud)
                        {
                            if (__0.field_Private_APIUser_0.isFriend)
                            {
                                Logs.HUD($"<color=#e1add3>{__0.GetDisplayName()}</color> <color=green>joined</color>", 3f);
                            }
                        }
                        else
                        {
                            Logs.HUD($"<color=#e1add3>{__0.GetDisplayName()}</color> <color=green>joined</color>", 3f);
                        }
                    }
                    Logs.Log($"[JOIN] {__0.GetDisplayName()}", ConsoleColor.Green);
                    Logs.Debug($"[<color=green>+</color>] <color=#e1add3>{__0.GetDisplayName()}</color>");
                }

                foreach (var m in BlazeInfo.Modules) m.PlayerJoined(__0);
            }
            catch { }
        }

        private static void PlayerLeft(ref VRC.Player __0)
        {
            try
            {
                if (MainConfig.Instance.LogPlayersLeave && __0.GetUserID() != PlayerUtils.CurrentUser().GetUserID() && PlayerUtils.CurrentUser().field_Private_VRCAvatarManager_0.field_Private_GameObject_0 != null)
                {
                    if (MainConfig.Instance.LogToHud)
                    {
                        if (MainConfig.Instance.LogOnlyFriendsToHud)
                        {
                            if (__0.field_Private_APIUser_0.isFriend)
                            {
                                Logs.HUD($"<color=#e1add3>{__0.GetDisplayName()}</color> <color=red>left</color>", 3f);
                            }
                        }
                        else
                        {
                            Logs.HUD($"<color=#e1add3>{__0.GetDisplayName()}</color> <color=red>left</color>", 3f);
                        }
                    }
                    Logs.Log($"[LEFT] {__0.GetDisplayName()}", ConsoleColor.Red);
                    Logs.Debug($"[<color=red>-</color>] <color=#e1add3>{__0.GetDisplayName()}</color>");
                }

                foreach (var m in BlazeInfo.Modules) m.PlayerLeft(__0);
            }
            catch { }
        }

        private static void PhotonJoin(ref Photon.Realtime.Player __0)
        {
            try
            {
                if (MainConfig.Instance.LogPhotonsJoin && __0.GetUserID() != PlayerUtils.CurrentUser().GetUserID() && PlayerUtils.CurrentUser().field_Private_VRCAvatarManager_0.field_Private_GameObject_0 != null)
                {
                    if (MainConfig.Instance.LogToHud)
                    {
                        if (MainConfig.Instance.LogOnlyFriendsToHud)
                        {
                            if (__0.GetPlayer().GetAPIUser().isFriend)
                            {
                                Logs.HUD($"<color=yellow>[P]</color> <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>) joining", 3f);
                            }
                        }
                        else
                        {
                            Logs.HUD($"<color=yellow>[P]</color> <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>) joining", 3f);
                        }
                    }
                    Logs.Log($"[PHOTON] {__0.GetDisplayName()} ({__0.GetPhotonID()}) is joining");
                    Logs.Debug($"[P<color=green>+</color>] <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>)");
                }
                foreach (var m in BlazeInfo.Modules) m.PhotonJoined(__0);
            }
            catch { }
        }

        private static void PhotonLeft(ref Photon.Realtime.Player __0)
        {
            try
            {
                if (MainConfig.Instance.LogPhotonsLeave && __0.GetUserID() != PlayerUtils.CurrentUser().GetUserID() && PlayerUtils.CurrentUser().field_Private_VRCAvatarManager_0.field_Private_GameObject_0 != null)
                {
                    if (MainConfig.Instance.LogToHud)
                    {
                        if (MainConfig.Instance.LogOnlyFriendsToHud)
                        {
                            if (__0.GetPlayer().GetAPIUser().isFriend)
                            {
                                Logs.HUD($"<color=yellow>[P]</color> <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>) left", 3f);
                            }
                        }
                        else
                        {
                            Logs.HUD($"<color=yellow>[P]</color> <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>) left", 3f);
                        }
                    }
                    Logs.Log($"[PHOTON] {__0.GetDisplayName()} ({__0.GetPhotonID()}) is left");
                    Logs.Debug($"[P<color=red>-</color>] <color=#e1add3>{__0.GetDisplayName()}</color> (<color=yellow>{__0.GetPhotonID()}</color>)");
                }
                foreach (var m in BlazeInfo.Modules) m.PhotonJoined(__0);
            }
            catch { }
        }

        private static bool EnterPortalPatch(PortalTrigger __instance)
        {
            try
            {
                if (Vector3.Distance(PlayerUtils.CurrentUser().transform.position, __instance.transform.position) > 1) return false;
                if (MainConfig.Instance.AntiPortal) return false;
                if (MainConfig.Instance.PortalPrompt)
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

        public static void AntiLockInstance(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
            {
                IEnumerator VRCPlayerWait()
                {
                    while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        yield return null;
                    }
                    if (MainConfig.Instance.AntiInstanceLock)
                    {
                        /* If New Update Test This
                        MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique.field_Internal_Static_MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        */
                        VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Private_MonoBehaviourPrivateBo1SiObNuObSiIn1UIUnique_0.field_Private_Boolean_0 = true;
                    }
                }
                MelonCoroutines.Start(VRCPlayerWait());
            }
        }

        private static bool GetNotificationPatch(ref Notification __0)
        {
            try
            {
                var username = __0.senderUsername;
                if (__0 != null && MainConfig.Instance.LogNotifications)
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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

                if (__0 != null && MainConfig.Instance.LogNotifications)
                {
                    switch (__0.type)
                    {
                        case "friendRequest":
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Friend Request");
                                Logs.Log($"You Sent {user.displayName} A Friend Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite");
                                Logs.Log($"You Sent {user.displayName} An Invite");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Request");
                                Logs.Log($"You Sent {user.displayName} An Invite Request");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                Logs.Log($"You Sent {user.displayName} An Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> A Message");
                                Logs.Log($"You Sent {user.displayName} A Message");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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
                            if (PlayerUtils.APIUserIsCached(__0.senderUserId))
                            {
                                var user = PlayerUtils.GetCachedAPIUser(__0.senderUserId);
                                Logs.Debug($"You Sent <color={user.GetRankColor()}>{user.displayName}</color> An Invite Response");
                                Logs.Log($"You Sent {user.displayName} An Invite Response");
                            }
                            else
                            {
                                APIUser.FetchUser(__0.receiverUserId, new Action<APIUser>(user =>
                                {
                                    if (!TrueRank.CachedApiUsers.Contains(user))
                                    {
                                        TrueRank.CachedApiUsers.Add(user);
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

        private static List<int> SentNoticeAboutInvisPlayer = new();
        private static List<int> SentNoticeAboutBotPlayer = new();

        private static bool OnEvent(ref EventData __0)
        {
            if (__0.Parameters == null) return true;
            bool result = true;
            try
            {
                var playerSender = PlayerManager.prop_PlayerManager_0.GetPlayerID(__0.sender);
                var photonSender = PhotonUtils.GetPhotonPlayer(__0.sender);
                var senderID = __0.sender;
                if (MainConfig.Instance.LogPhotonEvents)
                {
                    object Data = SerializationUtils.FromIL2CPPToManaged<object>(__0.Parameters);
                    if (__0.Code != 7 && __0.Code != 1)
                    {
                        Logs.Log($"[Event {__0.Code}] Event \n{JsonConvert.SerializeObject(Data, Formatting.Indented)}", ConsoleColor.Magenta);
                    }
                }

                /*if (photonSender.IsInvisible())
                {
                    if (PlayerUtils.CurrentUser() != null)
                    {
                        if (!SentNoticeAboutInvisPlayer.Contains(__0.sender))
                        {
                            Logs.HUD($"Blocking Events from (<color=yellow>{photonSender.GetDisplayName()}</color>) <i>[Invisible User]</i>", 3.5f);
                            SentNoticeAboutInvisPlayer.Add(__0.sender);
                            Functions.Delay(delegate
                            {
                                SentNoticeAboutInvisPlayer.Remove(senderID);
                            }, 10);
                        }
                        return false;
                    }
                }
                else
                {
                    if (playerSender._vrcplayer.IsBot())
                    {
                        if (PlayerUtils.CurrentUser() != null)
                        {
                            if (!SentNoticeAboutBotPlayer.Contains(__0.sender))
                            {
                                Logs.HUD($"Blocking Events from (<color=yellow>{photonSender.GetDisplayName()}</color>) <i>[Bot]</i>", 3.5f);
                                SentNoticeAboutBotPlayer.Add(__0.sender);
                                Functions.Delay(delegate
                                {
                                    SentNoticeAboutBotPlayer.Remove(senderID);
                                }, 10);
                            }
                            return false;
                        }
                    }
                }*/
                
                switch (__0.Code)
                {
                    // Voice Data
                    case 1:
                        if (BlazeInfo.ParrotMode)
                        {
                            if (playerSender.GetUserID() == BlazeInfo.Target.GetUserID())
                            {
                                PhotonUtils.OpRaiseEvent(1, __0.CustomData, new RaiseEventOptions()
                                {
                                    field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                                    field_Public_Byte_0 = 1,
                                    field_Public_Byte_1 = 1,

                                }, SendOptions.SendUnreliable);
                            }
                        }
                        break;

                    // Moderation Data
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
                                    if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationWarns)
                                    {
                                        Logs.Log("[Moderation] RoomOwner --> Warn You", ConsoleColor.DarkRed);
                                        if (MainConfig.Instance.LogToHud)
                                        {
                                            Logs.HUD("[Moderation] RoomOwner --> Warn You", 3.5f);
                                        }
                                    }
                                    result = !MainConfig.Instance.ModerationAntiWarns;
                                }
                                break;

                            //World Owner Mic OFF event
                            case 8:
                                if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationMicOff)
                                {
                                    Logs.Log("[Moderation] The RoomOwner --> MicOff You", ConsoleColor.DarkRed);
                                    if (MainConfig.Instance.LogToHud)
                                    {
                                        Logs.HUD("[Moderation] The RoomOwner --> MicOff You", 3.5f);
                                    }
                                }
                                result = !MainConfig.Instance.ModerationAntiMicOff;
                                break;

                            //Votekick Event
                            case 13:
                                string VoteMessage = InfoData[2] as string;
                                string RandomID = InfoData[3] as string;
                                if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationKicks)
                                {
                                    Logs.Log("[Moderation] [?] VoteKick --> [?]", ConsoleColor.DarkRed);
                                    if (MainConfig.Instance.LogToHud)
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
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationBlocks)
                                            {
                                                BlazeInfo.KnownBlocks.Add(SenderName);
                                                Logs.Log($"[Moderation] {SenderName} --> Blocked You", ConsoleColor.DarkRed);
                                                if (MainConfig.Instance.LogToHud)
                                                {
                                                    Logs.HUD($"[Moderation] {SenderName} --> Blocked You", 3.5f);
                                                }
                                                foreach (var m in BlazeInfo.Modules) m.BlockedPlayer(PhotonPlayer);
                                            }
                                        }
                                        else
                                        {
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationUnblocks)
                                            {
                                                if (BlazeInfo.KnownBlocks.Contains(SenderName))
                                                {
                                                    BlazeInfo.KnownBlocks.Remove(SenderName);
                                                    Logs.Log($"[Moderation] {SenderName} --> Unblocked You", ConsoleColor.DarkRed);
                                                    if (MainConfig.Instance.LogToHud)
                                                    {
                                                        Logs.HUD($"[Moderation] {SenderName} --> Unblocked You", 3.5f);
                                                    }
                                                    foreach (var m in BlazeInfo.Modules) m.UnBlockedPlayer(PhotonPlayer);
                                                }
                                            }
                                        }
                                        if (Muted)
                                        {
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationMutes)
                                            {
                                                BlazeInfo.KnownMutes.Add(SenderName);
                                                Logs.Log($"[Moderation] {SenderName} --> Muted You", ConsoleColor.DarkRed);
                                                if (MainConfig.Instance.LogToHud)
                                                {
                                                    Logs.HUD($"[Moderation] {SenderName} --> Muted You", 3.5f);
                                                }
                                                foreach (var m in BlazeInfo.Modules) m.MutedPlayer(PhotonPlayer);
                                            }
                                        }
                                        else
                                        {
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationUnmutes)
                                            {
                                                if (BlazeInfo.KnownMutes.Contains(SenderName))
                                                {
                                                    BlazeInfo.KnownMutes.Remove(SenderName);
                                                    Logs.Log($"[Moderation] {SenderName} --> Unmuted You", ConsoleColor.DarkRed);
                                                    if (MainConfig.Instance.LogToHud)
                                                    {
                                                        Logs.HUD($"[Moderation] {SenderName} --> Unmuted You", 3.5f);
                                                    }
                                                    foreach (var m in BlazeInfo.Modules) m.UnMutedPlayer(PhotonPlayer);
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
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationBlocks)
                                            {
                                                foreach (var blockid in BlockedList)
                                                {
                                                    var BlockPlayer = PhotonUtils.LoadBalancingPeer.GetPhotonPlayer(blockid);
                                                    Logs.Log($"[Moderation] {BlockPlayer.GetDisplayName()} Has you Blocked!", ConsoleColor.DarkRed);
                                                    if (MainConfig.Instance.LogToHud)
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
                                            if (MainConfig.Instance.ShowModerations && MainConfig.Instance.ModerationMutes)
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

        private static string StopDuplicatingBlacklistNoticeslol;
        private static string LastCheckedAvatarIDPosted;

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
                if (BlacklistedAvatars.blockList.Contains(cachedAvatarManager.prop_ApiAvatar_0.id) && user.id != PlayerUtils.CurrentUser().GetAPIUser().id)
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
                        limitedTransforms = ProcessTransform(transforms[i], limitedTransforms);
                    }

                    //Rigidbody
                    for (int i = 0; i < rigidbodies.Count; i++)
                    {
                        if (rigidbodies[i] == null)
                        {
                            continue;
                        }
                        ProcessRigidbody(rigidbodies[i]);
                    }

                    //Collider
                    int nukedColliders = 0;
                    for (int i = 0; i < colliders.Count; i++)
                    {
                        if (colliders[i] == null)
                        {
                            continue;
                        }
                        if (ProcessCollider(colliders[i]) == true)
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
                        if (ProcessJoint(joints[i]) == true)
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
                    AntiCrashClothPostProcess postProcessReport = new();
                    for (int i = 0; i < clothes.Count; i++)
                    {
                        if (clothes[i] == null)
                        {
                            continue;
                        }
                        postProcessReport = ProcessCloth(clothes[i], postProcessReport.nukedCloths, postProcessReport.currentVertexCount);
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
                    AntiCrashParticleSystemPostProcess postProcessReport = new();
                    for (int i = 0; i < particleSystems.Count; i++)
                    {
                        if (particleSystems[i] == null)
                        {
                            continue;
                        }
                        ProcessParticleSystem(particleSystems[i], ref postProcessReport);
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
                    AntiCrashDynamicBoneColliderPostProcess postDynamicBoneColliderProcessReport = new();
                    for (int i = 0; i < dynamicBoneColliders.Count; i++)
                    {
                        if (dynamicBoneColliders[i] == null)
                        {
                            continue;
                        }
                        postDynamicBoneColliderProcessReport = ProcessDynamicBoneCollider(dynamicBoneColliders[i], postDynamicBoneColliderProcessReport.nukedDynamicBoneColliders, postDynamicBoneColliderProcessReport.dynamicBoneColiderCount);
                    }

                    //DynamicBone
                    List<DynamicBone> dynamicBones = GameObjectManager.FindAllComponentsInGameObject<DynamicBone>(instantiatedGameObject);
                    AntiCrashDynamicBonePostProcess postDynamicBoneProcessReport = new();
                    for (int i = 0; i < dynamicBones.Count; i++)
                    {
                        if (dynamicBones[i] == null)
                        {
                            continue;
                        }
                        postDynamicBoneProcessReport = ProcessDynamicBone(dynamicBones[i], postDynamicBoneProcessReport.nukedDynamicBones, postDynamicBoneProcessReport.dynamicBoneCount);
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
                    AntiCrashLightSourcePostProcess postProcessReport = new AntiCrashLightSourcePostProcess();
                    for (int i = 0; i < lightSources.Count; i++)
                    {
                        if (lightSources[i] == null)
                        {
                            continue;
                        }
                        postProcessReport = ProcessLight(lightSources[i], postProcessReport.nukedLightSources, postProcessReport.lightSourceCount);
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
                    AntiCrashRendererPostProcess postProcessReport = new();
                    for (int i = 0; i < renderers.Count; i++)
                    {
                        if (renderers[i] == null)
                        {
                            continue;
                        }
                        ProcessRenderer(renderers[i], antiMeshCrash, antiMaterialCrash, antiShaderCrash, ref postProcessReport);
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
                    Functions.GetInfoComp(user.id).SetAntiCrashBool(LabelToggle);
                }
                LastCheckedAvatarIDPosted = currentAviID;
                return true;
            }
            catch {}
            return true;
        }

        private static bool PortalDestroyPatch()
        {
            bool result = true;
            try
            {
                if (BlazeInfo.InfinitePortals) result = false;
            }
            catch {}
            return result;
        }

        private static void FramesSpoof(ref int __result)
        {
            try
            {
                if (MainConfig.Instance.PingSpoof)
                    __result = MainConfig.Instance.PingSpoofValue;
                return;
            }
            catch { }
        }

        private static void PingSpoof(ref float __result)
        {
            try
            {
                if (MainConfig.Instance.FramesSpoof)
                    __result = MainConfig.Instance.FramesSpoofValue;
                return;
            }
            catch { }
        }

        // DO NOT RE-ADD! WORLD & OFFLINE SPOOF ARE DETECTED!
        /*private static bool OnlineLocationPatch(ref string __0, ref Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> __2)
        {
            try
            {
                if (__2 != null && (__0 == "visits" || __0 == "joins"))
                {
                    if (MainConfig.Instance.OfflineSpoof) return false;
                    if (MainConfig.Instance.WorldSpoof)
                    {
                        __2.Clear();
                        __2.Add("userId", APIUser.CurrentUser.id);
                        __2.Add("worldId", MainConfig.Instance.WorldSpoofID);
                    }
                }
            }
            catch { }
            return true;
        }*/


        // DO NOT RE-ADD! QUEST SPOOF IS DETECTED!
        /*private static bool UnpatchedQuestSpoof;
        private static void QuestSpoof(ref string __result)
        {
            if (MainConfig.Instance.QuestSpoof)
            {
                try
                {
                    __result = "android";
                    if (!UnpatchedQuestSpoof)
                    {
                        UnpatchedQuestSpoof = true;
                        Functions.Delay(delegate
                        {
                            // unpatches the method at the right time so that it doesn't break it to where you can't enter pc worlds
                            QuestPatch.Instance.Unpatch(AccessTools.Property(typeof(Tools), "Platform").GetMethod, 0, QuestPatch.Instance.Id);
                        }, 3.3f);
                    }
                }
                catch
                {
                }
            }
        }*/

        private static bool MenuOpenedPatch()
        {
            try
            {
                Logs.Log("Menu Opened!");
            }
            catch {}
            return true;
        }

        private static bool MenuClosedPatch()
        {
            try
            {
                Logs.Log("Menu Closed!");
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
                        if (MainConfig.Instance.LogAvatarsSwitched && PlayerUtils.CurrentUser() != null)
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
                            ExtraAviLists.AddToRecentlyUsed(a);
                        }
                        else
                        {
                            ExtraAviLists.AddToRecentlySeen(a);
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
                                        Version = a.version
                                    }
                                }
                            }));
                        }

                        foreach (var m in BlazeInfo.Modules) m.AvatarIsReady(__instance, a);
                    }
                }));
            }
            catch {}
        }

        private static bool PlaceUiPatch(VRCUiManager __instance, bool __0)
        {
            if (!XRDevice.isPresent || !MainConfig.Instance.ComfyVRMenu) return true;
            float num = MiscUtils.GetVRCTrackingManager() != null ? MiscUtils.GetVRCTrackingManager().transform.localScale.x : 1f;
            if (num <= 0f)
            {
                num = 1f;
            }
            var playerTrackingDisplay = __instance.transform;
            var unscaledUIRoot = __instance.transform.Find("UnscaledUI");
            playerTrackingDisplay.position = MiscUtils.GetWorldCameraPosition();
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

            foreach (var m in BlazeInfo.Modules)
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
                MelonCoroutines.Start(EnforceTargetInstanceType(flowManager, InstanceAccessType.FriendsOfGuests, isButton ? 10 : 30));
            }
        }

        private static int ourRequestId;

        private static IEnumerator EnforceTargetInstanceType(VRCFlowManager manager, InstanceAccessType type, float time)
        {
            var endTime = Time.time + time;
            var currentRequestId = ++ourRequestId;
            while (Time.time < endTime && ourRequestId == currentRequestId)
            {
                manager.field_Protected_InstanceAccessType_0 = type;
                yield return null;
            }
        }

        private static void LeftRoom()
        {
            foreach (var m in BlazeInfo.Modules) m.LeftWorld();
        }

        /*[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ApplyPlayerMotion(Vector3 playerWorldMotion, Quaternion playerWorldRotation);
        private static ApplyPlayerMotion origApplyPlayerMotion;
        private static void ApplyPlayerMotionPatch(Vector3 playerWorldMotion, Quaternion playerWorldRotation)
        {
            origApplyPlayerMotion(playerWorldMotion, RotationSystem.Rotating ? Quaternion.identity : playerWorldRotation);
        }*/
        #endregion
    }
}
