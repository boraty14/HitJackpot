using Game.Scripts.SlotElement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.UI
{
    public class SpinButtonSystem : IStartable
    {
        private readonly SpinButton _spinButton;
        private readonly SlotController _slotController;

        [Inject]
        public SpinButtonSystem(SpinButton spinButton, SlotController slotController)
        {
            _spinButton = spinButton;
            _slotController = slotController;
            spinButton.onEnableState = OnEnable;
            spinButton.onDisableState = OnDisable;
        }

        private void OnEnable()
        {
            _spinButton.spinButton.onClick.AddListener(OnSpinClick);
            _slotController.OnSpinStateChange += OnSpinStateChange;
            _spinButton.spinButtonImage.sprite = _spinButton.slotModelAtlas.GetSprite("Spin_button");
        }

        private void OnDisable()
        {
            _spinButton.spinButton.onClick.RemoveListener(OnSpinClick);
            _slotController.OnSpinStateChange -= OnSpinStateChange;
        }

        private void OnSpinClick()
        {
            int randomSpinIndex = Random.Range(0, 3);
            _slotController.Spin(randomSpinIndex);
        }

        private void OnSpinStateChange(bool isSpinning)
        {
            _spinButton.spinButton.interactable = !isSpinning;
        }

        public void Start()
        {
        }
    }
}