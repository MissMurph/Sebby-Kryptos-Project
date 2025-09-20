using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Ratworx.Kryptos
{
    public class EncryptionController : MonoBehaviour
    {
        private static char[] ALPHABET =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z',
        };

        [SerializeField] private TMP_InputField _inputText;
        [SerializeField] private TMP_InputField _outputText;
        [SerializeField] private TMP_InputField _cypherInputText;

        private string _cypherBet;

        public void EncodeText()
        {
            // TODO: Create a way of displaying an error to a user here
            if (string.IsNullOrEmpty(_cypherInputText.text)
                || IsCypherInvalid(_cypherInputText.text)
                || string.IsNullOrEmpty(_inputText.text)) return;
            
            CreateVigenereCypherbet();

            string encodedText = EncodeVigenereMessage(_inputText.text);

            _outputText.text = encodedText;
        }

        public void DecodeText()
        {
            // TODO: Create a way of displaying an error to a user here
            if (string.IsNullOrEmpty(_cypherInputText.text)
                || IsCypherInvalid(_cypherInputText.text)
                || string.IsNullOrEmpty(_inputText.text)) return;
            
            CreateVigenereCypherbet();

            string decodedText = DecodeVigenereMessage(_inputText.text);

            _outputText.text = decodedText;
        }

        private bool IsCypherInvalid(string cypher)
        {
            List<char> foundChars = new();
            
            foreach (var character in cypher)
            {
                if (foundChars.Contains(character)) 
                    return true;
                
                foundChars.Add(character);
            }

            return false;
        }

        /// <summary>
        /// This kind of cypher is created by extracting the characters matching the cypher word and moving them
        /// to the start of the alphabet. This function achieves this by using a <see cref="StringBuilder"/> to
        /// sequentially place these at the start of the string and then append the remaining letters of the alphabet
        /// onto this string, filtering out characters we've already added.
        /// </summary>
        private void CreateVigenereCypherbet()
        {
            var stringBuilder = new StringBuilder(string.Empty);

            int index = 0;

            while (index < _cypherInputText.text.Length - 1)
            {
                foreach (var character in ALPHABET)
                {
                    if (index >= _cypherInputText.text.Length) break;
                    if (character == _cypherInputText.text[index])
                    {
                        stringBuilder.Append(character);
                        index++;
                    }
                }
            }

            for (int i = 0; i < ALPHABET.Length; i++)
            {
                if (stringBuilder.ToString().Contains(ALPHABET[i])) continue;
                stringBuilder.Append(ALPHABET[i]);
            }

            _cypherBet = stringBuilder.ToString();
        }

        /// <summary>
        /// This will encrypt a message using a Vigenere cypher method with a cypher word set. Vigenere encryption will
        /// first replace every character with the cypher word to determine a 'cypher interval'. This interval is then
        /// added with the position of the character on the normal alphabet. We then get the character on the cypherbet
        /// that corresponds with this position, and that's the resulting encoded character.
        /// </summary>
        /// <param name="message">The message to encrypt.</param>
        /// <remarks>This will respect casing and non-alphabetical characters. These will simply be copied over to the
        /// encoded message without any change to them.</remarks>
        /// <returns>The encrypted message.</returns>
        private string EncodeVigenereMessage(string message)
        {
            var encodedText = new StringBuilder(string.Empty);

            int cypherIndex = 0;

            for (var i = 0; i < message.Length; i++)
            {
                var regex = new Regex("[^a-zA-Z]");

                if (regex.IsMatch(message[i].ToString()))
                {
                    encodedText.Append(message[i].ToString());
                    continue;
                }

                bool isCased = char.IsUpper(message[i]);

                int alphabetIndex = GetAlphabetIndexOfLetter(message[i]);

                int cypheredIndex = alphabetIndex + cypherIndex;

                if (cypheredIndex >= _cypherBet.Length) cypheredIndex -= _cypherBet.Length;

                if (cypheredIndex < 0 || cypheredIndex >= _cypherBet.Length)
                {
                    Debug.LogError(
                        $"Cypher out of range:" +
                        $"\ncyphered index: {cypheredIndex} " +
                        $"\nalphabet index: {alphabetIndex} " +
                        $"\ncypher word position: {cypherIndex}");
                }

                var encodedCharacter = _cypherBet[cypheredIndex].ToString();

                if (isCased) encodedCharacter = encodedCharacter.ToUpperInvariant();

                encodedText.Append(encodedCharacter);

                cypherIndex++;

                if (cypherIndex >= _cypherInputText.text.Length) cypherIndex = 0;
            }

            return encodedText.ToString();
        }

        /// <summary>
        /// This will decrypt a message using a Vigenere cypher method with a cypher word set. Vigenere decryption will
        /// first replace every character with the cypher word to determine a 'cypher interval'. We then must determine
        /// the position of the encoded character on the cypherbet and subtract the cypher interval from this position.
        /// We then get the letter on the normal alphabet that corresponds with this new calculated position.
        /// </summary>
        /// <param name="encodedMessage">The message to decrypt.</param>
        /// <remarks>This will respect casing and non-alphabetical characters. These will simply be copied over to the
        /// decoded message without any change to them.</remarks>
        /// <returns>The decrypted message.</returns>
        private string DecodeVigenereMessage(string encodedMessage)
        {
            var decodedText = new StringBuilder(string.Empty);

            int decodingCypherIndex = 0;

            for (int i = 0; i < encodedMessage.Length; i++)
            {
                var regex = new Regex("[^a-zA-Z]");

                if (regex.IsMatch(encodedMessage[i].ToString()))
                {
                    decodedText.Append(encodedMessage[i].ToString());
                    continue;
                }

                bool isCased = char.IsUpper(encodedMessage[i]);

                var decodingCypheredIndex = GetCypheredIndexOfCharacter(encodedMessage[i]);

                int cypheredIndex = decodingCypheredIndex - decodingCypherIndex;

                if (cypheredIndex < 0) cypheredIndex += _cypherBet.Length;

                if (cypheredIndex < 0 || cypheredIndex >= _cypherBet.Length)
                {
                    Debug.LogError(
                        $"Cypher out of range:" +
                        $"\ncyphered index: {cypheredIndex} " +
                        $"\ndecoding cyphered index: {decodingCypheredIndex} " +
                        $"\ncypher word position: {decodingCypherIndex}");
                }

                var decodedCharacter = ALPHABET[cypheredIndex].ToString();

                if (isCased) decodedCharacter = decodedCharacter.ToUpperInvariant();

                decodedText.Append(decodedCharacter);

                decodingCypherIndex++;

                if (decodingCypherIndex >= _cypherInputText.text.Length) decodingCypherIndex = 0;
            }

            return decodedText.ToString();
        }

        private int GetAlphabetIndexOfLetter(char character)
        {
            var lowerInvariant = character.ToString().ToLowerInvariant();

            for (int i = 0; i < ALPHABET.Length; i++)
            {
                if (ALPHABET[i] == lowerInvariant[0]) return i;
            }

            Debug.LogError($"Character not found in Alphabet. How have you written a non-existent character?");
            return -1;
        }

        private int GetCypheredIndexOfCharacter(char character)
        {
            var lowerInvariant = character.ToString().ToLowerInvariant();

            for (int i = 0; i < _cypherBet.Length; i++)
            {
                if (_cypherBet[i] == lowerInvariant[0]) return i;
            }

            Debug.LogError($"Character not found in Cypherbet. How have you written a non-existent character?");
            return -1;
        }

        public void QuitApp()
        {
            Application.Quit();
        }
    }
}