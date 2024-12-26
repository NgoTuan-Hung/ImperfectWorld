using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill : BaseAction
{
	public List<SkillDataSo> skillDatas = new List<SkillDataSo>();

	public override void Awake()
	{
		base.Awake();
	}

    public override void Start()
    {
        if (!customMono.isBot) InitSkill();
    }
    
	void InitSkill()
	{
		skillDatas.ForEach(skillData => 
		{
			SkillBase skill = gameObject.AddComponent(Type.GetType(skillData.skillName)) as SkillBase;
			GameUIManager.Instance.MainView.AddSkillToScrollView(skillData, skill.Trigger);
		});
	}		
}