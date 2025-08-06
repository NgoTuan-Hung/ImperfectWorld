using System;

public enum FloatStatModifierType
{
    Additive,
    Multiplicative,
}

[Serializable]
public class FloatStatModifier
{
    public float value;
    public FloatStatModifierType type;

    public FloatStatModifier(float value, FloatStatModifierType type)
    {
        this.value = value;
        this.type = type;
    }
}
