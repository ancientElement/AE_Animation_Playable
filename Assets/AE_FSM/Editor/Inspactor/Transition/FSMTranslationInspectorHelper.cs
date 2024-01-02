using System;
using UnityEditor;

namespace AE_FSM
{
    public class FSMTranslationInspectorHelper : ScriptableObjectSingleton<FSMTranslationInspectorHelper>
    {
        public RunTimeFSMController contorller;
        public FSMTranslationData translationData;
        private Action<FSMTranslationData> m_callback;

        public void Inspector(RunTimeFSMController contorller, FSMTranslationData translationData)
        {
            this.contorller = contorller;
            this.translationData = translationData;
            Selection.activeObject = this;
            m_callback?.Invoke(translationData);
        }

        public void AddSelectAction(Action<FSMTranslationData> action)
        {
            m_callback += action;
        }
    }
}