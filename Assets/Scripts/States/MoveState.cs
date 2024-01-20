using AE_FSM;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveState : BaseState
{
    protected bool enableInput;

    protected bool isTurning;
    private int turnTimes = 0;
    protected float speedBeforTurn;

    private float lastblendTreeValue = 0;

    private Vector3 desierRotation;
    private Vector3 desiredVelocity;

    private Coroutine stopTurnCoroutine;

    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        PreventRootMotion();
        enableInput = true;
        m_sensor.speed = 0;
        m_anim.TransitionTo("MoveBlendTree", null, 0.2f);
        m_anim.BlendTree1DSetValue("MoveBlendTree", m_sensor.speed);
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
        if (desiredVelocity != Vector3.zero)
        {
            Quaternion from = m_modle.localRotation;
            Quaternion to = Quaternion.LookRotation(desiredVelocity);
            float angle = Quaternion.Angle(from, to);
            //Debug.Log(angle);
            if (m_params.run && angle > m_ctx.turnStartAngle)
            {
                Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                desierRotation = rotation * m_modle.forward;
                speedBeforTurn = m_sensor.speed;
                RunTurn180Angle(controller);
                return;
            }
            m_modle.localRotation = Quaternion.RotateTowards(from, to, Time.fixedDeltaTime * m_ctx.rotateSpeed);
        }

        if (!m_sensor.OnGround)
        {
            m_sensor.velocity.y += -m_sensor.grivaty * Time.fixedDeltaTime;
        }

        m_characterController.Move(m_sensor.velocity * Time.fixedDeltaTime);
    }

    public override void OnUpdate(FSMController controller)
    {
        UpdateVelocity();

        controller.SetBool("isTurning", isTurning);

        float blendTreeValue = m_anim.BlendTree1DGetValue("MoveBlendTree");
        float muilt = blendTreeValue > m_ctx.walkSpeed ? 4f : 8;
        float value = Mathf.Lerp(blendTreeValue, m_sensor.speed, Time.deltaTime * muilt);
        m_anim.BlendTree1DSetValue("MoveBlendTree", value);
        lastblendTreeValue = value;

        Debug.DrawRay(m_modle.position, desierRotation, Color.blue);
        Debug.DrawRay(m_modle.position, desiredVelocity, Color.gray);
    }

    /// <summary>
    /// 更新速度
    /// </summary>
    private void UpdateVelocity()
    {
        if (isTurning) return;
        m_sensor.speed = m_params.run ? m_ctx.runSpeed : m_ctx.walkSpeed;

        if (m_sensor.playerInputSpace)
        {
            Vector3 forward = m_sensor.playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = m_sensor.playerInputSpace.right;
            right.y = 0f;
            right.Normalize();

            desiredVelocity = (forward * m_params.moveInput.y + right * m_params.moveInput.x) * m_sensor.speed;
        }
        else
        {
            desiredVelocity = new Vector3(m_params.moveInput.x, 0f, m_params.moveInput.y) * m_sensor.speed;
        }

        float maxSpeedChange = m_ctx.maxAcceleration * Time.deltaTime;
        m_sensor.velocity.x = Mathf.MoveTowards(m_sensor.velocity.x, desiredVelocity.x, maxSpeedChange);
        m_sensor.velocity.z = Mathf.MoveTowards(m_sensor.velocity.z, desiredVelocity.z, maxSpeedChange);
    }
    /// <summary>
    /// 检测角度变化大于180
    /// </summary>
    /// <param name="controller"></param>
    public void RunTurn180Angle(FSMController controller)
    {
        isTurning = true;
        ApplayRootMotion();
        m_anim.TransitionTo("RunTurn180_" + turnTimes % 2);
        if (stopTurnCoroutine != null)
            m_anim.StopCoroutine(stopTurnCoroutine);
        stopTurnCoroutine = m_anim.StartCoroutine(StopTurn(controller));
    }
    /// <summary>
    /// 停止转弯协程
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
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
        m_anim.TransitionTo("MoveBlendTree");
        isTurning = false;
        turnTimes += 1;
        yield break;
    }
    /// <summary>
    /// 允许RootMotion
    /// </summary>
    protected override void ApplayRootMotion()
    {
        enableInput = false;
        base.ApplayRootMotion();
    }
    /// <summary>
    /// 禁止RootMotion
    /// </summary>
    protected override void PreventRootMotion()
    {
        enableInput = true;
        base.PreventRootMotion();
    }
}