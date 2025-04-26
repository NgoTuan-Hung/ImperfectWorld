using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollideAndDamage : MonoSelfAware
{
	public HashSet<string> allyTags = new();
	public new Rigidbody2D rigidbody2D;
	public enum CollideType {Single, Multiple}
	public CollideType collideType = CollideType.Single;
	public float collideDamage = 1f;
	public float multipleCollideInterval = 0.05f;
	Action<Collider2D> onTriggerEnter2D = (other) => { };
	Action<Collider2D> onTriggerStay2D = (other) => { };
	/// <summary>
	/// Get customMono from this collision if any.
	/// </summary>
	Action<Collider2D> doSomethingIfTargetIsNotAlly = (collider2D) => {};
	/// <summary>
	/// After getting customMono from collision, you can do
	/// any thing with it (add force, etc.)
	/// </summary>
	Action<CustomMono, Collider2D> doSomethingWithCustomMonoAfterCollide = (customMono, collider2D) => {};
	public bool pushEnemyOnCollide = false;
	public enum PushEnemyOnCollideType {Random, BothSide};
	public PushEnemyOnCollideType pushEnemyOnCollideType = PushEnemyOnCollideType.Random;
	public float pushEnemyOnCollideForce = 1f;
	public bool deactivateOnCollide = false;
	public bool spawnEffectOnCollide = false;
	public CollisionEffectPool collisionEffectPoolType;
	public ObjectPool collisionEffectPool;
	public override void Awake() 
	{
		base.Awake();
		if (collideType == CollideType.Single)
		{	
			doSomethingIfTargetIsNotAlly = (other) => 
			{
			    /* Since parent will have customMono, not this */
				CustomMono customMono = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject);
				if (customMono != null) 
				{
					customMono.stat.Health -= collideDamage;
					doSomethingWithCustomMonoAfterCollide(customMono, other);
				}
			};
			
			onTriggerEnter2D = (Collider2D other) => 
			{
				if (!allyTags.Contains(other.transform.parent.tag))
				{
					doSomethingIfTargetIsNotAlly(other);
				}
			};
			
			if (pushEnemyOnCollide)
			{
				switch (pushEnemyOnCollideType)
				{
					case PushEnemyOnCollideType.Random:
					{
						doSomethingWithCustomMonoAfterCollide += (customMono, collider2D) => 
						{
							customMono.rigidbody2D.AddForce
							(
								pushEnemyOnCollideForce * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized
								, ForceMode2D.Impulse
							);
						};
						break;
					}
					case PushEnemyOnCollideType.BothSide:
					{
						doSomethingWithCustomMonoAfterCollide += (customMono, collider2D) => 
						{
							customMono.rigidbody2D.AddForce
							(
								pushEnemyOnCollideForce * (Random.Range(-1, 1) == 0 ? 1 : -1)
								* transform.TransformDirection(Vector3.up).normalized
								, ForceMode2D.Impulse
							);
						};
						break;
					}
					default:
					{
						break;
					}
				}
			}
			
			if (deactivateOnCollide) doSomethingIfTargetIsNotAlly += (collider2D) => deactivate();
		}
		else
		{
			onTriggerStay2D = (Collider2D other) =>
			{
				if (!allyTags.Contains(other.transform.parent.tag))
				{
					CustomMono customMono = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject);
					if (customMono != null)
					{
						/* Check if the next collision is allowed, if yes, reset the timer
						for this customMono and decrease its health, otherwise do nothing.
						The timer progression is handled in CustomMono.FixedUpdate. */
						
						try
						{
							if (customMono.multipleCollideTimersDict[GetHashCode()].currentTime <= 0)
							{
								customMono.multipleCollideTimersDict[GetHashCode()].currentTime = multipleCollideInterval;
								customMono.stat.Health -= collideDamage;
								doSomethingWithCustomMonoAfterCollide(customMono, other);
							}
						}
						catch (KeyNotFoundException)
						{
							customMono.AddMultipleCollideTimer(GetHashCode(), multipleCollideInterval);
							customMono.stat.Health -= collideDamage;
							doSomethingWithCustomMonoAfterCollide(customMono, other);
						}
					}
				}	
			};
		}
	}
	
	private void OnEnable() 
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public override void Start()
	{
		base.Start();
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => 
		{
			collisionEffectPool = null;
		};
		#endif
		
		if (spawnEffectOnCollide)
		{
			collisionEffectPool = GameManager.Instance.GetCollisionEffectPool(collisionEffectPoolType);
			doSomethingWithCustomMonoAfterCollide += (customMono, collider2D) => 
			{
				GameEffect collisionEffect = collisionEffectPool.PickOne().gameEffect;
				collisionEffect.transform.position = customMono.firePoint.transform.position;
			};
		}
	}

	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		onTriggerEnter2D(other);
	}
	
	private void OnTriggerStay2D(Collider2D other) {
		onTriggerStay2D(other);
	}
}