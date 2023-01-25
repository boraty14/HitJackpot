using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SlotModel : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _slotModelAtlas;
    [SerializeField] private Image _slotBackImage;
    [SerializeField] private Image _slotGradientImage;
    [SerializeField] private Image _slotWindowImage;

    private void OnEnable()
    {
        _slotBackImage.sprite = _slotModelAtlas.GetSprite("Slot_back");
        _slotGradientImage.sprite = _slotModelAtlas.GetSprite("Slot_gradient");
        _slotWindowImage.sprite = _slotModelAtlas.GetSprite("slot_window");
    }
}