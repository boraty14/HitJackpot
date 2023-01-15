using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    [SerializeField] private SpinGenerator _spinGenerator;

    private void OnEnable()
    {
        _spinGenerator.LoadSpinData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _spinGenerator.SaveSpinData();
    }

    private void OnApplicationQuit()
    {
        _spinGenerator.SaveSpinData();
    }
}
