using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BotAttack : Attackable
{
	
	[SerializeField] private float attackRange = 0.5f;
	public enum AttackMode {AttackWhenNear, LongRangeAttack};
	public AttackMode attackMode = AttackMode.AttackWhenNear;

	public void ChangeMode(AttackMode mode)
	{
		switch (mode)
		{
			case AttackMode.AttackWhenNear:
				attackMode = AttackMode.AttackWhenNear;
				HandleAttackWhenNear();
				break;
			case AttackMode.LongRangeAttack:
				attackMode = AttackMode.LongRangeAttack;
				HandleLongRangeAttack();
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
		Vector3 targetVector;
		while (true)
		{
			targetVector = customMono.Target.transform.position - transform.position;
			if (CanAttack && targetVector.magnitude < attackRange)
			{
				customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
				ToggleAttackAnim(true);
				canAttack = false;
				CollideAndDamage attackCollider = attackColliderPool.PickOne().collideAndDamage;
				attackCollider.AlliesTag = customMono.AlliesTag;
				attackCollider.transform.position = transform.position;
				attackCollider.Rigidbody2D.AddForce
				(
					targetVector.normalized * colliderForce,
					ForceMode2D.Impulse
				);
			}
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	
	void HandleLongRangeAttack()
	{
		StartCoroutine(LongRangeAttackCoroutine());
	}
	
	IEnumerator LongRangeAttackCoroutine()
	{
		Vector3 targetVector, projectileVector;
		while (true)
		{
			targetVector = customMono.Target.transform.position - transform.position;
			if (canAttack && targetVector.magnitude < attackRange)
			{
				customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
				ToggleAttackAnim(true);
				canAttack = false;
				GameEffect projectileEffect = longRangeProjectilePool.PickOne().gameEffect;
				projectileEffect.collideAndDamage.AlliesTag = customMono.AlliesTag;
				projectileEffect.transform.position = longRangeFirePoint.transform.position;
				projectileEffect.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle
				(
					Vector2.right,
					projectileVector = customMono.Target.transform.position - projectileEffect.transform.position
				));
				
				projectileEffect.KeepFlyingAt(projectileVector);
			}

			while (GetAttackBool())
			{
				customMono.SetUpdateDirectionIndicator(targetVector, UpdateDirectionIndicatorPriority.Low);
				yield return new WaitForSeconds(Time.fixedDeltaTime);
				targetVector = customMono.Target.transform.position - transform.position;
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
		customMono.stat.SetDefaultMoveSpeed();
		StartCoroutine(ResetAttackCoroutine());
	}
	
	public IEnumerator ResetAttackCoroutine()
	{
		yield return new WaitForSeconds(attackCooldown);
		canAttack = true;
	}
}