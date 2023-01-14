using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [SerializeField] private List<Slot> _slots;
    [SerializeField] private SpinData _spinData;
    [SerializeField] private SpinSettings _spinSettings;
    [SerializeField] private CoinEffect _coinEffectPrefab;
    [SerializeField] private Transform _coinEffectTransform;
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
        StartSpinning();

        var spinResult = await SpinDefault();
        var spinSlowDownDuration = GetSpinSlowDownDuration(spinIndex);
        var coinEffectRate = GetCoinEffectEmissionRate(spinResult);
        if (spinSlowDownDuration < 0f)
        {
            StopSpinning(coinEffectRate);
            return;
        }
        
        await _slots[2].SpinDelayedSlotToState(spinResult.thirdSpin, spinSlowDownDuration);
        StopSpinning(coinEffectRate);
    }

    private void StartSpinning()
    {
        _isSpinning = true;
        OnSpinStateChange?.Invoke(_isSpinning);
    }

    private void StopSpinning(float coinEffectRate)
    {
        _isSpinning = false;
        OnSpinStateChange?.Invoke(_isSpinning);

        if (coinEffectRate < 0.01f) return;
        var coinEffect = LeanPool.Spawn(_coinEffectPrefab,_coinEffectTransform);
        coinEffect.SetParticleEmission(coinEffectRate);
    }

    private async Task<SpinResult> SpinDefault()
    {
        var spinResult = _spinData.Spin();
        Debug.Log(spinResult.firstSpin + " " + spinResult.secondSpin + " " + spinResult.thirdSpin);
        _ = _slots[0].SpinDefaultSlotToState(spinResult.firstSpin,_spinSettings.DefaultSpinTurnCount);
        _ = _slots[1].SpinDefaultSlotToState(spinResult.secondSpin,_spinSettings.DefaultSpinTurnCount + _spinSettings.DefaultSpinTurnOffset);
        await _slots[2].SpinDefaultSlotToState(spinResult.thirdSpin,_spinSettings.DefaultSpinTurnCount + 2 * _spinSettings.DefaultSpinTurnOffset);
        return spinResult;
    }

    private float GetSpinSlowDownDuration(int spinIndex)
    {
        return spinIndex switch
        {
            0 => -1f,
            1 => _spinSettings.NormalSpinSlowDownDuration,
            2 => _spinSettings.SlowSpinSlowDownDuration,
            _ => -1f
        };
    }

    private float GetCoinEffectEmissionRate(SpinResult spinResult)
    {
        if (spinResult.firstSpin == spinResult.secondSpin && spinResult.secondSpin == spinResult.thirdSpin)
        {
            return spinResult.firstSpin switch
            {
                SpinType.Jackpot => 5f * _spinSettings.CoinEffectRate,
                SpinType.Wild => 4f * _spinSettings.CoinEffectRate,
                SpinType.Seven => 3f * _spinSettings.CoinEffectRate,
                SpinType.Bonus => 2f * _spinSettings.CoinEffectRate,
                SpinType.A => _spinSettings.CoinEffectRate,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        return 0f;
    }
}