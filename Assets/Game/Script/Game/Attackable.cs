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
	public float colliderForce = 5f;
	public Action<Vector2> Attack;
	public AttackType attackType;

	public override void Awake() 
	{
		base.Awake();
		boolHash = Animator.StringToHash("Attack");
		cooldown = defaultCooldown = defaultStateSpeed = 1f;
		if (attackType == AttackType.Melee) 
		{
			Attack = MeleeAttack;
			damage = defaultDamage = 15f;
			attackColliderPrefab = Resources.Load("AttackCollider") as GameObject;
			attackColliderPool ??= new ObjectPool(attackColliderPrefab, 100, new PoolArgument(ComponentType.CollideAndDamage, PoolArgument.WhereComponent.Self));
		}
		else 
		{
			Attack = RangedAttack;
			damage = defaultDamage = 10f;
			longRangeProjectilePool ??= new ObjectPool(longRangeProjectilePrefab, 100, new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self));
		}
		
		AddActionManuals();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}
	
	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.MeleeDamage, (direction, location, nextActionChoosingIntervalProposal) => AttackTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.RangedDamage, (direction, location, nextActionChoosingIntervalProposal) => AttackTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.Passive, (direction, location, nextActionChoosingIntervalProposal) => Idle(direction, nextActionChoosingIntervalProposal), 0.5f));
		botActionManuals.Add(new(ActionUse.Roam, (direction, location, nextActionChoosingIntervalProposal) => Idle(direction, nextActionChoosingIntervalProposal), 1f));
	}
	
	public override void Start()
	{
		base.Start();
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => 
		{
			attackColliderPool = null;
			longRangeProjectilePool = null;
		};
		#endif
		
		StatChangeRegister();
		endAnimCallback += () => 
		{
			customMono.stat.SetDefaultMoveSpeed();
			customMono.actionBlocking = false;
			customMono.animationEventFunctionCaller.endAttack = false;
		};
	}

	public override void StatChangeRegister()
	{
		base.StatChangeRegister();
		/* Attack speed change => animator_state_speed *= attack_speed
							   => attack_cooldown /= attack_speed */
		customMono.stat.attackSpeedChangeEvent.action += () => 
		{
			customMono.AnimatorWrapper.animator.SetFloat("AttackAnimSpeed", defaultStateSpeed * customMono.stat.AttackSpeed);
			cooldown = defaultCooldown / customMono.stat.AttackSpeed;
			botActionManuals[0].nextActionChoosingIntervalProposal = 0.5f / customMono.stat.AttackSpeed;
			botActionManuals[1].nextActionChoosingIntervalProposal = 0.5f / customMono.stat.AttackSpeed;
			botActionManuals[2].nextActionChoosingIntervalProposal = 0.5f / customMono.stat.AttackSpeed;
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
		customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
		customMono.audioSource.PlayOneShot(audioClip);
		customMono.SetUpdateDirectionIndicator(attackDirection, UpdateDirectionIndicatorPriority.Low);
		CollideAndDamage attackCollider = attackColliderPool.PickOne().collideAndDamage;
		attackCollider.allyTags = customMono.allyTags;
		attackCollider.transform.position = customMono.firePoint.transform.position;
		attackCollider.collideDamage = damage;
		attackCollider.rigidbody2D.AddForce
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
	}
	
	IEnumerator RangedAttackCoroutine(Vector2 attackDirection)
	{
		while (!customMono.animationEventFunctionCaller.attack) yield return new WaitForSeconds(Time.fixedDeltaTime);
		
		customMono.animationEventFunctionCaller.attack = false;
		customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
		customMono.audioSource.PlayOneShot(audioClip);
		customMono.SetUpdateDirectionIndicator(attackDirection, UpdateDirectionIndicatorPriority.Low);
		GameEffect projectileEffect = longRangeProjectilePool.PickOne().gameEffect;
		projectileEffect.collideAndDamage.allyTags = customMono.allyTags;
		projectileEffect.collideAndDamage.collideDamage = damage;
		projectileEffect.transform.position = customMono.firePoint.transform.position;
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