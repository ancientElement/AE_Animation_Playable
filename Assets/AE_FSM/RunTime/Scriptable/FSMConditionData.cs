using System;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public enum CompareType
    {
        Greate,
        Less,
        Equal,
        NotEqual
    }

    [Serializable]
    public class FSMConditionData
    {
        public float tragetValue;
        public string paramterName;
        public CompareType compareType;
    }
}