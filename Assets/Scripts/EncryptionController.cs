using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class EncryptionController
    : MonoBehaviour
{
    private static char[] ALPHABET = {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    };
    
    [SerializeField] private string _inputText;
    [SerializeField] private string _cypherWord;

    private string _cypherBet;

    private void Awake()
    {
        var stringBuilder = new StringBuilder(string.Empty);
        
        int index = 0;

        while (index < _cypherWord.Length - 1)
        {
            foreach (var character in ALPHABET)
            {
                if (index >= _cypherWord.Length) break;
                if (character == _cypherWord[index])
                {
                    stringBuilder.Append(character);
                    index++;
                }
            }
        }

        Debug.Log(stringBuilder);

        for (int i = 0; i < ALPHABET.Length; i++)
        {
            if (stringBuilder.ToString().Contains(ALPHABET[i])) continue;
            stringBuilder.Append(ALPHABET[i]);
        }
        
        Debug.Log(stringBuilder);

        _cypherBet = stringBuilder.ToString();

        string encodedText = EncodeMessage(_inputText);
        
        Debug.Log($"Input Text: {_inputText}");

        Debug.Log($"Encoded Text: {encodedText}");
        
        string decodedText = DecodeMessage(encodedText);

        Debug.Log($"Decoded Text: {decodedText}");
    }

    private string EncodeMessage(string message)
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
                Debug.LogError($"Cypher out of range, cyphered index: {cypheredIndex} alphabet index: {alphabetIndex} cypher word position: {cypherIndex}");
            }
            
            var encodedCharacter = _cypherBet[cypheredIndex].ToString();

            if (isCased) encodedCharacter = encodedCharacter.ToUpperInvariant();

            encodedText.Append(encodedCharacter);

            cypherIndex++;

            if (cypherIndex >= _cypherWord.Length) cypherIndex = 0;
        }

        return encodedText.ToString();
    }

    private string DecodeMessage(string encodedMessage)
    {
        var decodedText = new StringBuilder(string.Empty);
        
        int decodingCypherIndex = 0;

        for (int i = 0; i < encodedMessage.Length; i++)
        {
            var regex = new Regex("[^a-zA-Z]");

            if (regex.IsMatch(_inputText[i].ToString()))
            {
                decodedText.Append(_inputText[i].ToString());
                continue;
            }
            
            bool isCased = char.IsUpper(encodedMessage[i]);
            
            var decodingCypheredIndex = GetCypheredIndexOfCharacter(encodedMessage[i]);
            
            int cypheredIndex = decodingCypheredIndex - decodingCypherIndex;

            if (cypheredIndex < 0) cypheredIndex += _cypherBet.Length;

            if (cypheredIndex < 0 || cypheredIndex >= _cypherBet.Length)
            {
                Debug.LogError($"Cypher out of range, cyphered index: {cypheredIndex} decoding cyphered index: {decodingCypheredIndex} cypher word position: {decodingCypherIndex}");
            }

            var decodedCharacter = ALPHABET[cypheredIndex].ToString();
            
            if (isCased) decodedCharacter = decodedCharacter.ToUpperInvariant();

            decodedText.Append(decodedCharacter);
            
            decodingCypherIndex++;

            if (decodingCypherIndex >= _cypherWord.Length) decodingCypherIndex = 0;
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
}
