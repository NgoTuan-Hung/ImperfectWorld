using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAndDamage : MonoBehaviour
{
	Dictionary<string, bool> alliesTag = new Dictionary<string, bool>();
	new Rigidbody2D rigidbody2D;
	public enum CollideType {Single, Multiple}
	private CollideType collideType = CollideType.Single;
	[SerializeField] private float collideDamage = 1f;
	public Rigidbody2D Rigidbody2D { get => rigidbody2D; set => rigidbody2D = value; }
    public Dictionary<string, bool> AlliesTag { get => alliesTag; set => alliesTag = value; }
	
	private void OnEnable() 
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!alliesTag.ContainsKey(other.tag))
		{
			GameManager.Instance.GetCustomMono(other.gameObject).stat.Health -= collideDamage;
		}
	}
}