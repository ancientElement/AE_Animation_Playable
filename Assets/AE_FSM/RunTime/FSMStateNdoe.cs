using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AE_FSM
{
    public class FSMStateNode
    {
        private bool m_enable;
        public FSMStateNodeData stateNodeData;
        public FSMController controller;
        public List<FSMTransition> transitions = new List<FSMTransition>();

        public FSMStateNode(FSMStateNodeData stateNodeData, FSMController controller)
        {
            this.stateNodeData = stateNodeData;
            this.controller = controller;
        }

        public void Enter()
        {
            m_enable = true;
            controller.excuteState.Enter(this);
        }

        public void Update()
        {
            if (!m_enable) return;
            controller.excuteState.Update(this);
            controller.CheckTransfrom();
        }
        public void LateUpdate()
        {
            if (!m_enable) return;
            controller.excuteState.LaterUpdate(this);
        }
        public void FixUpdate()
        {
            if (!m_enable) return;
            controller.excuteState.FixUpdate(this);
        }
        public void Exit()
        {
            m_enable = true;
            controller.excuteState.Exit(this);
        }
    }
}