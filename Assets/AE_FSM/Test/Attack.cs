using AE_FSM;
using UnityEngine;

public class Attack : IFSMState
{
    public void Enter(FSMController controller)
    {
        Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "Enter");
    }
    public void Update(FSMController controller)
    {
       
    }
    public void LaterUpdate(FSMController controller)
    {
      
    }
    public void FixUpdate(FSMController controller)
    {

    }
    public void Exit(FSMController controller)
    {
        Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "Exit");
    }
}
