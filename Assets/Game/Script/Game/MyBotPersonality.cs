using System;
using System.Collections;
using UnityEngine;

public enum MyBotCombatBehaviour {Melee, Ranged}
public class MyBotPersonality : BaseIntelligence
{
	Vector2 targetDirection;
	float distanceToTarget;
	Vector3 targetPosition;
	public float logicalAttackRange = 1f;
	public float targetTooCloseRange = 1f;
	public MyBotCombatBehaviour myBotCombatBehaviour;
	Action combatThinking;
	bool canSetTargetPos = true;
	public override void Awake()
	{
		base.Awake();
		if (myBotCombatBehaviour == MyBotCombatBehaviour.Melee) combatThinking = MeleeThinking;
		else combatThinking = RangedThinking;
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
	
	void Think()
	{
		if (customMono.target == null)
		{
			customMono.TryPickRandomTarget();
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Roam, 1);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 1);
		}
		else
		{
			combatThinking();
		}
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
		customMono.movementIntelligence.ExecuteAnyActionThisFrame(customMono.movementActionInterval, targetDirection, targetPosition);
		customMono.actionIntelligence.ExecuteAnyActionThisFrame(customMono.actionInterval, targetDirection, targetPosition);
	}
	
	void ThinkAboutNumbers()
	{
		if (customMono.target == null)
		{
			targetDirection = default;
			if (canSetTargetPos) targetPosition = default;
		}
		else
		{
			if (canSetTargetPos) 
			{
				targetPosition = customMono.target.transform.position;
				targetDirection = targetPosition - transform.position;
			}
			distanceToTarget = targetDirection.magnitude;	
		}
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
			customMono.actionIntelligence.PreSumActionChance(ActionUse.SummonShortRange, 15);
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
	
	public void ForceUsingAction(ActionUse actionUse, bool targetPositionBool, Vector3 targetPositionParam, float duration)
	{
		StartCoroutine(ForceUsingActionCoroutine(actionUse, targetPositionBool, targetPositionParam, duration));
	}
	
	IEnumerator ForceUsingActionCoroutine(ActionUse actionUse, bool targetPositionBool, Vector3 targetPositionParam, float duration)
	{
		float currentTime = 0;
		if (targetPositionBool)
		{
			canSetTargetPos = false;
			targetPosition = targetPositionParam;
			targetDirection = targetPosition - transform.position;
		}
		
		while (currentTime < duration)
		{
			customMono.actionIntelligence.PreSumActionChance(actionUse, 9999);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime += Time.fixedDeltaTime;
		}
		
		canSetTargetPos = true;
	}
}