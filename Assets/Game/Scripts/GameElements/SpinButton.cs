using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Game.Scripts.GameElements
{
    public class SpinButton : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
        [SerializeField] private SlotController _slotController;
        [SerializeField] private SpriteAtlas _slotModelAtlas;
        [SerializeField] private Image _spinButtonImage;
        
        private void OnEnable()
        {
            _spinButton.onClick.AddListener(OnSpinClick);
            _slotController.OnSpinStateChange += OnSpinStateChange;
            _spinButtonImage.sprite = _slotModelAtlas.GetSprite("Spin_button");
        }

        private void OnDisable()
        {
            _spinButton.onClick.RemoveListener(OnSpinClick);
            _slotController.OnSpinStateChange -= OnSpinStateChange;
        }

        private void OnSpinStateChange(bool isSpinning)
        {
            _spinButton.interactable = !isSpinning;
        }

        private void OnSpinClick()
        {
            int randomSpinIndex = Random.Range(0, 3);
            _slotController.Spin(randomSpinIndex);
        }
    }
}