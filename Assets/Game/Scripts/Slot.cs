using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform _slotRectTransform;
    [SerializeField] private float _scrollDuration;
    [SerializeField] private List<SlotItem> _slotItems;
    private int _currentSlotItemIndex;
    private float _slotRollEndHeight;
    private const float SlotItemHeight = 260f;

    private void Start()
    {
        SetSlotItems();
        StartCoroutine(SpinSlot());
    }

    private void SetSlotItems()
    {
        _slotRollEndHeight = _slotItems.Count % 2 == 0 ? -SlotItemHeight / 2f : 0f;
        _slotRectTransform.anchoredPosition = new Vector2(_slotRectTransform.anchoredPosition.x, _slotRollEndHeight);
        _currentSlotItemIndex = (_slotItems.Count-1) / 2;
    }

    private IEnumerator SpinSlot()
    {
        while (true)
        {
            MoveLastSlotItemUp();
            _slotRectTransform.anchoredPosition += Vector2.up * SlotItemHeight;
            yield return _slotRectTransform.DOAnchorPosY(_slotRollEndHeight, _scrollDuration)
                .SetEase(Ease.Linear).WaitForCompletion();
        }
    }

    private void MoveLastSlotItemUp()
    {
        var lastTransform = transform.GetChild(_slotItems.Count - 1);
        lastTransform.SetSiblingIndex(0);
    }

    private void Update()
    {
        //TODO remove scroll rect dont need it no more

        if (_slotRectTransform.anchoredPosition.y < 0f)
        {
        }
    }
}