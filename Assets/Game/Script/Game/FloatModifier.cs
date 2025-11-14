using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class FloatModifier
{
    /// <summary>
    /// Modifiers which are meant to be changed
    /// </summary>
    public List<FloatStatModifier> referenceModifiers = new();

    /// <summary>
    /// Modifiers which should be static/unchanged
    /// </summary>
    public List<FloatStatModifier> modifiers = new();

    public void AddModifier(FloatStatModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(FloatStatModifier modifier)
    {
        modifiers.Remove(modifier);
    }
}
