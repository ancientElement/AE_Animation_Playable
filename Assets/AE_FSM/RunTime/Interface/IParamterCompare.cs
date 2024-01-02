using UnityEngine;

namespace AE_FSM
{
    public interface IParamterCompare
    {
        /// <summary>
        /// 条件是否满足
        /// </summary>
        bool IsMeetCondition(FSMParameterData parameterData, float value);
    }

    public class LessCompare : IParamterCompare
    {
        public bool IsMeetCondition(FSMParameterData parameterData, float value)
        {
            //Debug.Log(parameterData.value + "___" + value);  
            return parameterData.Value.CompareTo(value) == -1;// 1 > ,0 = ,-1 <
        }
    }

    public class GreatCompare : IParamterCompare
    {
        public bool IsMeetCondition(FSMParameterData parameterData, float value)
        {
            return parameterData.Value.CompareTo(value) == 1;// 1 > ,0 = ,-1 <
        }
    }

    public class EqualCompare : IParamterCompare
    {
        public bool IsMeetCondition(FSMParameterData parameterData, float value)
        {
            return parameterData.Value.Equals(value);
        }
    }
    public class NotEqualCompare : IParamterCompare
    {
        public bool IsMeetCondition(FSMParameterData parameterData, float value)
        {
            return !parameterData.Value.Equals(value);
        }
    }
}