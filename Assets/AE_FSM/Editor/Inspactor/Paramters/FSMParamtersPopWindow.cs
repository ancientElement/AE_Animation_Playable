using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AE_FSM
{
    /// <summary>
    /// 参数选择弹出窗口
    /// </summary>
    public class FSMParamtersPopWindow : PopupWindowContent
       {
        private float width;
        private FSMConditionData conditionData;
        private RunTimeFSMController controller;

        private SearchField searchField;
        private Rect searchRect;
        private float searchHeight = 25f;

        private Rect labelRect;
        private float labelHeight;

        private FSMParamterTree paramterTree;
        private TreeViewState paramterTreeState;
        private Rect paramterRect;

        public FSMParamtersPopWindow(float width, FSMConditionData conditionData, RunTimeFSMController controller)
        {
            this.width = width;
            this.conditionData = conditionData;
            this.controller = controller;
        }

        /// <summary>
        /// 绘制弹出框
        /// </summary>
        /// <param name="rect"></param>
        public override void OnGUI(Rect rect)
        {
            if (paramterTree == null)
            {
                if (paramterTreeState == null)
                {
                    paramterTreeState = new TreeViewState();
                }
                paramterTree = new FSMParamterTree(paramterTreeState, controller, conditionData);
                paramterTree.Reload();
            }

            //搜索框
            if (searchField == null)
            {
                searchField = new SearchField();
            }
            searchRect.Set(rect.x + 5, rect.y + 5, this.width - 10, searchHeight);
            paramterTree.searchString = searchField.OnGUI(searchRect, paramterTree.searchString);

            //标签
            labelRect.Set(rect.x, rect.y, rect.width, labelHeight);
            EditorGUI.LabelField(labelRect, conditionData.paramterName, GUI.skin.GetStyle("AC BoldHeader"));

            //参数列表
            paramterRect.Set(rect.x, rect.y + searchHeight + labelHeight, rect.width, rect.height - searchHeight - labelHeight);
            paramterTree.OnGUI(paramterRect);
        }

        /// <summary>
        /// 弹出框大小
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetWindowSize()
        {
            return new Vector2(this.width, 120);
        }
    }
}