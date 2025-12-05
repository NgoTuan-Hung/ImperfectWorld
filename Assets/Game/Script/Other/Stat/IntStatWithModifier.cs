using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Final = (Base + Additive) * (1 + Multiplicative)
/// </summary>
[Serializable]
public class IntStatWithModifier
{
    [SerializeField]
    int baseValue;

    [SerializeField]
    private int finalValue;
    public int FinalValue
    {
        get => finalValue;
    }

    public int BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            RecalculateFinalValue();
        }
    }

    public List<IntStatModifier> modifiers = new();
    public Action finalValueChangeEvent = () => { };

    public void AddModifier(IntStatModifier modifier)
    {
        modifiers.Add(modifier);
        RecalculateFinalValue();
    }

    public void RemoveModifier(IntStatModifier modifier)
    {
        modifiers.Remove(modifier);
        RecalculateFinalValue();
    }

    public void RecalculateFinalValue()
    {
        finalValue =
            (BaseValue + modifiers.Where(m => m.type == ModifierType.Additive).Sum(m => m.value))
            * (1 + modifiers.Where(m => m.type == ModifierType.Multiplicative).Sum(m => m.value));
        finalValueChangeEvent();
    }

    /// <summary>
    /// Remove all modifiers which are not permanent
    /// </summary>
    public void ClearModifiers()
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != ModifierLiveTime.Permanent)
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
            if (modifiers[i].liveTime != ModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
    }

    /// <summary>
    /// Remove all modifiers which are not permanent and reset the base value
    /// </summary>
    /// <param name="resetBaseValue"></param>
    public void ClearModifiers(int resetBaseValue)
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != ModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
        baseValue = resetBaseValue;
        RecalculateFinalValue();
    }

    public void ClearModifiersWithoutRecalculate(int resetBaseValue)
    {
        for (int i = 0; i < modifiers.Count; i++)
            if (modifiers[i].liveTime != ModifierLiveTime.Permanent)
                modifiers.RemoveAt(i);
        baseValue = resetBaseValue;
    }
}
