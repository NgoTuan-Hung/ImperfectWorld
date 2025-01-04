using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MoveAction {MoveToTarget, MoveAwayFromTarget, MoveRandomly, Idle}
public class MovementIntelligence : BaseIntelligence
{	
	Vector2 moveVector;
	public override void Start() 
	{
		base.Start();
		InitAction();
	}
	
	public override void InitAction()
	{
		actionChances = new int[4];
		actionCumulativeDistribution = new float[actionChances.Length];
		actions = new ActionDelegate[actionChances.Length];
		actionIntervals = new float[] {0.5f, 0.5f, 0.5f, 0.5f};
		
		actions[0] = MoveToTarget;
		actions[1] = MoveAwayFromTarget;
		actions[2] = MoveRandomly;
		actions[3] = Idle;
	}

	public void MoveToTarget(Vector2 direction)
	{
		StartCoroutine(MoveToTargetCoroutine(direction));
	}
	
	IEnumerator MoveToTargetCoroutine(Vector2 direction)
	{
		float currentTime = 0;
		actionPerforming = true;
		while (currentTime < actionIntervals[(int)MoveAction.MoveToTarget])
		{
			customMono.movable.Move(direction);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime += Time.fixedDeltaTime;
		}
		actionPerforming = false;
	}
	
	public void MoveRandomly(Vector2 direction)
	{
		StartCoroutine(MoveRandomlyCoroutine(direction));
	}
	
	IEnumerator MoveRandomlyCoroutine(Vector2 direction)
	{
		float currentTime = 0;
		actionPerforming = true;
		moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		while (currentTime < actionIntervals[(int)MoveAction.MoveRandomly])
		{
			customMono.movable.Move(moveVector);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime += Time.fixedDeltaTime;
		}
		actionPerforming = false;
	}
	
	public void MoveAwayFromTarget(Vector2 direction)
	{
		StartCoroutine(MoveAwayFromTargetCoroutine(direction));
	}
	
	IEnumerator MoveAwayFromTargetCoroutine(Vector2 direction)
	{
		float currentTime = 0;
		actionPerforming = true;
		while (currentTime < actionIntervals[(int)MoveAction.MoveAwayFromTarget])
		{
			customMono.movable.Move(-direction);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime += Time.fixedDeltaTime;
		}
		actionPerforming = false;
	}
	
	public void Idle(Vector2 direction)
	{
		StartCoroutine(IdleCoroutine(direction));
	}
	
	IEnumerator IdleCoroutine(Vector2 direction)
	{
		actionPerforming = true;
		customMono.movable.ToggleMoveAnim(false);
		yield return new WaitForSeconds(actionIntervals[(int)MoveAction.Idle]);
		
		actionPerforming = false;
	}
}