using HarmonyLib;
using HawkNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowLib.Networking
{
    public static class PluginNetworkManager
    {
        public static HawkProcessSceneNetworkBehaviours currentHawkProcess;
        public static Action onRegisteredCallback;

        private static readonly List<HawkNetworkBehaviour> moddedNetworkBehaviours = new();

        public static void AddNetworkBehaviour(HawkNetworkBehaviour networkBehaviour, Action onRegisteredCallback = null)
        {
            moddedNetworkBehaviours.Add(networkBehaviour);

            if(onRegisteredCallback != null)
            {
                PluginNetworkManager.onRegisteredCallback += onRegisteredCallback;
            }
        }

        public static void RemoveNetworkBehaviour(HawkNetworkBehaviour networkBehaviour)
        {
            moddedNetworkBehaviours.Remove(networkBehaviour);
        }

        public static List<HawkNetworkBehaviour> GetNetworkBehaviours()
        {
            return moddedNetworkBehaviours;
        }

        internal static void RegisterAllBehaviors()
        {
            var behaviors = moddedNetworkBehaviours.ToList();

            foreach(var behaviour in behaviors)
            {
                if (currentHawkProcess.GetBehaviours().Contains(behaviour) || behaviour == null || behaviour.networkObject != null)
                {
                    moddedNetworkBehaviours.Remove(behaviour);
                    continue;
                }

                Plugin.LogSource.LogMessage($"Registering behavior {behaviour}");
                currentHawkProcess.GetBehaviours().Add(behaviour);

                moddedNetworkBehaviours.Remove(behaviour);
            }
        }
    }

    public static class HawkNetworkManagerPatch
    {
        [HarmonyPatch(typeof(HawkNetworkManager), "InitalizeScene")]
        [HarmonyPrefix]
        public static bool InitializeScenePrefix(ref HawkNetworkManager __instance, ref Scene scene, ref LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                foreach (Type type in Plugin.NetworkBehaviours)
                {
                    var obj = new GameObject(type.FullName);
                    SceneManager.MoveGameObjectToScene(obj, scene);
                    obj.AddComponent(type);
                }
            }

            foreach (var obj in scene.GetRootGameObjects())
            {
                if(obj.TryGetComponent<HawkProcessSceneNetworkBehaviours>(out var hawkProcessSceneNetworkBehaviours))
                {
                    PluginNetworkManager.currentHawkProcess = hawkProcessSceneNetworkBehaviours;
                    PluginNetworkManager.RegisterAllBehaviors();
                    break;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(HawkNetworkManager), "InitalizeScene")]
        [HarmonyPostfix]
        public static void InitializeScenePostfix(Scene scene, LoadSceneMode loadMode)
        {
            PluginNetworkManager.onRegisteredCallback?.Invoke();
            PluginNetworkManager.onRegisteredCallback = null;
        }
    }
}
