using System.Runtime.InteropServices.ComTypes;
using AE_FSM;
using AE_Motion;
using UnityEngine;

public class IdleState : BaseState
{
    private bool isSpeedToZero;

    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        isSpeedToZero = false;
        #region  Test
        //播放动画
        if (controller.preState != null)
        {
            if (m_params.lastRun)
            {
                ApplayRootMotion();
                m_anim.TransitionTo("RunStop", () =>
                {
                    PreventRootMotion();
                    m_anim.TransitionTo("Idle");
                });
            }
        }
        #endregion
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

    public override void Update(FSMController controller)
    {
        base.Update(controller);
        if (!isSpeedToZero)
        {
            m_ctx.currentSpeed = Mathf.Lerp(m_ctx.currentSpeed, 0, 0.05f);
            m_ctx.currentTurnSpeed = Mathf.Lerp(m_ctx.currentTurnSpeed, 0, 0.05f);
            if (m_ctx.currentSpeed <= m_ctx.zero)
            {
                PreventRootMotion();
                isSpeedToZero = true;
                m_anim.TransitionTo("Idle");
            }
            m_anim.BlendTree1DSetValue("MoveBlendTree", m_ctx.currentSpeed);
            // m_anim.BlendClipTree2DSetPointer("MoveBlendTree2D", new Vector2(0,m_ctx.currentSpeed));
            // m_anim.AnimatorSetFloat("Velocity", m_ctx.currentSpeed);
            // m_anim.AnimatorSetFloat("TurnSpeed", m_ctx.currentTurnSpeed);
        }
    }
}