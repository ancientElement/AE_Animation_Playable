using AE_FSM;
using AE_Motion;
using UnityEngine;

public class EmergencyStopState : BaseState
{
    private float walkStopAnimTime = 1.1f;
    private float runStopAnimTime = 1.2f;

    public override void Enter(FSMController  controller)
    {
        base.Enter(controller);
        ApplayRootMotion();
        if (controller.preState != null)
        {
            if (controller.preState.stateNodeData.name == "WalkState")
            {
                m_anim.TransitionTo("WalkStop");
                controller.SwitchState("IdleState", walkStopAnimTime);
            }
            else if (controller.preState.stateNodeData.name == "RunState")
            {
                m_anim.TransitionTo("RunStop");
                controller.SwitchState("IdleState", runStopAnimTime);
            }
        }
    }

    public override void Exit(FSMController  controller)
    {
        PreventRootMotion();
    }

    public override void FixUpdate(FSMController  controller)
    {
    }

    public override void LaterUpdate(FSMController  controller)
    {
    }

    public override void Update(FSMController  controller)
    {
    }
}
