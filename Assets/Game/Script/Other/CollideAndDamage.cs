using System;
using System.Collections;
using UnityEngine;

public class CollideAndDamage : MonoBehaviour
{
	new Rigidbody2D rigidbody2D;
	public enum CollideType {Single, Multiple}
	private CollideType collideType = CollideType.Single;
	[SerializeField] private bool isDeactivatedAfterTime = false;
	Action deactiveAfterTime = () => {};
	[SerializeField] private float deactivateTime = 1f;
	[SerializeField] private float collideDamage = 1f;

	public Rigidbody2D Rigidbody2D { get => rigidbody2D; set => rigidbody2D = value; }

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
		GameManager.Instance.GetCustomMono(other.gameObject).Stat.Health -= collideDamage;	
	}
}