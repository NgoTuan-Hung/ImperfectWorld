using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BotAttack : Attackable
{
	[SerializeField] private float attackRange = 0.5f;
	public enum AttackMode {AttackWhenNear};
	private AttackMode attackMode = AttackMode.AttackWhenNear;
	
	public void ChangeMode(AttackMode mode)
	{
		switch (mode)
		{
			case AttackMode.AttackWhenNear:
				attackMode = AttackMode.AttackWhenNear;
				HandleAttackWhenNear();
				break;
			default:
				break;
		}
	}
	
	public void HandleAttackWhenNear()
	{
		StartCoroutine(AttackWhenNearCoroutine());
	}
	
	IEnumerator AttackWhenNearCoroutine()
	{
		while (true)
		{
			if (CanAttack && Vector3.Distance(transform.position, customMono.Target.transform.position) < attackRange)
			{
				customMono.BotMovable.MoveSpeed = MoveSpeedReduced;
				ToggleAttackAnim(true);
				CanAttack = false;
			}
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	
	public override void Awake() 
	{
		base.Awake();
	}
	
	public override void Start() 
	{
		base.Start();
		customMono.AnimationEventFunctionCaller.resetAttackAction += ResetAttack;
		customMono.AnimatorWrapper.AddAnimationEvent(AttackClip, "ResetAttack", AnimatorWrapper.AddAnimationEventMode.End);
		ChangeMode(attackMode);	
	}
	
	public void ResetAttack()
	{
		ToggleAttackAnim(false);
		customMono.BotMovable.SetDefaultMoveSpeed();
		StartCoroutine(ResetAttackCoroutine());
	}
	
	public IEnumerator ResetAttackCoroutine()
	{
		yield return new WaitForSeconds(AttackCooldown);
		CanAttack = true;
	}
}