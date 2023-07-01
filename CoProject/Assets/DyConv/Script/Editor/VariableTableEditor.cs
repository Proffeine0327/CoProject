using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using Dialogue;

[CustomEditor(typeof(VariableTable))]
public class VariableTableEditor : Editor
{
    SerializedProperty keys;
    SerializedProperty values;
    ReorderableList table;
    int tableSelectedIndex;

    private void OnEnable()
    {
        keys = serializedObject.FindProperty("keys");
        values = serializedObject.FindProperty("values");
        table = new ReorderableList(serializedObject, keys, true, true, true, true);
        table.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "table");
        table.drawElementCallback = (rect, index, active, focus) =>
        {
            var i = index;
            rect.y += 2;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), $"Element {i}: ");

            EditorGUI.BeginChangeCheck();
            var stringValue = EditorGUI.DelayedTextField(new Rect(rect.x + rect.width / 4, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), keys.GetArrayElementAtIndex(i).stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                if (!(target as VariableTable).variables.ContainsKey(stringValue))
                {
                    keys.GetArrayElementAtIndex(i).stringValue = stringValue;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            values.GetArrayElementAtIndex(i).intValue = EditorGUI.IntField(new Rect(rect.x + rect.width * 3 / 4, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), values.GetArrayElementAtIndex(i).intValue);
        };
        table.onAddCallback = list =>
        {
            VariableTableAddWindow.Open((st) =>
            {
                keys.arraySize++;
                keys.GetArrayElementAtIndex(keys.arraySize - 1).stringValue = st;
                values.arraySize++;

                list.Select(list.count - 1);
                serializedObject.ApplyModifiedProperties();
            });
        };
        table.onSelectCallback = list => tableSelectedIndex = list.index;
        table.onReorderCallback = list =>
        {
            values.MoveArrayElement(tableSelectedIndex, list.index);
            serializedObject.ApplyModifiedProperties();
        };
    }

    public override void OnInspectorGUI()
    {
        table.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif