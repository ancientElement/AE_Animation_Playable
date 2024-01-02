using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    /// <summary>
    /// Bool类型条件类
    /// </summary>
    public class FSMConditionBoolInspector : FSMConditionInspector
    {
        public override void OnGUI(Rect rect, RunTimeFSMController controller, FSMConditionData conditionData)
        {
            if (EditorGUI.DropdownButton(rect, new GUIContent(conditionData.tragetValue == 1 ? "True" : "False"), FocusType.Keyboard))
            {
                GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("True"), conditionData.tragetValue == 1, () =>
                {
                    conditionData.tragetValue = 1;
                    controller.Save();
                });

                genericMenu.AddItem(new GUIContent("False"), conditionData.tragetValue == 0, () =>
                {
                    conditionData.tragetValue = 0;
                    controller.Save();
                });
                genericMenu.ShowAsContext();
            }
        }
    }
}