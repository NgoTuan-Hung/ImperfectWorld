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
	[SerializeField] private bool isDeactivatedAfterTime = false;
	Action deactiveAfterTime = () => {};
	[SerializeField] private float deactivateTime = 1f;
	[SerializeField] private float collideDamage = 1f;

	public Rigidbody2D Rigidbody2D { get => rigidbody2D; set => rigidbody2D = value; }
    public Dictionary<string, bool> AlliesTag { get => alliesTag; set => alliesTag = value; }

    private void Awake() 
	{
		if (isDeactivatedAfterTime) deactiveAfterTime += () => 
		{
			StartCoroutine(DeactivateAfterTimeCoroutine());
		};
	}
	
	private void OnEnable() 
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
		deactiveAfterTime();	
	}
	
	IEnumerator DeactivateAfterTimeCoroutine()
	{
		yield return new WaitForSeconds(deactivateTime);
		gameObject.SetActive(false);
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!alliesTag.ContainsKey(other.tag))
		{
			GameManager.Instance.GetCustomMono(other.gameObject).Stat.Health -= collideDamage;	
		}
	}
}