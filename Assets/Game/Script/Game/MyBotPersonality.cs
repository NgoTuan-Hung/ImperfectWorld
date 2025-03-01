using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyBotCombatBehaviour {Melee, Ranged}
public enum ModificationPriority {VeryLow = 4, Low = 3, Medium = 2, High = 1, VeryHigh = 0}
public class MyBotPersonality : CustomMonoPal
{
	public Vector2 targetDirection;
	List<Vector2> targetDirections;
	public float distanceToTarget;
	public Vector3 targetPosition;
	List<Vector3> targetPositions;
	public float logicalAttackRange = 1f;
	public float targetTooCloseRange = 1f;
	public MyBotCombatBehaviour myBotCombatBehaviour;
	Action combatThinking;
	/* Target enemy is the enemy we are currently seeing and targeting, detect enemy is the unknown
	enemy we detected from the radar (GameManager) and not yet been seen. */
	CustomMono targetEnemy, detectEnemy;
	int priorityLength;
	Action forceUsingAction = () => {};
	public PausableScript pausableScript = new();
	public override void Awake()
	{
		base.Awake();
		if (myBotCombatBehaviour == MyBotCombatBehaviour.Melee) combatThinking = MeleeThinking;
		else combatThinking = RangedThinking;
		
		targetDirections = new List<Vector2>();
		targetPositions = new List<Vector3>();
		
		priorityLength = Enum.GetNames(typeof(ModificationPriority)).Length;
		for (int i=0;i<priorityLength;i++)
		{
			targetDirections.Add(Vector2.zero);
			targetPositions.Add(Vector3.zero);
		}
		
		customMono.someOneExitView += (person) => 
		{
			if (targetEnemy != null)
			{
				if (targetEnemy.Equals(person))
				{
					targetEnemy = null;
					SetTargetDirection(Vector2.zero, ModificationPriority.VeryLow);
					SetTargetPosition(Vector3.zero, ModificationPriority.VeryLow);
				}
			}
		};
		
		customMono.nearestEnemyChanged += (person) => targetEnemy = person;
		
		pausableScript.resumeFixedUpdate = () => pausableScript.fixedUpdate = DoFixedUpdate;
		pausableScript.pauseFixedUpdate = () => pausableScript.fixedUpdate = () => {};
		pausableScript.resumeFixedUpdate();
	}
	
	private void OnEnable() 
	{
		targetDirection = Vector2.zero;
		targetPosition = Vector3.zero;
	}

	public override void Start()
	{
		base.Start();
	}
	
	private void FixedUpdate() 
	{
		pausableScript.fixedUpdate();
	}
	
	void DoFixedUpdate()
	{
	    ThinkAndPrepare();
		DoAction();
	}
	
	void ResetField()
	{
		for (int i=0;i<targetPositions.Count;i++) targetPositions[i] = Vector3.zero;
		for (int i=0;i<targetDirections.Count;i++) targetDirections[i] = Vector2.zero;
	}
	
	public void SetTargetDirection(Vector2 direction, ModificationPriority priority)
	{
		targetDirections[(int)priority] = direction;
	}
	
	public void SetTargetPosition(Vector3 position, ModificationPriority priority)
	{
		targetPositions[(int)priority] = position;
	}
	
	void SetFinalField()
	{	
		for (int i=0;i<targetPositions.Count;i++)
		{
			if (targetPositions[i] != Vector3.zero)
			{
				targetPosition = targetPositions[i];
				break;
			}
		}
		
		for (int i=0;i<targetDirections.Count;i++)
		{
			if (targetDirections[i] != Vector2.zero)
			{
				targetDirection = targetDirections[i];
				break;
			}
		}
	}
	
	void ThinkAndPrepare()
	{
		ResetField();
		forceUsingAction();
		if (targetEnemy == null)
		{
			if (detectEnemy == null) detectEnemy = GameManager.Instance.GetRandomPlayerAlly();
			SetTargetDirection(detectEnemy.transform.position - transform.position, ModificationPriority.VeryLow);
		}
		else
		{
			ThinkAboutNumbers();
		}
		
		SetFinalField();
		
		if (targetEnemy == null)
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Roam, 1);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 1);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
		}
		else
		{
			combatThinking();
		}
	}
	
	void MeleeThinking()
	{
		if (distanceToTarget > logicalAttackRange)
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
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
			customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 5);
		}
		
		// if (customMono.attackable.onCooldown)
		// {
		// 	customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		// 	customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		// }
		
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
		SetTargetPosition(targetEnemy.transform.position, ModificationPriority.VeryLow);
		SetTargetDirection(targetEnemy.transform.position - transform.position, ModificationPriority.VeryLow);
		
		distanceToTarget = targetDirections[(int)ModificationPriority.VeryLow].magnitude;
	}
	
	void RangedThinking()
	{	
		customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
		customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
		customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 1);	
		
		
		// if (customMono.attackable.onCooldown)
		// {
		// 	customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
		// 	customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		// 	customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
		// 	customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
		// }
		
		if (distanceToTarget > targetTooCloseRange)
		{
			if (distanceToTarget < logicalAttackRange)
			{
				customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
				customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
				customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
			}
			else 
			{
				customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
				customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
				customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
			}
		}
		else
		{
			customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
			customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
			customMono.actionIntelligence.PreSumActionChance(ActionUse.PushAway, 10);
		}
	}
	
	public void ForceUsingAction(ActionUse actionUse, Vector3 targetPositionParam, float duration)
	{
		Action t_forceUsingAction = () => 
		{
			SetTargetPosition(targetPositionParam, ModificationPriority.VeryHigh);
			SetTargetDirection(targetPositionParam - transform.position, ModificationPriority.VeryHigh);
			customMono.actionIntelligence.PreSumActionChance(actionUse, 9999);
		};
		
		forceUsingAction += t_forceUsingAction;
		StartCoroutine(ForceUsingActionCoroutine(t_forceUsingAction, duration));
	}
	
	IEnumerator ForceUsingActionCoroutine(Action action, float duration)
	{
		yield return new WaitForSeconds(duration);
		
		forceUsingAction -= action;
	}
}