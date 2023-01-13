using System.Collections;
using Lean.Pool;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private SpinSettings _spinSettings;

    public void SetParticleEmission(float emissionRate)
    {
        var particleSystemEmission = _particleSystem.emission;
        particleSystemEmission.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);
        _particleSystem.Simulate(0f,true,true);
        _particleSystem.Play();
        StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(_spinSettings.CoinEffectDuration);
        LeanPool.Despawn(this);
    }
}