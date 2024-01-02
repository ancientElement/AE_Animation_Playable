using System;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    /// <summary>
    /// Int类型条件类
    /// </summary>
    public class FSMConditionIntInspector : FSMConditionInspector
    {
        private Rect left_rect = new Rect();
        private Rect right_rect = new Rect();

        public override void OnGUI(Rect rect, RunTimeFSMController contorller, FSMConditionData conditionData)
        {
            left_rect.Set(rect.x, rect.y, rect.width / 2, rect.height);
            right_rect.Set(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height);

            //条件
            if (EditorGUI.DropdownButton(left_rect, new GUIContent(conditionData.compareType.ToString()), FocusType.Keyboard))
            {
                GenericMenu genericMenu = new GenericMenu();

                for (int i = 0; i < Enum.GetValues(typeof(CompareType)).Length; i++)
                {
                    CompareType compareType = (CompareType)Enum.GetValues(typeof(CompareType)).GetValue(i);

                    genericMenu.AddItem(new GUIContent(compareType.ToString()), conditionData.compareType == compareType, () =>
                    {
                        conditionData.compareType = compareType;
                        contorller.Save();
                    });
                }
                genericMenu.ShowAsContext();
            }

            //目标值
            conditionData.tragetValue = EditorGUI.IntField(right_rect, (int)conditionData.tragetValue);
            UnityEditor.EditorUtility.SetDirty(contorller);
        }
    }
}