using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Dialogue;

[CustomEditor(typeof(Sentence))]
public class SentenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(20);

        if (serializedObject.FindProperty("type").enumValueFlag == (int)SentenceType.start)
            return;

        if(serializedObject.FindProperty("type").enumValueFlag != (int)SentenceType.displayPlayable)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("narrator"));

        EditorGUILayout.LabelField(serializedObject.FindProperty("type").enumValueFlag != (int)SentenceType.displayPlayable ? "Text" : "Memo");
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        style.fixedHeight = EditorGUIUtility.singleLineHeight * 3;
        serializedObject.FindProperty("text").stringValue =
            EditorGUILayout.TextArea(serializedObject.FindProperty("text").stringValue, style);

        EditorGUILayout.Space(10);

        if (serializedObject.FindProperty("type").enumValueFlag != (int)SentenceType.displayPlayable)
        {
            EditorGUILayout.PrefixLabel("Sprite");
            serializedObject.FindProperty("sprite").objectReferenceValue =
                EditorGUILayout.ObjectField(serializedObject.FindProperty("sprite").objectReferenceValue, typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64));
        }

        EditorGUILayout.Space(10);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isPlayTimeline"));
        if(serializedObject.FindProperty("isPlayTimeline").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("betweenTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wrapMode"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("passTime"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif