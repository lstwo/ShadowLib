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

        private static List<HawkNetworkBehaviour> moddedNetworkBehaviours = new();

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

        /// <summary>
        /// NOTE: This will NOT Register them! This only ADDS them to the current HawkProcessSceneNetworkBehaviours Instance!!!
        /// </summary>
        public static void RegisterAllBehaviors()
        {
            foreach(var behaviour in moddedNetworkBehaviours)
            {
                if (currentHawkProcess.GetBehaviours().Contains(behaviour) || behaviour == null) continue;

                if (behaviour.networkObject != null) continue;

                Plugin.LogSource.LogMessage($"Registering behavior {behaviour}");
                currentHawkProcess.GetBehaviours().Add(behaviour);
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
                    hawkProcessSceneNetworkBehaviours = obj.GetComponent<HawkProcessSceneNetworkBehaviours>();
                    PluginNetworkManager.currentHawkProcess = hawkProcessSceneNetworkBehaviours;
                    PluginNetworkManager.RegisterAllBehaviors();
                    break;
                }
            }

            return true;
        }
    }
}
