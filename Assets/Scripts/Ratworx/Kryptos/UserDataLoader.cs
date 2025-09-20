using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Ratworx.Kryptos
{
    public static class UserDataLoader
    {
        private static string _dataPath = Path.Combine(Application.persistentDataPath, "UserData.json");
        
        public static bool TryLoadUserData(out UserInfo userData)
        {
            try
            {
                if (!File.Exists(_dataPath))
                {
                    userData = null;
                    return false;
                }
            
                using FileStream fileStream = File.Open(_dataPath, FileMode.Open);
                byte[] data = new byte[fileStream.Length];
                _ = fileStream.Read(data, 0, data.Length);

                string rawtext = Encoding.UTF8.GetString(data);

                userData = JsonConvert.DeserializeObject<UserInfo>(rawtext);
                return true;

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            userData = null;
            return false;
        }

        public static async Task WriteUserData(UserInfo userData)
        {
            try
            {
                var rawtext = JsonConvert.SerializeObject(userData);

                byte[] data = Encoding.UTF8.GetBytes(rawtext);
            
                await using FileStream fileStream = File.Open(_dataPath, FileMode.Create);
                fileStream.Position = 0;
                await fileStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}