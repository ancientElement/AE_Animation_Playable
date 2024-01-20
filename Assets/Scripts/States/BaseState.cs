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
    protected PlayerLocomotionContext m_ctx;

    public virtual void Enter(FSMController controller)
    {
        m_anim = controller.GetComponent<PlayerMotion>().Anim;
        m_modle = controller.GetComponent<PlayerMotion>().Modle;
        m_params = controller.GetComponent<PlayerMotion>().Param;
        m_sensor = controller.GetComponent<PlayerMotion>().Sensor;
        m_ctx = controller.GetComponent<PlayerMotion>().LocomtionCtx;
        m_characterController = controller.GetComponent<PlayerMotion>().CharactorController;
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


    public virtual void OnUpdate(FSMController controller)
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