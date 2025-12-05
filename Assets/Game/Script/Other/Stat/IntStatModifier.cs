using System;

[Serializable]
public class IntStatModifier
{
    public int value;
    public ModifierType type;
    public ModifierLiveTime liveTime = ModifierLiveTime.Temporary;

    public IntStatModifier(int value, ModifierType type)
    {
        this.value = value;
        this.type = type;
    }

    public IntStatModifier(int value, ModifierType type, ModifierLiveTime liveTime)
    {
        this.value = value;
        this.type = type;
        this.liveTime = liveTime;
    }
}
