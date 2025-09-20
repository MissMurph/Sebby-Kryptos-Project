using System;
using TMPro;
using UnityEngine;

namespace Ratworx.Kryptos
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private string _initialText;
        
        private void OnEnable()
        {
            _initialText = _text.text;
            
            ChatController.OnDecryptionCypherChanged += OnDecryptCypherChanged;

            if (!string.IsNullOrEmpty(ChatController.DecryptionCypher)) 
                DecryptMessage();
        }

        private void DecryptMessage()
        {
            var splitString = _initialText.Split(": ");

            if (splitString.Length != 2) return;

            var decryptedText = VigenereEncryptionHelper.DecodeVigenereMessage(splitString[1], ChatController.DecryptionCypher);

            _text.text = $"{splitString[0]}: {decryptedText}";
        }

        private void OnDecryptCypherChanged(string cypher) => DecryptMessage();
    }
}
