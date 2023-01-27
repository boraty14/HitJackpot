using System;
using Game.Scripts.Spin;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveHandler : MonoBehaviour
    {
        public Action onLoadState;
        public Action onGameCloseState;

        private void OnEnable()
        {
            Debug.Log(2);
            onLoadState?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            onGameCloseState?.Invoke();
        }

        private void OnApplicationQuit()
        {
            onGameCloseState?.Invoke();
        }
    }
}