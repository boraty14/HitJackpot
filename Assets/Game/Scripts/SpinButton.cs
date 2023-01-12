using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private Button _spinButton;
    [SerializeField] private SlotController _slotController;
    
    private void OnEnable()
    {
        _spinButton.onClick.AddListener(OnSpinClick);
        _slotController.OnSpinStateChange += OnSpinStateChange;
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