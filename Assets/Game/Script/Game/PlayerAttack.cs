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
		customMono.AnimationEventFunctionCaller.resetAttackAction += ResetAttack;
		customMono.AnimatorWrapper.AddAnimationEvent(AttackClip, "ResetAttack", AnimatorWrapper.AddAnimationEventMode.End);
		GameUIManager.Instance.MainView.holdAttackButtonEvent = (vector2) => 
		{
			if (canAttack)
			{
				canAttack = false;
				ToggleAttackAnim(true);
				customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
				CollideAndDamage attackCollider = attackColliderPool.PickOne().collideAndDamage;
				attackCollider.AlliesTag = customMono.AlliesTag;
				attackCollider.transform.position = transform.position;
				attackCollider.Rigidbody2D.AddForce
				(
					vector2.normalized * colliderForce,
					ForceMode2D.Impulse
				);
			}
			
		};
	}

	public void ResetAttack()
	{
		ToggleAttackAnim(false);
		customMono.stat.SetDefaultMoveSpeed();
		StartCoroutine(ResetAttackCoroutine());
	}
	
	public IEnumerator ResetAttackCoroutine()
	{
		yield return new WaitForSeconds(attackCooldown);
		canAttack = true;
	}
}