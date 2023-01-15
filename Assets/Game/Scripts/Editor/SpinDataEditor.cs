using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpinData))]
public class SpinDataEditor : Editor
{
    public override void OnInspectorGUI()
    { 
        base.OnInspectorGUI();
        SpinData spinData = (SpinData)target;

        if (GUILayout.Button("Generate Spin Result List"))
        {
            spinData.GenerateSpinListNew();
            PrintResultList(spinData);
        }
        

        if (GUILayout.Button("Reset States"))
        {
            spinData.ResetSpinIndex();
        }
        
        
    }

    private void PrintResultList(SpinData spinData)
    {
        Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
        foreach (var spinResult in spinData.spinResults)
        {
            var keyName = GetKeyName(spinResult);
            resultDictionary.Add(keyName,new List<int>());
        }
        
        for (int i = 0; i < 100; i++)
        {
            resultDictionary[GetKeyName(spinData.sp.spinResultList[i])].Add(i);
        }
        
        foreach (var spinResult in spinData.spinResults)
        {
            var keyName = GetKeyName(spinResult);
            Debug.Log(keyName + " | | percentage " + spinResult.percentage + " | |  " + string.Join(", ",resultDictionary[keyName]));
        }
        
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }
}