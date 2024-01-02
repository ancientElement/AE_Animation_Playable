using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AE_FSM
{
    public class ParamterLayer : GraphLayers
    {
        private ReorderableList reorderableList;
        private Vector2 scrollView;

        private Rect left_container;
        private Rect right_container;

        private bool isRenaming;
        private string tempname;

        public ParamterLayer(EditorWindow fSMEditorWindow) : base(fSMEditorWindow)
        {

        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            GUI.Box(rect, string.Empty, GUI.skin.GetStyle("CN Box"));

            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(this.Context.RunTimeFSMContorller.paramters, typeof(FSMParameterData));

                reorderableList.onAddCallback += AddParamter;
                reorderableList.onRemoveCallback += RemoveParamter;
                reorderableList.drawElementCallback += DrawOneParamter;

                reorderableList.onCanAddCallback += CanAddOrDeleteParamter;
            }
            reorderableList.list = this.Context.RunTimeFSMContorller.paramters;

            scrollView = GUILayout.BeginScrollView(scrollView);
            reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public override void ProcessEvent()
        {
            base.ProcessEvent();
        }

        private bool CanAddOrDeleteParamter(ReorderableList list)
        {
            return Application.isPlaying == false;
        }

        /// <summary>
        /// 绘制单条参数
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawOneParamter(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index > this.Context.RunTimeFSMContorller.paramters.Count - 1)
                return;
            FSMParameterData parameterData = this.Context.RunTimeFSMContorller.paramters[index];

            left_container.Set(rect.x, rect.y, rect.width * 0.5f, rect.height);
            right_container.Set(left_container.x + left_container.width, left_container.y, rect.width * 0.5f, rect.height);

            if (isFocused && EventUtility.IsMouseDown(0))
            {
                isRenaming = true;
            }


            //参数名
            if (isRenaming && reorderableList.index == index)
            {
                EditorGUI.BeginChangeCheck();
                tempname = EditorGUI.DelayedTextField(left_container, parameterData.name);
                if (EditorGUI.EndChangeCheck())
                {
                    FSMParamterFactory.RenameParamter(this.Context.RunTimeFSMContorller, parameterData, tempname);
                    isRenaming = false;
                }
            }
            else
            {
                EditorGUI.LabelField(left_container, parameterData.name);
            }

            if (isRenaming && EventUtility.IsMouseDown(0))
            {
                isRenaming = false;
            }

            switch (parameterData.paramterType)
            {
                case ParamterType.Float:
                    parameterData.Value = EditorGUI.DelayedFloatField(right_container, parameterData.Value);
                    break;
                case ParamterType.Int:
                    parameterData.Value = EditorGUI.DelayedIntField(right_container, (int)parameterData.Value);
                    break;
                case ParamterType.Bool:
                    parameterData.Value = EditorGUI.Toggle(right_container, parameterData.Value == 1) ? 1 : 0;
                    break;
            }

        }

        private void RemoveParamter(ReorderableList list)
        {
            //TODO
            FSMParamterFactory.DeleteParamter(this.Context.RunTimeFSMContorller, list.index);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="list"></param>
        private void AddParamter(ReorderableList list)
        {
            //TODO
            GenericMenu genericMenu = new GenericMenu();

            for (int i = 0; i < Enum.GetNames(typeof(ParamterType)).Length; i++)
            {
                ParamterType paramterType = (ParamterType)Enum.GetValues(typeof(ParamterType)).GetValue(i);
                genericMenu.AddItem(new GUIContent(Enum.GetNames(typeof(ParamterType))[i]), false, () =>
                {
                    FSMParamterFactory.CreateParamter(this.Context.RunTimeFSMContorller, paramterType);
                });
            }
            genericMenu.ShowAsContext();
        }
    }
}