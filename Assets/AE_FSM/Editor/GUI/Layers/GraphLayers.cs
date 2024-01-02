using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class GraphLayers
    {
        protected Rect posotion;

        public FSMEditorWindow FSMEditorWindow { get; private set; }

        public Context Context => FSMEditorWindow.Context;

        /// <summary>
        /// 初始化
        /// </summary>
        public GraphLayers(EditorWindow fSMEditorWindow)
        {
            FSMEditorWindow = (FSMEditorWindow)fSMEditorWindow;
            //m_elementRoot = fSMEditorWindow.StateAreaElement;
            //FSMEditorWindow.Root.Add(m_elementRoot.Root);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public virtual void OnGUI(Rect rect)
        {
            posotion = rect;
            UpdateTranslateMatrix();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public virtual void ProcessEvent()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
        }

        private Matrix4x4 transfromMatrix = Matrix4x4.identity;

        /// <summary>
        /// 更新缩放矩阵
        /// </summary>
        public void UpdateTranslateMatrix()
        {
            var centerMat = Matrix4x4.Translate(-posotion.center);
            var translationMat = Matrix4x4.Translate(this.Context.DragOffset / this.Context.ZoomFactor);
            var scaleMat = Matrix4x4.Scale(Vector3.one * this.Context.ZoomFactor);

            this.transfromMatrix = centerMat.inverse * scaleMat * translationMat * centerMat;
        }

        /// <summary>
        ///屏幕位置转缩放位置缩放后的Rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rect GetTransfromRect(Rect rect)
        {
            Rect resulte = new Rect();
            resulte.position = transfromMatrix.MultiplyPoint(rect.position);
            resulte.size = transfromMatrix.MultiplyVector(rect.size);

            return resulte;
        }

        /// <summary>
        /// 点到了Node没有?
        /// </summary>
        /// <returns></returns>
        public bool IsNotClickedNode()
        {
            if (this.Context.RunTimeFSMContorller != null)
                foreach (var item in this.Context.RunTimeFSMContorller.states)
                {
                    if (GetTransfromRect(item.rect).IsContainsCurrentMouse())
                    {
                        return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// 屏幕鼠标位置转缩放位置缩放后位置
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector2 MousePosition(Vector2 mousePosition)
        {
            Vector2 center = mousePosition + (mousePosition - this.posotion.center) * (1 - this.Context.ZoomFactor) / this.Context.ZoomFactor;
            center -= this.Context.DragOffset / this.Context.ZoomFactor;
            return center;
        }
    }
}