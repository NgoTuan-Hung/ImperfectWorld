using System;

public enum StatBuffType
{
    HP,
    MP,
    MIGHT,
    REFLEX,
    WISDOM,
    ATK,
    ASPD,
    ARMOR,
    HPREGEN,
    MPREGEN,
    MSPD,
    DMGMOD,
    OMNIVAMP,
    CRIT,
    CRITMOD,
    DMGREDUC,
    ATKRANGE,
}

[Serializable]
public class StatBuff
{
    public StatBuffType statBuffType;
    public FloatStatModifier modifier;
}
