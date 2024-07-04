using BepInEx;
using UnityEngine.Diagnostics;
using UnityEngine;
using ShadowLib.ChatLog;

namespace ShadowLib
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle { get; private set; }

        private void Awake()
        {
            bundle = AssetUtils.LoadAssetBundleFromPluginsFolder("lstwo.shadowlib");
            GameInstance.onAssignedPlayerController += PlayerUtils.OnAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += PlayerUtils.OnUnassignedPlayerController;

            var logManager = new GameObject("ChatLogManager");
            DontDestroyOnLoad(logManager);
            logManager.AddComponent<ChatLogManager>().Init();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
