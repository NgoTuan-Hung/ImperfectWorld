using System;
using System.Collections.Generic;

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
			SkillBase skill = gameObject.AddComponent(Type.GetType(skillDataSO.skillName)) as SkillBase;
			skillBases.Add(skill);
		});

		customMono.startPhase1 += () => InitSkill();
	}

	public override void Start()
	{
	}
	
	void InitSkill()
	{
		for (int i=0;i<skillDataSOs.Count;i++)
		{
			customMono.actionIntelligence.AddManuals(skillBases[i].botActionManuals);
		}
	
		if (customMono.isControllable)
		{
			for (int i=0;i<skillDataSOs.Count;i++)
			{
			    customMono.actionIntelligence.AddManuals(skillBases[i].botActionManuals);
				skillDatas.Add(new(skillDataSOs[i], skillBases[i]));
			} 

			GameUIManager.Instance.charInfoView.PopulateSkillTree
			(
				GameManager.Instance.GetCharData(customMono).individualView, skillDatas
			);
		}
	}		
}