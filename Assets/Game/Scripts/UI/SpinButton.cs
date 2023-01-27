using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpinButton : MonoBehaviour
    {
        public Button spinButton;
        public SpriteAtlas slotModelAtlas;
        public Image spinButtonImage;
        public Action onEnableState;
        public Action onDisableState;
        
        private void OnEnable()
        {
            onEnableState?.Invoke();
        }

        private void OnDisable()
        {
            onDisableState?.Invoke();
            
        }
    }
}