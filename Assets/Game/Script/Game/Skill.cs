using System;
using System.Collections.Generic;

public class Skill : BaseAction
{
	public List<SkillDataSO> skillDataSOs = new();
	public List<SkillData> skillDatas = new();

	public override void Awake()
	{
		base.Awake();
		skillDataSOs.ForEach(skillDataSO => 
		{
			SkillBase skill = gameObject.AddComponent(Type.GetType(skillDataSO.skillName)) as SkillBase;
			customMono.actionIntelligence.AddManuals(skill.botActionManuals);
		});

		customMono.startPhase1 += () => InitSkill();
	}

	public override void Start()
	{
	}
	
	void InitSkill()
	{
		if (!customMono.isBot)
		{
			// GameUIManager.Instance.mainView.AddSkillToScrollView
			// (
			// 	skillData,
			// 	(touch, direction) => skill.Trigger(touch, direction: direction),
			// 	skill.StartAndWait,
			// 	skill.WhileWaiting
			// );
			skillDataSOs.ForEach(skillDataSO => skillDatas.Add(new(skillDataSO)));
			GameUIManager.Instance.charInfoView.PopulateSkillTree(GameManager.Instance.GetCharData(customMono).individualView, skillDatas);
		}

		
	}		
}