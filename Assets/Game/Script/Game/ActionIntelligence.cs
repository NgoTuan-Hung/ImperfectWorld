using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum MyAction {MeleeAttack, RangedAttack, NoAttack}
public class ActionIntelligence : BaseIntelligence
{	
	[SerializeField] private float attackRange = 0.5f;
	public enum AttackMode {AttackWhenNear, LongRangeAttack};
	public AttackMode attackMode = AttackMode.AttackWhenNear;

	public void ChangeMode(AttackMode mode)
	{
		switch (mode)
		{
			default:
				break;
		}
	}

	public override void InitAction()
	{
		actionChances = new int[3];
		actionCumulativeDistribution = new float[actionChances.Length];
		actions = new ActionDelegate[actionChances.Length];
		
		actions[0] = customMono.attackable.MeleeAttack;
		actions[1] = customMono.attackable.RangedAttack;
		actions[2] = (vector) => {return;};
	}
	
	public override void Awake() 
	{
		base.Awake();
	}
	
	public override void Start() 
	{
		base.Start();
		InitAction();
	}
}