using System.Collections.Generic;
using UnityEditor;

namespace AE_FSM
{
    public class FSMConditionFactory
    {
        public static void CreateFSMCondition(RunTimeFSMController contorller, FSMTranslationData translationData)
        {
            FSMConditionData conditionData = new FSMConditionData();

            FSMParameterData parameterData = null;

            string paramterName = string.Empty;

            if (contorller.paramters.Count > 0)
            {
                parameterData = contorller.paramters[0];
                paramterName = contorller.paramters[0].name;
            }

            if (parameterData != null)
            {
                switch (parameterData.paramterType)
                {
                    case ParamterType.Float:
                        conditionData.compareType = CompareType.Greate;
                        break;
                    case ParamterType.Int:
                        conditionData.compareType = CompareType.Greate;
                        break;
                    case ParamterType.Bool:
                        conditionData.compareType = CompareType.Equal;
                        break;
                }
            }
            else
            {
                conditionData.compareType = CompareType.Greate;
            }

            conditionData.paramterName = paramterName;
            conditionData.tragetValue = 0;

            if(translationData.conditions == null)
            { translationData.conditions = new List<FSMConditionData>(); }

            translationData.conditions.Add(conditionData);

            EditorUtility.SetDirty(contorller);

            AssetDatabase.SaveAssets();
        }

        public static void RemoveFSMCondition(RunTimeFSMController contorller, FSMTranslationData translationData, int conditionIndex)
        {
            if (conditionIndex < 0 || conditionIndex > translationData.conditions.Count - 1)
                return;
            translationData.conditions.RemoveAt(conditionIndex);

            EditorUtility.SetDirty(contorller);

            AssetDatabase.SaveAssets();
        }

    }
}