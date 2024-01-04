using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AE_Motion;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEditorInternal.VersionControl.ListControl;

namespace AE_FSM
{
    public class FSMController : MonoBehaviour
    {
        public PlayerMotion Motion { get; private set;}

        /// <summary>
        /// 配置文件
        /// </summary>
        [SerializeField] private RunTimeFSMController runTimeFSMController;
        private RunTimeFSMController _runTimeFSMController;

        /// <summary>
        /// 驱动器
        /// </summary>
        internal IExcuteState excuteState { get; private set; } = new DefualtIExcuteState();

        /// <summary>
        /// 设置驱动
        /// </summary>
        /// <param name="_excuteState"></param>
        public void SetIExcuteState(IExcuteState _excuteState)
        {
            if (excuteState != null)
            {
                excuteState = _excuteState;
            }
        }

        /// <summary>
        /// 参数
        /// </summary>
        internal Dictionary<string, FSMParameterData> parameters = new Dictionary<string, FSMParameterData>();
        /// <summary>
        /// 状态
        /// </summary>
        internal Dictionary<string, FSMStateNode> states = new Dictionary<string, FSMStateNode>();
        ///// <summary>
        ///// 过渡
        ///// </summary>
        //internal List<FSMTransition> transitions = new List<FSMTransition>();

        /// <summary>
        /// 默认状态
        /// </summary>
        internal FSMStateNode defaultState { get; private set; } = null;
        /// <summary>
        /// 当前状态
        /// </summary>
        public FSMStateNode currentState { get; private set; } = null;
        /// <summary>
        /// 上一个状态
        /// </summary>
        public FSMStateNode preState { get; private set; } = null;

        /// <summary>
        /// 退出时间
        /// </summary>
        [Range(0.05f, 10f)] public float DefaultExitTime = 0.1f;
        /// <summary>
        /// 退出状态
        /// </summary>
        public bool isExitingState;
        /// <summary>
        /// 是否在切换
        /// </summary>
        private bool isSwitching;
        public bool IsSwitching => isSwitching;

        /// <summary>
        /// 运行时 和 _ 的配置文件
        /// </summary>
        public RunTimeFSMController RunTimeFSMController
        {
            get
            {
                if (Application.isPlaying)
                { return _runTimeFSMController; }
                return runTimeFSMController;
            }
        }

        private void Update()
        {
            if (currentState != null) { currentState.Update(); }
        }

        private void LateUpdate()
        {
            if (currentState != null) { currentState.LateUpdate(); }
        }

        private void FixedUpdate()
        {
            if (currentState != null) { currentState.FixUpdate(); }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init(PlayerMotion playerMotion)
        {
            //复制数据
            if (runTimeFSMController == null) return;

            _runTimeFSMController = GameObject.Instantiate(runTimeFSMController);

            Motion = playerMotion;


            //初始化参数

            foreach (FSMParameterData item in _runTimeFSMController.paramters)
            {
                if (parameters.ContainsKey(item.name))
                {
                    Debug.LogWarning($"参数{item.name}已存在!!!");
                    continue;
                }
                item.onValueChage = null;
                parameters.Add(item.name, item);
            }

            //状态
            for (int i = 0; i < _runTimeFSMController.states.Count; i++)
            {
                FSMStateNodeData nodeData = _runTimeFSMController.states[i];
                FSMStateNode stateNode = new FSMStateNode(nodeData, this);

                if (states.ContainsKey(nodeData.name))
                {
                    Debug.LogWarning($"状态重复{nodeData.name}");
                    continue;
                }

                if (nodeData.defualtState == true)
                {
                    defaultState = stateNode;
                }
                states.Add(nodeData.name, stateNode);
            }

            //过渡
            for (int i = 0; i < _runTimeFSMController.states.Count; i++)
            {
                foreach (FSMTranslationData item in _runTimeFSMController.states[i].trasitions)
                {
                    FSMTransition transition = new FSMTransition(item, this);
                    states[item.fromState].transitions.Add(transition);
                }
            }

            //foreach (var item in _runTimeFSMController.trasitions)
            //{
            //    FSMTransition transition = new FSMTransition(item, this);
            //    transitions.Add(transition);
            //}

            //切换到默认状态
            SwitchState(defaultState);
        }

        /// <summary>
        /// 判断是否需要过渡
        /// </summary>
        public void CheckTransfrom()
        {
            foreach (FSMTransition item in currentState.transitions)
            {
                item.CheckAllConditionMeet();
            }
        }


        #region 设置参数

        public void SetBool(string paramterName, bool value)
        {
            SetParams(paramterName, value ? 1 : 0, ParamterType.Bool);
        }

        public void SetFloat(string paramterName, float value)
        {
            SetParams(paramterName, value, ParamterType.Float);
        }

        public void SetInt(string paramterName, int value)
        {
            SetParams(paramterName, value, ParamterType.Int);
        }

        private void SetParams(string paramterName, float value, ParamterType paramterType)
        {
            if (parameters.TryGetValue(paramterName, out FSMParameterData paramterData))
            {
                if (paramterData.paramterType == paramterType)
                {
                    paramterData.Value = value;
                }
                else
                {
                    Debug.LogWarning($"参数{paramterName}类型错误:{paramterType}");
                }
            }
            else
            {
                Debug.LogWarning($"参数{paramterName}不存在!!!");
            }
        }

        #endregion

        #region  切换状态
        public void SwitchState(string state, float exitTime = 0f, bool toself = false)
        {
            isSwitching = true;
            SwitchState(states[state], exitTime, toself);
        }

        public void SwitchState(FSMStateNode stateNode, float exitTime = 0f, bool toself = false)
        {
            if (exitTime == 0f)
                SwitchStateDirect(stateNode, toself);
            else
                SwitchStateWait(stateNode, exitTime);
        }

        /// <summary>
        /// 直接切换
        /// </summary>
        /// <param name="stateNode"></param>
        private void SwitchStateDirect(FSMStateNode stateNode, bool toself = false)
        {
            if (!toself)
                if (currentState == stateNode) return;

            if (stateNode == null) return;

            if (currentState != null)
                currentState.Exit();

            preState = currentState;
            currentState = stateNode;

            currentState.Enter();

            isSwitching = false;
        }

        /// <summary>
        /// 有退出时间的切换
        /// </summary>
        /// <param name="stateNode"></param>
        private void SwitchStateWait(FSMStateNode stateNode, float exitTime = -1f)
        {
            if (exitTime == -1f) exitTime = DefaultExitTime;
            isExitingState = true;
            StopAllCoroutines();
            StartCoroutine(DoWaitSwitchState(stateNode, exitTime));
        }

        private IEnumerator DoWaitSwitchState(FSMStateNode stateNode, float exitTime)
        {
            yield return new WaitForSeconds(exitTime);
            SwitchStateDirect(stateNode);
            isExitingState = false;
            isSwitching = false;
            yield break;
        }
        #endregion
    }
}