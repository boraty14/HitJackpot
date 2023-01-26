using System.Collections.Generic;
using Game.Scripts.Spin;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpinDataHolder))]
public class SpinDataEditor : Editor
{
    /*public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpinDataHolder spinDataHolder = (SpinDataHolder)target;

        if (GUILayout.Button("Generate Spin Result List"))
        {
            var spinGenerator = new SpinGenerator(spinDataHolder);
            spinGenerator.GenerateSpinListNew();
            PrintResultList(spinDataHolder);
        }


        if (GUILayout.Button("Reset States"))
        {
            spinDataHolder.ResetStates();
        }
    }

    private void PrintResultList(SpinDataHolder spinDataHolder)
    {
        Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
        foreach (var spinData in spinDataHolder.spinDataList)
        {
            var keyName = GetKeyName(spinData.spinResult);
            resultDictionary.Add(keyName, new List<int>());
        }

        for (int i = 0; i < 100; i++)
        {
            resultDictionary[GetKeyName(spinDataHolder.spinResultList.Value[i])].Add(i);
        }

        var remainExtensionCountDictionary = spinDataHolder.GetRemainExtensionCountDictionary();
        var intervalListDictionary = spinDataHolder.GetIntervalListDictionary();
        foreach (var spinData in spinDataHolder.spinDataList)
        {
            var keyName = GetKeyName(spinData.spinResult);
            Debug.Log(keyName + " | | percentage " + spinData.percentage + " | |  " +
                      string.Join(", ", resultDictionary[keyName]) + " count: " + resultDictionary[keyName].Count +
                      " remain : " + remainExtensionCountDictionary[spinData]);
            Debug.Log("Interval: " + string.Join(", ", intervalListDictionary[spinData]));
        }
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }*/
}