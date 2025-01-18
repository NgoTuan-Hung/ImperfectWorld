using System;
using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class AirRollSkill : SkillBase
{
	Vector2 p1, p2, p3, mid;
	float t, oneMinusT, moveLength, landDelay = 0.5f;
	int landBoolHash;
	public override void Awake()
	{
		base.Awake();
		duration = 1f;
		moveLength = Time.fixedDeltaTime / duration;
		boolHash = Animator.StringToHash("AirRoll");
		landBoolHash = Animator.StringToHash("Land");
		audioClip = Resources.Load<AudioClip>("AudioClip/air-roll-landing");
		
		AddActionManuals();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}

	public override void AddActionManuals()
	{
		base.AddActionManuals();
		
		botActionManuals.Add(new BotActionManual
		(
			ActionUse.AirRoll,
			(direction, location, nextActionChoosingIntervalProposal) => AirRollTo(direction, location, nextActionChoosingIntervalProposal),
			1f
		));
	}

	
	public override void Start()
	{
		base.Start();
	}

	public override void Trigger(Touch touch = default, Vector2 location = default, Vector2 direction = default)
	{
		if (canUse && !customMono.actionBlocking)
		{
			canUse = false;
			customMono.actionBlocking = true;
			customMono.movementActionBlocking = true;
			ToggleAnim(boolHash, true);
			StartCoroutine(TriggerCoroutine(touch, location, direction));
		}
	}
	
	IEnumerator TriggerCoroutine(Touch touch, Vector2 location, Vector2 direction)
	{
		p1 = transform.position;
		p3 = location;
		mid = (p1 + p3) / 2;
		p2 = p3 - p1;
		p2 = Rotate(p2, 90) + mid;
		
		customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
		
		t = 0;
		while (t < duration)
		{
			oneMinusT = 1 - t;
			transform.position = oneMinusT * oneMinusT * p1 + 2 * t * oneMinusT * p2 + t * t * p3;
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			t += moveLength;
		}
		
		ToggleAnim(boolHash, false);
		ToggleAnim(landBoolHash, true);
		customMono.audioSource.PlayOneShot(audioClip);
		
		yield return new WaitForSeconds(landDelay);
		ToggleAnim(landBoolHash, false);
		customMono.actionBlocking = false;
		customMono.movementActionBlocking = false;
	}
	
	public void AirRollTo(Vector2 direction, Vector2 location, float duration)
	{
		StartCoroutine(AirRollToCoroutine(direction, location, duration));
	}
	
	IEnumerator AirRollToCoroutine(Vector2 direction, Vector2 location, float duration)
	{
		customMono.actionInterval = true;
		Trigger(location: location, direction: direction);
		yield return new WaitForSeconds(duration);
		customMono.actionInterval = false;
	}
	
	public Vector2 Rotate(Vector2 v, float degrees)
	{
		return new Vector2((float)(Math.Cos(degrees) * v.x - Math.Sin(degrees) * v.y), (float)(Math.Sin(degrees) * v.x + Math.Cos(degrees) * v.y));
	}
}