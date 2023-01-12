using UnityEngine;

public class SlotItem : MonoBehaviour
{
    [SerializeField] private GameObject _netItemImage;
    [SerializeField] private GameObject _blurItemImage;

    public void SetImageState(bool state)
    {
        _netItemImage.SetActive(state);
        _blurItemImage.SetActive(!state);
    }
}