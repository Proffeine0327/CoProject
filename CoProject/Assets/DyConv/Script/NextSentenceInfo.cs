using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum CriteriaType 
    {
        equal,
        notequal,
        more,
        less
    }

    public enum ModificationType
    {
        equal,
        add,
        subtract
    }

    public class NextSentenceInfo : ScriptableObject
    {
        public int order;
        public List<string> criteria_key = new List<string>();
        public List<CriteriaType> criteria_type = new List<CriteriaType>();
        public List<int> criteria_value = new List<int>();

        public Sentence next;

        public List<string> modification_key = new List<string>();
        public List<ModificationType> modification_type = new List<ModificationType>();
        public List<int> modification_value = new List<int>();
    }
}

