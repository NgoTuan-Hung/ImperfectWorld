using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum SkillUse {Dodge, MoveAway, GetCloser, BetterUseIfTargetIsStunned, LongRange, DealDamage}
public class SkillBase : BaseAction
{
	public bool canUse = true;
	public float cooldown;
	public float duration;
	public float maxRange;
	public Vector3 effectActiveLocation = new Vector3(0, 999, 0);
	public HashSet<SkillUse> skillUses = new HashSet<SkillUse>();
	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}
	
	public virtual void Trigger(Touch touch = default, Vector2 location = default, Vector2 direction = default)
	{
		
	}
}