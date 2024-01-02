using UnityEngine;
using UnityEngine.UIElements;

namespace AE_FSM
{
    public class ElementBase
    {
        protected VisualElement m_root;
        public VisualElement Root => m_root;
        public Color color;

        public ElementBase(VisualElement fatherElement)
        {
            m_root = new VisualElement();
            fatherElement.Add(m_root);
            m_root.style.position = Position.Absolute;
        }

        /// <summary>
        /// 自定义按钮
        /// </summary>
        /// <param name="rect">布局</param>
        /// <param name="clickedAction">点击回调</param>
        public ElementBase(VisualElement fatherElement, Rect rect, Color color)
        {
            this.color = color;
            m_root = new VisualElement();
            fatherElement.Add(m_root);
            //m_root.style.position = Position.Absolute;
            ResetView(rect, color);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void ResetView(Rect rect, Color color)
        {
            this.color = color;
            m_root.transform.position = rect.position;
            m_root.style.width = rect.width;
            m_root.style.height = rect.height;
            m_root.style.backgroundColor = color;
        }

        public void ResetView(Rect rect)
        {
            ResetView(rect, this.color);
        }

        public void ResetView(Color color)
        {
            ResetView(m_root.contentRect, color);
        }

        public void ResetView()
        {
            ResetView(m_root.contentRect, this.color);
        }
    }
}