using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Basically MonoBehaviour but we need to handle some domain reloading stuffs
/// with static fields, objects, ... SO WE DON'T HAVE TO WAIT FOR DOMAIN RELOADING
/// EVERY TIME WE HIT PLAY IN EDITOR.
/// </summary>
public class MonoEditor : MonoBehaviour
{
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