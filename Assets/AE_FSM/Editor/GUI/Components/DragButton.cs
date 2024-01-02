using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AE_FSM
{
    public class DragButton : ButtonBase
    {
        public bool isDraging;
        public Vector2 startDragMousePosition;
        private Action<MouseMoveEvent> dragAction;

        public DragButton(VisualElement fatherElement) : base(fatherElement)
        {
            m_root.style.position = Position.Absolute;
            RegisterGrag();
        }

        public DragButton(VisualElement fatherElement, Rect rect, Color color, Action<MouseDownEvent> clickedAction) : base(fatherElement, rect, color, clickedAction)
        {
            m_root.style.position = Position.Absolute;
            RegisterGrag();
        }

        public void RegisterDrage(Action<MouseMoveEvent> dragAction)
        {
            this.dragAction = dragAction;
        }

        protected override void MouseDown(MouseDownEvent evt)
        {
            base.MouseDown(evt);
            isDraging = true;
            startDragMousePosition = evt.mousePosition;
        }

        public void MouseUp(MouseUpEvent evt)
        {
            isDraging = false;
        }

        private void MouseMove(MouseMoveEvent evt)
        {
            if (isDraging)
            {
                dragAction?.Invoke(evt);
            }
        }

        private void MouseOut(MouseOutEvent evt)
        {
            isDraging = false;
        }

        public void RegisterGrag()
        {
            this.m_root.RegisterCallback<MouseMoveEvent>(MouseMove);
            this.m_root.RegisterCallback<MouseOutEvent>(MouseOut);
            this.m_root.RegisterCallback<MouseUpEvent>(MouseUp);
        }
    }
}