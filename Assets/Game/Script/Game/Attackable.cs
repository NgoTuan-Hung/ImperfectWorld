
using System;
using UnityEngine;

public class Attackable : BaseAction
{
	GameObject attackColliderPrefab;
	protected static ObjectPool attackColliderPool;
	public GameObject longRangeProjectilePrefab;
	protected static ObjectPool longRangeProjectilePool;
	public GameObject longRangeFirePoint;
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
	}

	public void ToggleAttackAnim(bool value)
	{
		base.ToggleAnim(attackBoolHash, value);
	}
	
	public bool GetAttackBool() => base.GetBool(attackBoolHash); 
}