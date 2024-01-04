using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class BackgraoundLayer : GraphLayers
    {
        /// <summary>
        /// 小格子的边长
        /// </summary>
        public static float Small_Grid_Length { get; } = 30f;

        /// <summary>
        /// 大格子的边长
        /// </summary>
        public static float Big_Grid_Length { get; } = 300f;

        private Vector2 mousePosition;

        private bool canDragObject;

        public BackgraoundLayer(FSMEditorWindow fSMEditorWindow) : base(fSMEditorWindow)
        {
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rect, ColorConst.Backgraound_Color);
                DrawGrid(rect, Small_Grid_Length, ColorConst.Grid_Color);
                DrawGrid(rect, Big_Grid_Length, ColorConst.Grid_Color);
            }
        }

        /// <summary>
        /// 事件
        /// </summary>
        public override void ProcessEvent()
        {
            base.ProcessEvent();

            //拖拽
            if (Event.current.type == EventType.MouseDrag && Event.current.button == (int)MouseButton.Middle && posotion.Contains(Event.current.mousePosition))
            {
                this.Context.DragOffset += Event.current.delta;
                Event.current.Use();
            }

            //缩放
            if (Event.current.type == EventType.ScrollWheel && posotion.Contains(Event.current.mousePosition))
            {
                //当 f = Event.current.delta.y 为正数或零时，返回值为 1，当 f 为负数时，返回值为 -1。
                this.Context.ZoomFactor -= Mathf.Sign(Event.current.delta.y) / 20f;
                this.Context.ZoomFactor = Mathf.Clamp(this.Context.ZoomFactor, 0.2f, 1f);
                Event.current.Use();
            }

            //取消选中
            if (posotion.IsContainsCurrentMouse() && IsNotClickedNode() && EventUtility.IsMouseDown(0))
            {
                this.Context.ClearAllSelectNode();
                this.Context.StopPriviewTransition();
            }

            //右键菜单
            if (posotion.IsContainsCurrentMouse() && IsNotClickedNode() && EventUtility.IsMouseUp(1) && this.Context.RunTimeFSMContorller != null)
            {
                mousePosition = Event.current.mousePosition;
                CreateMenu();
            }

            //拖拽脚本
            if (Event.current.type == EventType.DragUpdated)
            {
                UnityEngine.Object[] objs = DragAndDrop.objectReferences;
                canDragObject = true;
                foreach (var item in objs)
                {
                    if ((item as MonoScript).GetClass().GetInterfaces().Where(x => x == typeof(IFSMState)).FirstOrDefault() == null)
                    {
                        canDragObject = false;
                    }
                }
                if (canDragObject)
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }

            if (Event.current.type == EventType.DragPerform && canDragObject)
            {
                UnityEngine.Object[] objs = DragAndDrop.objectReferences;
                mousePosition = Event.current.mousePosition;
                foreach (Object item in objs)
                {
                    CreateState((item as MonoScript).GetClass().FullName, item as MonoScript);
                    mousePosition.x += 30;
                    mousePosition.y += 30;
                }
            }
        }

        /// <summary>
        /// 创建右键菜单
        /// </summary>
        private void CreateMenu()
        {
            this.Context.ClearAllSelectNode();

            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create State"), false, () =>
            {
                //TODO:创建状态
                CreateState();
            });
            genericMenu.ShowAsContext();
        }

        /// <summary>
        /// 创建状态
        /// </summary>
        private void CreateState()
        {
            var rect = new Rect(0, 0, FSMConst.stateWidth, FSMConst.stateHeight);
            rect.center = MousePosition(mousePosition);
            FSMStateNodeFactory.CreateFSMNode(this.Context.RunTimeFSMContorller, this.Context.RunTimeFSMContorller.states.Count == 2, rect);
        }

        private void CreateState(string scriptName, MonoScript monoScript)
        {
            var rect = new Rect(0, 0, FSMConst.stateWidth, FSMConst.stateHeight);
            rect.center = MousePosition(mousePosition);
            FSMStateNodeFactory.CreateFSMNode(this.Context.RunTimeFSMContorller, scriptName, monoScript,this.Context.RunTimeFSMContorller.states.Count == 2, rect, scriptName);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="gridSpace"></param>
        /// <param name="color"></param>
        private void DrawGrid(Rect rect, float gridSpace, Color color)
        {
            if (rect.width < gridSpace) { return; }
            if (gridSpace == 0) { return; }
            gridSpace *= this.Context.ZoomFactor;
            DrawHorizontal(rect, gridSpace, color);
            DrawHorizontal(rect, -gridSpace, color, 1);
            DrawVertical(rect, gridSpace, color);
            DrawVertical(rect, -gridSpace, color, 1);
        }

        /// <summary>
        /// 绘制竖线
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="gradSpace"></param>
        /// <param name="color"></param>
        /// <param name="startIndex"></param>
        public void DrawVertical(Rect rect, float gradSpace, Color color, int startIndex = 0)
        {
            Vector2 center = rect.center + this.Context.DragOffset;
            Vector2 start;
            Vector2 end;

            int i = startIndex;

            if (center.x > rect.position.x + rect.width && gradSpace < 0)
            {
                i = Mathf.CeilToInt((center.x - (rect.position.x + rect.width)) / Mathf.Abs(gradSpace));
                //Debug.Log(i);
            }

            if (center.x < rect.position.x && gradSpace > 0)
            {
                i = Mathf.CeilToInt((rect.position.x - center.x) / Mathf.Abs(gradSpace));
                //Debug.Log(i);
            }

            do
            {
                start = new Vector2(center.x + gradSpace * i, rect.center.y - rect.height / 2);
                end = new Vector2(center.x + gradSpace * i, rect.center.y + rect.height / 2);
                if (rect.Contains((start + end) / 2))
                {
                    DrawLine(start, end, color);
                    i++;
                }
            } while (rect.Contains((start + end) / 2));
        }

        /// <summary>
        /// 绘制横线
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="gradSpace"></param>
        /// <param name="color"></param>
        /// <param name="startIndex"></param>
        public void DrawHorizontal(Rect rect, float gradSpace, Color color, int startIndex = 0)
        {
            Vector2 center = rect.center + this.Context.DragOffset;
            Vector2 start;
            Vector2 end;

            int i = startIndex;

            if (center.y > rect.position.y + rect.height && gradSpace < 0)
            {
                i = Mathf.CeilToInt((center.y - (rect.position.y + rect.height)) / Mathf.Abs(gradSpace));
                //Debug.Log(i);
            }

            if (center.y < rect.position.y && gradSpace > 0)
            {
                i = Mathf.CeilToInt((rect.position.y - center.x) / Mathf.Abs(gradSpace));
                //Debug.Log(i);
            }

            do
            {
                start = new Vector2(rect.center.x - rect.width / 2, center.y + gradSpace * i);
                end = new Vector2(rect.center.x + rect.width / 2, center.y + gradSpace * i);
                if (rect.Contains((start + end) / 2))
                {
                    DrawLine(start, end, color);
                    i++;
                }
            } while (rect.Contains((start + end) / 2));
        }

        /// <summary>
        /// 绘制直线
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// </summary>
        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex(start);
            GL.Vertex(end);

            GL.End();
        }
    }
}