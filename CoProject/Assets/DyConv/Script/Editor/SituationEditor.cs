using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Dialogue;

[CustomEditor(typeof(Situation))]
public class SituationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Situation Editor"))
        {
            SituationEditorWindow.Open(target as Situation);
            Selection.objects = null;
        }
    }
}
#endif