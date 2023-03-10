using Game.Scripts.Spin;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.Scripts.SlotElement
{
    public class SlotItem : MonoBehaviour
    {
        [SerializeField] private SpriteAtlas _allAtlas;
        [SerializeField] private SpinType _spinType;
        [SerializeField] private string _netImageName;
        [SerializeField] private string _blurImageName;
        [SerializeField] private SpriteRenderer _image;

        public SpinType GetSpinType() => _spinType;
    
        public void SetImageState(bool state)
        {
            var spriteName = state ? _netImageName : _blurImageName;
            _image.sprite = _allAtlas.GetSprite(spriteName);
        }

        private void OnEnable()
        {
            SetImageState(true);
        }
    }
}