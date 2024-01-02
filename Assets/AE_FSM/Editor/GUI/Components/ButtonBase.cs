using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AE_FSM
{
    public class ButtonBase : ElementBase
    {
        protected Action<MouseDownEvent> m_clickedAction;

        public ButtonBase(VisualElement fatherElement) : base(fatherElement)
        {
            m_root.RegisterCallback<MouseDownEvent>(MouseDown);
        }

        public ButtonBase(VisualElement fatherElement, Rect rect, Color color, Action<MouseDownEvent> clickedAction) : base(fatherElement, rect, color)
        {
            ResetView(rect, color);

            m_clickedAction = clickedAction;
            m_root.RegisterCallback<MouseDownEvent>(MouseDown);
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void MouseDown(MouseDownEvent evt)
        {
            m_clickedAction?.Invoke(evt);
        }

        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="action"></param>
        /// <param name="status"></param>
        public void RegisterRightClickMenu(string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
        {
            m_root.AddOneRightMenue(actionName, action, status);
        }
    }
}