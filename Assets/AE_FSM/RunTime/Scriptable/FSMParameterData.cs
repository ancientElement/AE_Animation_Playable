using System;

namespace AE_FSM
{
    public enum ParamterType
    {
        Float = 0,
        Int,
        Bool
    }

    [Serializable]
    public class FSMParameterData
    {
        public string name;
        public float value;
        public ParamterType paramterType;
        public Action onValueChage;

        public float Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    onValueChage?.Invoke();
                }
            }
        }
    }
}
