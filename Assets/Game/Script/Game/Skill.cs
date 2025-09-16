using System;
using System.Collections.Generic;
using System.Linq;

public class Skill : CustomMonoPal
{
    public List<SkillDataSO> skillDataSOs = new();
    public List<SkillBase> skillBases = new();
    public List<SkillData> skillDatas = new();

    public override void Awake()
    {
        base.Awake();
        skillDataSOs.ForEach(skillDataSO =>
        {
            SkillBase skill =
                gameObject.AddComponent(Type.GetType(skillDataSO.skillName)) as SkillBase;
            skillBases.Add(skill);
        });

        customMono.startPhase1 += () => InitSkill();
    }

    public override void Start() { }

    void InitSkill()
    {
        for (int i = 0; i < skillDataSOs.Count; i++)
        {
            customMono.actionIntelligence.AddManuals(skillBases[i].botActionManuals);
        }

        if (customMono.isControllable)
        {
            for (int i = 0; i < skillDataSOs.Count; i++)
            {
                skillDatas.Add(new(skillDataSOs[i], skillBases[i]));
            }

            // GameUIManager.Instance.charInfoView.PopulateSkillTree
            // (
            // 	GameManager.Instance.GetCharData(customMono).individualView, skillDatas
            // );
        }
    }

    public void HandleSkillUnlock(List<SpawnEnemySkillInfo> spawnEnemySkillInfos)
    {
        BaseAction t_skill;
        spawnEnemySkillInfos.ForEach(sEKI =>
        {
            if (sEKI.unlocked)
            {
                t_skill = skillBases.FirstOrDefault(sB => sB.GetType().Equals(sEKI.skillType));
                if (!t_skill.unlocked)
                    UnlockSkill(t_skill);
            }
        });
    }

    public void UnlockSkill(BaseAction p_skill)
    {
        p_skill.Unlock();
        customMono.actionIntelligence.AddManuals(p_skill.botActionManuals);
    }
}
