﻿using BepInEx;
using UnityEngine.Diagnostics;
using UnityEngine;
using ShadowLib.ChatLog;
using ShadowLib.Networking;
using BepInEx.Logging;

namespace ShadowLib
{
    [BepInPlugin("lstwo.shadowlib", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle { get; private set; }
        public static ManualLogSource LogSource { get; private set; }

        private void Awake()
        {
            LogSource = Logger;

            bundle = AssetUtils.LoadAssetBundleFromPluginsFolder("lstwo.shadowlib");
            GameInstance.onAssignedPlayerController += PlayerUtils.OnAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += PlayerUtils.OnUnassignedPlayerController;

            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            obj.AddComponent<ChatLogManager>();

            QuickHarmony h = new("lstwo.shadowlib");
            h.Init(typeof(HawkNetworkManagerPatch));

            Logger.LogInfo($"Plugin \"lstwo.shadowlib\" is loaded!");
        }
    }
}
