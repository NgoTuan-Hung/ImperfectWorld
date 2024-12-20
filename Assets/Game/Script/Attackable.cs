
using System;
using UnityEngine;

public class Attackable : BaseAction
{
	GameObject attackColliderPrefab;
	protected static ObjectPool attackColliderPool;
	private int attackBoolHash = Animator.StringToHash("Attack");
	private AnimationClip attackClip;
	protected bool canAttack = true;
	[SerializeField] private float attackCooldown = 1f;
	protected float moveSpeedReduceRate = 0.1f;
	protected float moveSpeedReduced;
	public float colliderForce = 1f;
	public bool CanAttack
	{
		get {return canAttack;}
	}
	
	public AnimationClip AttackClip { get => attackClip; set => attackClip = value; }
	public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
	public float MoveSpeedReduced { get => moveSpeedReduced;}

	public override void Awake() 
	{
		base.Awake();
		attackColliderPrefab = Resources.Load("AttackCollider") as GameObject;
		attackColliderPool ??= new ObjectPool(attackColliderPrefab, 100, new PoolArgument(typeof(CollideAndDamage), PoolArgument.WhereComponent.Self));
	}
	
	public override void Start()
	{
		base.Start();
		attackClip = customMono.AnimatorWrapper.GetAnimationClip("Attack");	
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => attackColliderPool = null;
		#endif
	}

	public void ToggleAttackAnim(bool value)
	{
		base.ToggleAnim(attackBoolHash, value);
	}
}