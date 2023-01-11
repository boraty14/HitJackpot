using System;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform _contentRectTransform;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _scrollSpeed;
    private int _slotItemCount;
    private const float SlotItemHeight = 260f;

    private void Start()
    {
        _slotItemCount = _contentTransform.childCount;
    }

    private void Update()
    {
        _scrollRect.verticalNormalizedPosition += _scrollSpeed * Time.deltaTime;

        if (_contentRectTransform.anchoredPosition.y < 0f)
        {
            var lastTransform = _contentRectTransform.GetChild(_slotItemCount - 1);
            lastTransform.SetSiblingIndex(0);
            _contentRectTransform.anchoredPosition += Vector2.up * SlotItemHeight;
        }
    }
}
