using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpinData : ScriptableObject
{ 
    public List<SpinResult> spinResults;
    [HideInInspector] public int spinIndex;
    private const string SaveKey = "SAVEKEY";

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
public struct SpinResult
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