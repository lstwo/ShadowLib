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
    }

    public static class HawkNetworkManagerPatch
    {
        [HarmonyPatch(typeof(HawkNetworkManager), "InitalizeScene")]
        [HarmonyPrefix]
        public static bool InitializeScenePrefix(ref HawkNetworkManager __instance, ref Scene scene, ref LoadSceneMode loadMode)
        {
            // Getting private fields

            var qr = new QuickReflection<HawkNetworkManager>(__instance, BindingFlags.Instance | BindingFlags.NonPublic);

            List<GameObject> rootGameObjectsCache = (List<GameObject>)qr.GetField("rootGameObjectsCache");
            int registerInstanceid = (int)qr.GetField("registerInstanceid");
            List<KeyValuePair<int, int>> registeringSceneBehavioursInstances = (List<KeyValuePair<int, int>>)qr.GetField("registeringSceneBehavioursInstances");
            List<HawkNetworkBehaviour> sceneLoadBehavioursAllCache = (List<HawkNetworkBehaviour>)qr.GetField("sceneLoadBehavioursAllCache");
            List<HawkNetworkBehaviour> sceneLoadBehavioursCache = (List<HawkNetworkBehaviour>)qr.GetField("sceneLoadBehavioursCache");

            // Original Code

            Action<Scene, LoadSceneMode> action = __instance.onPreInitalizeScene;
            if (action != null)
            {
                action(scene, loadMode);
            }
            rootGameObjectsCache.Clear();
            scene.GetRootGameObjects(rootGameObjectsCache);
            bool flag = false;
            for (int i = rootGameObjectsCache.Count - 1; i >= 0; i--)
            {
                if (rootGameObjectsCache[i].TryGetComponent<HawkProcessSceneNetworkBehaviours>(out var hawkProcessSceneNetworkBehaviours))
                {
                    // Modded Code
                    PluginNetworkManager.currentHawkProcess = hawkProcessSceneNetworkBehaviours;

                    List<HawkNetworkBehaviour> networkBehaviours = (List<HawkNetworkBehaviour>)QuickReflection<HawkProcessSceneNetworkBehaviours>.GetField(hawkProcessSceneNetworkBehaviours,
                        BindingFlags.Instance | BindingFlags.NonPublic, "behaviours");

                    foreach (HawkNetworkBehaviour behaviour in PluginNetworkManager.GetNetworkBehaviours())
                    {
                        if (networkBehaviours.Contains(behaviour)) continue;
                        networkBehaviours.Add(behaviour);
                    }

                    QuickReflection<HawkProcessSceneNetworkBehaviours>.SetField(hawkProcessSceneNetworkBehaviours,
                        BindingFlags.Instance | BindingFlags.NonPublic, "behaviours", networkBehaviours);

                    // Original Code
                    flag = true;
                    int num = registerInstanceid;
                    registerInstanceid = num + 1;
                    int num2 = num;
                    registeringSceneBehavioursInstances.Add(new KeyValuePair<int, int>(scene.path.GetHashCode(), num2));
                    __instance.StartCoroutine((IEnumerator)qr.GetMethod("RegisterSceneNetworkBehavioursBatch", hawkProcessSceneNetworkBehaviours.GetBehaviours(), scene.path.GetHashCode(), new int?(num2),
                        hawkProcessSceneNetworkBehaviours));
                    break;
                }
            }
            if (!flag)
            {
                sceneLoadBehavioursAllCache.Clear();
                for (int j = 0; j < rootGameObjectsCache.Count; j++)
                {
                    GameObject gameObject = rootGameObjectsCache[j];
                    sceneLoadBehavioursCache.Clear();
                    gameObject.GetComponentsInChildren(true, sceneLoadBehavioursCache);
                    sceneLoadBehavioursAllCache.AddRange(sceneLoadBehavioursCache);
                }
                qr.GetMethod("RegisterNetworkBehaviours", scene, sceneLoadBehavioursAllCache);
            }

            // Setting private fields

            qr.SetField("rootGameObjectsCache", rootGameObjectsCache);
            qr.SetField("registerInstanceid", registerInstanceid);
            qr.SetField("registeringSceneBehavioursInstances", registeringSceneBehavioursInstances);
            qr.SetField("sceneLoadBehavioursAllCache", sceneLoadBehavioursAllCache);
            qr.SetField("sceneLoadBehavioursCache", sceneLoadBehavioursCache);

            return false;
        }
    }
}
