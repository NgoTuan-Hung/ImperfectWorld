using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Final = (Base + Additive) * (1 + Multiplicative)
/// </summary>
[Serializable]
public class FloatStatWithModifier : INotifyBindablePropertyChanged
{
    [SerializeField]
    float baseValue;

    [SerializeField]
    private float finalValue;
    public float FinalValue
    {
        get => finalValue;
    }

    [CreateProperty]
    public float BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            RecalculateFinalValue();
            Notify();
        }
    }

    public List<FloatStatModifier> referenceModifiers = new();
    public List<FloatStatModifier> modifiers = new();
    public Action finalValueChangeEvent = () => { };

    public void AddModifier(FloatStatModifier modifier)
    {
        modifiers.Add(modifier);
        RecalculateFinalValue();
    }

    public void RemoveModifier(FloatStatModifier modifier)
    {
        modifiers.Remove(modifier);
        RecalculateFinalValue();
    }

    public void RecalculateFinalValue()
    {
        finalValue =
            (
                BaseValue
                + modifiers.Where(m => m.type == FloatStatModifierType.Additive).Sum(m => m.value)
            )
            * (
                1
                + modifiers
                    .Where(m => m.type == FloatStatModifierType.Multiplicative)
                    .Sum(m => m.value)
            );
        finalValueChangeEvent();
    }

    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}
