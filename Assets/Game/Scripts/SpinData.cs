using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class SpinData : ScriptableObject
{
    public SpinResultList sp;
    public List<SpinResult> spinResults;
    public int spinIndex;
    private const string SaveKey = "SAVEKEY";

    
    public void GenerateNewSpinList()
    {
        sp.spinResultList = new SpinResult[100];
        Dictionary<SpinResult, int> resultAppearDictionary = new Dictionary<SpinResult, int>();
        foreach (var result in spinResults)
        { 
            resultAppearDictionary[result] = 0;
        }
        
        
        var leastPercentage = spinResults.OrderBy(result => result.percentage).First().percentage;
        int spinPeriod = 100 / leastPercentage;

        for (int i = 0; i < leastPercentage; i++)
        {
            foreach (var spinResult in spinResults)
            {
                
            }
        }
        
        
    }
    
    public void GenerateSpinList()
    {
        sp.spinResultList = new SpinResult[100];
        var sortedResults = spinResults.OrderByDescending(result => result.percentage).ToArray();
        bool[] resultOccupiedArray = new bool[100];

        Dictionary<SpinResult, int> resultAppearDictionary = new Dictionary<SpinResult, int>();
        foreach (var result in sortedResults)
        {
            resultAppearDictionary[result] = 0;
        }

        int resultCount = sortedResults.Length;
        for (int i = 0; i < resultCount; i++)
        {
            var currentResult = sortedResults[i];
            int resultInterval = 100 / currentResult.percentage;
            int remainFromDivide = 100 - (resultInterval * currentResult.percentage);
            int currentStartIndex = 0;
            for (int j = 0; j < currentResult.percentage; j++)
            {
                Debug.Log("current start index " + currentStartIndex);
                int remainExtension = j >= remainFromDivide ? 0 : 1;

                int randomPlacementIndexOffset = UnityEngine.Random.Range(0, resultInterval + remainExtension);
                //int placementIndex = j * resultInterval + randomPlacementIndexOffset;
                int placementIndex = currentStartIndex + randomPlacementIndexOffset;
                int counter = 0;

                while (resultOccupiedArray[placementIndex])
                {
                    randomPlacementIndexOffset = (randomPlacementIndexOffset + 1) % (resultInterval + remainExtension); 
                    placementIndex = currentStartIndex + randomPlacementIndexOffset;
                    counter++;
                    if (counter > 50)
                    {
                        Debug.LogError("percentage " + currentResult.percentage + " j: " + j);
                        PrintResults();
                        for (int k = 0; k < 100; k++)
                        {
                            Debug.Log(k + " " + resultOccupiedArray[k]);
                        }
                        return;
                    }
                }

                sp.spinResultList[placementIndex] = currentResult;
                resultOccupiedArray[placementIndex] = true;
                currentStartIndex += resultInterval + remainExtension;
            }
        }

        
        
        
        
        /*for (int i = 0; i < 100; i++)
        {
            sortedResults = spinResults.OrderBy(result => resultAppearDictionary[result]).ToArray();
            for (int j = 0; j < resultCount; j++)
            {
                var currentResult = sortedResults[j];
                if(i < resultAppearDictionary[currentResult] * (100/currentResult.percentage)) continue;

                sp.spinResultList[i] = currentResult;
                resultAppearDictionary[currentResult]++;
                break;
            }
        }*/
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
    [Range(1,50)]public int percentage;
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