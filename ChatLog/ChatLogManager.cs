using ShadowLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniverseLib.UI;

namespace ShadowLib.ChatLog
{
    public static class ChatLogManager
    {
        public static GameObject canvas;
        public static GameObject chatLogRoot;

        public static GameObject chatLogItem;
        public static GameObject canvasPrefab;

        internal static void Init()
        {
            canvasPrefab = Plugin.bundle.LoadAsset<GameObject>("ChatLogCanvas");
            canvas = Object.Instantiate(canvasPrefab);
            Object.DontDestroyOnLoad(canvas);
            chatLogRoot = canvas.transform.GetChild(0).gameObject;
            chatLogItem = Plugin.bundle.LoadAsset<GameObject>("ChatLogItem");
        }

        public static GameObject SendLogMessage(string text, Color textColor)
        {
            GameObject itemObj = Object.Instantiate(chatLogItem);
            Object.DontDestroyOnLoad(itemObj);
            itemObj.transform.SetParent(chatLogRoot.transform);

            ChatLogItem item = itemObj.AddComponent<ChatLogItem>();
            item.text = itemObj.transform.Find("Text").GetComponent<Text>();

            item.text.text = text;
            item.text.color = textColor;

            return itemObj;
        }

        public static GameObject SendLogMessage(string text)
        {
            return SendLogMessage(text, Color.white);
        }
    }
}
