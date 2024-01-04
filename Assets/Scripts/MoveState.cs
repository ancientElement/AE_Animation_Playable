using System.Globalization;
using System.Collections;
using AE_FSM;
using Unity.Rendering.HybridV2;
using UnityEngine;
using AE_Motion;

public class MoveState : BaseState
{
    protected bool enableInput;
    private bool isTurning;
    private Quaternion desierQuaternion;
    private Coroutine stopTurnCoroutine;

    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        PreventRootMotion();
        enableInput = true;
        m_ctx.currentSpeed = 0;
        m_ctx.currentTurnSpeed = 0;
        m_anim.TransitionTo("MoveBlendTree");
        // m_anim.TransitionTo("MoveBlendTree2D");
        // ApplayRootMotion();
        // m_anim.AnimatorCroosFade("MoveState", 0.1f);
    }

    public override void Exit(FSMController controller)
    {
        PreventRootMotion();
    }

    public override void FixUpdate(FSMController controller)
    {
        if (enableInput)
        {

            //可以考虑使用Quaternion.RotateTowards。这个方法可以让你设置一个最大的旋转角度，以确保平滑过渡而不会瞬间完成旋转。
            //转向完成之前不许旋转
            if (!isTurning && m_sensor.desiredVelocity != Vector3.zero)
            {
                m_modle.localRotation = Quaternion.RotateTowards(m_modle.localRotation, Quaternion.LookRotation(m_sensor.desiredVelocity), Time.fixedDeltaTime * m_ctx.rotateSpeed);
            }

            // TODO:转向
            // float angle = Vector2.SignedAngle(new Vector2(m_modle.forward.x, m_modle.forward.z), new Vector2(m_sensor.desiredVelocity.x, m_sensor.desiredVelocity.z));
            // angle = Mathf.Deg2Rad * angle/Time.fixedDeltaTime;
            // m_ctx.currentTurnSpeed = Mathf.Lerp(m_ctx.currentTurnSpeed, angle, 0.1f);
            // if()

            m_body.velocity = m_sensor.desiredVelocity;
            // m_characterController.SimpleMove(m_sensor.desiredVelocity);
        }
    }

    public override void LaterUpdate(FSMController controller)
    {

    }

    public override void Update(FSMController controller)
    {
        if (enableInput && !isTurning)
        {
            CheckRunTurn180Angle(controller);
            UpdateVelocity();
        }
        m_anim.BlendTree1DSetValue("MoveBlendTree", m_ctx.currentSpeed);
        // m_anim.BlendClipTree2DSetPointer("MoveBlendTree2D", new Vector2(m_ctx.currentTurnSpeed, m_ctx.currentSpeed));
        // m_anim.AnimatorSetFloat("Velocity", m_ctx.currentSpeed);
        // m_anim.AnimatorSetFloat("TurnSpeed", m_ctx.currentTurnSpeed);
    }

    //更新速度
    private void UpdateVelocity()
    {
        if (m_params.run)
        {
            m_ctx.currentSpeed = Mathf.Lerp(m_ctx.currentSpeed, m_ctx.runSpeed * 1.5f, 0.05f);
        }
        else
        {
            m_ctx.currentSpeed = Mathf.Lerp(m_ctx.currentSpeed, m_ctx.walkSpeed, 0.1f);
        }

        if (m_sensor.playerInputSpace)
        {
            Vector3 forward = m_sensor.playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = m_sensor.playerInputSpace.right;
            right.y = 0f;
            right.Normalize();

            m_sensor.desiredVelocity = (forward * m_params.moveInput.y + right * m_params.moveInput.x) * m_ctx.currentSpeed;
        }
        else
        {
            m_sensor.desiredVelocity = new Vector3(m_params.moveInput.x, 0f, m_params.moveInput.y) * m_ctx.currentSpeed;
        }

        #region  Test
        // float angle = Vector2.SignedAngle(new Vector2(m_modle.forward.x, m_modle.forward.z), new Vector2(m_sensor.desiredVelocity.x, m_sensor.desiredVelocity.z));

        // if (Mathf.Abs(angle) > 5)
        // {
        //     if (angle > 0)
        //         m_ctx.currentTurnSpeed = -Mathf.Lerp(m_ctx.currentTurnSpeed, m_params.run ? m_ctx.walkRotateSpeed : m_ctx.runRoateSpeed, 0.7f);
        //     else if (angle < 0)
        //         m_ctx.currentTurnSpeed = Mathf.Lerp(m_ctx.currentTurnSpeed, m_params.run ? m_ctx.walkRotateSpeed : m_ctx.runRoateSpeed, 0.7f);
        // }
        // else
        //     m_ctx.currentTurnSpeed = Mathf.Lerp(m_ctx.currentTurnSpeed, 0,0.1f);
        #endregion
    }

    //检测角度变化大于180
    public void CheckRunTurn180Angle(FSMController controller)
    {
        // 直接使用 输入 与 方向 相比较
        // Vector3.SignedAngle 计算两个向量之间的有符号角度，正值表示顺时针旋转，负值表示逆时针旋转。
        float angle = Mathf.Abs(Vector3.SignedAngle(m_modle.forward, m_sensor.desiredVelocity, m_modle.up));
        if (angle > 150f && m_ctx.currentSpeed > m_ctx.walkSpeed)
        {
            isTurning = true;
            ApplayRootMotion();
            controller.SetBool("isTurning", isTurning);
            desierQuaternion = Quaternion.Euler(new Vector3(m_modle.localEulerAngles.x, m_modle.localEulerAngles.y + 180f, m_modle.localEulerAngles.z));
            m_anim.TransitionTo("RunTurn180", () =>
            {
                if (stopTurnCoroutine != null)
                    m_anim.StopCoroutine(stopTurnCoroutine);
                stopTurnCoroutine = m_anim.StartCoroutine(StopTurn(controller));
                PreventRootMotion();
                m_anim.TransitionTo("MoveBlendTree");
                // m_anim.TransitionTo("MoveBlendTree2D");
                // m_anim.AnimatorCroosFade("MoveState", 0.1f);
            }, -1f, 0.8567f);
        }
    }

    private IEnumerator StopTurn(FSMController controller)
    {
        while (Quaternion.Angle(m_modle.localRotation, desierQuaternion) > 1f)
            m_modle.localRotation = Quaternion.Slerp(m_modle.transform.localRotation, desierQuaternion, 0.2f);
        yield return new WaitForSeconds(0.15f);
        isTurning = false;
        controller.SetBool("isTurning", isTurning);
        yield break;
    }

    protected override void ApplayRootMotion()
    {
        enableInput = false;
        base.ApplayRootMotion();
    }
    protected override void PreventRootMotion()
    {
        enableInput = true;
        base.PreventRootMotion();
    }
}
