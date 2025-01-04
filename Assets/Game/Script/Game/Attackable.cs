
using System;
using System.Collections;
using UnityEngine;

public class Attackable : BaseAction
{
	GameObject attackColliderPrefab;
	protected static ObjectPool attackColliderPool;
	public GameObject longRangeProjectilePrefab;
	protected static ObjectPool longRangeProjectilePool;
	public GameObject longRangeFirePoint;
	Vector3 projectileVector;
	private int attackBoolHash = Animator.StringToHash("Attack");
	private AnimationClip attackClip;
	protected bool canAttack = true;
	protected float attackCooldown = 1f;
	float defaultAttackCooldown = 1f;
	float defaultAttackStateSpeed = 1f;
	public float colliderForce = 1f;
	public bool CanAttack
	{
		get {return canAttack;}
	}
	public AnimationClip AttackClip { get => attackClip; set => attackClip = value; }

	public override void Awake() 
	{
		base.Awake();
		attackColliderPrefab = Resources.Load("AttackCollider") as GameObject;
		attackColliderPool ??= new ObjectPool(attackColliderPrefab, 100, new PoolArgument(typeof(CollideAndDamage), PoolArgument.WhereComponent.Self));
		if (longRangeProjectilePrefab  != null)
			longRangeProjectilePool ??= new ObjectPool(longRangeProjectilePrefab, 100, new PoolArgument(typeof(GameEffect), PoolArgument.WhereComponent.Self));
	}
	
	public override void Start()
	{
		base.Start();
		attackClip = customMono.AnimatorWrapper.GetAnimationClip("Attack");	
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => 
		{
			attackColliderPool = null;
			longRangeProjectilePool = null;
		};
		#endif
		
		/* Attack speed change => animator_state_speed *= attack_speed
							   => attack_cooldown /= attack_speed */
		customMono.stat.attackSpeedChangeEvent.action += () => 
		{
			customMono.AnimatorWrapper.animator.SetFloat("AttackAnimSpeed", defaultAttackStateSpeed * customMono.stat.AttackSpeed);
			attackCooldown = defaultAttackCooldown / customMono.stat.AttackSpeed;
		};
		
		customMono.AnimationEventFunctionCaller.resetAttackAction += ResetAttack;
		customMono.AnimatorWrapper.AddAnimationEvent(AttackClip, "ResetAttack", AnimatorWrapper.AddAnimationEventMode.End);
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

	public void ToggleAttackAnim(bool value)
	{
		base.ToggleAnim(attackBoolHash, value);
	}
	
	public bool GetAttackBool() => base.GetBool(attackBoolHash);
	
	public void MeleeAttack(Vector2 attackDirection)
	{
		if (CanAttack)
		{
			customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
			ToggleAttackAnim(true);
			canAttack = false;
			CollideAndDamage attackCollider = attackColliderPool.PickOne().collideAndDamage;
			attackCollider.AlliesTag = customMono.AlliesTag;
			attackCollider.transform.position = transform.position;
			attackCollider.Rigidbody2D.AddForce
			(
				attackDirection.normalized * colliderForce,
				ForceMode2D.Impulse
			);
		}
	}
	
	public void RangedAttack(Vector2 attackDirection)
	{
		if (canAttack)
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
				attackDirection
			));
			
			projectileEffect.KeepFlyingAt(attackDirection);
		}
		
		// while (GetAttackBool())
		// {
		// 	customMono.SetUpdateDirectionIndicator(targetVector, UpdateDirectionIndicatorPriority.Low);
		// 	yield return new WaitForSeconds(Time.fixedDeltaTime);
		// 	targetVector = customMono.Target.transform.position - transform.position;
		// }
	}
}