using System;

[Serializable]
public class FloatStatWithCap
{
    float value;
    public float cap;
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
