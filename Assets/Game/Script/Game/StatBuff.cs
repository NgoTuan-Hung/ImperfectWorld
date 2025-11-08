using System;

public enum StatBuffType
{
    Health,
    Mana,
    Damage,
}

[Serializable]
public class StatBuff
{
    public StatBuffType statBuffType;
    public FloatStatModifier modifier;
}
