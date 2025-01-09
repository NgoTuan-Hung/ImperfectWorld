using System;
using System.Collections.Generic;

public class Skill : BaseAction
{
	public List<SkillDataSo> skillDatas = new List<SkillDataSo>();

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		InitSkill();
	}
	
	void InitSkill()
	{
		skillDatas.ForEach(skillData => 
		{
			SkillBase skill = gameObject.AddComponent(Type.GetType(skillData.skillName)) as SkillBase;
			customMono.actionIntelligence.AddManuals(skill.botActionManuals);

			if (!customMono.isBot)
			{
				GameUIManager.Instance.MainView.AddSkillToScrollView(skillData,
				(touch, direction) => skill.Trigger(touch, direction: direction));
			}
		});
	}		
}