using System;

[Serializable]
public class FloatStatModifier
{
    public float value;
    public ModifierType type;
    public ModifierLiveTime liveTime = ModifierLiveTime.Temporary;

    public FloatStatModifier()
    {
        value = 0;
        type = ModifierType.Additive;
        liveTime = ModifierLiveTime.Temporary;
    }

    public FloatStatModifier(float value, ModifierType type)
    {
        this.value = value;
        this.type = type;
    }

    public FloatStatModifier(float value, ModifierType type, ModifierLiveTime liveTime)
    {
        this.value = value;
        this.type = type;
        this.liveTime = liveTime;
    }
}
