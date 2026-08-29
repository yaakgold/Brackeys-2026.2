using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;

namespace Text
{
    public class ChatManager : NetworkBehaviour
    {
        public static ChatManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        [SerializeField] private PanelRenderer panelRenderer;
        [SerializeField] private List<ChatMessage> chats = new();

        private VisualElement _root;

        
        void OnEnable()
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        void OnDisable()
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
        
        void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
        {
            _root = rootElement;

            var chatLog = _root.Q<ListView>("lstChat");
            chatLog.itemsSource = chats;
            chatLog.makeItem = () => chatLog.itemTemplate.Instantiate();
            chatLog.bindItem = (element, i) =>
            {
                element.Q<Label>("sender").text = DateTime.Now.ToShortTimeString() + " " + chats[i].sender;
                element.Q<Label>("message").text = chats[i].message;
            };

            _root.Q<Button>("btnSend").RegisterCallback<ClickEvent>(SendChat);
            
            _root.Q<Button>("btnChat").RegisterCallback<ClickEvent>(ToggleChatVisual);
        }

        private void ToggleChatVisual(ClickEvent evt)
        {
            var display = _root.Q("chatDisplay");
            display.style.display = display.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex; 
            
            //_root.Q<Label>("message").Focus();
        }

        private void SendChat(ClickEvent evt)
        {
            var message = _root.Q<TextField>("txtInput").text;
            if (message.IsNullOrEmpty()) return;
            
            SendChatMessageServerRpc(AuthenticationService.Instance.PlayerName.Split("#")[0], message);
        }

        private void AddMessage(string sender, string message)
        {
            chats.Add(new ChatMessage
            {
                sender = sender,
                message = message
            });
            
            _root.Q<ListView>("lstChat").RefreshItems();
            _root.Q<TextField>("txtInput").value = string.Empty;
        }

        [Rpc(SendTo.Server)]
        private void SendChatMessageServerRpc(string sender, string message)
        {
            ReceiveChatMessageClientRpc(sender, message);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ReceiveChatMessageClientRpc(string sender, string message)
        {
            AddMessage(sender, message);
        }

        public void EnableChat()
        {
            _root.Q<Button>("btnChat").enabledSelf = true;
        }

        public void DisableChat()
        {
            _root.Q<Button>("btnChat").enabledSelf = false;
            _root.Q("chatDisplay").style.display = DisplayStyle.None;
        }
    }
}
