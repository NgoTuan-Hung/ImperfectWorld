using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MagicLaserSkill : SkillBase
{
	static ObjectPool magicLaserPool;
	GameObject magicLaserPrefab;

	public override void Awake()
	{
		base.Awake();
		cooldown = 10f;
		damage = defaultDamage = 1f;
		boolHash = Animator.StringToHash("CastingMagic");
		
		magicLaserPrefab = Resources.Load("MagicLaser") as GameObject;
		magicLaserPool ??= new ObjectPool(magicLaserPrefab, 100, new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self));
		AddActionManuals();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}

	
	public override void Start()
	{
		base.Start();
		actionClip = customMono.AnimatorWrapper.GetAnimationClip("CastingMagic");
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => magicLaserPool = null;
		#endif
		
		StatChangeRegister();
		endAnimCallback += () => 
		{
			customMono.actionBlocking = false;
			customMono.animationEventFunctionCaller.endCastingMagic = false;
		};
	}

	public override void StatChangeRegister()
	{
		base.StatChangeRegister();
		customMono.stat.magickaChangeEvent.action += () => 
		{
			// magicka 0 -> 100
			damage = defaultDamage + customMono.stat.Magicka * 0.1f;
		};
	}

	
	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.RangedDamage, (direction, location, nextActionChoosingIntervalProposal) => FireAt(location, nextActionChoosingIntervalProposal), 0.5f));
	}
	
	public override void Trigger(Vector2 location = default, Vector2 direction = default)
	{
		if (canUse && !customMono.actionBlocking)
		{
			canUse = false;
			customMono.actionBlocking = true;
			ToggleAnim(boolHash, true);
			StartCoroutine(TriggerCoroutine(location, direction));
			EndAnimWait(() => customMono.animationEventFunctionCaller.endCastingMagic);
		}
	}
	
	IEnumerator TriggerCoroutine(Vector2 location = default, Vector2 direction = default)
	{
		while(!customMono.animationEventFunctionCaller.castingMagic) yield return new WaitForSeconds(Time.fixedDeltaTime);
		
		customMono.animationEventFunctionCaller.castingMagic = false;
		bool t_animatorLocalScale = customMono.AnimatorWrapper.animator.transform.localScale.x > 0;
		
		GameEffect gameEffect = magicLaserPool.PickOne().gameEffect;
		gameEffect.collideAndDamage.allyTags = customMono.allyTags;
		gameEffect.collideAndDamage.collideDamage = damage;
			
		if (t_animatorLocalScale) gameEffect.transform.SetPositionAndRotation(location - new Vector2(6, 0), Quaternion.Euler(0, 0, 0));
		else gameEffect.transform.SetPositionAndRotation(location + new Vector2(6, 0), Quaternion.Euler(0, 180, 0));
	}
	
	public void FireAt(Vector2 location, float duration)
	{
		StartCoroutine(FireAtCoroutine(location, duration));
	}
	
	IEnumerator FireAtCoroutine(Vector2 location, float duration)
	{
		customMono.actionInterval = true;
		Trigger(location: location);
		yield return new WaitForSeconds(duration);
		
		customMono.actionInterval = false;
	}
}