using System;

public enum StatBuffType
{
    Health,
    Mana,
    Might,
    Reflex,
    Wisdom,
    Damage,
    AttackSpeed,
    Armor,
}

[Serializable]
public class StatBuff
{
    public StatBuffType statBuffType;
    public FloatStatModifier modifier;
}
