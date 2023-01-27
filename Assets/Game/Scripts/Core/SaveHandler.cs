using System;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveHandler : MonoBehaviour
    {
        public Action onLoadState;
        public Action onGameCloseState;

        private void OnEnable()
        {
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