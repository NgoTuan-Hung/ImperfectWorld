using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : Attackable
{
	public override void Awake() 
	{
		base.Awake();	
	}

	public override void Start()
	{
		base.Start();
		GameUIManager.Instance.MainView.holdAttackButtonEvent = (vector2) => MeleeAttack(vector2);
	}
}