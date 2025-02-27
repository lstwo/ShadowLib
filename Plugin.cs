using BepInEx;
using UnityEngine;
using ShadowLib.Networking;
using BepInEx.Logging;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ShadowLib
{
    [BepInPlugin("lstwo.shadowlib", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle { get; private set; }
        public static ManualLogSource LogSource { get; private set; }

        public static List<Type> NetworkBehaviours { get; private set; } = new();

        private void Awake()
        {
            LogSource = Logger;
            bundle = AssetUtils.LoadFromEmbeddedResources("ShadowLib.Resources.assets.bundle");
            
            GameInstance.onAssignedPlayerController += PlayerUtils.OnAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += PlayerUtils.OnUnassignedPlayerController;

            QuickHarmony h = new("lstwo.shadowlib");
            h.Init(typeof(HawkNetworkManagerPatch));

            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                
            };

            Logger.LogInfo($"Plugin \"lstwo.shadowlib\" is loaded!");
        }

        private void Start()
        {
            NetworkBehaviours = GetAllInheritingTypes<ShadowNetworkSingleton>();
        }

        public static List<Type> GetAllInheritingTypes<TParent>()
        {
            var parentType = typeof(TParent);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var derivedTypes = new List<Type>();

            foreach (var assembly in loadedAssemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && parentType.IsAssignableFrom(type) && type != typeof(ShadowNetworkBehaviour));
                    derivedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var types = ex.Types.Where(type => type != null && type.IsClass && !type.IsAbstract && parentType.IsAssignableFrom(type) && type != typeof(ShadowNetworkBehaviour));
                    derivedTypes.AddRange(types);
                }
            }

            return derivedTypes;
        }
    }
}
