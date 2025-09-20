using System;
using UnityEngine;

namespace Ratworx.Kryptos
{
    [Serializable]
    public class UserInfo
    {
        public string UserId = string.Empty;
        public string[] Cyphers = Array.Empty<string>();
    }
}