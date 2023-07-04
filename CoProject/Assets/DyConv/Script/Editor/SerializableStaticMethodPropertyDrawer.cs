using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(SerializableStaticMethod), true), CanEditMultipleObjects]
public class SerializableStaticMethodPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Type selected = null;
        var classType = property.FindPropertyRelative("classType");
        var methodName = property.FindPropertyRelative("methodName");
        var openParameter = property.FindPropertyRelative("openParameter");
        var parameters = property.FindPropertyRelative("parameters");
        var flag = BindingFlags.Public | BindingFlags.Static;

        EditorGUI.LabelField(new Rect(position.x, position.y, 100, EditorGUIUtility.singleLineHeight), "Method :");
        if (GUI.Button(new Rect(position.x + 100, position.y, position.width - 100, EditorGUIUtility.singleLineHeight), $"{classType.stringValue}/{methodName.stringValue}", EditorStyles.popup))
        {
            var scripts =
                Resources.FindObjectsOfTypeAll(typeof(MonoScript)).
                Cast<MonoScript>().
                Where(c => c.hideFlags == 0).
                Where(c => c.GetClass() != null).
                Where(c => c.GetClass().BaseType == typeof(MonoBehaviour) || c.GetClass().BaseType == typeof(ScriptableObject)).
                Where(c => c.GetClass().GetMethods(flag).Length != 0).
                Where(c => AssetDatabase.GetAssetPath(c).Substring(0, 9) != "Packages/").
                Select(c => c.GetClass()).
                ToList();

            var menu = new GenericMenu();
            for (int i = 0; i < scripts.Count; i++)
            {
                var script = scripts[i];
                var methods = script.GetMethods(flag);

                for (int j = 0; j < methods.Length; j++)
                {
                    var method = methods[j];

                    var continueMethod = false;
                    foreach (var par in method.GetParameters())
                    {
                        if
                        (
                            par.ParameterType != typeof(int) &&
                            par.ParameterType != typeof(float) &&
                            par.ParameterType != typeof(string) &&
                            !IsUnityObject(par.ParameterType)
                        )
                            continueMethod = true;
                    }
                    if (continueMethod) continue;

                    menu.AddItem(new GUIContent($"{script.Name}/{method.Name}"), false, () =>
                    {
                        classType.stringValue = script.Name;
                        methodName.stringValue = method.Name;
                        selected = script;
                        ApplyChange(parameters, script, method.Name);
                    });
                }
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("None"), false, () =>
            {
                classType.stringValue = "";
                methodName.stringValue = "";
                selected = null;
                parameters.arraySize = 0;
                ApplyChange(parameters);
            });
            menu.ShowAsContext();
        }

        position.y += EditorGUIUtility.singleLineHeight;
        openParameter.boolValue =
            EditorGUI.Foldout(new Rect(position.x + 10, position.y, position.width - 10, EditorGUIUtility.singleLineHeight), openParameter.boolValue, "Parameters", true);

        if (openParameter.boolValue)
        {
            for (int i = 0; i < parameters.arraySize; i++)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                var pars = parameters.GetArrayElementAtIndex(i);
                if (pars.FindPropertyRelative("paramType").enumValueFlag == (int)ParamType.Int)
                {
                    EditorGUI.LabelField(new Rect(position.x + 20, position.y, 100, EditorGUIUtility.singleLineHeight), "Int: ");
                    pars.FindPropertyRelative("intValue").intValue =
                        EditorGUI.IntField(new Rect(position.x + 120, position.y, position.width - 120, EditorGUIUtility.singleLineHeight), pars.FindPropertyRelative("intValue").intValue);
                }

                if (pars.FindPropertyRelative("paramType").enumValueFlag == (int)ParamType.Float)
                {
                    EditorGUI.LabelField(new Rect(position.x + 20, position.y, 100, EditorGUIUtility.singleLineHeight), "Float: ");
                    pars.FindPropertyRelative("floatValue").floatValue =
                        EditorGUI.FloatField(new Rect(position.x + 120, position.y, position.width - 120, EditorGUIUtility.singleLineHeight), pars.FindPropertyRelative("floatValue").floatValue);
                }

                if (pars.FindPropertyRelative("paramType").enumValueFlag == (int)ParamType.String)
                {
                    EditorGUI.LabelField(new Rect(position.x + 20, position.y, 100, EditorGUIUtility.singleLineHeight), "String: ");
                    pars.FindPropertyRelative("stringValue").stringValue =
                        EditorGUI.TextField(new Rect(position.x + 120, position.y, position.width - 120, EditorGUIUtility.singleLineHeight), pars.FindPropertyRelative("stringValue").stringValue);
                }

                if (pars.FindPropertyRelative("paramType").enumValueFlag == (int)ParamType.Object)
                {
                    var labelDisplay = pars.FindPropertyRelative("objectType").stringValue.Split('.');
                    
                    EditorGUI.LabelField(new Rect(position.x + 20, position.y, 100, EditorGUIUtility.singleLineHeight), $"{labelDisplay[labelDisplay.Length - 1]}: ");
                    pars.FindPropertyRelative("objectReferenceValue").objectReferenceValue =
                        EditorGUI.ObjectField(
                            new Rect(position.x + 120, position.y, position.width - 120, EditorGUIUtility.singleLineHeight),
                            pars.FindPropertyRelative("objectReferenceValue").objectReferenceValue,
                            FindType(pars.FindPropertyRelative("objectType").stringValue),
                            false
                            );
                }
            }
        }
        property.serializedObject.ApplyModifiedProperties();
    }

    private bool IsUnityObject(Type par)
    {
        for (var current = par; current != null; current = current.BaseType)
            if (current == typeof(UnityEngine.Object)) return true;
        return false;
    }

    private Type FindType(string typeName)
    {
        var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        foreach (var assembly in assemblies)
        {
            var a = Assembly.Load(assembly);
            if (a != null)
            {
                var type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }

    private void ApplyChange(SerializedProperty parameterProperty, Type type = null, string methodName = null)
    {
        if (type == null)
        {
            parameterProperty.arraySize = 0;
            return;
        }

        var method = type.GetMethod(methodName);
        var pars = method.GetParameters();
        parameterProperty.arraySize = pars.Length;

        for (int i = 0; i < pars.Length; i++)
        {
            var parameterValue = parameterProperty.GetArrayElementAtIndex(i);
            var paramType = parameterProperty.GetArrayElementAtIndex(i).FindPropertyRelative("paramType");

            if (IsUnityObject(pars[i].ParameterType))
            {
                paramType.enumValueFlag = (int)ParamType.Object;
                parameterValue.FindPropertyRelative("objectType").stringValue = pars[i].ParameterType.ToString();
                parameterValue.FindPropertyRelative("objectReferenceValue").objectReferenceValue = null;
            }

            if (pars[i].ParameterType == typeof(int))
            {
                paramType.enumValueFlag = (int)ParamType.Int;
                parameterValue.FindPropertyRelative("intValue").intValue = 0;
            }

            if (pars[i].ParameterType == typeof(float))
            {
                paramType.enumValueFlag = (int)ParamType.Float;
                parameterValue.FindPropertyRelative("floatValue").floatValue = 0;
            }

            if (pars[i].ParameterType == typeof(string))
            {
                paramType.enumValueFlag = (int)ParamType.String;
                parameterValue.FindPropertyRelative("stringValue").stringValue = "";
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var openParameter = property.FindPropertyRelative("openParameter");
        var parameters = property.FindPropertyRelative("parameters");
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * (!openParameter.boolValue ? 1 : (1 + parameters.arraySize));
    }
}
#endif
