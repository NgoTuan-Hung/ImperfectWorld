using System;
using System.Collections.Generic;
using UnityEngine;

public class CollideAndDamage : MonoBehaviour
{
	public HashSet<string> allyTags = new();
	new Rigidbody2D rigidbody2D;
	public enum CollideType {Single, Multiple}
	public CollideType collideType = CollideType.Single;
	public float collideDamage = 1f;
	public float multipleCollideInterval = 0.05f;
	public Rigidbody2D Rigidbody2D { get => rigidbody2D; set => rigidbody2D = value; }
	Action<Collider2D> onTriggerEnter2D = (other) => { };
	Action<Collider2D> onTriggerStay2D = (other) => { };
	private void Awake() 
	{
		if (collideType == CollideType.Single)
		{
			onTriggerEnter2D = (Collider2D other) => 
			{
				if (!allyTags.Contains(other.transform.parent.tag))
				{
					/* Since parent will have customMono, not this */
					CustomMono customMono = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject);
					if (customMono != null) customMono.stat.Health -= collideDamage;
				}
			};
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
							}
						}
						catch (KeyNotFoundException)
						{
							customMono.AddMultipleCollideTimer(GetHashCode(), multipleCollideInterval);
							customMono.stat.Health -= collideDamage;
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
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		onTriggerEnter2D(other);
	}
	
	private void OnTriggerStay2D(Collider2D other) {
		onTriggerStay2D(other);
	}
}