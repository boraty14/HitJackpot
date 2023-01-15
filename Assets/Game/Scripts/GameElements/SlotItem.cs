using UnityEngine;

namespace Game.Scripts.GameElements
{
    public class SlotItem : MonoBehaviour
    {
        [SerializeField] private GameObject _netItemImage;
        [SerializeField] private GameObject _blurItemImage;
        [SerializeField] private SpinType _spinType;

        public SpinType GetSpinType() => _spinType;
    
        public void SetImageState(bool state)
        {
            _netItemImage.SetActive(state);
            _blurItemImage.SetActive(!state);
        }
    }
}