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
		cooldown = 2f;
		skillUses.Add(SkillUse.Dodge); skillUses.Add(SkillUse.MoveAway); skillUses.Add(SkillUse.GetCloser);
		
		dashEffectPrefab = Resources.Load("DashEffect") as GameObject;
		dashEffectPool ??= new ObjectPool(dashEffectPrefab, 100, new PoolArgument(typeof(GameEffect), PoolArgument.WhereComponent.Self));
		spawnEffectInterval = duration / totalEffect;
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
		if (canUse)
		{
			canUse = false;
			List<PoolObject> poolObjects = dashEffectPool.PickAndPlace(totalEffect, effectActiveLocation);
			StartCoroutine(Dashing(poolObjects, direction));
			StartCoroutine(DashCooldownCoroutine());
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
		gameEffect.gameObject.SetActive(false);
	}
	
	public IEnumerator DashCooldownCoroutine()
	{
		yield return new WaitForSeconds(cooldown);
		canUse = true;
	}
}