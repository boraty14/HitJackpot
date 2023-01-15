using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform _slotRectTransform;
    [SerializeField] private List<SlotItem> _slotItems;
    [SerializeField] private SpinSettings _spinSettings;
    private readonly List<SpinType> _spinTypes = new List<SpinType>();
    private int _currentSlotItemIndex;
    private float _slotRollEndHeight;
    private const float SlotItemHeight = 260f;

    public void Initialize()
    {
        SetSlotItems();
        RandomizeSlotAtStart();
        SetSlotItemImages(true);
    }

    private void SetSlotItems()
    {
        _slotRollEndHeight = _slotItems.Count % 2 == 0 ? -SlotItemHeight / 2f : 0f;
        _slotRectTransform.anchoredPosition = new Vector2(_slotRectTransform.anchoredPosition.x, _slotRollEndHeight);
        _currentSlotItemIndex = (_slotItems.Count - 1) / 2;

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

    public async Task SpinDefaultSlotToState(SpinType selectedSpinType, int turnCount)
    {
        SetSlotItemImages(false);
        for (int i = 0; i < turnCount * _spinTypes.Count; i++)
        {
            await SpinOneItem(_spinSettings.FastSpinItemPassDuration);
        }

        var selectedSpinIndex = (int)selectedSpinType;
        while (selectedSpinIndex != _currentSlotItemIndex)
        {
            await SpinOneItem(_spinSettings.FastSpinItemPassDuration);
        }
        SetSlotItemImages(true);
    }

    public async Task SpinDelayedSlotToState(SpinType selectedSpinType, float spinDuration)
    {
        var selectedSpinIndex = (int)selectedSpinType;
        var spinCount = (_currentSlotItemIndex - selectedSpinIndex) % _spinTypes.Count;
        if (spinCount <= 0) spinCount += _spinTypes.Count;
        spinCount += _spinTypes.Count * Mathf.RoundToInt(spinDuration);

        var spinWaitFactor = spinDuration / ((spinCount * (spinCount + 1)) / 2);
        for (int i = 0; i < spinCount; i++)
        {
            await SpinOneItem(spinWaitFactor * (i + 1));
        }
    }

    private async Task SpinOneItem(float spinDuration)
    {
        MoveLastSlotItemUp();
        _slotRectTransform.anchoredPosition += Vector2.up * SlotItemHeight;
        await _slotRectTransform.DOAnchorPosY(_slotRollEndHeight, spinDuration)
            .SetEase(Ease.Linear).AsyncWaitForCompletion();
    }

    private void MoveLastSlotItemUp()
    {
        var lastTransform = transform.GetChild(_slotItems.Count - 1);
        lastTransform.SetSiblingIndex(0);
        SetSlotItemIndex();
    }

    private void SetSlotItemIndex()
    {
        _currentSlotItemIndex = (_currentSlotItemIndex - 1) % _spinTypes.Count;
        if (_currentSlotItemIndex < 0) _currentSlotItemIndex += _spinTypes.Count;
    }

    private void SetSlotItemImages(bool state)
    {
        for (int i = 0; i < _slotItems.Count; i++)
        {
            _slotItems[i].SetImageState(state);        
        }
    }
}