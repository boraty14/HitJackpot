using UnityEngine;
using UnityEngine.U2D;

namespace Game.Scripts.SlotElement
{
    public class SlotModel : MonoBehaviour
    {
        [SerializeField] private SpriteAtlas _slotModelAtlas;
        [SerializeField] private SpriteRenderer _slotBackImage;
        [SerializeField] private SpriteRenderer _slotGradientImage;
        [SerializeField] private SpriteRenderer _slotWindowImage;

        private void OnEnable()
        {
            _slotBackImage.sprite = _slotModelAtlas.GetSprite("Slot_back");
            _slotBackImage.size = new Vector2(7.6f, 5.06f);
            _slotGradientImage.sprite = _slotModelAtlas.GetSprite("Slot_gradient");
            _slotWindowImage.sprite = _slotModelAtlas.GetSprite("slot_window");
        }
    }
}