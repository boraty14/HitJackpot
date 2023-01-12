using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [SerializeField] private List<Slot> _slots;
    [SerializeField] private SpinData _spinData;
    [SerializeField] private SpinSettings _spinSettings;
    public Action<bool> OnSpinStateChange;
    private bool _isSpinning;
    
    private void Start()
    {
        foreach (var slot in _slots)
        {
            slot.Initialize();
        }
    }
    
    public async void Spin(int spinIndex)
    {
        if (_isSpinning) return;

        _isSpinning = true;
        OnSpinStateChange?.Invoke(_isSpinning);
        var spinResult = await SpinDefault();

        var spinSlowDownDuration = GetSpinSlowDownDuration(spinIndex);
        if (spinSlowDownDuration < 0f)
        {
            _isSpinning = false;
            OnSpinStateChange?.Invoke(_isSpinning);
            return;
        }
        
        await _slots[2].SpinDelayedSlotToState(spinResult.thirdSpin, spinSlowDownDuration);
        _isSpinning = false;
        OnSpinStateChange?.Invoke(_isSpinning);
    }

    private async Task<SpinResult> SpinDefault()
    {
        var spinResult = _spinData.Spin();
        Debug.Log(spinResult.firstSpin + " " + spinResult.secondSpin + " " + spinResult.thirdSpin);
        _ = _slots[0].SpinDefaultSlotToState(spinResult.firstSpin,_spinSettings.DefaultSpintTurnCount + 0);
        _ = _slots[1].SpinDefaultSlotToState(spinResult.secondSpin,_spinSettings.DefaultSpintTurnCount + 1);
        await _slots[2].SpinDefaultSlotToState(spinResult.thirdSpin,_spinSettings.DefaultSpintTurnCount + 2);
        return spinResult;
    }

    private float GetSpinSlowDownDuration(int spinIndex)
    {
        return spinIndex switch
        {
            0 => -1f,
            1 => _spinSettings.NormalSpinSlowDownDuration,
            2 => _spinSettings.FastSpinItemPassDuration,
            _ => -1f
        };
    }
    
}