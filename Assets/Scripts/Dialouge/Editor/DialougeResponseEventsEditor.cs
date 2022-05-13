using UnityEngine;
using UnityEditor;

//Editor for the response events, when you add/ change repsonse events you can press the refresh button to instantly update the method in the Unity editor.
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
