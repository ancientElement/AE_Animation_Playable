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
        m_anim = controller.playerMotion.Anim;
        m_modle = controller.playerMotion.Modle;
        m_params = controller.playerMotion.Param;
        m_sensor = controller.playerMotion.Sensor;
        m_ctx = controller.playerMotion.LocomtionCtx;
        m_characterController = controller.playerMotion.CharactorController;
    }

    public virtual void Exit(FSMController controller)
    {
        throw new System.NotImplementedException();
    }

    public virtual void FixUpdate(FSMController controller)
    {
        throw new System.NotImplementedException();
    }

    public virtual void LaterUpdate(FSMController controller)
    {
        throw new System.NotImplementedException();
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