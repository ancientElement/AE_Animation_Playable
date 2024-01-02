using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AE_FSM
{
    public class FSMEditorWindow : EditorWindow
    {
        private VisualElement m_root;
        public VisualElement Root => m_root;

        private float lastHeight;
        private float lastWidth;

        [MenuItem("Tools/AE_FSM/EdrtorWindow")]
        public static void ShowMe()
        {
            GetWindow<FSMEditorWindow>().titleContent = new GUIContent("AE状态机");
            GetWindow<FSMEditorWindow>().Show();
        }

        private void OnEnable()
        {
            wantsMouseEnterLeaveWindow = true;
        }

        public void CreateGUI()
        {
            m_root = GetWindow<FSMEditorWindow>().rootVisualElement;

            StateAreaElement = new ElementLayerIMGUI(m_root);
            StateAreaElement.Root.name = "StateAreaElement";

            ParamsAreaElement = new ElementLayerIMGUI(m_root);
            ParamsAreaElement.Root.name = "ParamsAreaElement";

            ParamResizeAreaElement = new DragButton(m_root);
            ParamResizeAreaElement.Root.name = "ParamResizeAreaElement";

            ParamResizeAreaElement.RegisterDrage(ParamsResizeAreaDragEvent);
            ParamResizeAreaElement.Root.SetCursor(MouseCursor.ResizeHorizontal);

            ResetRect();
            ResetView();

            StateAreaElement.Container.onGUIHandler = StateAreaOnGUI;
            ParamsAreaElement.Container.onGUIHandler = ParamsAreaOnGUI;
        }

        private void Update()
        {
            foreach (var item in graphLayerList)
            {
                item.Update();
            }
        }

        private void OnGUI()
        {
            if (this.position.width != lastWidth || this.position.height != lastHeight)
            {
                lastHeight = this.position.height;
                lastWidth = this.position.width;
                ResetRect();
                ResetView();
            }
        }

        /// <summary>
        /// Paramter区域的OnGUI
        /// </summary>
        private void ParamsAreaOnGUI()
        {
            if (this.Context.RunTimeFSMContorller == null)
                return;

            if (paramterLayer == null)
            {
                paramterLayer = new ParamterLayer(this);
            }
            paramterLayer.OnGUI(ParamsAreaElement.Container.contentRect);
            paramterLayer.ProcessEvent();
        }

        /// <summary>
        /// State区域的OnGUI
        /// </summary>
        private void StateAreaOnGUI()
        {
            if (this.Context.RunTimeFSMContorller == null)
            {
                GUILayout.BeginVertical();
                {
                    var style = new GUIStyle();
                    style.fontSize = 100;
                    style.fontStyle = FontStyle.Bold;
                    style.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("AE_FSM", style);

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("请选择配置文件", GUILayout.Width(100));
                        this.Context.SetFSMController(EditorGUILayout.ObjectField(this.Context.RunTimeFSMContorller, typeof(RunTimeFSMController), true, GUILayout.Width(200)) as RunTimeFSMController);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                return;
            };

            if (graphLayerList.Count == 0)
            {
                InitGraphLayers();
            }

            foreach (var item in graphLayerList)
            {
                item.OnGUI(StateAreaElement.Container.contentRect);
            }

            for (int i = graphLayerList.Count - 1; i >= 0; i--)
            {
                graphLayerList[i].ProcessEvent();
            }
        }

        /// <summary>
        /// 选择事件
        /// </summary>
        private void OnSelectionChange()
        {
            if (Selection.activeObject as RunTimeFSMController != null || Selection.activeObject as GameObject != null)
            {
                Repaint();
            }
        }

        /// <summary>
        /// 初始化层级
        /// </summary>
        private void InitGraphLayers()
        {
            graphLayerList.Add(new BackgraoundLayer(this));
            graphLayerList.Add(new TranslationLayer(this));
            graphLayerList.Add(new StateNodeLayer(this));
        }

        /// <summary>
        /// 所有绘制层
        /// </summary>
        private List<GraphLayers> graphLayerList = new List<GraphLayers>();
        private ParamterLayer paramterLayer;

        /// <summary>
        /// 上下文对象
        /// </summary>
        public Context Context { get; private set; } = new Context();

        #region 总体布局

        private Rect paramsArea;
        public ElementLayerIMGUI StateAreaElement;

        private Rect stateArea;
        public ElementLayerIMGUI ParamsAreaElement;

        private Rect paramResizeArea;
        public DragButton ParamResizeAreaElement;

        private float precent_of_paramsArea = 0.4f;
        private const float ResizeAreaWidth = 10f;

        /// <summary>
        ///  参数区域与状态区域
        /// </summary>
        private void ResetRect()
        {
            paramsArea.Set(0, 0, this.position.width * precent_of_paramsArea - ResizeAreaWidth / 2, this.position.height);
            stateArea.Set(paramsArea.width + ResizeAreaWidth, 0, this.position.width * (1 - precent_of_paramsArea) - ResizeAreaWidth / 2, this.position.height);
            paramResizeArea.Set(paramsArea.width, 0, ResizeAreaWidth, this.position.height);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        private void ResetView()
        {
            StateAreaElement?.ResetView(stateArea, ColorConst.Backgraound_Color);
            ParamsAreaElement?.ResetView(paramsArea, ColorConst.Backgraound_Color);
            ParamResizeAreaElement?.ResetView(paramResizeArea, ColorConst.Backgraound_Color);
        }

        /// <summary>
        /// 参数区域与状态区域的大小拖拽线
        /// </summary>
        private void ParamsResizeAreaDragEvent(MouseMoveEvent evt)
        {
            paramResizeArea.Set(paramsArea.width, 0, ResizeAreaWidth, this.position.height);
            precent_of_paramsArea = Mathf.Clamp(Event.current.mousePosition.x / this.position.width, 0.1f, 0.5f);
            ResetRect();
            ResetView();
            Repaint();
        }

        #endregion 总体布局
    }
}