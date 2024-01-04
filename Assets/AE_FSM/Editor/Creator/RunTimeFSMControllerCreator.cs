using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace AE_FSM
{
    public class RunTimeFSMControllerCreator : EndNameEditAction
    {
        //按回车执行Action方法
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            RunTimeFSMController controller = ScriptableObject.CreateInstance<RunTimeFSMController>();
            AssetDatabase.CreateAsset(controller, pathName);
            Selection.activeObject = controller;

            //创建默认状态
            FSMStateNodeFactory.CreateFSMNode(controller, FSMConst.anyState, null, false, new Rect(0, 100, FSMConst.stateWidth, FSMConst.stateHeight));
            FSMStateNodeFactory.CreateFSMNode(controller, FSMConst.enterState, null, false, new Rect(0, -100, FSMConst.stateWidth, FSMConst.stateHeight));
        }
    }
}