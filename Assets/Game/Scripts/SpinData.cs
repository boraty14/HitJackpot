using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class SpinData : ScriptableObject
{ 
    public List<SpinResult> spinResults;
    public int spinIndex;
    private readonly List<SpinResult> _possibleSpinResults = new List<SpinResult>();
    private const string SaveKey = "SAVEKEY";

    public SpinResult Spin()
    {
        _possibleSpinResults.Clear();
        foreach (var spinResult in spinResults)
        {
            if (spinIndex >= spinResult.resultAppearCount * (100 / spinResult.percentage))
            {
                _possibleSpinResults.Add(spinResult);
            }
        }

        int randomSpinResultIndex = Random.Range(0, _possibleSpinResults.Count);
        _possibleSpinResults[randomSpinResultIndex].resultAppearCount++;
        
        IncreaseSpinIndex();
        return _possibleSpinResults[randomSpinResultIndex];
    }

    private void IncreaseSpinIndex()
    {
        spinIndex++;
        if (spinIndex != 100) return;
        
        ResetStates();
        
    }

    public void ResetStates()
    {
        spinIndex = 0;
        foreach (var spinResult in spinResults)
        {
            spinResult.resultAppearCount = 0;
        }
    }

    public void SaveSpinData()
    {
        var spinSave = new SpinSave
        {
            spinResults = this.spinResults,
            spinIndex = this.spinIndex
        };
        ES3.Save<SpinSave>(SaveKey,spinSave);
    }

    public void LoadSpinData()
    {
        if (!ES3.KeyExists(SaveKey)) return;
        var savedSpinData = ES3.Load<SpinSave>(SaveKey);
        spinResults = savedSpinData.spinResults;
        spinIndex = savedSpinData.spinIndex;
    }
}

[Serializable]
public class SpinResult
{
    public SpinType firstSpin;
    public SpinType secondSpin;
    public SpinType thirdSpin;
    public int percentage;
    [HideInInspector] public int resultAppearCount;
}

[Serializable]
public enum SpinType 
{
    Jackpot,
    Wild,
    Seven,
    Bonus,
    A
}

[Serializable]
public class SpinSave
{
    public List<SpinResult> spinResults;
    public int spinIndex;
}