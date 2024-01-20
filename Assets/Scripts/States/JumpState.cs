using AE_FSM;
using UnityEngine;

public class JumpState : BaseState
{
    private float time;

    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        time = Mathf.Sqrt(2f * m_ctx.jumpHeight / m_sensor.grivaty);
    }

    public override void Exit(FSMController controller)
    {
    }

    public override void FixUpdate(FSMController controller)
    {
        if (time > 0)
        {
            float jumpVelocity = Mathf.Sqrt(2f * m_sensor.grivaty * m_ctx.jumpHeight);
            m_sensor.velocity.y += jumpVelocity;
        }
        else
        {
            if (!m_sensor.OnGround)
            {
                m_sensor.velocity.y += -m_sensor.grivaty * Time.fixedDeltaTime;
            }
        }
        m_sensor.velocity.y += m_sensor.grivaty * Time.deltaTime;
        m_characterController.Move(m_sensor.velocity);
    }

    public override void LaterUpdate(FSMController controller)
    {
    }

    public override void OnUpdate(FSMController controller)
    {
        time -= Time.deltaTime;
        controller.SetBool("OnGound",m_sensor.OnGround); 
    }
}