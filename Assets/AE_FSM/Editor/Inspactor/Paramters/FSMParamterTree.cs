using System.Diagnostics;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AE_FSM
{
    /// <summary>
    /// 弹出框的参数列表
    /// </summary>
    public class FSMParamterTree : TreeView
    {
        private RunTimeFSMController controller;
        private FSMConditionData conditionData;

        public FSMParamterTree(TreeViewState state, RunTimeFSMController controller, FSMConditionData conditionData) : base(state)
        {
            this.controller = controller;
            this.conditionData = conditionData;

            showBorder = true;//边框
            showAlternatingRowBackgrounds = true;//交替显示
        }

        /// <summary>
        /// 绘制Tree的每个元素
        /// </summary>
        /// <returns></returns>
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1);

            if (controller != null)
            {
                for (int i = 0; i < controller.paramters.Count; i++)
                {
                    root.AddChild(new TreeViewItem(i, 0, controller.paramters[i].name));
                }
            }

            return root;
        }

        /// <summary>
        /// 绘制单个元素
        /// </summary>
        /// <param name="args"></param>
        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            if(args.label == conditionData.paramterName)
            {
                GUI.Label(args.rowRect, "√");
            }
        }

        /// <summary>
        /// 点击单个元素
        /// </summary>
        /// <param name="id"></param>
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            string paramterName = FindItem(id, rootItem).displayName;

            FSMParameterData parameterData = controller.paramters.Where(x => x.name == paramterName).FirstOrDefault();
            if (parameterData != null)
            {
                conditionData.paramterName = parameterData.name;
                switch (parameterData.paramterType)
                {
                    case ParamterType.Float:
                    case ParamterType.Int:
                        conditionData.compareType = CompareType.Greate;
                        break;
                    case ParamterType.Bool:
                        conditionData.compareType = CompareType.Equal;
                        break;
                }
                controller.Save();
            }
            else
            {
                UnityEngine.Debug.Log($"不存在参数{paramterName}");
                return;
            }
        }
    }
}