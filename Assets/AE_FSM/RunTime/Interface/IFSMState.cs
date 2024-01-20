namespace AE_FSM
{
    public interface IFSMState
    {
        public void Enter(FSMController controller);
        public void OnUpdate(FSMController controller);
        public void LaterUpdate(FSMController controller);
        public void FixUpdate(FSMController controller);
        public void Exit(FSMController controller);
    }
}