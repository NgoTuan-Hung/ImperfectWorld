using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class DashSkill : SkillBase
{
	static ObjectPool dashEffectPool;
	GameObject dashEffectPrefab;
	public int totalEffect = 10;
	public float dashAmmountPerFrame = 0.5f;
	public float effectLifeTime = 0.5f;
	public float spawnEffectInterval;
	public override void Awake()
	{
		base.Awake();
		duration = 1f;
		cooldown = 8f;
		
		dashEffectPrefab = Resources.Load("DashEffect") as GameObject;
		dashEffectPool ??= new ObjectPool(dashEffectPrefab, 100, new PoolArgument(typeof(GameEffect), PoolArgument.WhereComponent.Self));
		spawnEffectInterval = duration / totalEffect;
		AddActionManuals();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}
	
	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.GetCloser, (direction, location, nextActionChoosingIntervalProposal) => DashTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.GetAway, (direction, location, nextActionChoosingIntervalProposal) => DashTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, -1));
		botActionManuals.Add(new BotActionManual(ActionUse.Dodge, (direction, location, nextActionChoosingIntervalProposal) => DashTo(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), nextActionChoosingIntervalProposal), 0.5f));
	}

	
	public override void Start()
	{
		base.Start();
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => dashEffectPool = null;
		#endif
	}

	public override void Trigger(Touch touch = default, Vector2 location = default, Vector2 direction = default)
	{
		if (canUse && !customMono.movementActionBlocking)
		{
			canUse = false;
			List<PoolObject> poolObjects = dashEffectPool.PickAndPlace(totalEffect, effectActiveLocation);
			StartCoroutine(Dashing(poolObjects, direction));
			StartCoroutine(CooldownCoroutine());
		}
	}
	
	public IEnumerator Dashing(List<PoolObject> poolObjects, Vector3 direction)
	{
		GameEffect gameEffect;
		direction = direction.normalized * dashAmmountPerFrame;
		customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
		for (int i = 0; i < poolObjects.Count; i++)
		{
			gameEffect = poolObjects[i].gameEffect;
			gameEffect.spriteRenderer.sprite = customMono.SpriteRenderer.sprite;
			gameEffect.spriteRenderer.transform.localScale = customMono.SpriteRenderer.transform.localScale;
			gameEffect.transform.position = customMono.transform.position;
			gameEffect.DeactivateAfterTime(effectLifeTime);
			
			transform.position += direction;
			StartCoroutine(DashEffectFadeout(gameEffect));
			
			yield return new WaitForSeconds(spawnEffectInterval);
		}
	}
	
	public IEnumerator DashEffectFadeout(GameEffect gameEffect)
	{
		float currentTime = effectLifeTime;
		do 
		{
			gameEffect.spriteRenderer.color = new Color(1, 1, 1, currentTime / effectLifeTime);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime -= Time.fixedDeltaTime;
		}
		while (currentTime >= 0);
	}
	
	public void DashTo(Vector2 direction, float duration)
	{
		StartCoroutine(DashToCoroutine(direction, duration));
	}
	
	IEnumerator DashToCoroutine(Vector2 direction, float duration)
	{
		customMono.actionInterval = true;
		Trigger(direction: direction);
		yield return new WaitForSeconds(duration);
		
		customMono.actionInterval = false;
	}
}