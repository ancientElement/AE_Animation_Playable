using System;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class FSMConditionFloatInspector : FSMConditionInspector
    {
        private Rect left_rect;
        private Rect right_rect;

        public override void OnGUI(Rect rect, RunTimeFSMController contorller, FSMConditionData conditionData)
        {
            left_rect.Set(rect.x, rect.y, rect.width * 0.5f, rect.height);
            right_rect.Set(left_rect.x + left_rect.width, rect.y, rect.width * 0.5f, rect.height);

            //条件
            if (EditorGUI.DropdownButton(left_rect, new GUIContent(conditionData.compareType.ToString()),FocusType.Passive))
            {
                GenericMenu genericMenu = new GenericMenu();


                for (int i = 0; i < Enum.GetValues(typeof(CompareType)).Length; i++)
                {
                    CompareType compareType = (CompareType)Enum.GetValues(typeof(CompareType)).GetValue(i);

                    if (compareType == CompareType.Equal || compareType == CompareType.NotEqual) { continue; }

                    genericMenu.AddItem(new GUIContent(compareType.ToString()), conditionData.compareType == compareType, () =>
                    {
                        conditionData.compareType = compareType;
                        contorller.Save();
                    });
                }

                genericMenu.ShowAsContext();
            }

            //目标值
            conditionData.tragetValue = EditorGUI.FloatField(right_rect, conditionData.tragetValue);
            UnityEditor.EditorUtility.SetDirty(contorller);
        }
    }
}