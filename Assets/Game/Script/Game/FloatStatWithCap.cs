using System;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class FloatStatWithCap
{
    [SerializeField]
    float value;
    public float cap;

    [CreateProperty]
    public float Value
    {
        get { return value; }
        set
        {
            if (value > cap)
                this.value = cap;
            else
                this.value = value;

            valueChangeEvent();
        }
    }
    public Action valueChangeEvent = () => { };
}
