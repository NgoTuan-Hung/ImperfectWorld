using System;
using System.Collections.Generic;
using System.Linq;

public class Skill : CustomMonoPal
{
    public List<SkillDataSO> skillDataSOs = new();
    public List<SkillBase> skillBases = new();
    public Attack attack;
    public SkillBase mainSkill;

    public override void Awake()
    {
        base.Awake();
        skillDataSOs.ForEach(skillDataSO =>
        {
            SkillBase skill =
                gameObject.AddComponent(Type.GetType(skillDataSO.skillName)) as SkillBase;
            skillBases.Add(skill);
        });

        GetSkillAndAttack();
    }

    void GetSkillAndAttack()
    {
        attack = skillBases[0] as Attack;
        if (skillBases.Count > 1)
        {
            mainSkill = skillBases[1];
        }
    }
}
