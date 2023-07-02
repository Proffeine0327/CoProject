using System.Linq;
using System.Collections;
using System.Collections.Generic;
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