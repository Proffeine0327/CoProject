using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "VariableTable", menuName = "Dialogue/VariableTable", order = 1)]
    public class VariableTable : ScriptableObject, ISerializationCallbackReceiver
    {
        public static VariableTable table;

        [SerializeField] private List<string> keys = new List<string>();
        [SerializeField] private List<int> values = new List<int>();
        public Dictionary<string, int> variables = new Dictionary<string, int>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var variable in variables)
            {
                keys.Add(variable.Key);
                values.Add(variable.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            variables = new Dictionary<string, int>();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++) variables.Add(keys[i], values[i]);
        }

#if UNITY_EDITOR
        private void OnEnable() 
        {
            UnityEditor.EditorApplication.update += Init;    
        }

        private void OnDestroy() 
        {
            if(table == this) table = null;    
        }

        private void Init()
        {
            if(!UnityEditor.EditorUtility.IsPersistent(this)) return;

            if (table == null) table = this;

            if (table != this)
            {
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(this));
                UnityEditor.AssetDatabase.Refresh();
                Debug.LogWarning("There are more than 2 VariableTable. The later things were deleted.");
            }

            UnityEditor.EditorApplication.update -= Init;
        }
#endif
    }
}
