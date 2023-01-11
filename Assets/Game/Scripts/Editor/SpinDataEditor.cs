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
        
        if (GUILayout.Button("Test 100 Spin"))
        {
            Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
            foreach (var spinResult in spinData.spinResults)
            {
                var keyName = GetKeyName(spinResult);
                resultDictionary.Add(keyName,new List<int>());
            }
            
            for (int i = 0; i < 100; i++)
            {
                var spinIndex = spinData.spinIndex;
                var spinResult = spinData.Spin();
                resultDictionary[GetKeyName(spinResult)].Add(spinIndex);
            }

            foreach (var spinResult in spinData.spinResults)
            {
                var keyName = GetKeyName(spinResult);
                Debug.Log(keyName + " | | percentage " + spinResult.percentage + " | |  " + string.Join(", ",resultDictionary[keyName]));
            }
        }

        if (GUILayout.Button("Reset States"))
        {
            spinData.ResetStates();
        }
        
        
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }
}