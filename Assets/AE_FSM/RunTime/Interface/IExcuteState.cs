namespace AE_FSM
{
    public interface IExcuteState
    {
        public void Enter(FSMStateNode node) { }
        public void Update(FSMStateNode node) { }
        public void LaterUpdate(FSMStateNode node) { }
        public void FixUpdate(FSMStateNode node) { }
        public void Exit(FSMStateNode node) { }
    }
}