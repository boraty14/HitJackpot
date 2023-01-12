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
            slotController.Spin();
        }
        if (GUILayout.Button("Test Normal Spin"))
        {
            slotController.Spin(1f);
        }
        if (GUILayout.Button("Test Slow Spin"))
        {
            slotController.Spin(2.5f);
        }
    }
}