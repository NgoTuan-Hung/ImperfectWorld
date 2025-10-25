using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Final = (Base + Additive) * (1 + Multiplicative)
/// </summary>
[Serializable]
public class FloatStatWithModifier
{
    [SerializeField]
    float baseValue;

    [SerializeField]
    private float finalValue;
    public float FinalValue
    {
        get => finalValue;
    }

    public float BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            RecalculateFinalValue();
        }
    }

    /// <summary>
    /// Modifiers which are meant to be changed
    /// </summary>
    public List<FloatStatModifier> referenceModifiers = new();

    /// <summary>
    /// Modifiers which should be static/unchanged
    /// </summary>
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
}
