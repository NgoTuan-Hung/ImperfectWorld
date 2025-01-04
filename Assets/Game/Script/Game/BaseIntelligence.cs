using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CustomMono))]
public class BaseIntelligence : MonoBehaviour 
{
	protected CustomMono customMono;
	public int[] actionChances;
	/// <summary>
	/// Say we have 3 action with action chance 30 60 10
	/// then the normalized values will be 0.3 0.6 0.1
	/// and the cumulative distribution will be 0.3 0.9 1.
	/// </summary>
	public float[] actionCumulativeDistribution;
	public delegate void ActionDelegate(Vector2 direction = default);
	public ActionDelegate[] actions;
	/// <summary>
	/// A small interval for each action to perform until
	/// we choose the next action.
	/// </summary>
	public float[] actionIntervals;
	public bool actionPerforming = false;
	
	public virtual void Awake() 
	{
		customMono = GetComponent<CustomMono>();
	}
	
	public virtual void InitAction(){}
	
	/// <summary>
	/// Normalize action chances and calculate cumulative distribution of 
	/// each action.
	/// </summary>
	void RecalculateActionCumulativeDistribution()
	{
		float t_total = actionChances.Sum();
		
		actionCumulativeDistribution[0] = actionChances[0] / t_total;
		for (int i=1;i<actionCumulativeDistribution.Length;i++)
		{
			actionCumulativeDistribution[i] = actionCumulativeDistribution[i-1] + actionChances[i] / t_total;
		}
		actionCumulativeDistribution[^1] = (float)Math.Ceiling(actionCumulativeDistribution[^1]);
	}
	
	/// <summary>
	/// So you have cumulativeDistribution of 0.3 0.9 1, that means 0-0.3 is the distribution of 
	/// action 0 and it has 30% chance, 0.3-0.9 is the distribution of action 1 and it has 60% chance
	/// and so on. To pick a random action, we can search if our random value is less than any
	/// cumulative distribution while we iterate from left to right, or we can do a binary search.
	/// </summary>
	/// <returns></returns>
	public ActionDelegate ExecuteAnyActionThisFrame(Vector2 direction = default)
	{
		if (actionPerforming) return null;
		RecalculateActionCumulativeDistribution();
		var rand = Random.Range(0, 1f);
		ActionDelegate t_action = actions[actionCumulativeDistribution.CumulativeDistributionBinarySearch
		(
			0, actionCumulativeDistribution.Length-1, rand
		)];
		
		t_action(direction);
		
		RefreshMoveActionChances();
		
		return t_action;
	}
	
	public void RefreshMoveActionChances()
	{
		for (int i = 0; i < actionChances.Length; i++) actionChances[i] = 0;
	}
	
	public void AddActionChance(int action, int value) => actionChances[action] += value;
	
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