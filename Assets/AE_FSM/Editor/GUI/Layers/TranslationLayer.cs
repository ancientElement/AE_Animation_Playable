using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

namespace AE_FSM
{
    public class TranslationLayer : GraphLayers
    {
        private bool addSelectAction;

        public TranslationLayer(FSMEditorWindow fSMEditorWindow) : base(fSMEditorWindow)
        {
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            if (!addSelectAction)
            {
                FSMTranslationInspectorHelper.Instance.AddSelectAction((item) =>
                {
                    this.Context.SelectTransition = item;
                    addSelectAction = true;
                });
            }

            FSMStateNodeData defualtState = this.Context.RunTimeFSMContorller.states.Where(x => x.defualtState).FirstOrDefault();
            FSMStateNodeData enterState = this.Context.RunTimeFSMContorller.states.Where(x => x.name == FSMConst.enterState).FirstOrDefault();

            //默认状态
            DrawTransition(enterState, defualtState, Color.yellow);

            //其他状态
            foreach (FSMStateNodeData item_state in this.Context.RunTimeFSMContorller.states)
            {
                foreach (FSMTranslationData item_transition in item_state.trasitions)
                {
                    DrawTransition(item_transition.fromState, item_transition.toState, item_transition == this.Context.SelectTransition ? ColorConst.Select_Color : Color.white);
                }
            }

            //绘制预览
            if (this.Context.isPriviewingTransilation)
            {
                if (this.Context.hoverState == null || this.Context.hoverState.name == FSMConst.enterState || this.Context.hoverState.name == FSMConst.anyState)
                {
                    DrawTransition(GetTransfromRect(this.Context.fromState.rect).center, Event.current.mousePosition, Color.white);
                }
                else
                {
                    DrawTransition(GetTransfromRect(this.Context.fromState.rect).center, this.Context.hoverState.rect.center, Color.white);
                }
                this.FSMEditorWindow.Repaint();
            }
        }

        public override void ProcessEvent()
        {
            CheckTransitionClick();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                if (this.Context.SelectTransition != null)
                    FSMTranslationFactory.DeleteTransition(this.Context.RunTimeFSMContorller, this.Context.SelectTransition);
            }
        }

        /// <summary>
        /// 检测点击过渡线
        /// </summary>
        private void CheckTransitionClick()
        {
            if (EventUtility.IsMouseUp(0))
                foreach (FSMStateNodeData state in this.Context.RunTimeFSMContorller.states)
                {
                    foreach (FSMTranslationData item in state.trasitions)
                    {
                        FSMStateNodeData fromSatteData = this.Context.RunTimeFSMContorller.states.Where(x => x.name == item.fromState).FirstOrDefault();
                        FSMStateNodeData toStateData = this.Context.RunTimeFSMContorller.states.Where(x => x.name == item.toState).FirstOrDefault();

                        Rect fromRect = GetTransfromRect(fromSatteData.rect);
                        Rect toRect = GetTransfromRect(toStateData.rect);

                        Vector2 offset = GetTransitionOffset(fromRect.center, toRect.center);
                        Vector2 fromPos = fromRect.center + offset;
                        Vector2 toPos = toRect.center + offset;

                        float width = Mathf.Clamp(Mathf.Abs(toPos.x - fromPos.x), 10f, Mathf.Abs(toPos.x - fromPos.x));
                        float height = Mathf.Clamp(Mathf.Abs(toPos.y - fromPos.y), 10f, Mathf.Abs(toPos.y - fromPos.y));
                        Rect rect = new Rect(0, 0, width, height);
                        rect.center = fromPos + (toPos - fromPos) * 0.5f;

                        if (rect.IsContainsCurrentMouse())
                        {
                            if (GetMinDistanceToLine(fromPos, toPos, Event.current.mousePosition))
                            {
                                ShowInspactor(item);
                                //this.Context.SelectTransition = item;
                                Event.current.Use();
                                break;
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// 显示Inspector
        /// </summary>
        /// <param name="translationData"></param>
        private void ShowInspactor(FSMTranslationData translationData)
        {
            FSMTranslationInspectorHelper.Instance.Inspector(this.Context.RunTimeFSMContorller, translationData);
        }

        /// <summary>
        /// 鼠标点击到线的距离是否成立
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool GetMinDistanceToLine(Vector2 start, Vector2 end, Vector2 point)
        {
            Vector2 direction = end - start;
            Vector2 start2point = point - start;

            Vector2 projectDir = start2point.magnitude * Vector2.Dot(direction.normalized, start2point.normalized) * direction.normalized;
            Vector2 pointProject = start + projectDir;
            float distance = Vector3.Distance(pointProject, point);

            if (distance < 5)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 绘制带三角方向的线 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="isShowArrow"></param>
        private void DrawTransition(string start, string end, Color color, bool isShowArrow = true)
        {
            FSMStateNodeData fromState = this.Context.RunTimeFSMContorller.states.Where(x => x.name == start).FirstOrDefault();
            FSMStateNodeData toState = this.Context.RunTimeFSMContorller.states.Where(x => x.name == end).FirstOrDefault();

            DrawTransition(fromState, toState, color, isShowArrow, 0);
        }

        /// <summary>
        /// 绘制带三角方向的线 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="isShowArrow"></param>
        private void DrawTransition(FSMStateNodeData start, FSMStateNodeData end, Color color, bool isShowArrow = true, int ArrowIndex = 0)
        {
            if (start == null || end == null) return;

            Rect startRect = GetTransfromRect(start.rect);
            Rect endRect = GetTransfromRect(end.rect);

            Vector2 offset = GetTransitionOffset(startRect.center, endRect.center);

            if (this.posotion.Contains(startRect.center + offset) ||
                this.posotion.Contains(endRect.center + offset) ||
                this.posotion.Contains((endRect.center - startRect.center) * 0.5f + startRect.center + offset))
            {
                DrawTransition(startRect.center + offset, endRect.center + offset, color, isShowArrow, ArrowIndex);
            }
        }

        /// <summary>
        /// 获取偏移使得相反方向不重叠
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="traget"></param>
        /// <returns></returns>
        private Vector2 GetTransitionOffset(Vector2 origin, Vector2 traget)
        {
            Vector2 direction = traget - origin;

            Vector2 offset = Vector2.zero;

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                //上下
                offset.x += direction.y < 0 ? 10 : -10;
            }
            else
            {
                //左右
                offset.y += direction.x < 0 ? 10 : -10;
            }

            return offset * this.Context.ZoomFactor;
        }

        /// <summary>
        /// 绘制带三角方向的线 实际绘制
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="isShowArrow"></param>
        private void DrawTransition(Vector2 start, Vector2 end, Color color, bool isShowArrow = true, int ArrowIndex = 0)
        {
            Handles.BeginGUI();

            Handles.color = color;
            Handles.DrawAAPolyLine(5, start, end);

            if (isShowArrow)
            {
                Vector2 direction = end - start;

                Vector2 lineCenter = start + (direction / 2) * (1 + ArrowIndex * 30);

                Vector2 cross = Vector3.Cross(direction, Vector3.forward);

                Vector3[] triangles = new Vector3[]
                {
                lineCenter + cross.normalized * 5,
                lineCenter - cross.normalized * 5,
                lineCenter + (direction).normalized * 10
                };

                Handles.DrawAAConvexPolygon(triangles);
            }

            Handles.EndGUI();
        }
    }
}