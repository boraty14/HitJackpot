using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class SpinGenerator : ScriptableObject
{
    public SpinResultList spinResultList;
    public List<SpinData> spinDataList;
    [HideInInspector] public int spinIndex;
    private const string SaveKey = "SAVEKEY";
    public Dictionary<SpinData, int> _remainExtensionCountDictionary;
    
    public void GenerateSpinListNew()
    {
        ES3.DeleteKey(SaveKey);
        spinIndex = 0;
        spinResultList.Value = new SpinResult[100];
        bool[] resultOccupiedArray = new bool[100];
        Dictionary<SpinData, int> resultAppearCountDictionary = new Dictionary<SpinData, int>();
        Dictionary<SpinData, int> remainExtensionCountDictionary = new Dictionary<SpinData, int>();
        Dictionary<SpinData, int> startIndexDictionary = new Dictionary<SpinData, int>();
        
        var sortedResults = spinDataList.OrderByDescending(result => result.percentage).ToList();
        //set dictionaries
        foreach (var result in spinDataList)
        { 
            resultAppearCountDictionary[result] = 0;
            startIndexDictionary[result] = 0;
            remainExtensionCountDictionary[result] = 0;
        }

        for (int i = 0; i < 100; i++)
        {
            var currentResult = sortedResults[0];
            for (int j = 1; j < sortedResults.Count; j++)
            {
                var comparingResult = sortedResults[j];
                var comparingResultInterval = 100 / comparingResult.percentage;
                var comparingResultRemainFactor = (100 % comparingResult.percentage != 0) &&
                                                  remainExtensionCountDictionary[comparingResult] < (100 % comparingResult.percentage) ?
                    1:0;

                var currentResultInterval = 100 / currentResult.percentage;
                var currentResultRemainFactor = (100 % currentResult.percentage != 0) &&
                                                remainExtensionCountDictionary[currentResult] <
                                                (100 % currentResult.percentage)
                    ? 1
                    : 0;

                if (startIndexDictionary[comparingResult] + comparingResultInterval + comparingResultRemainFactor ==
                    startIndexDictionary[currentResult] + currentResultInterval + currentResultRemainFactor)
                {
                    if (startIndexDictionary[comparingResult] < startIndexDictionary[currentResult])
                    {
                        currentResult = comparingResult;
                    }

                    else if(comparingResult.percentage == currentResult.percentage)
                    {
                        int randomSelectIndex = Random.Range(0, 2);
                        if (randomSelectIndex == 1) currentResult = comparingResult;
                    }
                    
                }
                
                if (startIndexDictionary[comparingResult] + comparingResultInterval + comparingResultRemainFactor <
                    startIndexDictionary[currentResult] + currentResultInterval + currentResultRemainFactor)
                {
                    currentResult = comparingResult;
                }
            }

            var resultInterval = 100 / currentResult.percentage;
            var resultRemainFactor = (100 % currentResult.percentage != 0) &&
                                     remainExtensionCountDictionary[currentResult] < (100 % currentResult.percentage) ?
                1:0;
            var placementIndex = startIndexDictionary[currentResult];
            var placementIndexOffset = 0;
            while (resultOccupiedArray[placementIndex + placementIndexOffset])
            {
                placementIndexOffset++;
                if (placementIndex + placementIndexOffset >= 100)
                {
                    //SHOULD NOT REACH HERE BUT JUST IN CASE IF I MADE A MISTAKE
                    Debug.LogError("placement index " + placementIndex + " percentage " + currentResult.percentage); 
                    return;
                }
            }

            resultOccupiedArray[placementIndex + placementIndexOffset] = true;
            startIndexDictionary[currentResult] += resultInterval;
            if ((placementIndexOffset >= resultInterval + resultRemainFactor - 1 && resultRemainFactor == 1) ||
                ((100 % currentResult.percentage) - remainExtensionCountDictionary[currentResult]) ==
                (currentResult.percentage) - resultAppearCountDictionary[currentResult] && 
                100 % currentResult.percentage != 0)
            {
                startIndexDictionary[currentResult]++;
                remainExtensionCountDictionary[currentResult]++;
            }
            resultAppearCountDictionary[currentResult]++;
            spinResultList.Value[placementIndex + placementIndexOffset] = currentResult.spinResult;

        }

        _remainExtensionCountDictionary = remainExtensionCountDictionary;
    }

    public SpinResult Spin()
    {
        var result = spinResultList.Value[spinIndex];
        spinIndex = (spinIndex + 1) % 100;
        return result;
    }

    public void ResetStates()
    {
        ES3.DeleteKey(SaveKey);
        spinResultList.Value = null;
        spinIndex = 0;
    }

    public void SaveSpinData()
    {
        var spinSave = new SpinSave
        {
            spinResults = spinResultList.Value,
            spinIndex = this.spinIndex
        };
        ES3.Save<SpinSave>(SaveKey,spinSave);
    }

    public void LoadSpinData()
    {
        if (ES3.KeyExists(SaveKey))
        {
            var savedSpinData = ES3.Load<SpinSave>(SaveKey);
            spinResultList.Value = savedSpinData.spinResults;
            spinIndex = savedSpinData.spinIndex;
        }
        else if (spinResultList.Value == null || spinResultList.Value.Length == 0)
        {
            GenerateSpinListNew();        
        }
        
    }
}

[Serializable]
public class SpinData
{
    public SpinResult spinResult;
    [Range(1,100)]public int percentage;
}

[Serializable]
public struct SpinResult
{
    public SpinType firstSpin;
    public SpinType secondSpin;
    public SpinType thirdSpin;
}

[Serializable]
public enum SpinType 
{
    Jackpot = 0,
    Wild = 1,
    Seven = 2,
    Bonus = 3,
    A = 4
}

[Serializable]
public class SpinSave
{
    public SpinResult[] spinResults;
    public int spinIndex;
}