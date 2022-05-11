using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialougeResponseEvents))]
public class DialougeResponseEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialougeResponseEvents responseEvents = (DialougeResponseEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            responseEvents.OnValidate();
        }
    }
}
