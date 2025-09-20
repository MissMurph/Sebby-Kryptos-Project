using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using TMPro;
using UnityEngine;

namespace Ratworx.Kryptos
{
    public class ChatTest : MonoBehaviour, IChatClientListener
    {
        [SerializeField] private string _appId;
        // [SerializeField] private string _userId;
        [SerializeField] private TMP_InputField _inputText;

        [SerializeField] private GameObject _loginPanel;
        
        private ChatClient _client;

        private UserInfo _userData;

        private const string CHANNEL = "test_channel";
        
        private void Awake()
        {
            _userData = Resources.Load<UserInfo>("UserData");

            if (_userData is null
                || string.IsNullOrEmpty(_userData.UserId))
            {
                OpenLoginPanel();
                return;
            }
            
            _loginPanel?.SetActive(false);
            Connect();
        }

        public void OpenLoginPanel()
        {
            _loginPanel?.SetActive(true);
        }

        public void Connect()
        {
            _client = new ChatClient(this);
            _client.ChatRegion = "ASIA";
            _client.Connect(_appId, "yeet", new AuthenticationValues(_userData.UserId));
        }

        private void Update() => _client?.Service();

        public void SendMessage()
        {
            if (string.IsNullOrEmpty(_inputText.text)) 
                return;

            _client.PublishMessage(CHANNEL, _inputText.text);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            // throw new NotImplementedException();
        }

        public void OnDisconnected()
        {
            // throw new NotImplementedException();
        }

        public void OnConnected()
        {
            Debug.Log($"Connected to chat with UserId {_userData.UserId}");
            
            var subscribed = _client.Subscribe(new[] { CHANNEL });

            if (!subscribed) 
                Debug.LogError($"Failed to subscribe to channel {CHANNEL}");
        }

        public void OnChatStateChange(ChatState state)
        {
            // throw new NotImplementedException();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            Debug.Log(
                $"Received message from channel [{channelName}] from senders {string.Join(',', senders)} with messages {string.Join(',', messages)}");
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            // throw new NotImplementedException();
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log($"Subscribed to channels: {string.Join(',', channels)}" + 
                $"\n with results: {string.Join(',', results)}");
        }

        public void OnUnsubscribed(string[] channels)
        {
            // throw new NotImplementedException();
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            // throw new NotImplementedException();
        }

        public void OnUserSubscribed(string channel, string user)
        {
            // throw new NotImplementedException();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            // throw new NotImplementedException();
        }
    }
}