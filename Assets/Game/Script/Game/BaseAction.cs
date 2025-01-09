using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ActionUse {GetCloser, GetAway, Dodge, Passive, MeleeDamage, RangedDamage}
[RequireComponent(typeof(CustomMono))]
public class BaseAction : MonoBehaviour 
{
	protected CustomMono customMono;
	public float cooldown;
	public bool onCooldown = false;
	public float defaultCooldown;
	public float defaultStateSpeed;
	/// <summary>
	/// this action should only be executed when 
	/// we can use it, and we can use it again
	/// after some cooldown.
	/// </summary>
	public bool canUse = true;
	public List<BotActionManual> botActionManuals = new();
	public int boolHash;
	/// <summary>
	/// Executing custom callback when animation end.
	/// </summary>
	public Action endAnimCallback = () => { };
	public AnimationClip actionClip;
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
	}
	
	/// <summary>
	/// Only one child should call this
	/// </summary>
	public virtual void Start()
	{
		#if UNITY_EDITOR
		if (!onExitPlayModeAdded)
		{
			EditorApplication.playModeStateChanged += OnExitPlayMode;
			onExitPlayModeAdded = true;
		}
		#endif
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
	
	#if UNITY_EDITOR
	public static Action onExitPlayModeEvent;
	public static bool onExitPlayModeAdded = false;
	static void OnExitPlayMode(PlayModeStateChange playModeStateChange)
	{
		if (onExitPlayModeEvent == null) return;
		if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
		{
			Debug.Log("Exiting Play Mode");
			onExitPlayModeEvent();
			onExitPlayModeEvent = null;	
			EditorApplication.playModeStateChanged -= OnExitPlayMode;
			onExitPlayModeAdded = false;
		}
	}
	#endif
}