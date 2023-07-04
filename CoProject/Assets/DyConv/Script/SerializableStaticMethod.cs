using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Dialogue
{
    [Serializable]
    public class SerializableStaticMethod
    {
        public string classType;
        public string methodName;
        public bool openParameter;
        public List<ParamInfo> parameters = new List<ParamInfo>();

        public object Invoke()
        {
            if (string.IsNullOrEmpty(classType))
                return null;

            if (string.IsNullOrEmpty(methodName))
                return null;

            Type t = Type.GetType(classType);
            MethodInfo info = t.GetMethod(methodName);

            var methodp = info.GetParameters();
            var p = new object[parameters.Count];
            for (int i = 0; i < methodp.Length; i++)
            {
                switch (parameters[i].paramType)
                {
                    case ParamType.Int:
                        p[i] = Convert.ChangeType(parameters[i].intValue, methodp[i].ParameterType);
                        break;
                    case ParamType.Float:
                        p[i] = Convert.ChangeType(parameters[i].floatValue, methodp[i].ParameterType);
                        break;
                    case ParamType.String:
                        p[i] = Convert.ChangeType(parameters[i].stringValue, methodp[i].ParameterType);
                        break;
                    case ParamType.Object:
                        p[i] = Convert.ChangeType(parameters[i].objectReferenceValue, methodp[i].ParameterType);
                        break;
                }
            }

            return info.Invoke(null, p);
        }
    }

    public enum ParamType
    {
        Int,
        Float,
        String,
        Object
    }

    [Serializable]
    public class ParamInfo
    {
        public ParamType paramType;

        public int intValue;
        public float floatValue;
        public string stringValue;

        public UnityEngine.Object objectReferenceValue;
        public string objectType;

        public void Reset()
        {
            intValue = 0;
            floatValue = 0;
            stringValue = "";
            objectReferenceValue = null;
        }
    }
}
