using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AE_FSM
{
    public class FSMStateNode
    {
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
            controller.excuteState.Enter(this);
        }

        public void Update()
        {
            controller.excuteState.Update(this);
            controller.CheckTransfrom();
        }
        public void LateUpdate()
        {
            controller.excuteState.LaterUpdate(this);
        }
        public void FixUpdate()
        {
            controller.excuteState.FixUpdate(this);
        }
        public void Exit()
        {
            controller.excuteState.Exit(this);
        }
    }
}