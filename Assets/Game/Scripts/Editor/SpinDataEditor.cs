using System.Collections.Generic;
using Game.Scripts.GameElements;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpinGenerator))]
public class SpinDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpinGenerator spinGenerator = (SpinGenerator)target;

        if (GUILayout.Button("Generate Spin Result List"))
        {
            spinGenerator.GenerateSpinListNew();
            PrintResultList(spinGenerator);
        }


        if (GUILayout.Button("Reset States"))
        {
            spinGenerator.ResetStates();
        }
    }

    private void PrintResultList(SpinGenerator spinGenerator)
    {
        Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
        foreach (var spinData in spinGenerator.spinDataList)
        {
            var keyName = GetKeyName(spinData.spinResult);
            resultDictionary.Add(keyName, new List<int>());
        }

        for (int i = 0; i < 100; i++)
        {
            resultDictionary[GetKeyName(spinGenerator.spinResultList.Value[i])].Add(i);
        }

        foreach (var spinData in spinGenerator.spinDataList)
        {
            var keyName = GetKeyName(spinData.spinResult);
            Debug.Log(keyName + " | | percentage " + spinData.percentage + " | |  " +
                      string.Join(", ", resultDictionary[keyName]) + " count: " + resultDictionary[keyName].Count +
                      " remain : " + spinGenerator.RemainExtensionCountDictionary[spinData]);
        }
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }
}