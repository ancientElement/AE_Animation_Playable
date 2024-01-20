using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AE_FSM
{
    [CustomEditor(typeof(FSMStateInspectorHelper))]
    public class FSMStateInspector : Editor
    {
        public string stateName;
        private ReorderableList reorderableList;
        private int clickCount;
        private int preListIndex;

        private void OnEnable()
        {
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;
            reorderableList = new ReorderableList(helper.stateNodeData.trasitions, typeof(FSMTranslationData), true, true, false, true);
            reorderableList.onRemoveCallback += RemoveParamter;
            reorderableList.drawElementCallback += DrawOneParamter;
            reorderableList.onCanAddCallback += CanAddOrDeleteParamter;
            reorderableList.onMouseUpCallback = OnMouseUpCallbackParamter;
            reorderableList.drawHeaderCallback += DrawHeaderParamter;
        }

        public override void OnInspectorGUI()
        {
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;
            reorderableList.list = helper.stateNodeData.trasitions;

            bool disable = EditorApplication.isPlaying || helper.stateNodeData.name == FSMConst.enterState || helper.stateNodeData.name == FSMConst.anyState;

            EditorGUI.BeginDisabledGroup(disable);
            //脚本名
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("scriptName", GUILayout.Width(80));
            helper.stateNodeData.scriptName = EditorGUILayout.DelayedTextField(helper.stateNodeData.scriptName);
            if (helper.stateNodeData.scriptName != stateName)
            {
                stateName = helper.stateNodeData.scriptName;
                EditorUtility.SetDirty(helper.contorller);
            }
            EditorGUILayout.EndHorizontal();

            //脚本文件
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("script", GUILayout.Width(80));
            MonoScript tempScript = (MonoScript)EditorGUILayout.ObjectField(helper.stateNodeData.script, typeof(MonoScript), false);
            if (tempScript != helper.stateNodeData.script)
            {
                helper.stateNodeData.script = tempScript;
                EditorUtility.SetDirty(helper.contorller);
                AssetDatabase.SaveAssetIfDirty(helper.contorller);
            }
            EditorGUILayout.EndHorizontal();
            //过渡列表
            reorderableList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
        }

        protected override void OnHeaderGUI()
        {
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;

            bool disable = EditorApplication.isPlaying || helper.stateNodeData.name == FSMConst.enterState || helper.stateNodeData.name == FSMConst.anyState;

            string name = null;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(EditorGUIUtility.IconContent("icons/processed/unityeditor/animations/animatorstate icon.asset"), GUILayout.Width(30), GUILayout.Height(30));
                EditorGUILayout.LabelField("Name", GUILayout.Width(80));

                EditorGUI.BeginDisabledGroup(disable);
                name = EditorGUILayout.DelayedTextField(helper.stateNodeData.name);

                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                FSMStateNodeFactory.ReNameFSMNode(helper.contorller, helper.stateNodeData, name);
            }

            var rect = EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();
            Handles.color = Color.black;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }


        private void DrawHeaderParamter(Rect rect)
        {
            GUI.Label(rect, "Transition");
        }
        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnMouseUpCallbackParamter(ReorderableList list)
        {
            if (Event.current.button == 0)
            {
                if (preListIndex == list.index)
                {
                    clickCount++;
                }
                else if (preListIndex != list.index)
                {
                    clickCount = 0;
                }

                if (clickCount == 2)
                {
                    clickCount = 0;
                    // 双击事件处理逻辑
                    SelectCallback(list);
                }

                preListIndex = list.index;
            }
        }
        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="list"></param>
        private void SelectCallback(ReorderableList list)
        {
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;
            FSMTranslationInspectorHelper.Instance.Inspector(helper.contorller, helper.stateNodeData.trasitions[list.index]);
            FSMEditorWindow.GetWindow<FSMEditorWindow>().Repaint();
        }
        /// <summary>
        /// 是否可以移除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool CanAddOrDeleteParamter(ReorderableList list)
        {
            return Application.isPlaying == false;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawOneParamter(Rect rect, int index, bool isActive, bool isFocused)
        {
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;
            Repaint();
            EditorGUI.LabelField(rect, helper.stateNodeData.trasitions[index].fromState + "--->" + helper.stateNodeData.trasitions[index].toState);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="list"></param>
        private void RemoveParamter(ReorderableList list)
        {
            //TODO
            FSMStateInspectorHelper helper = target as FSMStateInspectorHelper;
            if (helper == null) return;
            FSMTranslationFactory.DeleteTransition(helper.contorller, helper.stateNodeData.trasitions[list.index]);
        }
    }
}