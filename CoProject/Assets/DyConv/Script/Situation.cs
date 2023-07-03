using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Situation", fileName = "situation")]
    public class Situation : ScriptableObject
    {
        public TimelineAsset playable;
        public VariableTable variableTable;
        public Sentence currentSentence;
        public List<Sentence> sentences = new List<Sentence>();

        public void StartDialogue()
        {
            currentSentence = sentences[0];
        }

        public bool canContinue => CanContinue();
        private bool CanContinue()
        {
            foreach (var next in currentSentence.Nexts)
            {
                if (next.criteria_key.Count == 0) return true;

                for (int i = 0; i < next.criteria_key.Count; i++)
                    if (VariableManager.CheckCriteria(next.criteria_key[i], next.criteria_type[i], next.criteria_value[i])) return true;
            }

            return false;
        }

        public List<Sentence> GetNexts(bool ignoreCriteria)
        {
            List<Sentence> list = new List<Sentence>();
            if (ignoreCriteria)
            {
                foreach (var next in currentSentence.Nexts)
                    list.Add(next.next);
            }
            else
            {
                foreach (var next in currentSentence.Nexts)
                {
                    if (next.criteria_key.Count == 0)
                    {
                        list.Add(next.next);
                        continue;
                    }

                    for (int i = 0; i < next.criteria_key.Count; i++)
                        if (VariableManager.CheckCriteria(next.criteria_key[i], next.criteria_type[i], next.criteria_value[i]))
                            list.Add(next.next);
                }
            }

            return list;
        }

        public void Continue(int index = -1)
        {
            if (index >= 0)
            {
                for (int k = 0; k < currentSentence.Nexts[index].modification_key.Count; k++)
                    VariableManager.ModifyVariable(currentSentence.Nexts[index].modification_key[k], currentSentence.Nexts[index].modification_type[k], currentSentence.Nexts[index].modification_value[k]);
                currentSentence = currentSentence.Nexts[index].next;

                return;
            }

            foreach (var next in currentSentence.Nexts)
            {
                if (next.criteria_key.Count == 0)
                {
                    currentSentence = next.next;
                    for (int k = 0; k < next.modification_key.Count; k++)
                        VariableManager.ModifyVariable(next.modification_key[k], next.modification_type[k], next.modification_value[k]);
                    return;
                }

                for (int i = 0; i < next.criteria_key.Count; i++)
                {
                    if (VariableManager.CheckCriteria(next.criteria_key[i], next.criteria_type[i], next.criteria_value[i]))
                    {
                        currentSentence = next.next;
                        for (int k = 0; k < next.modification_key.Count; k++)
                            VariableManager.ModifyVariable(next.modification_key[i], next.modification_type[k], next.modification_value[k]);
                        return;
                    }
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AfterAssemblyCompile()
        {
            var situations = GetAllInstances<Situation>();

            foreach (var situation in situations)
            {
                foreach (var sentence in situation.sentences)
                    foreach (var m in sentence.Methods)
                    {
                        Type type = null;
                        foreach (var t in TypeCache.GetTypesDerivedFrom<Component>())
                        {
                            if (t.Name == m.classType)
                            {
                                type = t;
                                continue;
                            }
                        }

                        if (type == null)
                        {
                            m.classType = "";
                            m.methodName = "";

                            continue;
                        }

                        var method = type.GetMethod(m.methodName);

                        if (method == null)
                        {
                            m.classType = "";
                            m.methodName = "";
                            m.parameters.Clear();

                            continue;
                        }

                        var pars = method.GetParameters();

                        while (m.parameters.Count != pars.Length)
                        {
                            if (m.parameters.Count < pars.Length)
                            {
                                m.parameters.Add(new ParamInfo());
                                m.parameters[m.parameters.Count - 1].parameterValue = "";
                            }
                            else
                                m.parameters.RemoveAt(m.parameters.Count - 1);
                        }

                        for (int i = 0; i < pars.Length; i++)
                        {
                            var parameterValue = m.parameters[i];
                            if (pars[i].ParameterType == typeof(int))
                            {
                                if (!int.TryParse(parameterValue.parameterValue, out var result))
                                    parameterValue.parameterValue = "0";
                            }

                            if (pars[i].ParameterType == typeof(float))
                            {
                                if (!float.TryParse(parameterValue.parameterValue, out var result))
                                    parameterValue.parameterValue = "0";
                            }
                        }

                        for (int i = 0; i < pars.Length; i++)
                        {
                            var parameterType = m.parameters[i];
                            if (pars[i].ParameterType == typeof(int))
                                parameterType.parameterType = typeof(int).ToString();

                            if (pars[i].ParameterType == typeof(float))
                                parameterType.parameterType = typeof(float).ToString();

                            if (pars[i].ParameterType == typeof(string))
                                parameterType.parameterType = typeof(string).ToString();
                        }
                    }
                EditorUtility.SetDirty(situation);
                AssetDatabase.SaveAssetIfDirty(situation);
            }
        }

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

        public List<Connection> connections = new List<Connection>();

        private void OnEnable()
        {
            EditorApplication.update += Init;
            EditorApplication.update += SetTable;
        }

        private void Init()
        {
            if (sentences.Count == 0)
            {
                if (EditorUtility.IsPersistent(this))
                {
                    var start = ScriptableObject.CreateInstance<Sentence>();
                    start.hideFlags = HideFlags.HideInHierarchy;
                    start.name = "start";
                    start.rect.position = Vector2.one * 300;
                    start.target = this;
                    sentences.Add(start);

                    AssetDatabase.AddObjectToAsset(start, this);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssetIfDirty(this);
                }
            }
            else
            {
                EditorApplication.update -= Init;
            }
        }

        private void SetTable()
        {
            if (variableTable == null)
            {
                if (VariableTable.table != null) variableTable = VariableTable.table;
            }
            else EditorApplication.update -= SetTable;
        }
#endif
    }
}