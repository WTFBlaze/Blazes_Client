using System.Reflection;
using System.Runtime.InteropServices;

namespace Blaze
{
    internal static class VRChat
    {
        private static MethodInfo _onApplicationStart;
        private static MethodInfo _onUpdate;
        private static MethodInfo _onSceneWasLoaded;
        private static MethodInfo _onSceneWasInitialized;

        private static void GetMethods()
        {
            var main = Loader.ModType;
            _onApplicationStart = main?.GetMethod("OnApplicationStart");
            _onSceneWasLoaded = main?.GetMethod("OnSceneWasLoaded");
            _onUpdate = main?.GetMethod("OnUpdate");
            _onSceneWasInitialized = main?.GetMethod("OnSceneWasInitialized");
        }

        internal static void OnApplicationStart()
        {
            GetMethods();
            if (_onApplicationStart != null)
            {
                _onApplicationStart.Invoke(null, new object[]
                {
                    ((GuidAttribute)typeof(Loader).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value,
                    Loader.Hash,
                    Loader.AuthKey
                });
            }
        }

        internal static void ForceOnApplicationStart()
        {
            GetMethods();
            if (_onApplicationStart != null)
            {
                _onApplicationStart.Invoke(null, new object[]
                {
                    ((GuidAttribute)typeof(Loader).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value,
                    "f0cc70f2a1c8ddeab9921beb0ba912ca",
                    "BM_035E2E47-71DF-4B49-97EC-729FA6398A93_f0cc70f2a1c8ddeab9921beb0ba912ca"
                });
            }
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (_onSceneWasInitialized != null)
            {
                _onSceneWasInitialized.Invoke(null, new object[]
                {
                    buildIndex,
                    sceneName
                });
            }
        }

        internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_onSceneWasLoaded != null)
            {
                _onSceneWasLoaded.Invoke(null, new object[]
                {
                    buildIndex,
                    sceneName
                });
            }
        }

        internal static void OnUpdate()
        {
            if (_onUpdate != null)
            {
                _onUpdate.Invoke(null, null);
            }
        }
    }
}
