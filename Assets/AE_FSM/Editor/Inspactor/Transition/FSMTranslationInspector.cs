using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AE_FSM
{
    [CustomEditor(typeof(FSMTranslationInspectorHelper))]
    public class FSMTranslationInspector : Editor
    {
        /// <summary>
        /// 可排序的列表
        /// </summary>
        private ReorderableList reorderableList;

        private Rect left_container;
        private Rect right_container;
        private Dictionary<ParamterType, FSMConditionInspector> conditionInspectors = new Dictionary<ParamterType, FSMConditionInspector>();

        private Rect popRect;

        private void OnEnable()
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;
            reorderableList = new ReorderableList(helper.translationData.conditions, typeof(FSMConditionData), true, true, true, true);
            reorderableList.onAddCallback += AddCondition;
            reorderableList.onRemoveCallback += RemoveCondition;
            reorderableList.drawElementCallback += DrawElement;
            InitConditionInspectors();
        }

        /// <summary>
        /// 绘制Inspector面板
        /// </summary>
        public override void OnInspectorGUI()
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;
            reorderableList.list = helper.translationData.conditions;
            reorderableList.DoLayoutList();
        }

        /// <summary>
        /// 绘制面板头部
        /// </summary>
        protected override void OnHeaderGUI()
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(EditorGUIUtility.IconContent("icons/processed/unityeditor/animations/animatorstatetransition icon.asset"), GUILayout.Width(30), GUILayout.Height(30));

            GUILayout.Label($" {helper.translationData.fromState} ---> {helper.translationData.toState} ");

            EditorGUILayout.EndHorizontal();


            var rect = EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();
            Handles.color = Color.black;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 初始化Condition的绘制面板
        /// </summary>
        private void InitConditionInspectors()
        {
            conditionInspectors.Add(ParamterType.Int, new FSMConditionIntInspector());
            conditionInspectors.Add(ParamterType.Float, new FSMConditionFloatInspector());
            conditionInspectors.Add(ParamterType.Bool, new FSMConditionBoolInspector());
        }

        /// <summary>
        /// ReorderableList  绘制单个Condition
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;

            FSMConditionData conditionData = helper.translationData.conditions[index];

            left_container.Set(rect.x, rect.y, rect.width * 0.5f, rect.height);
            right_container.Set(left_container.x + left_container.width, left_container.y, rect.width * 0.5f, rect.height);
            //左半边 条件的参数
            if (helper.contorller.paramters.Count > 0)
            {
                if (EditorGUI.DropdownButton(left_container, new GUIContent(conditionData.paramterName), FocusType.Keyboard))
                {
                    //TODO 弹出下拉菜单
                    popRect.Set(rect.x, rect.y + 2, rect.width / 2, rect.height);
                    PopupWindow.Show(popRect, new FSMParamtersPopWindow(rect.width / 2, conditionData, helper.contorller));
                }
            }

            //右半边 条件的目标值(类型)
            FSMParameterData parameterData = helper.contorller.paramters.Where(x => x.name == conditionData.paramterName).FirstOrDefault();
            if (parameterData == null)
            {
                EditorGUI.LabelField(right_container, "缺少参数!!!");
            }
            else
            {
                //参数类型绘制Type
                if (conditionInspectors.Keys.Contains(parameterData.paramterType))
                {
                    conditionInspectors[parameterData.paramterType].OnGUI(right_container, helper.contorller, conditionData);
                }
                else
                {
                    Debug.LogError($"没有对应参数类型的绘制方式{parameterData.paramterType}");
                }
            }
        }

        /// <summary>
        /// ReorderableList 移除Codition
        /// </summary>
        /// <param name="list"></param>
        private void RemoveCondition(ReorderableList list)
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;
            FSMConditionFactory.RemoveFSMCondition(helper.contorller, helper.translationData, list.index);
        }

        /// <summary>
        /// ReorderableList  添加Condition
        /// </summary>
        /// <param name="list"></param>
        private void AddCondition(ReorderableList list)
        {
            FSMTranslationInspectorHelper helper = target as FSMTranslationInspectorHelper;
            if (helper == null) return;
            FSMConditionFactory.CreateFSMCondition(helper.contorller, helper.translationData);
        }
    }
}