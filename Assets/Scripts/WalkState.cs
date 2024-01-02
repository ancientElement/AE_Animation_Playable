using AE_FSM;

public class WalkState : MoveState
{
    public override void Enter(FSMController controller)
    {
        base.Enter(controller);
        m_anim.TransitionTo("Walk");
        // m_anim.AnimatorCroosFade("Walk", 0f);
    }
    public override void Exit(FSMController controller)
    {
        base.Exit(controller);
    }

    public override void FixUpdate(FSMController controller)
    {
        base.FixUpdate(controller);
    }
    public override void Update(FSMController controller)
    {
        base.Update(controller);
    }

}