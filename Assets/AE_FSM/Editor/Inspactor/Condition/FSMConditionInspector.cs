using UnityEngine;

namespace AE_FSM
{
    /// <summary>
    /// 单个条件基类
    /// </summary>
    public abstract class FSMConditionInspector
    {
        public abstract void OnGUI(Rect rect, RunTimeFSMController contorller, FSMConditionData conditionData);
    }
}