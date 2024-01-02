using System;
using System.Collections.Generic;

namespace AE_FSM
{
    [Serializable]
    public class FSMTranslationData
    {
        public string fromState;

        public string toState;

        public List<FSMConditionData> conditions;
    }
}