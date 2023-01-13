using UnityEngine;

[CreateAssetMenu]
public class SpinSettings : ScriptableObject
{
    [field: SerializeField] public float FastSpinItemPassDuration { get; private set; }
    [field: SerializeField] public int DefaultSpinTurnCount { get; private set; }
    [field: SerializeField] public float NormalSpinSlowDownDuration { get; private set; }
    [field: SerializeField] public float SlowSpinSlowDownDuration { get; private set; }
    [field: SerializeField] public float CoinEffectRate { get; private set; }
    [field: SerializeField] public float CoinEffectDuration { get; private set; }
}