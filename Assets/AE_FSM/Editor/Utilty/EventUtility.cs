using UnityEngine;

namespace AE_FSM
{
    public static class EventUtility
    {
        /// <summary>
        /// 鼠标在rect范围类
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsContainsCurrentMouse(this Rect rect)
        {
            return (rect.Contains(Event.current.mousePosition));
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseDown(int button = -1)
        {
            if (button == -1)
            {
                return Event.current.type == EventType.MouseDown;
            }
            else if (button == 0 || button == 1 || button == 2)
            {
                return Event.current.button == button && Event.current.type == EventType.MouseDown;
            }
            return false;
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseUp(int button = -1)
        {
            if (button == -1)
            {
                return Event.current.type == EventType.MouseUp;
            }
            else if (button == 0 || button == 1 || button == 2)
            {
                return Event.current.button == button && Event.current.type == EventType.MouseUp;
            }
            return false;
        }
    }
}