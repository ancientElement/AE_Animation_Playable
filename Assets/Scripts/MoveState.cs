using System.Collections;
using AE_FSM;
using UnityEngine;

public class MoveState : BaseState
{
    protected bool enableInput;
    protected bool isTurning;
    private Vector3 desierRotation;
    private Coroutine stopTurnCoroutine;

    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        PreventRootMotion();
        enableInput = true;
        m_ctx.currentSpeed = 0;
        m_ctx.currentTurnSpeed = 0;
        m_anim.TransitionTo("MoveBlendTree");
    }

    public override void Exit(FSMController controller)
    {
        PreventRootMotion();
    }

    public override void FixUpdate(FSMController controller)
    {
        if (!enableInput) return;

        if (isTurning) return;

        //可以考虑使用Quaternion.RotateTowards。
        //这个方法可以让你设置一个最大的旋转角度，以确保平滑过渡而不会瞬间完成旋转。
        if (m_sensor.desiredVelocity != Vector3.zero)
        {
            Quaternion from = m_modle.localRotation;
            Quaternion to = Quaternion.LookRotation(m_sensor.desiredVelocity);
            float angle = Quaternion.Angle(from, to);
            //Debug.Log(angle);
            if (m_ctx.currentSpeed >= m_ctx.runSpeed && angle > m_ctx.turnStartAngle)
            {
                Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                desierRotation = rotation * m_modle.forward;
                RunTurn180Angle(controller);
                return;
            }
            m_modle.localRotation = Quaternion.RotateTowards(from, to, Time.fixedDeltaTime * m_ctx.rotateSpeed);
        }

        m_characterController.SimpleMove(m_sensor.desiredVelocity);
    }

    public override void LaterUpdate(FSMController controller)
    {

    }

    public override void Update(FSMController controller)
    {
        UpdateVelocity();
        m_anim.BlendTree1DSetValue("MoveBlendTree", m_ctx.currentSpeed);
        controller.SetBool("isTurning", isTurning);
        Debug.DrawRay(m_modle.position, desierRotation, Color.blue);
    }

    //更新速度
    private void UpdateVelocity()
    {
        if (isTurning) return;

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
    }

    //检测角度变化大于180
    public void RunTurn180Angle(FSMController controller)
    {
        isTurning = true;
        //Debug.Log("转弯");
        ApplayRootMotion();
        m_anim.TransitionTo("RunTurn180");
        if (stopTurnCoroutine != null)
            m_anim.StopCoroutine(stopTurnCoroutine);
        stopTurnCoroutine = m_anim.StartCoroutine(StopTurn(controller));
    }

    private IEnumerator StopTurn(FSMController controller)
    {
        Vector3 forward = new Vector3(m_modle.forward.x, 0, m_modle.forward.z);
        float angle = Vector3.Angle(desierRotation, forward);

        while (angle > m_ctx.turnStopAngle)
        {
            forward = new Vector3(m_modle.forward.x, 0, m_modle.forward.z);
            angle = Vector3.Angle(desierRotation, forward);
            yield return null;
        }

        m_modle.forward = desierRotation;

        PreventRootMotion();
        m_anim.TransitionTo("MoveBlendTree",null,0.01f);//如果不设置一个极小的事件下一次连续的 RunTurn180 --> RunTurn180将会非常短几乎看不出动画
        //Debug.Log("转弯结束");
        isTurning = false;
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
