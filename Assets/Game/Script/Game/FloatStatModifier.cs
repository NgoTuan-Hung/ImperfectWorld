using System;

public enum FloatStatModifierType
{
    Additive,
    Multiplicative,
}

public enum FloatStatModifierLiveTime
{
    Permanent,
    Temporary,
}

[Serializable]
public class FloatStatModifier
{
    public float value;
    public FloatStatModifierType type;
    public FloatStatModifierLiveTime liveTime = FloatStatModifierLiveTime.Temporary;

    public FloatStatModifier(float value, FloatStatModifierType type)
    {
        this.value = value;
        this.type = type;
    }

    public FloatStatModifier(
        float value,
        FloatStatModifierType type,
        FloatStatModifierLiveTime liveTime
    )
    {
        this.value = value;
        this.type = type;
        this.liveTime = liveTime;
    }
}
