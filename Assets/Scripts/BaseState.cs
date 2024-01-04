using AE_FSM;
using AE_Motion;
using UnityEngine;

public class BaseState : IFSMState
{
    protected PlayerAnim m_anim;
    protected PlayerParam m_params;
    protected PlayerSensor m_sensor;
    protected Transform m_modle;
    protected CharacterController m_characterController;
    protected Rigidbody m_body;
    protected PlayerLocomotionContext m_ctx;

    public virtual void Enter(FSMController controller)
    {
        m_anim = controller.Motion.Anim;
        m_modle = controller.Motion.Modle;
        m_params = controller.Motion.Param;
        m_sensor = controller.Motion.Sensor;
        m_ctx = controller.Motion.LocomtionCtx;
        m_body = controller.Motion.Body;
        m_characterController = controller.Motion.CharactorController;
    }

    public virtual void Exit(FSMController controller)
    {
    }

    public virtual void FixUpdate(FSMController controller)
    {
    }

    public virtual void LaterUpdate(FSMController controller)
    {
    }

    public virtual void Update(FSMController controller)
    {
    }

    protected virtual void ApplayRootMotion()
    {
        m_anim.ApplayRootMotion();
    }

    protected virtual void PreventRootMotion()
    {
        m_anim.PreventRootMotion();
    }
}