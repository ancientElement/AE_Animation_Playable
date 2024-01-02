using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AE_FSM
{
    public class ElementLayerIMGUI : ElementBase
    {
        private IMGUIContainer m_container;
        public IMGUIContainer Container => m_container;
        public const string StylePath = "Assets/AE_FSM/Editor/Assets/IMGUIContainer.uss";

        public ElementLayerIMGUI(VisualElement fatherElement) : base(fatherElement)
        {
            m_root.style.position = Position.Absolute;
            m_container = new IMGUIContainer();
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            m_container.styleSheets.Add(style);
            m_root.Add(m_container);
        }

        public ElementLayerIMGUI(VisualElement fatherElement, Rect rect, Color color) : base(fatherElement, rect, color)
        {
            m_root.style.position = Position.Absolute;
            m_container = new IMGUIContainer();
            m_root.Add(m_container);
        }
    }
}