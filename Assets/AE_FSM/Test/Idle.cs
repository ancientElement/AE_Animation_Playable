using AE_FSM;
using UnityEngine;

public class Idle : IFSMState
{
    public void Enter(FSMController controller)
    {
        //Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "Enter");
    }
    public void Update(FSMController controller)
    {
      
    }
    public void LaterUpdate(FSMController controller)
    {
        //Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "LaterUpdate");
    }
    public void FixUpdate(FSMController controller)
    {
        //Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "FixUpdate");
    }
    public void Exit(FSMController controller)
    {
        //Debug.Log(this.GetType().ToString() + controller.gameObject.name + "sss" + "Exit");
    }
}
