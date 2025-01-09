using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
		AddManuals(customMono.attackable.botActionManuals);
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