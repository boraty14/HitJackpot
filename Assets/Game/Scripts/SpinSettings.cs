using UnityEngine;

[CreateAssetMenu]
public class SpinSettings : ScriptableObject
{
    [field:SerializeField] public float FastSpinDuration { get; private set; }
}
