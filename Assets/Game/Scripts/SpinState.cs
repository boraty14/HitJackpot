using UnityEngine;

public class SpinState : ScriptableObject
{
    [SerializeField] private SpinResult _spinResult;
    
    public int SpinIndex { get; private set; }
}
