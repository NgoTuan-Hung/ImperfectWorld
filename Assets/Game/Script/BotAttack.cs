using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BotAttack : Attackable
{
	private float colliderForce = 1f;
	[SerializeField] private float attackRange = 0.5f;
	public enum AttackMode {AttackWhenNear};
	private AttackMode attackMode = AttackMode.AttackWhenNear;

	public float ColliderForce { get => colliderForce; set => colliderForce = value; }

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
		Vector3 targetVector;
		while (true)
		{
			targetVector = customMono.Target.transform.position - transform.position;
			if (CanAttack && targetVector.magnitude < attackRange)
			{
				customMono.BotMovable.MoveSpeed = MoveSpeedReduced;
				ToggleAttackAnim(true);
				CanAttack = false;
				CollideAndDamage attackCollider = attackColliderPool.PickOne().CollideAndDamage;
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