using BepInEx;
using UnityEngine.Diagnostics;
using UnityEngine;
using ShadowLib.ChatLog;
using ShadowLib.Networking;
using BepInEx.Logging;
using UnityEngine.SceneManagement;
using System.CodeDom;
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

            bundle = AssetUtils.LoadAssetBundleFromPluginsFolder("lstwo.shadowlib");
            GameInstance.onAssignedPlayerController += PlayerUtils.OnAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += PlayerUtils.OnUnassignedPlayerController;

            QuickHarmony h = new("lstwo.shadowlib");
            h.Init(typeof(HawkNetworkManagerPatch));

            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                if(loadMode == LoadSceneMode.Single)
                {
                    foreach (Type type in NetworkBehaviours)
                    {
                        var obj = new GameObject();
                        SceneManager.MoveGameObjectToScene(obj, scene);
                        obj.AddComponent(type);
                    }
                }
            };

            Logger.LogInfo($"Plugin \"lstwo.shadowlib\" is loaded!");
        }

        private void Start()
        {
            NetworkBehaviours = GetAllInheritingTypes<ShadowNetworkSingleton>();
        }

        /// <summary>
        /// Gets all types that inherit from a specified parent type across all loaded assemblies.
        /// </summary>
        /// <typeparam name="TParent">The parent type to search for inheritors.</typeparam>
        /// <returns>A list of types that inherit from the specified parent type.</returns>
        public static List<Type> GetAllInheritingTypes<TParent>()
        {
            var parentType = typeof(TParent);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Iterate through each assembly and search for types that inherit from the parent type.
            var derivedTypes = new List<Type>();
            foreach (var assembly in loadedAssemblies)
            {
                try
                {
                    // Get all types in the assembly
                    var types = assembly.GetTypes()
                        .Where(type => type.IsClass && !type.IsAbstract && parentType.IsAssignableFrom(type));

                    derivedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle types that couldn't be loaded
                    var types = ex.Types
                        .Where(type => type != null && type.IsClass && !type.IsAbstract && parentType.IsAssignableFrom(type));
                    derivedTypes.AddRange(types);
                }
            }

            return derivedTypes;
        }
    }
}
