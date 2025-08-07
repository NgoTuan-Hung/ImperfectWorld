using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

[Serializable]
public class FloatStatWithCap : INotifyBindablePropertyChanged
{
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
            Notify();
        }
    }
    public Action valueChangeEvent = () => { };

    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}
