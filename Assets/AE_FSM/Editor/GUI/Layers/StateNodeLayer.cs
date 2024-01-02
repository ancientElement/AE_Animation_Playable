using System;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class StateNodeLayer : GraphLayers
    {
        private StateStyle m_stateStyle = new StateStyle();

      

        private bool isSelecting = false;
        private Vector2 startSelectPosition;
        private Rect selectBox = new Rect();
        private GUIStyle selectBoxStyle = new GUIStyle("SelectionRect");
        private float runStateProcess;
        private Rect runStateProcessRect;
        private GUIStyle runStateProcessBkStyle = new GUIStyle("MeLivePlayBackground");
        private GUIStyle runStateProcessStyle = new GUIStyle("MeLivePlayBar");

        public StateNodeLayer(FSMEditorWindow fSMEditorWindow) : base(fSMEditorWindow)
        {
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (Event.current.type == EventType.Repaint)
            {
                if (Context.RunTimeFSMContorller == null)
                    return;

                m_stateStyle.ApplyZoomFactory(this.Context.ZoomFactor);

                for (int i = 0; i < Context.RunTimeFSMContorller.states.Count; i++)
                {
                    DrawNode(Context.RunTimeFSMContorller.states[i]);
                }
            }
        }

        /// <summary>
        /// 事件
        /// </summary>
        public override void ProcessEvent()
        {
            base.ProcessEvent();

            if (this.Context.RunTimeFSMContorller == null)
                return;

            #region 选中

            foreach (FSMStateNodeData item in this.Context.RunTimeFSMContorller.states)
            {
                CheckNodeClick(item);
            }

            #endregion 

            #region 框选

            if (EventUtility.IsMouseDown(0) && IsNotClickedNode())
            {
                isSelecting = true;
                startSelectPosition = Event.current.mousePosition;
            }

            if (EventUtility.IsMouseUp(0))
            {
                if (isSelecting)
                {
                    isSelecting = false;
                    DrawSelectBox();
                    this.FSMEditorWindow.Repaint();
                }
            }

            //移除窗口
            if (Event.current.type == EventType.MouseLeaveWindow) { isSelecting = false; }

            DrawSelectBox();

            #endregion 框选

            #region 拖拽Node

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                if (!isSelecting)
                {
                    foreach (FSMStateNodeData item in this.Context.SelectNodes)
                    {
                        item.rect.position += Event.current.delta / this.Context.ZoomFactor;
                        EditorUtility.SetDirty(this.Context.RunTimeFSMContorller);
                    }
                }
                Event.current.Use();
            }

            #endregion 拖拽

            #region 右键菜单

            if (EventUtility.IsMouseUp(1))
            {
                foreach (var item in this.Context.RunTimeFSMContorller.states)
                {
                    if (GetTransfromRect(item.rect).IsContainsCurrentMouse())
                    {
                        CreateMenu(item);
                        Event.current.Use();
                    }
                }
            }

            #endregion 右键菜单

            #region 删除

            if (Event.current.keyCode == KeyCode.Delete && this.Context.SelectNodes != null && this.Context.SelectNodes.Count > 0)
            {
                foreach (var item in this.Context.SelectNodes)
                {
                    FSMStateNodeFactory.DeleteFSMNode(this.Context.RunTimeFSMContorller, item);
                }
                this.FSMEditorWindow.Repaint();
            }

            #endregion

        }

        public override void Update()
        {
            if (Application.isPlaying && this.Context.FSMController != null && this.Context.FSMController.currentState != null)
            {
                runStateProcess += Time.deltaTime;
                runStateProcess %= 1;
            }
        }

        /// <summary>
        /// 右键菜单
        /// </summary>
        private void CreateMenu(FSMStateNodeData item)
        {
            bool is_any = item.name == FSMConst.anyState;
            bool is_enter = item.name == FSMConst.enterState;
            var genericMenu = new GenericMenu();

            if (is_enter)
            {
                genericMenu.AddItem(new GUIContent("Make Transition"), false, null);
            }
            else if (is_any)
            {
                genericMenu.AddItem(new GUIContent("Make Transition"), false, () =>
                {
                    //TOOD:
                    this.Context.StartPriviewTransition(item);
                });
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Make Transition"), false, () =>
                {
                    //TOOD:
                    this.Context.StartPriviewTransition(item);
                });
                genericMenu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    //TOOD:删除状态
                    DeleteNode();
                });
                genericMenu.AddItem(new GUIContent("Set DefaltState"), false, () =>
                {
                    //TOOD:
                    SetDefaultState(item);
                });
            }
            genericMenu.ShowAsContext();
        }

        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="state"></param>
        private void SetDefaultState(FSMStateNodeData state)
        {
            if (state.defualtState == true) return;
            foreach (FSMStateNodeData item in this.Context.RunTimeFSMContorller.states)
            {
                item.defualtState = false;
            }
            state.defualtState = true;
            this.Context.RunTimeFSMContorller.Save();
        }

        /// <summary>
        /// 删除状态
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeleteNode()
        {
            foreach (FSMStateNodeData item in this.Context.SelectNodes)
            {
                FSMStateNodeFactory.DeleteFSMNode(this.Context.RunTimeFSMContorller, item);
            }
        }

        /// <summary>
        /// 检测选中
        /// </summary>
        /// <param name="nodeData"></param>
        private void CheckNodeClick(FSMStateNodeData nodeData)
        {
            if (GetTransfromRect(nodeData.rect).IsContainsCurrentMouse())
            {
                if (EventUtility.IsMouseDown(0) || EventUtility.IsMouseDown(1))
                {
                    this.Context.SelectTransition = null;

                    if (this.Context.SelectNodes.Contains(nodeData))
                        return;

                    this.Context.SelectNodes.Clear();
                    this.Context.SelectNodes.Add(nodeData);
                    FSMStateInspectorHelper.Instance.Inspector(this.Context.RunTimeFSMContorller, nodeData);

                    //是否在预览添加过渡
                    if (this.Context.isPriviewingTransilation)
                    {
                        //添加过渡
                        FSMTranslationFactory.CreateTransition(this.Context.RunTimeFSMContorller, this.Context.fromState.name, nodeData.name);
                        this.Context.StopPriviewTransition();
                    }

                    Event.current.Use();
                }
            }
        }

        /// <summary>
        /// 绘制节点
        /// </summary>
        /// <param name="nodeData"></param>
        private void DrawNode(FSMStateNodeData nodeData)
        {
            if (nodeData != null)
            {
                Rect rect = GetTransfromRect(nodeData.rect);
                if (!posotion.Overlaps(rect))
                    return;
                GUI.Box(rect, nodeData.name, GetStateStyle(nodeData));
                if (Application.isPlaying && this.Context.FSMController != null && this.Context.FSMController.currentState != null && this.Context.FSMController.currentState.stateNodeData.name == nodeData.name)
                {
                    runStateProcessRect.Set(rect.x, rect.y + rect.height * 3 / 4, rect.width, rect.height / 4);
                    GUI.Box(runStateProcessRect, string.Empty, runStateProcessBkStyle);
                    runStateProcessRect.Set(rect.x, rect.y + rect.height * 3 / 4, rect.width, rect.height / 4);
                    runStateProcessRect.width *= runStateProcess;
                    GUI.Box(runStateProcessRect, string.Empty, runStateProcessStyle);
                    this.FSMEditorWindow.Repaint();
                }
            }
        }

        /// <summary>
        /// 获取节点样式
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        private GUIStyle GetStateStyle(FSMStateNodeData nodeData)
        {
            bool isSelect = Context.SelectNodes.Contains(nodeData);

            if (Application.isPlaying && this.Context.FSMController != null && this.Context.FSMController.currentState != null && this.Context.FSMController.currentState.stateNodeData.name == nodeData.name)
            {
                return m_stateStyle.GetStyle(isSelect ? Styles.OrangeOn : Styles.Orange);
            }
            else if (!Application.isPlaying && nodeData.defualtState)
            {
                return m_stateStyle.GetStyle(isSelect ? Styles.OrangeOn : Styles.Orange);
            }
            else if (nodeData.name == FSMConst.enterState)
            {
                return m_stateStyle.GetStyle(isSelect ? Styles.GreenOn : Styles.Green);
            }
            else if (nodeData.name == FSMConst.anyState)
            {
                return m_stateStyle.GetStyle(isSelect ? Styles.MiutOn : Styles.Miut);
            }
            else
            {
                return m_stateStyle.GetStyle(isSelect ? Styles.NormalOn : Styles.Normal);
            }
        }

        /// <summary>
        /// 绘制选择框
        /// </summary>
        private void DrawSelectBox()
        {
            if (!isSelecting) { selectBox = Rect.zero; return; }

            Vector2 detal = Event.current.mousePosition - startSelectPosition;
            selectBox.center = startSelectPosition + (detal / 2);
            selectBox.width = Mathf.Abs(detal.x);
            selectBox.height = Mathf.Abs(detal.y);

            GUI.Button(selectBox, "", selectBoxStyle);

            this.Context.ClearAllSelectNode();

            foreach (FSMStateNodeData item in Context.RunTimeFSMContorller.states)
            {
                CheckSelectBoxSelectNode(item);
            }
        }

        /// <summary>
        /// 检查选择框框选
        /// </summary>
        /// <param name="nodeData"></param>
        private void CheckSelectBoxSelectNode(FSMStateNodeData nodeData)
        {
            if (GetTransfromRect(nodeData.rect).Overlaps(selectBox, true))
            {
                this.Context.SelectNodes.Add(nodeData);
            }
        }
    }
}