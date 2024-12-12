
using System;
using UnityEngine;

public class Attackable : BaseAction
{
	private int attackBoolHash = Animator.StringToHash("Attack");
	private AnimationClip attackClip;
	private bool canAttack = true;
	[SerializeField] private float attackCooldown = 1f;
	private float moveSpeedReduceRate = 0.9f;
	private float moveSpeedReduced;
	public Action changeMoveSpeedReduceRate;
	public AnimationClip AttackClip { get => attackClip; set => attackClip = value; }
	public bool CanAttack { get => canAttack; set => canAttack = value; }
	public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
	public float MoveSpeedReduced { get => moveSpeedReduced; set => moveSpeedReduced = value;}
	public float MoveSpeedReduceRate { get => moveSpeedReduceRate; set 
	{
		moveSpeedReduceRate = value;
		moveSpeedReduced = customMono.BotMovable.DefaultMoveSpeed * moveSpeedReduceRate;
		#if UNITY_EDITOR
		changeMoveSpeedReduceRate?.Invoke();
		#endif
	} }

	public override void Awake() 
	{
		base.Awake();
	}
	
	public virtual void Start() 
	{
		attackClip = customMono.AnimatorWrapper.GetAnimationClip("Attack");	
		moveSpeedReduced = customMono.BotMovable.DefaultMoveSpeed * moveSpeedReduceRate;
	}

	public void ToggleAttackAnim(bool value)
	{
		base.ToggleAnim(attackBoolHash, value);
	}
}