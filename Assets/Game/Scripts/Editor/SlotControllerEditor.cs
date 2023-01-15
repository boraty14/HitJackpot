using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotController))]
public class SlotControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SlotController slotController = (SlotController)target;
        if (GUILayout.Button("Test Fast Spin"))
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Enter play mode to test it");
                return;
            }
            slotController.Spin(0);
        }
        if (GUILayout.Button("Test Normal Spin"))
        {
             if (!Application.isPlaying)
             {
                 Debug.Log("Enter play mode to test it"); 
                 return;
             } 
             slotController.Spin(1);
        }
        
        if (GUILayout.Button("Test Slow Spin"))
        { 
            if (!Application.isPlaying) 
            {
               Debug.Log("Enter play mode to test it"); 
               return;
            } 
            slotController.Spin(2);
        }
    }
}