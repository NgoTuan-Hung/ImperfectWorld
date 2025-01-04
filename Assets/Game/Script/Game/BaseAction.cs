using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CustomMono))]
public class BaseAction : MonoBehaviour 
{
	protected CustomMono customMono;
	
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
	
	public virtual bool GetBool(int boolHash) => customMono.AnimatorWrapper.animator.GetBool(boolHash);
	
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