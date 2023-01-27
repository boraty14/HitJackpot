using System;
using System.Threading.Tasks;
using Game.Scripts.Spin;
using Lean.Pool;
using UnityEngine;
using VContainer;

namespace Game.Scripts.SlotElement
{
    public class SlotController
    {
        private bool _isSpinning;
        private readonly SlotMachine _slotMachine;
        private readonly SpinGenerator _spinGenerator;
        private readonly SpinSettings _spinSettings;
        public Action<bool> OnSpinStateChange;

        [Inject]
        public SlotController(SlotMachine slotMachine, SpinSettings spinSettings,SpinGenerator spinGenerator)
        {
            _slotMachine = slotMachine;
            _spinSettings = spinSettings;
            _spinGenerator = spinGenerator;
            
            Initialize();
        }

        private void Initialize()
        {
            foreach (var slot in _slotMachine.slots)
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

            await _slotMachine.slots[2].SpinDelayedSlotToState(spinResult.thirdSpin, spinSlowDownDuration);
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
            var coinEffect = LeanPool.Spawn(_slotMachine.coinEffectPrefab, _slotMachine.coinEffectTransform);
            coinEffect.SetParticleEmission(coinEffectRate);
        }

        private async Task<SpinResult> SpinDefault()
        {
            var spinResult = _spinGenerator.Spin();
            Debug.Log(spinResult.firstSpin + " " + spinResult.secondSpin + " " + spinResult.thirdSpin);
            Task firstSpin = _slotMachine.slots[0].SpinDefaultSlotToState(spinResult.firstSpin, _spinSettings.DefaultSpinTurnCount);
            Task secondSpin = _slotMachine.slots[1].SpinDefaultSlotToState(spinResult.secondSpin,
                _spinSettings.DefaultSpinTurnCount + _spinSettings.DefaultSpinTurnOffset);
            Task thirdSpin =  _slotMachine.slots[2].SpinDefaultSlotToState(spinResult.thirdSpin,
                _spinSettings.DefaultSpinTurnCount + 2 * _spinSettings.DefaultSpinTurnOffset);
            await Task.WhenAll(firstSpin, secondSpin, thirdSpin);
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
            if (spinResult.IsFull())
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
}