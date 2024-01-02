using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class Context
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        private RunTimeFSMController m_rumtimeFSMController;
        public RunTimeFSMController RunTimeFSMContorller
        {
            get
            {
                RunTimeFSMController runTimeFSMContorller = GetFSMController();
                if (runTimeFSMContorller != null && runTimeFSMContorller != m_rumtimeFSMController)
                {
                    Reset();
                    m_rumtimeFSMController = runTimeFSMContorller;
                }
                return m_rumtimeFSMController;
            }
        }

        /// <summary>
        /// 状态机
        /// </summary>
        private FSMController _FSMController;
        public FSMController FSMController
        {
            get
            {
                if (Application.isPlaying)
                {
                    if ((Selection.activeObject as GameObject) != null && (Selection.activeObject as GameObject).GetComponent<FSMController>() != null)
                    {
                        _FSMController = (Selection.activeObject as GameObject).GetComponent<FSMController>();
                    }
                }
                return _FSMController;
            }
        }

        public List<FSMStateNodeData> m_selectNodeLsit = new List<FSMStateNodeData>();

        public List<FSMStateNodeData> SelectNodes
        { get { return m_selectNodeLsit; } }

        private FSMTranslationData m_selectTransition = null;

        public FSMTranslationData SelectTransition
        { get { return m_selectTransition; } set { m_selectTransition = value; } }

        public bool isPriviewingTransilation;
        public FSMStateNodeData fromState;
        public FSMStateNodeData hoverState;

        public void StartPriviewTransition(FSMStateNodeData fromState)
        {
            isPriviewingTransilation = true;
            this.fromState = fromState;
        }

        public void StopPriviewTransition()
        {
            isPriviewingTransilation = false;
            this.fromState = null;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        public float ZoomFactor { get; set; } = 0.3f;

        /// <summary>
        /// 拖拽偏移
        /// </summary>
        public Vector2 DragOffset { get; set; } = Vector2.zero;

        /// <summary>
        /// 获取状态配置文件
        /// </summary>
        /// <returns></returns>
        private RunTimeFSMController GetFSMController()
        {
            if ((Selection.activeObject as RunTimeFSMController) != null)
            {
                return Selection.activeObject as RunTimeFSMController;
            }
            if ((Selection.activeObject as GameObject) != null && (Selection.activeObject as GameObject).GetComponent<FSMController>() != null)
            {
                if ((Selection.activeObject as GameObject).GetComponent<FSMController>().RunTimeFSMController != null)
                {
                    return (Selection.activeObject as GameObject).GetComponent<FSMController>().RunTimeFSMController;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="fsmController"></param>
        public void SetFSMController(RunTimeFSMController fsmController)
        {
            m_rumtimeFSMController = fsmController;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            this.ZoomFactor = 0.3f;
            this.DragOffset = Vector2.zero;
        }

        /// <summary>
        /// 清除选择状态
        /// </summary>
        public void ClearAllSelectNode()
        {
            m_selectNodeLsit.Clear();
            m_selectTransition = null;
        }
    }
}