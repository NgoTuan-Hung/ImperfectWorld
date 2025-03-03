using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Slaughter : SkillBase
{
	GameObject projectilePrefab;
	static ObjectPool projectilePool;
	Vector3 projectileUpDir;
	public override void Awake()
	{
		base.Awake();
		boolHash = Animator.StringToHash("Slaughter");
		audioClip = Resources.Load<AudioClip>("AudioClip/slaughter");
		cooldown = defaultCooldown = 1f;
		damage = 5f;
		maxAmmo = 10;
		
		projectilePrefab = Resources.Load("SlaughterProjectile") as GameObject;
		projectilePool ??= new ObjectPool(projectilePrefab, 100, new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self));
		
		AddActionManuals();
	}
	
	IEnumerator RefillAmmo()
	{
		while (true)
		{
			if (currentAmmo < maxAmmo)
			{
				yield return new WaitForSeconds(cooldown);
				AddAmmo(1);
			}
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

    public override void AddAmmo(int ammount)
    {
        base.AddAmmo(ammount);
        if (currentAmmo > 0) botActionManuals[0].actionChanceAjuster = 100;
        else botActionManuals[0].actionChanceAjuster = 0;
    }

    
	public override void OnEnable()
	{
		base.OnEnable();
		currentAmmo = maxAmmo;
		StartCoroutine(RefillAmmo());
	}

	public override void Start()
	{
		base.Start();
		
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => 
		{
			projectilePool = null;
		};
		#endif
		
		endAnimCallback += () => 
		{
			customMono.stat.SetDefaultMoveSpeed();
			customMono.actionBlocking = false;
			customMono.animationEventFunctionCaller.endSlaughter = false;
		};
	}

	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new
		(
			ActionUse.RangedDamage
			, DoAuto
			, 0
		));
	}

	/* The logic of this ability is we can fire projectile whenever we have ammo,
	if there are no ammo, we can't fire, if ammo isn't full, we will reload it
	automatically (RefillAmmo coroutine). */
	public override void Trigger(Vector2 location = default, Vector2 direction = default)
	{
		if (canUse && !customMono.actionBlocking && currentAmmo > 0)
		{
			canUse = false;
			customMono.actionBlocking = true;
			customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
			customMono.audioSource.PlayOneShot(audioClip);
			AddAmmo(-1);
			ToggleAnim(boolHash, true);
			StartCoroutine(EndAnimWaitCoroutine());
			
			customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
			GameEffect projectileEffect = projectilePool.PickOne().gameEffect;
			projectileEffect.collideAndDamage.allyTags = customMono.allyTags;
			projectileEffect.collideAndDamage.collideDamage = damage;
			projectileEffect.transform.position = customMono.firePoint.transform.position;
			projectileEffect.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle
			(
				Vector2.right,
				direction
			));
			
			/* Place the projectile slightly above our current fire direction so:
			| (place it here instead)
			|
			|
			(fire point)---------------------------------> (fire direction) 
			|
			|
			| (or place it here)*/
			projectileEffect.transform.position += projectileEffect.transform.TransformDirection(Vector3.up).normalized * Random.Range(-0.3f, 0.3f);
			
			projectileEffect.KeepFlyingAt(direction);
		}
	}
	
	IEnumerator EndAnimWaitCoroutine()
	{	
		while (!customMono.animationEventFunctionCaller.endSlaughter) yield return new WaitForSeconds(Time.fixedDeltaTime);
		
		ToggleAnim(boolHash, false);
		endAnimCallback();
		canUse = true;
	}

    public override void DoAuto(Vector2 p_targetDirection, Vector2 p_targetPosition, float p_nextActionChoosingIntervalProposal)
    {
        Trigger(default, p_targetDirection);
    }
}