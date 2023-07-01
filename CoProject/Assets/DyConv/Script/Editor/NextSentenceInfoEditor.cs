using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using Dialogue;

[CustomEditor(typeof(NextSentenceInfo))]
public class NextSentenceInfoEditor : Editor
{
    SerializedProperty order;
    SerializedProperty next;

    SerializedProperty criteria_key;
    SerializedProperty criteria_type;
    SerializedProperty criteria_value;

    SerializedProperty modification_key;
    SerializedProperty modification_type;
    SerializedProperty modification_value;

    ReorderableList criteria;
    ReorderableList modification;

    int criteriaSelectedIndex;
    int modificationSelectedIndex;

    private void OnEnable()
    {
        order = serializedObject.FindProperty("order");
        next = serializedObject.FindProperty("next");

        criteria_key = serializedObject.FindProperty("criteria_key");
        criteria_type = serializedObject.FindProperty("criteria_type");
        criteria_value = serializedObject.FindProperty("criteria_value");

        modification_key = serializedObject.FindProperty("modification_key");
        modification_type = serializedObject.FindProperty("modification_type");
        modification_value = serializedObject.FindProperty("modification_value");

        #region criteria
        criteria = new ReorderableList(serializedObject, criteria_key, true, true, true, true);
        criteria.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "criteria");
        criteria.drawElementCallback = (rect, index, active, focused) =>
        {
            var i = index;
            rect.y += 2;

            var buttonStyle = new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleLeft };
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width / 2 - 40, EditorGUIUtility.singleLineHeight), criteria_key.GetArrayElementAtIndex(i).stringValue, buttonStyle))
            {
                var list = VariableTable.table.variables.Keys;

                var menu = new GenericMenu();
                foreach (var element in list)
                {
                    menu.AddItem(new GUIContent(element), false, () =>
                    {
                        criteria_key.GetArrayElementAtIndex(i).stringValue = element;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            criteria_type.GetArrayElementAtIndex(index).enumValueIndex = EditorGUI.Popup(
                new Rect(rect.x + rect.width / 2 - 40, rect.y, 40, rect.height),
                criteria_type.GetArrayElementAtIndex(index).enumValueIndex,
                new string[] { "==", "!=", ">", "<" },
                new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }
            );

            if (VariableTable.table == null || !VariableTable.table.variables.ContainsKey(criteria_key.GetArrayElementAtIndex(i).stringValue))
            {
                criteria_value.GetArrayElementAtIndex(index).intValue =
                    EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2 - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), criteria_value.GetArrayElementAtIndex(index).intValue);


                GUI.DrawTexture(new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), EditorGUIUtility.IconContent("Warning@2x").image);
            }
            else
            {
                criteria_value.GetArrayElementAtIndex(index).intValue =
                    EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), criteria_value.GetArrayElementAtIndex(index).intValue);
            }
        };
        criteria.onAddCallback = list =>
        {
            if (VariableTable.table == null)
            {
                Debug.LogError("VariableTable is not exist, Please create variable table");
                return;
            }
            if (VariableTable.table.variables.Count == 0)
            {
                Debug.LogWarning("VariableTable is empty");
                return;
            }

            criteria_key.arraySize++;
            criteria_key.GetArrayElementAtIndex(criteria_key.arraySize - 1).stringValue
                = VariableTable.table.variables.Keys.ToList()[0];
            criteria_type.arraySize++;
            criteria_value.arraySize++;
            serializedObject.ApplyModifiedProperties();
            list.Select(list.count - 1);
        };
        criteria.onRemoveCallback = list =>
        {
            criteria_type.DeleteArrayElementAtIndex(list.index);
            criteria_value.DeleteArrayElementAtIndex(list.index);
            criteria_key.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();

            list.Select(Mathf.Clamp(list.index, 0, list.count - 1));
        };
        criteria.onSelectCallback = list => criteriaSelectedIndex = list.index;
        criteria.onReorderCallback = list =>
        {
            criteria_key.MoveArrayElement(criteriaSelectedIndex, list.index);
            criteria_type.MoveArrayElement(criteriaSelectedIndex, list.index);
            criteria_value.MoveArrayElement(criteriaSelectedIndex, list.index);
        };
        #endregion

        #region modification
        modification = new ReorderableList(serializedObject, modification_key, true, true, true, true);
        modification.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "modification");
        modification.drawElementCallback = (rect, index, active, focused) =>
        {
            var i = index;
            rect.y += 2;

            var buttonStyle = new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleLeft };
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width / 2 - 40, EditorGUIUtility.singleLineHeight), modification_key.GetArrayElementAtIndex(i).stringValue, buttonStyle))
            {
                var list = VariableTable.table.variables.Keys;

                var menu = new GenericMenu();
                foreach (var element in list)
                {
                    menu.AddItem(new GUIContent(element), false, () =>
                    {
                        modification_key.GetArrayElementAtIndex(i).stringValue = element;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            modification_type.GetArrayElementAtIndex(index).enumValueIndex = EditorGUI.Popup(
                new Rect(rect.x + rect.width / 2 - 40, rect.y, 40, rect.height),
                modification_type.GetArrayElementAtIndex(index).enumValueIndex,
                new string[] { "=", "+=", "-=" },
                new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter }
            );

            if (VariableTable.table == null || !VariableTable.table.variables.ContainsKey(modification_key.GetArrayElementAtIndex(i).stringValue))
            {
                modification_value.GetArrayElementAtIndex(index).intValue =
                    EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2 - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), modification_value.GetArrayElementAtIndex(index).intValue);


                GUI.DrawTexture(new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), EditorGUIUtility.IconContent("Warning@2x").image);
            }
            else
            {
                modification_value.GetArrayElementAtIndex(index).intValue =
                    EditorGUI.IntField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), modification_value.GetArrayElementAtIndex(index).intValue);
            }
        };
        modification.onAddCallback = list =>
        {
            if (VariableTable.table == null)
            {
                Debug.LogError("VariableTable is not exist, Please create variable table");
                return;
            }
            if (VariableTable.table.variables.Count == 0)
            {
                Debug.LogWarning("VariableTable is empty");
                return;
            }

            modification_key.arraySize++;
            modification_key.GetArrayElementAtIndex(modification_key.arraySize - 1).stringValue
                = VariableTable.table.variables.Keys.ToList()[0];
            modification_type.arraySize++;
            modification_value.arraySize++;
            serializedObject.ApplyModifiedProperties();
            list.Select(list.count - 1);
        };
        modification.onRemoveCallback = list =>
        {
            modification_type.DeleteArrayElementAtIndex(list.index);
            modification_value.DeleteArrayElementAtIndex(list.index);
            modification_key.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();

            list.Select(Mathf.Clamp(list.index, 0, list.count - 1));
        };
        modification.onSelectCallback = list => modificationSelectedIndex = list.index;
        modification.onReorderCallback = list =>
        {
            modification_key.MoveArrayElement(modificationSelectedIndex, list.index);
            modification_type.MoveArrayElement(modificationSelectedIndex, list.index);
            modification_value.MoveArrayElement(modificationSelectedIndex, list.index);
        };
        #endregion
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(order);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(next);
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(20);

        criteria.DoLayoutList();
        modification.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif