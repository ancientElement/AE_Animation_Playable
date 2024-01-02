using UnityEngine;

namespace AAE_FSM
{
    public static class DebugHelper
    {
        public static void Print(this object me, string objName, object obj)
        {
            Debug.Log($"{objName} <color=yellow>###</color> {obj}");
        }
    }
}