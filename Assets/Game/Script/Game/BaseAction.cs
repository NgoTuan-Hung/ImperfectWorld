using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Diagnostics;

public enum ActionUse {GetCloser, GetAway, Dodge, Passive, MeleeDamage, RangedDamage, Roam, AirRoll, SummonShortRange, PushAway}
[RequireComponent(typeof(CustomMono))]
public class BaseAction : MonoEditor
{
	protected CustomMono customMono;
	public float cooldown;
	public bool onCooldown;
	public float defaultCooldown;
	public float defaultStateSpeed;
	/// <summary>
	/// this action should only be executed when 
	/// we can use it, and we can use it again
	/// after some cooldown.
	/// </summary>
	public bool canUse;
	public List<BotActionManual> botActionManuals = new();
	public int boolHash = 0;
	/// <summary>
	/// Executing custom callback when animation end.
	/// </summary>
	public Action endAnimCallback = () => { };
	public AnimationClip actionClip;
	public AudioClip audioClip;
	public float damage = 0;
	public float defaultDamage = 0;
	public Vector2 finalDirection;
	public Stopwatch stopwatch = new();
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
	}
	
	public virtual void OnEnable()
	{
		onCooldown = false;
		canUse = true;
	}
	
	/// <summary>
	/// Only one child should call this
	/// </summary>
	public override void Start()
	{	
		base.Start();
		/* Stop action when we die */
		customMono.stat.healthReachZeroEvent.action += () => StopAndDisable();
	}
	
	public virtual void ToggleAnim(int boolHash, bool value)
	{
		customMono.AnimatorWrapper.animator.SetBool(boolHash, value);
	}
	
	public virtual void AddActionManuals() {}
	
	public virtual bool GetBool(int boolHash) => customMono.AnimatorWrapper.animator.GetBool(boolHash);
	public IEnumerator CooldownCoroutine()
	{
		yield return new WaitForSeconds(cooldown);
		canUse = true;
	}
	
	/// <summary>
	/// Wait for the animation to end by checking the signal.
	/// You will want to check AnimationEventFunctionCaller 
	/// for this.
	/// </summary>
	/// <param name="endAnimCheck"></param>
	public virtual void EndAnimWait(Func<bool> endAnimCheck)
	{
		StartCoroutine(EndAnimWaitCoroutine(endAnimCheck));
	}
	
	IEnumerator EndAnimWaitCoroutine(Func<bool> endAnimCheck)
	{	
		while (!endAnimCheck()) yield return new WaitForSeconds(Time.fixedDeltaTime);
		
		onCooldown = true;
		ToggleAnim(boolHash, false);
		endAnimCallback();
		yield return new WaitForSeconds(cooldown);
		canUse = true;
		onCooldown = false;
	}
	
	public virtual void StatChangeRegister()
	{
		
	}
	
	public virtual void StopAndDisable()
	{
		StopAllCoroutines();
		canUse = false;
		if (boolHash != 0) ToggleAnim(boolHash, false);
	}
	
	public virtual void Trigger(Touch touch = default, Vector2 location = default, Vector2 direction = default)
	{
		
	}
	public virtual bool StartAndWait(){return true;}
	public virtual void WhileWaiting(Vector2 vector2){}
}