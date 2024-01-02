using AE_FSM;
using UnityEngine;

public class RunState : MoveState
{
    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        m_anim.TransitionTo("Run");
        // m_anim.AnimatorCroosFade("Run", 0f);
    }

    public override void FixUpdate(FSMController controller)
    {
        base.FixUpdate(controller);
    }

    public override void Exit(FSMController controller)
    {
        base.Exit(controller);
    }

    public override void Update(FSMController controller)
    {
        base.Update(controller);
    }

    float CrossProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}