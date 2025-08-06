using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Final = (Base + Additive) * (1 + Multiplicative)
/// </summary>
[Serializable]
public class FloatStatWithModifier
{
    public float baseValue;
    float finalValue;
    public float FinalValue
    {
        get => finalValue;
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
                baseValue
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
}
