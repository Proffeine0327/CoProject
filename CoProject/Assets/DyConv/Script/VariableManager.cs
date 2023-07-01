using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class VariableManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        private static VariableManager manager;

        public static bool CheckCriteria(string key, CriteriaType type, int value)
        {
            switch (type)
            {
                case CriteriaType.equal:
                    return manager.variables[key] == value;
                case CriteriaType.notequal:
                    return manager.variables[key] != value;
                case CriteriaType.more:
                    return manager.variables[key] > value;
                case CriteriaType.less:
                    return manager.variables[key] < value;
            }

            return false;
        }

        public static void ModifyVariable(string key, ModificationType type, int value)
        {
            switch (type)
            {
                case ModificationType.equal:
                    manager.variables[key] = value;
                    break;
                
                case ModificationType.add:
                    manager.variables[key] += value;
                    break;

                case ModificationType.subtract:
                    manager.variables[key] -= value;
                    break;
            }
        }

        [SerializeField] private VariableTable table;
        [SerializeField, HideInInspector] private List<string> keys = new List<string>();
        [SerializeField, HideInInspector] private List<int> values = new List<int>();
        private Dictionary<string, int> variables = new Dictionary<string, int>();

        private void Awake() 
        {
            if (manager == null)
            {
                variables = new Dictionary<string, int>(table.variables);
                manager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning($"VaribleTable is already exist, {gameObject.name} was delete.");
            }
        }

        private void Update() 
        {
            
        }

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
    }
}
