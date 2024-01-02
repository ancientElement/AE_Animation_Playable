using System.Collections.Generic;
using UnityEngine;

namespace AE_FSM
{
    public class FSMTransition
    {
        public FSMTranslationData translationData;
        private FSMController controller;
        private FSMStateNode toStateNode;
        public List<FSMCondition> conditions;

        public FSMTransition(FSMTranslationData translationData, FSMController controller)
        {
            this.translationData = translationData;
            this.controller = controller;

            this.conditions = new List<FSMCondition>();
            foreach (FSMConditionData item in translationData.conditions)
            {
                FSMCondition condition = new FSMCondition(item, this.controller);
                this.conditions.Add(condition); 
            }

            if (controller.states.ContainsKey(translationData.toState))
            {
                toStateNode = controller.states[translationData.toState];
            }
        }

        public void CheckAllConditionMeet()
        {
            if (conditions.Count == 0) return;

            foreach (var item in conditions)
            {
                if (item.state == ConditionState.NotMeet)
                {
                    return;
                }
            }

            if (toStateNode == null) { Debug.Log("没有目标状态"); return; }

            //起始状态 是 目标状态
            if (controller.currentState.stateNodeData.name == translationData.toState)
            {
                return;
            }

            controller.SwitchState(toStateNode);
        }
    }
}