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
	public Vector3 followSlowlyOffset = new(-0.8f, 1.5f, 0);
	public float followSlowlyPositionLerpTime = 0.04f;
	
	private void Awake() 
	{
		if (isDeactivatedAfterTime) deactiveAfterTime += () => 
		{
			StartCoroutine(DeactivateAfterTimeCoroutine(deactivateTime));
		};
		
		collideAndDamage = GetComponent<CollideAndDamage>();
		playableDirector = GetComponent<PlayableDirector>();
		
		if (isTimeline) onEnable += () => playableDirector.Play();
	}

	private void OnEnable() {
		deactiveAfterTime();	
		onEnable();
	}
	
	public void DeactivateAfterTime(float deactivateTime) => StartCoroutine(DeactivateAfterTimeCoroutine(deactivateTime));
	IEnumerator DeactivateAfterTimeCoroutine(float deactivateTime)
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
	
	public void FollowSlowly(Transform master)
	{
		StartCoroutine(FollowSlowlyCoroutine(master));
	}
	
	IEnumerator FollowSlowlyCoroutine(Transform master)
	{
		Vector2 newVector2Position = master.position + followSlowlyOffset, prevVector2Position, expectedVector2Position;
		float currentTime;
		
		while (true)
		{
			prevVector2Position = newVector2Position;
			/* Check current position */
			newVector2Position = master.position + followSlowlyOffset;
			
			/* Start lerping position for specified duration if position change detected.*/
			if (prevVector2Position != newVector2Position)
			{
				currentTime = 0;
				while (currentTime < followSlowlyPositionLerpTime + Time.fixedDeltaTime)
				{
					expectedVector2Position = Vector2.Lerp(prevVector2Position, newVector2Position, currentTime / followSlowlyPositionLerpTime);
					transform.position = new Vector2(expectedVector2Position.x, expectedVector2Position.y);
					
					yield return new WaitForSeconds(currentTime += Time.fixedDeltaTime);
				}
			}
			
			transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}