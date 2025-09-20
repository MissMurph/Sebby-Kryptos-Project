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
            
            ChatTest.OnDecryptionCypherChanged += OnDecryptCypherChanged;

            if (!string.IsNullOrEmpty(ChatTest.DecryptionCypher)) 
                DecryptMessage();
        }

        private void DecryptMessage()
        {
            var splitString = _initialText.Split(": ");

            if (splitString.Length != 2) return;

            var decryptedText = EncryptionController.DecodeVigenereMessage(splitString[1], ChatTest.DecryptionCypher);

            _text.text = decryptedText;
        }

        private void OnDecryptCypherChanged(string cypher) => DecryptMessage();
    }
}
