using AE_FSM;
using AE_Motion;
using UnityEngine;

public class Turn180State : BaseState
{
    // private float runTurnLength = 0.733f;
    // private float walkTurnLength = 1f;
    public override void Enter(FSMController  controller)
    {
        base.Enter(controller);
        ApplayRootMotion();
        // if (controller.preState != null)
        // {
        //     if (controller.preState.stateNodeData.name == "RunState")
        //     {
        //         m_anim.TransitionTo("RunTurn180");
        //         controller.SwitchState("RunState", runTurnLength);
        //     }
        //     else if (controller.preState.stateNodeData.name == "WalkState")
        //     {
        //         m_anim.TransitionTo("WalkTurn180");
        //         controller.SwitchState("WalkState", walkTurnLength);
        //     }
        //     else
        //     {
        //         controller.SwitchState("IdleState");
        //     }
        // }
        // else
        // {
        //     controller.SwitchState("IdleState");
        // }
        m_anim.AnimatorCroosFade("RunTurn180",0.2f);
    }

    public override void Exit(FSMController  controller)
    {
        PreventRootMotion();
    }
    public override void FixUpdate(FSMController  controller)
    {
    }
    public override void Update(FSMController  controller)
    {
    }
    public override void LaterUpdate(FSMController  controller)
    {
    }
}