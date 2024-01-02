using UnityEditor;

namespace AE_FSM
{
    public class FSMStateInspectorHelper : ScriptableObjectSingleton<FSMStateInspectorHelper>
    {
        public RunTimeFSMController contorller;
        public FSMStateNodeData stateNodeData;

        public void Inspector(RunTimeFSMController contorller, FSMStateNodeData stateNodeData)
        {
            this.contorller = contorller;
            this.stateNodeData = stateNodeData;
            Selection.activeObject = this;
        }
    }
}