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

    private int GetLimitIndex(SpinResult spinResult,int totalAppear,int currentStartIndex)
    {
        int resultInterval = 100 / spinResult.percentage;
        int remainFromDivide = 100 - (resultInterval * spinResult.percentage);
        int remainExtension = totalAppear >= remainFromDivide ? 0 : 1; // >= ???

        return currentStartIndex + resultInterval + remainExtension -1 ;
    }

    
    public void GenerateSpinListNew()
    {
        sp.spinResultList = new SpinResult[100];
        bool[] resultOccupiedArray = new bool[100];
        Dictionary<SpinResult, int> resultAppearDictionary = new Dictionary<SpinResult, int>();
        Dictionary<SpinResult, int> currentStartIndexDictionary = new Dictionary<SpinResult, int>();
        var sortedResults = spinResults.OrderByDescending(result => result.percentage).ToList();
        //set dictionaries
        foreach (var result in spinResults)
        { 
            resultAppearDictionary[result] = 0;
            currentStartIndexDictionary[result] = 0;
        }


        for (int i = 0; i < 100; i++)
        {
            //get min start index result
            SpinResult currentResult = sortedResults[0];
            for (int j = 1; j < sortedResults.Count; j++)
            {
                var sortedIndex = GetLimitIndex(sortedResults[j], resultAppearDictionary[sortedResults[j]],
                    currentStartIndexDictionary[sortedResults[j]]);
                var currentIndex = GetLimitIndex(currentResult, resultAppearDictionary[currentResult],
                    currentStartIndexDictionary[currentResult]);

                if (sortedIndex == currentIndex)
                {
                    var randomSelectIndex = Random.Range(0, 2);
                    currentResult = randomSelectIndex == 0 ? currentResult : sortedResults[j];
                }
                
                else if (sortedIndex <= currentIndex)
                {
                    currentResult = sortedResults[j];
                }
            }
            
            int resultInterval = 100 / currentResult.percentage;
            int remainFromDivide = 100 - (resultInterval * currentResult.percentage);
            //int remainExtension = resultAppearDictionary[currentResult] >= remainFromDivide ? 0 : 1;
            int remainExtension = resultAppearDictionary[currentResult] >= remainFromDivide ? 0 : 1;
            
            //int placementIndexOffset = UnityEngine.Random.Range(0, resultInterval + remainExtension);
            int placementIndexOffset = 0;
            int placementIndex = currentStartIndexDictionary[currentResult] + placementIndexOffset;

            int counter = 0;
            while (resultOccupiedArray[placementIndex])
            {
                placementIndexOffset = (placementIndexOffset + 1) % (resultInterval + remainExtension);
                placementIndex = currentStartIndexDictionary[currentResult] + placementIndexOffset;
                counter++;
                if (counter > 50 || placementIndex >= 100)
                {
                    Debug.LogError("percentage " + currentResult.percentage + " placement index: " + placementIndex + " i " + i);
                    Debug.LogError(currentStartIndexDictionary[currentResult]);
                    PrintResults();
                    for (int k = 0; k < 100; k++)
                    {
                        var keyName = !resultOccupiedArray[k] ? "" : GetKeyName(sp.spinResultList[k]);
                        Debug.Log(k + " " + resultOccupiedArray[k] + " " + keyName);
                    }

                    return;
                }
            }
            
            
            sp.spinResultList[placementIndex] = currentResult;
            resultOccupiedArray[placementIndex] = true;
            currentStartIndexDictionary[currentResult] += resultInterval + remainExtension;
            resultAppearDictionary[currentResult] = resultAppearDictionary[currentResult] + 1;
//            Debug.Log(currentResult.key);
            if (resultAppearDictionary[currentResult] == currentResult.percentage)
            {
                sortedResults.Remove(currentResult);
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
                //Debug.Log("current start index " + currentStartIndex);
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
                            var keyName = !resultOccupiedArray[k] ? "" : GetKeyName(sp.spinResultList[k]);
                            Debug.Log(k + " " + resultOccupiedArray[k] + " " + keyName);
                        }
                        return;
                    }
                }

                Debug.Log("placement Index " +placementIndex);
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
    public string key;
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