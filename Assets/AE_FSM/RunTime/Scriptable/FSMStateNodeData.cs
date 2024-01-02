using System;
using System.Collections.Generic;
using UnityEngine;

namespace AE_FSM
{
    [Serializable]
    public class FSMStateNodeData
    {
#if UNITY_EDITOR
        public Rect rect;
#endif

        /// <summary>
        /// 是否是默认
        /// </summary>
        public bool defualtState;

        /// <summary>
        /// 状态名
        /// </summary>
        public string name;

        /// <summary>
        /// 脚本的名称
        /// </summary>
        public string scriptName;

        public List<FSMTranslationData> trasitions = new List<FSMTranslationData>();
    }
}
