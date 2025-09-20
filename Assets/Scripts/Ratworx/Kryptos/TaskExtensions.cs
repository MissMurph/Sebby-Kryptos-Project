using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Ratworx.Kryptos
{
    public static class TaskExtensions
    {
        public static async void ForgetTaskSafely(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}