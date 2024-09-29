using HawkNetworking;
using ShadowLib;
using ShadowLib.Networking;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowLib.ChatLog
{
    public class ChatLogManager : ShadowNetworkBehaviour
    {
        public static GameObject canvas;
        public static GameObject chatLogRoot;

        public static GameObject chatLogItem;
        public static GameObject canvasPrefab;

        private static ChatLogManager _instance;

        public static ChatLogManager Instance { get { return _instance; } }

        private byte RPC_CLIENT_RECEIVE_MESSAGE;
        private byte RPC_CLIENT_RECEIVE_PRIVATE_MESSAGE;

        protected override void RegisterRPCs(HawkNetworkObject networkObject)
        {
            base.RegisterRPCs(networkObject);

            RPC_CLIENT_RECEIVE_MESSAGE = networkObject.RegisterRPC(ClientReceiveLogMessage);
            RPC_CLIENT_RECEIVE_PRIVATE_MESSAGE = networkObject.RegisterRPC(ClientReceivePrivateLogMessage);
        }

        protected override void Awake()
        {
            base.Awake();

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
            if (networkObject != null)
            {
                networkObject.SendRPC(RPC_CLIENT_RECEIVE_MESSAGE, RPCRecievers.All, text, textColor);
            } else
            {
                Debug.LogWarning("can't send log message; either not in active lobby or network object is null!");
            }
        }

        public void ServerSendPrivateLogMessage(string text, PlayerController receiver, Color textColor)
        {
            if (networkObject != null)
            {
                networkObject.SendRPC(RPC_CLIENT_RECEIVE_PRIVATE_MESSAGE, receiver.networkObject.GetOwner(), text, textColor, (Int32)receiver.networkObject.GetNetworkID());
            }
            else
            {
                Debug.LogWarning("can't send log message; either not in active lobby or network object is null!");
            }
        }

        public void ServerSendLogMessage(string text)
        {
            ServerSendLogMessage(text, Color.white);
        }

        public void ServerSendPrivateLogMessage(string text, PlayerController receiver)
        {
            ServerSendPrivateLogMessage(text, receiver, Color.white);
        }

        private void ClientReceiveLogMessage(HawkNetReader reader, HawkRPCInfo info)
        {
            var text = reader.ReadString();
            var textColor = reader.ReadColor();

            ClientSendLogMessage(text, textColor);
        }

        private void ClientReceivePrivateLogMessage(HawkNetReader reader, HawkRPCInfo info)
        {
            if(reader.ReadInt32() == PlayerUtils.GetMyPlayer().networkObject.GetNetworkID())
            {
                ClientReceiveLogMessage(reader, info);
            }
        }

        public void ClientSendLogMessage(string text, Color textColor)
        {
            Plugin.LogSource.LogMessage(text);

            var itemObj = Instantiate(chatLogItem);
            DontDestroyOnLoad(itemObj);
            itemObj.transform.SetParent(chatLogRoot.transform);

            var item = itemObj.AddComponent<ChatLogItem>();
            item.text = itemObj.transform.Find("Text").GetComponent<Text>();

            item.text.text = text;
            item.text.color = textColor;
        }

        public void ClientSendLogMessage(string text)
        {
            ClientSendLogMessage(text, Color.white);
        }
    }
}   
