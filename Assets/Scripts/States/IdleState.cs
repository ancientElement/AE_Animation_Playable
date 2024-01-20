using AE_FSM;
using System.Collections;
using UnityEngine;

public class IdleState : BaseState
{
    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        if (m_params.lastRun && m_anim.BlendTree1DGetValue("MoveBlendTree") > m_ctx.runSpeed * 0.68f)
        {
            ApplayRootMotion();
            m_anim.TransitionTo("RunStop", () => { PreventRootMotion(); m_anim.TransitionTo("Idle"); });
        }
        else
            m_anim.TransitionTo("Idle");
    }

    public override void Exit(FSMController controller)
    {

    }

    public override void FixUpdate(FSMController controller)
    {
    }

    public override void LaterUpdate(FSMController controller)
    {
    }

    public override void OnUpdate(FSMController controller)
    {
    }
}