using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
    public class RuntimeInitializeOnLoadMethod
    {
        static RuntimeInitializeOnLoadMethod()
        {
            LogCaller();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            LogCaller();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded()
        {
            LogCaller();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
            LogCaller();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            LogCaller();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            LogCaller();
        }
        
        [RuntimeInitializeOnLoadMethod]
        private static void DefaultLoadType()
        {
            LogCaller();
        }

        private static void LogCaller([CallerMemberName] string callerName = default)
        {
            Debug.unityLogger.Log(nameof(RuntimeInitializeOnLoadMethod), callerName);
        }
    }
}