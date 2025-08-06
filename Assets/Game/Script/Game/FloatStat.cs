using System;

[Serializable]
public class FloatStat
{
    float value;
    public float Value
    {
        get { return value; }
        set
        {
            this.value = value;
            valueChangeEvent();
        }
    }
    public Action valueChangeEvent = () => { };
}
