using HarmonyLib;
using HawkNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowLib.Networking
{
    public static class PluginNetworkManager
    {
        public static HawkProcessSceneNetworkBehaviours currentHawkProcess;

        private static readonly List<HawkNetworkBehaviour> moddedNetworkBehaviours = new();

        public static void AddNetworkBehaviour(HawkNetworkBehaviour networkBehaviour)
        {
            moddedNetworkBehaviours.Add(networkBehaviour);
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
            foreach(var obj in scene.GetRootGameObjects())
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
    }
}
