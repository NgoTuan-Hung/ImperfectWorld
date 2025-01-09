using System;
using System.Collections;
using UnityEngine;

public enum AttackType {Melee, Ranged}
public class Attackable : BaseAction
{
	GameObject attackColliderPrefab;
	protected static ObjectPool attackColliderPool;
	public GameObject longRangeProjectilePrefab;
	protected static ObjectPool longRangeProjectilePool;
	public GameObject longRangeFirePoint;
	Vector3 projectileVector;
	public float colliderForce = 1f;
	public Action<Vector2> Attack;
	public AttackType attackType;

	public override void Awake() 
	{
		base.Awake();
		boolHash = Animator.StringToHash("Attack");
		attackColliderPrefab = Resources.Load("AttackCollider") as GameObject;
		attackColliderPool ??= new ObjectPool(attackColliderPrefab, 100, new PoolArgument(typeof(CollideAndDamage), PoolArgument.WhereComponent.Self));
		if (longRangeProjectilePrefab  != null)
			longRangeProjectilePool ??= new ObjectPool(longRangeProjectilePrefab, 100, new PoolArgument(typeof(GameEffect), PoolArgument.WhereComponent.Self));
		
		cooldown = defaultCooldown = defaultStateSpeed = 1f;
		if (attackType == AttackType.Melee) Attack = MeleeAttack;
		else Attack = RangedAttack;
		
		AddActionManuals();
	}

	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.MeleeDamage, (direction, location) => AttackTo(direction, 0.5f), true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.RangedDamage, (direction, location) => AttackTo(direction, 0.5f), true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.Passive, (direction, location) => Idle(direction, 0.5f)));
	}
	
	public override void Start()
	{
		base.Start();
		actionClip = customMono.AnimatorWrapper.GetAnimationClip("Attack");
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
			customMono.AnimatorWrapper.animator.SetFloat("AttackAnimSpeed", defaultStateSpeed * customMono.stat.AttackSpeed);
			cooldown = defaultCooldown / customMono.stat.AttackSpeed;
		};
		
		endAnimCallback += () => 
		{
			customMono.stat.SetDefaultMoveSpeed();
			customMono.actionBlocking = false;
			customMono.animationEventFunctionCaller.endAttack = false;
		};
	}
	
	public void MeleeAttack(Vector2 attackDirection)
	{
		if (canUse && !customMono.actionBlocking) 
		{
			canUse = false;
			customMono.actionBlocking = true;
			ToggleAnim(boolHash, true);
			StartCoroutine(MeleeAttackCoroutine(attackDirection));
			EndAnimWait(() => customMono.animationEventFunctionCaller.endAttack);
		}
	}
	
	IEnumerator MeleeAttackCoroutine(Vector2 attackDirection)
	{
		while (!customMono.animationEventFunctionCaller.attack) yield return new WaitForSeconds(Time.fixedDeltaTime);

		customMono.animationEventFunctionCaller.attack = false;
		customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
		customMono.SetUpdateDirectionIndicator(attackDirection, UpdateDirectionIndicatorPriority.Low);
		CollideAndDamage attackCollider = attackColliderPool.PickOne().collideAndDamage;
		attackCollider.AlliesTag = customMono.AlliesTag;
		attackCollider.transform.position = transform.position;
		attackCollider.Rigidbody2D.AddForce
		(
			attackDirection.normalized * colliderForce,
			ForceMode2D.Impulse
		);
	}
	
	public void RangedAttack(Vector2 attackDirection)
	{
		if (canUse && !customMono.actionBlocking)
		{
			canUse = false;
			customMono.actionBlocking = true;
			ToggleAnim(boolHash, true);
			StartCoroutine(RangedAttackCoroutine(attackDirection));
			EndAnimWait(() => customMono.animationEventFunctionCaller.endAttack);
		}
		
		// while (GetAttackBool())
		// {
		// 	customMono.SetUpdateDirectionIndicator(targetVector, UpdateDirectionIndicatorPriority.Low);
		// 	yield return new WaitForSeconds(Time.fixedDeltaTime);
		// 	targetVector = customMono.Target.transform.position - transform.position;
		// }
	}
	
	IEnumerator RangedAttackCoroutine(Vector2 attackDirection)
	{
		while (!customMono.animationEventFunctionCaller.attack) yield return new WaitForSeconds(Time.fixedDeltaTime);
		
		customMono.animationEventFunctionCaller.attack = false;
		customMono.stat.MoveSpeed = customMono.stat.attackMoveSpeedReduced;
		customMono.SetUpdateDirectionIndicator(attackDirection, UpdateDirectionIndicatorPriority.Low);
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
	
	public void AttackTo(Vector2 direction, float duration)
	{
		StartCoroutine(AttackToCoroutine(direction, duration));
	}
	
	IEnumerator AttackToCoroutine(Vector2 direction, float duration)
	{
		customMono.actionInterval = true;
		Attack(direction);
		yield return new WaitForSeconds(duration);
		
		customMono.actionInterval = false;
	}
	
	public void Idle(Vector2 direction, float duration)
	{
		StartCoroutine(IdleCoroutine(direction, duration));
	}
	
	IEnumerator IdleCoroutine(Vector2 direction, float duration)
	{
		customMono.actionInterval = true;
		yield return new WaitForSeconds(duration);
		
		customMono.actionInterval = false;
	}
}