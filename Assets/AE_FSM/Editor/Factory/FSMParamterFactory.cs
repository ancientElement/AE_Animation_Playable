using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class FSMParamterFactory
    {
        public static void CreateParamter(RunTimeFSMController contorller, ParamterType paramterType)
        {
            FSMParameterData parameterData = new FSMParameterData();
            parameterData.name = GetName(contorller, paramterType);
            parameterData.paramterType = paramterType;
            parameterData.value = 0;
            contorller.paramters.Add(parameterData);
            contorller.Save();
        }

        private static string GetName(RunTimeFSMController contorller, ParamterType paramterType)
        {
            int i = 0;
            string name;
            FSMParameterData paramter;

            do
            {
                name = $"New {paramterType}_{i}";
                paramter = contorller.paramters.Where(x => x.name == name).FirstOrDefault();
                i += 1;
            } while (paramter != null);

            return name;
        }

        public static void DeleteParamter(RunTimeFSMController contorller, int index)
        {
            if (Application.isPlaying) { return; }

            if (contorller == null || index > contorller.paramters.Count - 1 || index < 0) { return; }

            FSMParameterData parameterData = contorller.paramters[index];

            List<FSMTranslationData> translationDatas = new List<FSMTranslationData>();

            foreach (FSMStateNodeData state in contorller.states)
            {
                foreach (FSMTranslationData translation in state.trasitions)
                {
                    if (translation.conditions == null) break;

                    foreach (FSMConditionData condition in translation.conditions)
                    {
                        if (condition.paramterName != null && condition.paramterName == parameterData.name)
                        {
                            translationDatas.Add(translation);
                            break;
                        }
                    }
                }
            }


            if (translationDatas.Count == 0)
            {
                contorller.paramters.RemoveAt(index);
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append($"确定删除条件{parameterData.name}吗?");
                str.Append("\n");
                str.Append($"以下过渡使用条件{parameterData.name}");
                str.Append("\n");
                foreach (var item in translationDatas)
                {
                    str.Append($"{item.fromState} -> {item.toState} \n");
                }
                if (EditorUtility.DisplayDialog($"删除参数{parameterData.name}", str.ToString(), "确定", "取消"))
                {
                    foreach (FSMTranslationData itemTranslation in translationDatas)
                    {
                        foreach (FSMConditionData itemCondition in itemTranslation.conditions)
                        {
                            if (itemCondition.paramterName == parameterData.name)
                            {
                                itemCondition.paramterName = null;
                            }
                        }
                    }
                    contorller.paramters.RemoveAt(index);
                }
                contorller.Save();
            }
        }

        public static void RenameParamter(RunTimeFSMController controller, FSMParameterData parameterData, string newName)
        {
            if (controller == null || newName == string.Empty) { return; }

            if (parameterData.name == newName) return;

            FSMParameterData temp = controller.paramters.Where(x => x.name == newName).FirstOrDefault();

            if (temp != null)
            {
                Debug.Log("已经有该参数!!!"); return;
            }

            foreach (FSMStateNodeData state in controller.states)
            {
                foreach (FSMTranslationData itemTrasition in state.trasitions)
                {
                    if (itemTrasition.conditions == null || itemTrasition.conditions.Count() == 0) { continue; }
                    foreach (FSMConditionData itemCondition in itemTrasition.conditions)
                    {
                        if (itemCondition.paramterName == parameterData.name)
                        {
                            itemCondition.paramterName = newName;
                        }
                    }
                }
            }

            parameterData.name = newName;
            controller.Save();
        }
    }
}
