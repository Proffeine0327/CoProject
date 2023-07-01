using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Dialogue;

public class VariableTableAddWindow : EditorWindow
{
    public static void Open(Action<string> action)
    {
        var window = EditorWindow.CreateInstance<VariableTableAddWindow>();
        
        window.action = action;
        var mousepos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousepos,Vector2.zero), new Vector2(200, 20));
    }

    string input;
    Action<string> action;

    private void OnGUI() 
    {
        EditorGUILayout.BeginHorizontal();
        input = EditorGUILayout.TextField(input);
        var isDisable = string.IsNullOrWhiteSpace(input) || VariableTable.table.variables.ContainsKey(input);
        EditorGUI.BeginDisabledGroup(isDisable);
        if(GUILayout.Button("Add", GUILayout.Width(50))) 
        {
            action.Invoke(input);
            this.Close();
        }

        if(Event.current.keyCode == KeyCode.Return && !isDisable)
        {
            action.Invoke(input);
            this.Close();
        }
        
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();    
    }
}
#endif