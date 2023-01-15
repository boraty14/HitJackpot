using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class SpinData : ScriptableObject
{
    public SpinResultList sp;
    public List<SpinResult> spinResults;
    public int spinIndex;
    private const string SaveKey = "SAVEKEY";
    public Dictionary<SpinResult, int> _remainExtensionCountDictionary;

    private int GetLimitIndex(SpinResult spinResult,int totalAppear,int currentStartIndex)
    {
        int resultInterval = 100 / spinResult.percentage;
        int remainFromDivide = 100 - (resultInterval * spinResult.percentage);
        int remainExtension = totalAppear >= remainFromDivide ? 0 : 1; // >= ???

        return currentStartIndex + resultInterval  -1 ;
    }

    
    public void GenerateSpinListNew()
    {
        sp.spinResultList = new SpinResult[100];
        bool[] resultOccupiedArray = new bool[100];
        Dictionary<SpinResult, int> resultAppearCountDictionary = new Dictionary<SpinResult, int>();
        Dictionary<SpinResult, int> remainExtensionCountDictionary = new Dictionary<SpinResult, int>();
        Dictionary<SpinResult, int> startIndexDictionary = new Dictionary<SpinResult, int>();
        
        var sortedResults = spinResults.OrderByDescending(result => result.percentage).ToList();
        //set dictionaries
        foreach (var result in spinResults)
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
                if (placementIndexOffset == resultInterval + resultRemainFactor)
                {
                    Debug.LogError("placement index " + placementIndex + " percentage " + currentResult.percentage);
                    break;
                }
                placementIndexOffset++;
            }

            resultOccupiedArray[placementIndex + placementIndexOffset] = true;
            startIndexDictionary[currentResult] += resultInterval;
            if ((placementIndexOffset == resultInterval + resultRemainFactor - 1 && resultRemainFactor == 1) ||
                ((100 % currentResult.percentage) - remainExtensionCountDictionary[currentResult]) ==
                (currentResult.percentage) - resultAppearCountDictionary[currentResult] && 
                100 % currentResult.percentage != 0)
            {
                //TODO not quite true for remaining system
                startIndexDictionary[currentResult]++;
                remainExtensionCountDictionary[currentResult]++;
            }
            resultAppearCountDictionary[currentResult]++;
            sp.spinResultList[placementIndex + placementIndexOffset] = currentResult;

        }

        _remainExtensionCountDictionary = remainExtensionCountDictionary;
    }
    
    private void PrintResults()
    {
        Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
        foreach (var spinResult in spinResults)
        {
            var keyName = GetKeyName(spinResult);
            resultDictionary.Add(keyName,new List<int>());
        }
                
        for (int i = 0; i < 100; i++)
        {
            if(sp.spinResultList[i] == null) continue;
            resultDictionary[GetKeyName(sp.spinResultList[i])].Add(i);
        }
                
        foreach (var spinResult in spinResults)
        {
            var keyName = GetKeyName(spinResult);
            Debug.Log(keyName + " | | percentage " + spinResult.percentage + " | |  " + string.Join(", ",resultDictionary[keyName]));
        }
    }
    
    
    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }

    public SpinResult Spin()
    {
        var result = sp.spinResultList[spinIndex % 100];
        spinIndex++;
        return result;
    }

    public void ResetSpinIndex()
    {
        spinIndex = 0;
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
    public string key;
    [Range(1,100)]public int percentage;
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
    public List<SpinResult> spinResults;
    public int spinIndex;
}