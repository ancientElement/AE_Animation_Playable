using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.VersionControl;

namespace AE_FSM
{
    /// <summary>
    /// 条件状态
    /// </summary>
    public enum ConditionState
    {
        Meet,//满足
        NotMeet//不满足
    }

    public class FSMCondition
    {
        private FSMConditionData conditionData;
        private FSMParameterData parameterData;

        public ConditionState state { get; private set; } = ConditionState.NotMeet;// 条件状态

        /// <summary>
        /// 当前条件的比较函数
        /// </summary>
        private IParamterCompare compare => GetCompare(conditionData.compareType);

        #region 初始化条件判断

        /// <summary>
        /// 参数类型的判断函数
        /// </summary>
        private static Dictionary<CompareType, IParamterCompare> Compares = new Dictionary<CompareType, IParamterCompare>();

        static FSMCondition()
        {
            InitCompares();
        }

        private static void InitCompares()
        {
            if (Compares.Count == 0)
            {
                Compares.Add(CompareType.Greate, new GreatCompare());
                Compares.Add(CompareType.Less, new LessCompare());
                Compares.Add(CompareType.Equal, new EqualCompare());
                Compares.Add(CompareType.NotEqual, new NotEqualCompare());
            }
        }
        #endregion

        public FSMCondition(FSMConditionData conditionData, FSMController controller)
        {
            this.conditionData = conditionData;

            if (controller.parameters.TryGetValue(conditionData.paramterName, out parameterData))
            {
                parameterData.onValueChage += CheckParamterValueChange;
            }

            CheckParamterValueChange();

        }

        /// <summary>
        /// 检测条件是否满足
        /// </summary>
        private void CheckParamterValueChange()
        {
            //检测条件是否满足
            if (compare.IsMeetCondition(this.parameterData, this.conditionData.tragetValue))
            {
                state = ConditionState.Meet;
            }
            else
            {
                state = ConditionState.NotMeet;
            }
        }

        /// <summary>
        /// 根据类型获取比较方式
        /// </summary>
        /// <param name="compareType"></param>
        /// <returns></returns>
        private static IParamterCompare GetCompare(CompareType compareType)
        {
            if (Compares.TryGetValue(compareType, out IParamterCompare compare))
            {
                return compare;
            }
            return null;
        }
    }
}