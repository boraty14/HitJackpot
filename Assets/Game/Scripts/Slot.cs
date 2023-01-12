using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform _slotRectTransform;
    [SerializeField] private float _scrollDuration;
    [SerializeField] private List<SlotItem> _slotItems;
    private readonly List<SpinType> _spinTypes = new List<SpinType>();
    private int _currentSlotItemIndex;
    private float _slotRollEndHeight;
    private const float SlotItemHeight = 260f;

    private void Start()
    {
        SetSlotItems();
        RandomizeSlotAtStart();
        StartCoroutine(SpinSlot());
    }

    private void SetSlotItems()
    {
        _slotRollEndHeight = _slotItems.Count % 2 == 0 ? -SlotItemHeight / 2f : 0f;
        _slotRectTransform.anchoredPosition = new Vector2(_slotRectTransform.anchoredPosition.x, _slotRollEndHeight);
        _currentSlotItemIndex = (_slotItems.Count-1) / 2;
        
        foreach (var slotItem in _slotItems)
        {
            _spinTypes.Add(slotItem.GetSpinType());
        }
    }

    private void RandomizeSlotAtStart()
    {
        var randomCount = Random.Range(0, _spinTypes.Count);
        for (int i = 0; i < randomCount; i++)
        {
            MoveLastSlotItemUp();
        }
    }

    private IEnumerator SpinSlot()
    {
        while (true)
        {
            MoveLastSlotItemUp();
            _slotRectTransform.anchoredPosition += Vector2.up * SlotItemHeight;
            yield return _slotRectTransform.DOAnchorPosY(_slotRollEndHeight, _scrollDuration)
                .SetEase(Ease.Linear).WaitForCompletion();
            _currentSlotItemIndex = (_currentSlotItemIndex - 1) % _spinTypes.Count;
        }
    }

    private void MoveLastSlotItemUp()
    {
        var lastTransform = transform.GetChild(_slotItems.Count - 1);
        lastTransform.SetSiblingIndex(0);
    }
}