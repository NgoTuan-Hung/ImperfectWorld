using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class GameEffect : MonoBehaviour 
{
	public Animator animator;
	public SpriteRenderer spriteRenderer;
	[SerializeField] private bool isDeactivatedAfterTime = false;
	Action deactiveAfterTime = () => {};
	[SerializeField] private float deactivateTime = 1f;
	public CollideAndDamage collideAndDamage;
	PlayableDirector playableDirector;
	public bool isTimeline = false;
	public float flyAtSpeed = 0.03f;
	Action onEnable = () => {};
	
	private void Awake() 
	{
		if (isDeactivatedAfterTime) deactiveAfterTime += () => 
		{
			StartCoroutine(DeactivateAfterTimeCoroutine());
		};
		
		collideAndDamage = GetComponent<CollideAndDamage>();
		playableDirector = GetComponent<PlayableDirector>();
		
		if (isTimeline) onEnable += () => playableDirector.Play();
	}

	private void OnEnable() {
		deactiveAfterTime();	
		onEnable();
	}
	
	IEnumerator DeactivateAfterTimeCoroutine()
	{
		yield return new WaitForSeconds(deactivateTime);
		gameObject.SetActive(false);
	}
	
	public void KeepFlyingAt(Vector3 direction)
	{
		StartCoroutine(KeepFlyingAtCoroutine(direction));
	}
	
	IEnumerator KeepFlyingAtCoroutine(Vector3 direction)
	{
		direction = direction.normalized * flyAtSpeed;
		while (true)
		{
			transform.position += direction;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}