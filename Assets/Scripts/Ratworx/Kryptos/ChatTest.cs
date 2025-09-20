using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ratworx.Kryptos
{
    public class ChatTest : MonoBehaviour, IChatClientListener
    {
        public static event Action<string> OnDecryptionCypherChanged;

        public static string DecryptionCypher;
        
        [SerializeField] private string _appId;
        // [SerializeField] private string _userId;
        [FormerlySerializedAs("_inputText")] [SerializeField] private TMP_InputField _userIdInputText;

        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _chatPanel;

        [SerializeField] private TMP_Text _messagePrefab;
        [SerializeField] private RectTransform _messageContentContainer;

        [SerializeField] private TMP_InputField _messageInputText;

        [FormerlySerializedAs("_dropdown")] [SerializeField] private TMP_Dropdown _encryptionDropdown;
        [SerializeField] private TMP_Dropdown _decryptionDropdown;
        
        [SerializeField] private TMP_InputField _addCypherInputText;
        
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

        public void Login()
        {
            if (string.IsNullOrEmpty(_userIdInputText.text)) 
                return;

            _userData.UserId = _userIdInputText.text;
            
            Connect();
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
            if (string.IsNullOrEmpty(_messageInputText.text)) 
                return;
            
            string cypher = _encryptionDropdown.options[_encryptionDropdown.value].text;

            var encryptedMessage = EncryptionController.EncodeVigenereMessage(_messageInputText.text, cypher);

            _client.PublishMessage(CHANNEL, encryptedMessage);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log($"Log {level}: {message}");
        }

        public void OnDisconnected()
        {
            // throw new NotImplementedException();
        }

        public void OnConnected()
        {
            Debug.Log($"Connected to chat with UserId {_userData.UserId}");
            
            UpdateCypherDropdowns();

            _loginPanel?.SetActive(false);
            _chatPanel?.SetActive(true);
            
            var subscribed = _client.Subscribe(new[] { CHANNEL });

            if (!subscribed) 
                Debug.LogError($"Failed to subscribe to channel {CHANNEL}");
        }

        private void UpdateCypherDropdowns()
        {
            _encryptionDropdown.options = new List<TMP_Dropdown.OptionData>();
            _decryptionDropdown.options = new List<TMP_Dropdown.OptionData>();
            
            foreach (var cypher in _userData.Cyphers)
            {
                _encryptionDropdown.options.Add(new TMP_Dropdown.OptionData(cypher));
                _decryptionDropdown.options.Add(new TMP_Dropdown.OptionData(cypher));
            }
            
            _encryptionDropdown.RefreshShownValue();
            _decryptionDropdown.RefreshShownValue();
        }

        public void OnChatStateChange(ChatState state)
        {
            // throw new NotImplementedException();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (var i = 0; i < senders.Length; i++)
            {
                var text = Instantiate(_messagePrefab, _messageContentContainer);
                text.text = $"<b>{senders[i]}</b>: {messages[i]}";
                text.gameObject.SetActive(true);
            }
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

        public void ChangeDecryptionCypher(int value)
        {
            DecryptionCypher = _decryptionDropdown.options[value].text;

            OnDecryptionCypherChanged?.Invoke(DecryptionCypher);
        }

        public void AddCypher()
        {
            if (string.IsNullOrEmpty(_addCypherInputText.text)) ;
            
            var newArray = new string[_userData.Cyphers.Length + 1];

            for (int i = 0; i < _userData.Cyphers.Length; i++)
            {
                newArray[i] = _userData.Cyphers[i];
            }

            newArray[^1] = _addCypherInputText.text;

            _userData.Cyphers = newArray;
            
            UpdateCypherDropdowns();
        }
    }
}