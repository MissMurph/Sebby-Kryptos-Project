using UnityEngine;

namespace Ratworx.Kryptos
{
    [CreateAssetMenu(fileName = "UserData.asset", menuName = "Ratworx/UserData")]
    public class UserInfo : ScriptableObject
    {
        public string UserId;
        public string[] Cyphers;
    }
}