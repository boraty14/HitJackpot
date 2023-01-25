using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Game.Scripts.GameElements
{
    public class SlotItem : MonoBehaviour
    {
        [SerializeField] private SpinType _spinType;
        [SerializeField] private SpriteAtlas _iconAtlas;
        [SerializeField] private string _netImageName;
        [SerializeField] private string _blurImageName;
        [SerializeField] private Image _image;

        public SpinType GetSpinType() => _spinType;
    
        public void SetImageState(bool state)
        {
            var spriteName = state ? _netImageName : _blurImageName;
            _image.sprite = _iconAtlas.GetSprite(spriteName);
        }
    }
}