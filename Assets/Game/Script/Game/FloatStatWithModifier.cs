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

    /// <summary>
    /// Remove all modifiers which are not permanent
    /// </summary>
    public void ClearModifiers()
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != FloatStatModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
        RecalculateFinalValue();
    }

    /// <summary>
    /// Call this if you want to do something after clearing modifiers
    /// and then recalculate yourself.
    /// </summary>
    /// <param name="resetBaseValue"></param>
    public void ClearModifiersWithoutRecalculate()
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != FloatStatModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
    }

    /// <summary>
    /// Remove all modifiers which are not permanent and reset the base value
    /// </summary>
    /// <param name="resetBaseValue"></param>
    public void ClearModifiers(float resetBaseValue)
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != FloatStatModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
        baseValue = resetBaseValue;
        RecalculateFinalValue();
    }

    public void ClearModifiersWithoutRecalculate(float resetBaseValue)
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != FloatStatModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
        baseValue = resetBaseValue;
    }
}
