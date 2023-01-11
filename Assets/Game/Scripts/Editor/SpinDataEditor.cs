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
            for (int i = 0; i < 100; i++)
            {
                spinData.Spin();
            }
        }
    }
}
