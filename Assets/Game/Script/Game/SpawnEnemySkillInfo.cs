using System;

public class SpawnEnemySkillInfo
{
    public Type skillType;
    public bool unlocked = false;

    public SpawnEnemySkillInfo(string p_skillType)
    {
        skillType = Type.GetType(p_skillType);
    }
}
