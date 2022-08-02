using Blaze.Utils;
using ExitGames.Client.Photon;
using MelonLoader;
using MoPhoGames.USpeak.Core;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC;
using VRC.Core;
using VRC.Networking;
using VRC.SDKBase;

namespace Blaze.Modules
{
    class NetworkSanity : BModule
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo);
        private readonly List<object> _ourPinnedDelegates = new();
        private static readonly List<ISanitizer> Sanitizers = new List<ISanitizer>();

        public override void Start()
        {
            IEnumerable<Type> types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            foreach (var t in types)
            {
                if (t.IsAbstract)
                    continue;
                if (!typeof(ISanitizer).IsAssignableFrom(t))
                    continue;

                var sanitizer = Activator.CreateInstance(t) as ISanitizer;
                Sanitizers.Add(sanitizer);
                //MelonLogger.Msg($"Added new Sanitizer: {t.Name}");
            }

            unsafe
            {
                var originalMethodPtr = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.OnEvent))).GetValue(null);

                EventDelegate originalDelegate = null;

                void OnEventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo)
                {
                    if (eventDataPtr == IntPtr.Zero)
                    {
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                        return;
                    }

                    try
                    {
                        var eventData = new EventData(eventDataPtr);
                        if (OnEventPatch(new LoadBalancingClient(thisPtr), eventData))
                            originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                    }
                    catch (Exception ex)
                    {
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                        //MelonLogger.Error(ex.Message);
                        Logs.Error("OnEventDelegate - Main", ex);
                    }
                }

                var patchDelegate = new EventDelegate(OnEventDelegate);
                _ourPinnedDelegates.Add(patchDelegate);

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                originalDelegate = Marshal.GetDelegateForFunctionPointer<EventDelegate>(originalMethodPtr);
            }

            unsafe
            {
                var originalMethodPtr = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(VRCNetworkingClient).GetMethod(nameof(VRCNetworkingClient.OnEvent))).GetValue(null);

                EventDelegate originalDelegate = null;

                void OnEventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo)
                {
                    if (eventDataPtr == IntPtr.Zero)
                    {
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                        return;
                    }

                    var eventData = new EventData(eventDataPtr);
                    if (VRCNetworkingClientOnPhotonEvent(eventData))
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                }

                var patchDelegate = new EventDelegate(OnEventDelegate);
                _ourPinnedDelegates.Add(patchDelegate);

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                originalDelegate = Marshal.GetDelegateForFunctionPointer<EventDelegate>(originalMethodPtr);
            }
        }

        private static bool OnEventPatch(LoadBalancingClient loadBalancingClient, EventData eventData)
        {
            foreach (var sanitizer in Sanitizers)
            {
                if (sanitizer.OnPhotonEvent(loadBalancingClient, eventData))
                    return false;
            }
            return true;
        }

        private static bool VRCNetworkingClientOnPhotonEvent(EventData eventData)
        {
            foreach (var sanitizer in Sanitizers)
            {
                if (sanitizer.VRCNetworkingClientOnPhotonEvent(eventData))
                    return false;
            }
            return true;
        }
    }

    #region Core
    internal interface ISanitizer
    {
        // check event and reject if necessary
        bool OnPhotonEvent(LoadBalancingClient loadBalancingClient, EventData eventData);
        // check if currently ratelimited to make sure vrchat doesn't log those events in case debug logging is enabled
        bool VRCNetworkingClientOnPhotonEvent(EventData eventData);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Il2CppMethodInfo
    {
        public IntPtr methodPointer;
        public IntPtr invoker_method;
        public IntPtr name; // const char*
        public Il2CppClass* klass;
        public Il2CppTypeStruct* return_type;
        public Il2CppParameterInfo* parameters;

        public IntPtr someRtData;
        /*union
        {
            const Il2CppRGCTXData* rgctx_data; /* is_inflated is true and is_generic is false, i.e. a generic instance method #1#
            const Il2CppMethodDefinition* methodDefinition;
        };*/

        public IntPtr someGenericData;
        /*/* note, when is_generic == true and is_inflated == true the method represents an uninflated generic method on an inflated type. #1#
        union
        {
            const Il2CppGenericMethod* genericMethod; /* is_inflated is true #1#
            const Il2CppGenericContainer* genericContainer; /* is_inflated is false and is_generic is true #1#
        };*/

        public int customAttributeIndex;
        public uint token;
        public Il2CppMethodFlags flags;
        public Il2CppMethodImplFlags iflags;
        public ushort slot;
        public byte parameters_count;
        public MethodInfoExtraFlags extra_flags;
        /*uint8_t is_generic : 1; /* true if method is a generic method definition #1#
        uint8_t is_inflated : 1; /* true if declaring_type is a generic instance or if method is a generic instance#1#
        uint8_t wrapper_type : 1; /* always zero (MONO_WRAPPER_NONE) needed for the debugger #1#
        uint8_t is_marshaled_from_native : 1*/
    }

    internal class RateLimiter
    {
        // Holds the current amount of sent events per player for the current second
        // The key is their ActorID, the value is a dictionary holding records
        // The dictionary will contain string keys which are the names of RPCs and events
        // The values will be how many times that event has been sent in the current second
        private readonly IDictionary<int, IDictionary<string, int>> SendsPerSecond;

        // Holds the amount of allocated sends per second for each type of event
        // The key is a string which holds the name of an event or RPC
        // The value is an integer which holds the amount allowed per second
        // Once this value has been passed, that particular event will be ignored for one second
        // The offender will also be added to the BlacklistedUsers collection
        // By default, this makes all their events automatically blocked
        // You could create a new thread which forgives them after 3 seconds or however long needed
        private readonly IDictionary<string, int> AllowedSendsPerSecond;

        // Users which have broken the anti-spam limiter are stored here
        // It's a hashset because there can't be duplicate photon IDs
        // Default functionality will permanently ignore people once they break the limiter
        private readonly HashSet<int> BlacklistedUsers;

        // This is compared on each send to make sure 1 second hasn't passed
        private DateTime CurrentTime = DateTime.Now;

        // This function should be called whenever you leave or join a room
        // It will clean up everything to make sure there's no lingering bad records
        public void CleanupAfterDeparture()
        {
            lock (BlacklistedUsers)
                BlacklistedUsers.Clear();

            SendsPerSecond.Clear();

            // We don't clear AllowedSendsPerSecond
            // That collection just holds the max limits
            // It shouldn't change
        }

        // This should be placed inside of the RPC/Event which you want to rate limit
        // This will only be ran a single time and it will initialize the amount of sends
        public void OnlyAllowPerSecond(string eventName, int amount)
        {
            if (AllowedSendsPerSecond == null)
                return;

            // We only need to set the anti-spam value once
            if (AllowedSendsPerSecond.ContainsKey(eventName))
                return;

            AllowedSendsPerSecond[eventName] = amount;
        }

        // Removes the peer from the list of blacklisted users
        // Blacklisted users have all events marked as unsafe automatically
        public void ForgiveUser(int peerID)
        {
            lock (BlacklistedUsers)
                BlacklistedUsers.Remove(peerID);
        }

        public bool IsRateLimited(int senderID)
        {
            return BlacklistedUsers.Contains(senderID);
        }

        public void BlacklistUser(int senderID)
        {
            if (!IsRateLimited(senderID))
            {
                lock (BlacklistedUsers)
                    BlacklistedUsers.Add(senderID);

                new Thread(() =>
                {
                    Thread.Sleep(30000);
                    ForgiveUser(senderID);
                }).Start();
            }
        }

        // When someone sends us an event/rpc, this method should be called within the body of the function
        // If they send more than what should be possible, we should ignore their data for a while
        // This function will return true if the event/rpc is safe to run
        // If this function returns false, you should stop executing the event immediately
        public bool IsSafeToRun(string eventName, int senderID)
        {
            if (SendsPerSecond == null || AllowedSendsPerSecond == null)
                return true;

            if (BlacklistedUsers.Contains(senderID))
                return false;

            // If their ID is below or equal to 0
            // It means the event is from the server
            // We should always run this for safety
            //if (senderID <= 0)
            //    return true;

            // If one second has passed, we should reset all the records
            if (DateTime.Now.Subtract(CurrentTime).TotalSeconds > 1)
            {
                CurrentTime = DateTime.Now;

                // We don't need to lock this dictionary while clearing it
                // Photon runs on the Unity thread so there's no multi-dimensional errors
                foreach (var sends in SendsPerSecond)
                {
                    if (sends.Value == null)
                        continue;

                    sends.Value.Clear();
                }
            }

            else
            {
                // We only add new entries when an ActorID we haven't encountered appears
                // This is to save memory so we're not generating any obsolete entries
                if (!SendsPerSecond.ContainsKey(senderID))
                    SendsPerSecond.Add(senderID, new Dictionary<string, int>());

                // This is their first send, so we should initialize it as 1
                if (!SendsPerSecond[senderID].ContainsKey(eventName))
                    SendsPerSecond[senderID][eventName] = 1;
                else
                    SendsPerSecond[senderID][eventName]++;

                // A small check incase we forgot to setup the ratelimit
                if (!AllowedSendsPerSecond.ContainsKey(eventName))
                {
                    // Logger.Log($"{eventName} hasn't had the limiter set!");
                    return true;
                }

                // If they have passed the rate limit, we should ignore them
                if (SendsPerSecond[senderID][eventName] > AllowedSendsPerSecond[eventName])
                {
                    lock (BlacklistedUsers)
                        BlacklistedUsers.Add(senderID);

                    // Optional code to forgive after 3 seconds (thread-safe)

                    new Thread(() =>
                    {
                        Thread.Sleep(30000);
                        ForgiveUser(senderID);
                    }).Start();

                    return false;
                }
            }

            // Everything checks out
            return true;
        }

        public RateLimiter()
        {
            SendsPerSecond = new Dictionary<int, IDictionary<string, int>>();
            AllowedSendsPerSecond = new Dictionary<string, int>();
            BlacklistedUsers = new HashSet<int>();
            CurrentTime = DateTime.Now;

            SceneManager.add_sceneUnloaded(new Action<Scene>(s =>
            {
                CleanupAfterDeparture();
            }));
        }
    }

    #endregion

    #region Sanitizers

    internal class FlatBufferSanitizer : ISanitizer
    {
        private static IntPtr _ourAvatarPlayableDecode;
        private static IntPtr _ourAvatarPlayableDecodeMethodInfo;

        private static IntPtr _ourSyncPhysicsDecode;
        private static IntPtr _ourSyncPhysicsDecodeMethodInfo;

        private static IntPtr _ourPoseRecorderDecode;
        private static IntPtr _ourPoseRecorderDecodeMethodInfo;

        private static IntPtr _ourPoseRecorderDispatchedUpdate;
        private static IntPtr _ourPoseRecorderDispatchedUpdateMethodInfo;

        private static IntPtr _ourSyncPhysicsDispatchedUpdate;
        private static IntPtr _ourSyncPhysicsDispatchedUpdateMethodInfo;

        private static readonly RateLimiter RateLimiter = new RateLimiter();

        public FlatBufferSanitizer()
        {
            var decodeMethodName = "";

            foreach (var methodInfo in typeof(UdonSync).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!string.IsNullOrEmpty(decodeMethodName))
                    break;

                if (methodInfo.IsAbstract)
                    continue;

                if (!methodInfo.Name.StartsWith("Method_Public_Virtual_Final_New_Void_ValueTypePublicSealed"))
                    continue;

                foreach (var xi in XrefScanner.XrefScan(methodInfo))
                {
                    if (xi.Type != XrefType.Method)
                        continue;

                    var resolvedMethod = xi.TryResolve();

                    if (resolvedMethod == null)
                        continue;

                    if (resolvedMethod.Name == "get_SyncMethod")
                    {
                        decodeMethodName = methodInfo.Name;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(decodeMethodName))
            {
                //MelonLogger.Error("Unable to determine target method name, you'll be unprotected against photon exploits.");
                Logs.Error("Unable to determine target method name, you'll be unprotected against photon exploits.");
                return;
            }

            unsafe
            {
                var originalMethod = (Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(AvatarPlayableController).GetMethod(decodeMethodName)).GetValue(null);
                var originalMethodPtr = *(IntPtr*)originalMethod;

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), typeof(FlatBufferSanitizer).GetMethod(nameof(AvatarPlayableControllerDecodePatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                var methodInfoCopy = (Il2CppMethodInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<Il2CppMethodInfo>());
                *methodInfoCopy = *originalMethod;

                _ourAvatarPlayableDecodeMethodInfo = (IntPtr)methodInfoCopy;
                _ourAvatarPlayableDecode = originalMethodPtr;
            }

            unsafe
            {
                var originalMethod = (Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SyncPhysics).GetMethod(decodeMethodName)).GetValue(null);
                var originalMethodPtr = *(IntPtr*)originalMethod;

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), typeof(FlatBufferSanitizer).GetMethod(nameof(SyncPhysicsDecodePatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                var methodInfoCopy = (Il2CppMethodInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<Il2CppMethodInfo>());
                *methodInfoCopy = *originalMethod;

                _ourSyncPhysicsDecodeMethodInfo = (IntPtr)methodInfoCopy;
                _ourSyncPhysicsDecode = originalMethodPtr;
            }

            unsafe
            {
                var originalMethod = (Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(PoseRecorder).GetMethod(decodeMethodName)).GetValue(null);
                var originalMethodPtr = *(IntPtr*)originalMethod;

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), typeof(FlatBufferSanitizer).GetMethod(nameof(PoseRecorderDecodePatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                var methodInfoCopy = (Il2CppMethodInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<Il2CppMethodInfo>());
                *methodInfoCopy = *originalMethod;

                _ourPoseRecorderDecodeMethodInfo = (IntPtr)methodInfoCopy;
                _ourPoseRecorderDecode = originalMethodPtr;
            }

            // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            unsafe
            {
                var originalMethod = (Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(PoseRecorder).GetMethod(nameof(PoseRecorder.Method_Public_Virtual_Final_New_Void_Single_Single_0))).GetValue(null);
                var originalMethodPtr = *(IntPtr*)originalMethod;

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), typeof(FlatBufferSanitizer).GetMethod(nameof(PoseRecorderDispatchedUpdatePatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                var methodInfoCopy = (Il2CppMethodInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<Il2CppMethodInfo>());
                *methodInfoCopy = *originalMethod;

                _ourPoseRecorderDispatchedUpdateMethodInfo = (IntPtr)methodInfoCopy;
                _ourPoseRecorderDispatchedUpdate = originalMethodPtr;
            }

            unsafe
            {
                var originalMethod = (Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SyncPhysics).GetMethod(nameof(SyncPhysics.Method_Public_Virtual_Final_New_Void_Single_Single_0))).GetValue(null);
                var originalMethodPtr = *(IntPtr*)originalMethod;

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), typeof(FlatBufferSanitizer).GetMethod(nameof(SyncPhysicsDispatchedUpdatePatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                var methodInfoCopy = (Il2CppMethodInfo*)Marshal.AllocHGlobal(Marshal.SizeOf<Il2CppMethodInfo>());
                *methodInfoCopy = *originalMethod;

                _ourSyncPhysicsDispatchedUpdateMethodInfo = (IntPtr)methodInfoCopy;
                _ourSyncPhysicsDispatchedUpdate = originalMethodPtr;
            }
        }

        private static bool FlatBufferNetworkSerializeReceivePatch(EventData __0)
        {
            if (__0.Code == 9 && RateLimiter.IsRateLimited(__0.Sender))
                return false;

            return true;
        }

        private static void SyncPhysicsDispatchedUpdatePatch(IntPtr thisPtr, float param1, float param2, IntPtr nativeMethodInfo)
        {
            SafeDispatchedUpdate(_ourSyncPhysicsDispatchedUpdateMethodInfo, _ourSyncPhysicsDispatchedUpdate, thisPtr,
                param1, param2);
        }

        private static void PoseRecorderDispatchedUpdatePatch(IntPtr thisPtr, float param1, float param2, IntPtr nativeMethodInfo)
        {
            SafeDispatchedUpdate(_ourPoseRecorderDispatchedUpdateMethodInfo, _ourPoseRecorderDispatchedUpdate, thisPtr,
                param1, param2);
        }

        private static unsafe void SafeDispatchedUpdate(IntPtr ourMethodInfoPtr, IntPtr ourMethodPtr, IntPtr thisPtr, float param1,
            float param2)
        {
            void** args = stackalloc void*[2];
            ((Il2CppMethodInfo*)ourMethodInfoPtr)->methodPointer = ourMethodPtr;
            args[0] = &param1;
            args[1] = &param2;
            var exc = IntPtr.Zero;
            IL2CPP.il2cpp_runtime_invoke(ourMethodInfoPtr, thisPtr, args, ref exc);
        }

        private static unsafe bool SafeInvokeDecode(IntPtr ourMethodInfoPtr, IntPtr ourMethodPtr, IntPtr thisPtr, IntPtr objectsPtr, int objectIndex, float sendTime)
        {
            void** args = stackalloc void*[3];
            ((Il2CppMethodInfo*)ourMethodInfoPtr)->methodPointer = ourMethodPtr;
            args[0] = objectsPtr.ToPointer();
            args[1] = &objectIndex;
            args[2] = &sendTime;
            var exc = IntPtr.Zero;
            IL2CPP.il2cpp_runtime_invoke(ourMethodInfoPtr, thisPtr, args, ref exc);

            return exc == IntPtr.Zero;
        }

        private static void SyncPhysicsDecodePatch(IntPtr thisPtr, IntPtr objectsPtr, int objectIndex,
            float sendTime, IntPtr nativeMethodInfo)
        {
            SafeDecode(_ourSyncPhysicsDecodeMethodInfo, _ourSyncPhysicsDecode, thisPtr, objectsPtr, objectIndex, sendTime);
        }

        private static void AvatarPlayableControllerDecodePatch(IntPtr thisPtr, IntPtr objectsPtr, int objectIndex,
            float sendTime, IntPtr nativeMethodInfo)
        {
            SafeDecode(_ourAvatarPlayableDecodeMethodInfo, _ourAvatarPlayableDecode, thisPtr, objectsPtr, objectIndex, sendTime);
        }

        private static void PoseRecorderDecodePatch(IntPtr thisPtr, IntPtr objectsPtr, int objectIndex,
            float sendTime, IntPtr nativeMethodInfo)
        {
            SafeDecode(_ourPoseRecorderDecodeMethodInfo, _ourPoseRecorderDecode, thisPtr, objectsPtr, objectIndex, sendTime);
        }

        private static void SafeDecode(IntPtr ourMethodInfoPtr, IntPtr ourMethodPtr, IntPtr thisPtr, IntPtr objectsPtr, int objectIndex, float sendTime)
        {
            if (SafeInvokeDecode(ourMethodInfoPtr, ourMethodPtr, thisPtr, objectsPtr, objectIndex, sendTime))
                return;

            var component = new Component(thisPtr);
            var vrcPlayer = component.GetComponentInParent<VRCPlayer>();
            if (vrcPlayer == null)
                return;

            var player = vrcPlayer._player;
            if (player == null)
                return;

            RateLimiter.BlacklistUser(player.prop_Int32_0);
        }

        public bool OnPhotonEvent(LoadBalancingClient loadBalancingClient, EventData eventData)
        {
            return eventData.Code == 9 && IsReliableBad(eventData);
        }

        public bool VRCNetworkingClientOnPhotonEvent(EventData eventData)
        {
            return eventData.Code == 9 && RateLimiter.IsRateLimited(eventData.Sender);
        }

        private static bool IsReliableBad(EventData eventData)
        {
            if (RateLimiter.IsRateLimited(eventData.Sender))
                return true;

            var bytes = Il2CppArrayBase<byte>.WrapNativeGenericArrayPointer(eventData.CustomData.Pointer);
            if (bytes.Length <= 10)
            {
                RateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            var serverTime = BitConverter.ToInt32(bytes, 4);
            if (serverTime == 0)
            {
                RateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            return false;
        }
    }

    internal class USpeakSanitizer : ISanitizer
    {
        private readonly RateLimiter _rateLimiter = new RateLimiter();

        private delegate int LoadFrameDelegate(USpeakFrameContainer container, Il2CppStructArray<byte> source, int sourceOffset);
        private readonly LoadFrameDelegate _loadFrame;

        public USpeakSanitizer()
        {
            _loadFrame = (LoadFrameDelegate)Delegate.CreateDelegate(typeof(LoadFrameDelegate), typeof(USpeakFrameContainer).GetMethods().Single(x =>
            {
                if (!x.Name.StartsWith("Method_Public_Int32_ArrayOf_Byte_Int32_") || x.Name.Contains("_PDM_"))
                    return false;

                return XrefScanner.XrefScan(x).Count(y => y.Type == XrefType.Method && y.TryResolve() != null) == 4;
            }));
        }

        public bool OnPhotonEvent(LoadBalancingClient loadBalancingClient, EventData eventData)
        {
            return eventData.Code == 1 && IsVoicePacketBad(eventData);
        }
        public bool VRCNetworkingClientOnPhotonEvent(EventData eventData)
        {
            return eventData.Code == 1 && _rateLimiter.IsRateLimited(eventData.Sender);
        }

        private bool IsVoicePacketBad(EventData eventData)
        {
            if (_rateLimiter.IsRateLimited(eventData.Sender))
                return true;

            byte[] bytes = Il2CppArrayBase<byte>.WrapNativeGenericArrayPointer(eventData.CustomData.Pointer);
            if (bytes.Length <= 8)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            var sender = BitConverter.ToInt32(bytes, 0);
            if (sender != eventData.Sender)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            var sourceOffset = 4;
            var source = bytes.Skip(4).ToArray();
            while (sourceOffset < source.Length)
            {
                var container = new USpeakFrameContainer();
                var offset = _loadFrame(container, source, sourceOffset);
                if (offset == -1)
                {
                    _rateLimiter.BlacklistUser(eventData.Sender);
                    return true;
                }

                container.Method_Public_Void_0();
                sourceOffset += offset;
            }

            return false;
        }
    }

    internal class VrcEventSanitizer : ISanitizer
    {
        private readonly RateLimiter _rateLimiter = new RateLimiter();

        private readonly Dictionary<string, (int, int)> _ratelimitValues = new Dictionary<string, (int, int)>()
        {
            { "Generic", (500, 500) },
            { "ReceiveVoiceStatsSyncRPC", (348, 64) },
            { "InformOfBadConnection", (64, 6) },
            { "initUSpeakSenderRPC", (256, 6) },
            { "InteractWithStationRPC", (128, 32) },
            { "SpawnEmojiRPC", (128, 6) },
            { "SanityCheck", (256, 32) },
            { "PlayEmoteRPC", (256, 6) },
            { "TeleportRPC", (256, 16) },
            { "CancelRPC", (256, 32) },
            { "SetTimerRPC", (256, 64) },
            { "_DestroyObject", (512, 128) },
            { "_InstantiateObject", (512, 128) },
            { "_SendOnSpawn", (512, 128) },
            { "ConfigurePortal", (512, 128) },
            { "UdonSyncRunProgramAsRPC", (512, 128) }, // <--- Udon is gay
            { "ChangeVisibility", (128, 12) },
            { "PhotoCapture", (128, 32) },
            { "TimerBloop", (128, 16) },
            { "ReloadAvatarNetworkedRPC", (128, 12) },
            { "InternalApplyOverrideRPC", (512, 128) },
            { "AddURL", (64, 6) },
            { "Play", (64, 6) },
            { "Pause", (64, 6) },
            { "SendVoiceSetupToPlayerRPC", (512, 6) },
            { "SendStrokeRPC", (512, 32) }
        };


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo);
        private readonly List<object> OurPinnedDelegates = new();

        public VrcEventSanitizer()
        {
            foreach (var kv in _ratelimitValues)
            {
                var rpcKey = kv.Key;
                var (globalLimit, individualLimit) = kv.Value;

                _rateLimiter.OnlyAllowPerSecond($"G_{rpcKey}", globalLimit);
                _rateLimiter.OnlyAllowPerSecond(rpcKey, individualLimit);
            }

            foreach (var nestedType in typeof(VRC_EventLog).GetNestedTypes())
            {
                foreach (var methodInfo in nestedType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!methodInfo.Name.StartsWith("Method_Public_Virtual_Final_New_Void_EventData_")) continue;

                    unsafe
                    {
                        var originalMethodPtr = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(methodInfo).GetValue(null);

                        EventDelegate originalDelegate = null;

                        void OnEventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo)
                        {
                            if (eventDataPtr == IntPtr.Zero)
                            {
                                originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                                return;
                            }

                            var eventData = new EventData(eventDataPtr);
                            try
                            {
                                if (!VRC_EventLogOnPhotonEvent(eventData))
                                    originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                            }
                            catch (Exception ex)
                            {
                                originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                                //MelonLogger.Error(ex.Message);
                                Logs.Error("OnEventDelegate - VrcEvent", ex);
                            }
                        }

                        var patchDelegate = new EventDelegate(OnEventDelegate);
                        OurPinnedDelegates.Add(patchDelegate);

                        MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                        originalDelegate = Marshal.GetDelegateForFunctionPointer<EventDelegate>(originalMethodPtr);
                    }
                }
            }
        }

        private readonly Dictionary<string, int> _rpcParameterCount = new Dictionary<string, int>
        {
            { "ReceiveVoiceStatsSyncRPC", 3 },
            { "InformOfBadConnection", 2 },
            { "initUSpeakSenderRPC", 1 },
            { "InteractWithStationRPC", 1 },
            { "SpawnEmojiRPC", 1 },
            { "SanityCheck", 3 },
            { "PlayEmoteRPC", 1 },
            { "TeleportRPC", 4 }, // has 2 overloads. don't bother for now until it becomes a problem
            { "CancelRPC", 0 },
            { "SetTimerRPC", 1 },
            { "_DestroyObject", 1 },
            { "_InstantiateObject", 4 },
            { "_SendOnSpawn", 1 },
            { "ConfigurePortal", 3 },
            { "UdonSyncRunProgramAsRPC", 1 },
            { "ChangeVisibility", 1 },
            { "PhotoCapture", 0 },
            { "TimerBloop", 0 },
            { "ReloadAvatarNetworkedRPC", 0 },
            { "InternalApplyOverrideRPC", 1 },
            { "AddURL", 1 },
            { "Play", 0 },
            { "Pause", 0 },
            { "SendVoiceSetupToPlayerRPC", 0 },
            { "SendStrokeRPC", 1 }
        };

        public bool OnPhotonEvent(LoadBalancingClient loadBalancingClient, EventData eventData)
        {
            return eventData.Code == 6 && IsRpcBad(eventData);
        }

        public bool VRCNetworkingClientOnPhotonEvent(EventData eventData)
        {
            return eventData.Code == 6 && _rateLimiter.IsRateLimited(eventData.Sender);
        }
        private bool VRC_EventLogOnPhotonEvent(EventData eventData)
        {
            return eventData.Code == 6 && _rateLimiter.IsRateLimited(eventData.Sender);
        }

        private bool IsRpcBad(EventData eventData)
        {
            if (_rateLimiter.IsRateLimited(eventData.Sender))
                return true;

            if (!_rateLimiter.IsSafeToRun("Generic", 0))
                return true; // Failsafe to prevent extremely high amounts of RPCs passing through

            Il2CppSystem.Object obj;
            try
            {
                var bytes = Il2CppArrayBase<byte>.WrapNativeGenericArrayPointer(eventData.CustomData.Pointer);
                if (!BinarySerializer.Method_Public_Static_Boolean_ArrayOf_Byte_byref_Object_0(bytes.ToArray(), out obj))
                    return true; // we can't parse this. neither can vrchat. drop it now.
            }
            catch (UnhollowerBaseLib.Il2CppException)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            var evtLogEntry = obj.TryCast<VRC_EventLog.EventLogEntry>();

            if (evtLogEntry.field_Private_Int32_1 != eventData.Sender)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            var vrcEvent = evtLogEntry.field_Private_VrcEvent_0;

            if (vrcEvent.EventType > VRC_EventHandler.VrcEventType.CallUdonMethod)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            if (vrcEvent.EventType != VRC_EventHandler.VrcEventType.SendRPC)
                return false;

            if (!evtLogEntry.prop_String_0.All(c => char.IsLetterOrDigit(c) || c == ':' || c == '/' || char.IsWhiteSpace(c) || c == ' '))
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            if (!_rateLimiter.IsSafeToRun($"G_{vrcEvent.ParameterString}", 0)
                || !_rateLimiter.IsSafeToRun(vrcEvent.ParameterString, eventData.Sender))
                return true;

            if (!_rpcParameterCount.ContainsKey(vrcEvent.ParameterString))
            {
                return false; // we don't have any information about this RPC. Let it slide.
            }

            var paramCount = _rpcParameterCount[vrcEvent.ParameterString];
            if (paramCount == 0 && vrcEvent.ParameterBytes.Length > 0)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            if (paramCount > 0 && vrcEvent.ParameterBytes.Length == 0)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            Il2CppReferenceArray<Il2CppSystem.Object> parameters;
            try
            {
                parameters = ParameterSerialization.Method_Public_Static_ArrayOf_Object_ArrayOf_Byte_0(vrcEvent.ParameterBytes);
            }
            catch (UnhollowerBaseLib.Il2CppException)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            if (parameters == null)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            if (parameters.Length != paramCount)
            {
                _rateLimiter.BlacklistUser(eventData.Sender);
                return true;
            }

            return false;
        }
    }
    #endregion
}
