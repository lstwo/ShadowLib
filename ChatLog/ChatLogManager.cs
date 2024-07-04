﻿using HawkNetworking;
using ShadowLib;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowLib.ChatLog
{
    public class ChatLogManager : HawkNetworkBehaviour
    {
        public static GameObject canvas;
        public static GameObject chatLogRoot;

        public static GameObject chatLogItem;
        public static GameObject canvasPrefab;

        private static ChatLogManager _instance;

        public static ChatLogManager Instance { get { return _instance; } }

        private byte RPC_CLIENT_RECEIVE_MESSAGE;

        protected override void RegisterRPCs(HawkNetworkObject networkObject)
        {
            base.RegisterRPCs(networkObject);

            RPC_CLIENT_RECEIVE_MESSAGE = networkObject.RegisterRPC(ClientReceiveLogMessage);
        }

        internal void Init()
        {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);

            canvasPrefab = Plugin.bundle.LoadAsset<GameObject>("ChatLogCanvas");
            canvas = Instantiate(canvasPrefab);
            DontDestroyOnLoad(canvas);
            chatLogRoot = canvas.transform.GetChild(0).gameObject;
            chatLogItem = Plugin.bundle.LoadAsset<GameObject>("ChatLogItem");
        }

        public void ServerSendLogMessage(string text, Color textColor)
        {
            Debug.Log(RPC_CLIENT_RECEIVE_MESSAGE);
            networkObject.SendRPC(RPC_CLIENT_RECEIVE_MESSAGE, RPCRecievers.All, text, textColor);
        }

        public void ServerSendLogMessage(string text)
        {
            ServerSendLogMessage(text, Color.white);
        }

        private void ClientReceiveLogMessage(HawkNetReader reader, HawkRPCInfo info)
        {
            var text = reader.ReadString();
            var textColor = reader.ReadColor();

            var itemObj = Instantiate(chatLogItem);
            DontDestroyOnLoad(itemObj);
            itemObj.transform.SetParent(chatLogRoot.transform);

            var item = itemObj.AddComponent<ChatLogItem>();
            item.text = itemObj.transform.Find("Text").GetComponent<Text>();

            item.text.text = text;
            item.text.color = textColor;
        }
    }
}   
