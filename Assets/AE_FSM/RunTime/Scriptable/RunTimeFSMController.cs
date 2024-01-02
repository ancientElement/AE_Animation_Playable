using System.Collections.Generic;
using UnityEngine;

namespace AE_FSM
{
    public class RunTimeFSMController : ScriptableObject
    {
        /// <summary>
        /// 所有状态数据
        /// </summary>
        public List<FSMStateNodeData> states = new List<FSMStateNodeData>();

        /// <summary>
        /// 所有参数数据
        /// </summary>
        public List<FSMParameterData> paramters = new List<FSMParameterData>();

        ///// <summary>
        ///// 所有的过渡
        ///// </summary>
        //public List<FSMTranslationData> trasitions = new List<FSMTranslationData>();

        public void Save()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}
