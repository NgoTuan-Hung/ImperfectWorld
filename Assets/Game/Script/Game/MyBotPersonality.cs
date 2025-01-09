using System;
using UnityEngine;

public enum MyBotCombatBehaviour {Melee, Ranged}
public class MyBotPersonality : BaseIntelligence
{
	Vector2 moveVector;
	float distanceToTarget;
	public float logicalAttackRange = 1f;
	public float targetTooCloseRange = 1f;
	public MyBotCombatBehaviour myBotCombatBehaviour;
	Action Think;
	public override void Awake()
	{
		base.Awake();
		if (myBotCombatBehaviour == MyBotCombatBehaviour.Melee) Think = MeleeThinking;
		else Think = RangedThinking;
	}

	public override void Start()
	{
		base.Start();
	}
	
	private void FixedUpdate() 
	{
		Think(); //
		DoAction();
	}
	
	void MeleeThinking()
	{
		ThinkAboutNumbers();
		ThinkAboutDistanceToTarget();
		if (customMono.attackable.onCooldown)
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		}
		
		customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 5);
	}
	
	void DoAction()
	{
		customMono.movementIntelligence.ExecuteAnyActionThisFrame(customMono.movementActionInterval, moveVector, customMono.Target.transform.position);
		customMono.actionIntelligence.ExecuteAnyActionThisFrame(customMono.actionInterval, moveVector, customMono.Target.transform.position);
	}
	
	void ThinkAboutNumbers()
	{
		moveVector = customMono.Target.transform.position - transform.position;
		distanceToTarget = moveVector.magnitude;
	}
	
	void ThinkAboutDistanceToTarget()
	{
		if (distanceToTarget > logicalAttackRange)
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
		}
		else
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.MeleeDamage, 30);
		}
	}
	
	void RangedThinking()
	{
		ThinkAboutNumbers();
		
		customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 5);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 5);	
		
		
		if (customMono.attackable.onCooldown)
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		}
		
		if (distanceToTarget > targetTooCloseRange)
		{
			if (distanceToTarget < logicalAttackRange)
			{
				customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
				customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 5);
				customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 5);
			}
			else 
			{
				customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
				customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
			}
		}
		else
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 30);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 30);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 5);
		}
	}
}