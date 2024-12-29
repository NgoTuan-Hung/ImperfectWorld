using System.Collections;
using UnityEngine;

public enum KeepDistanceAction {Idle, MoveRandomly, StopPickingAction, MoveToTarget}
public class BotMovable : Movable
{
	public enum MoveMode {MoveToTarget, KeepDistanceWithTarget}
	public MoveMode moveMode = MoveMode.MoveToTarget;
	public float keepDistanceAmmount = 3f;
	float distanceToTarget;
	public int[] keepDistanceActionChance = new int[4] {30, 60, 10, 80};
	public KeepDistanceAction[] keepDistanceActions = new KeepDistanceAction[4];
	float[] keepDistanceActionBoundValue;
	KeepDistanceActionBound[] keepDistanceActionBounds;
	public float[] keepDistanceActionDuration = new float[4] {0.5f, 0.5f, 0.5f, 1f};
	bool stopPickingAction = false;
	
	public override void Start() 
	{
		base.Start();
		keepDistanceActionBoundValue = new float[keepDistanceActionChance.Length*2];
		keepDistanceActionBounds  = new KeepDistanceActionBound[keepDistanceActionChance.Length];
		InitKeepDistanceAction();
		ChangeMode(moveMode);
	}
	
	void InitKeepDistanceAction()
	{
		float actionTotal = 0;
		for (int i=0;i<keepDistanceActionChance.Length;i++) actionTotal += keepDistanceActionChance[i];
		
		/* Prepare bound values */
		keepDistanceActionBoundValue[0] = 0;
		keepDistanceActionBoundValue[keepDistanceActionBoundValue.Length-1] = 1.01f;

		/* A sample if our chance is: 30 60 10 then bounds will be: 0 0+30/100 0.3 0.3+60/100 0.9 1.01.
		Our range will looks like this: [0------0.3][0.3------0.9][0.9------1.01], then we can choose
		a random number and check if it's in any range to choose our next action.*/
		for (int i=1;i<keepDistanceActionBoundValue.Length-1;i++)
		{
			if (i%2==0) keepDistanceActionBoundValue[i] = keepDistanceActionBoundValue[i-1];
			else keepDistanceActionBoundValue[i] = keepDistanceActionBoundValue[i-1] + keepDistanceActionChance[i/2] / actionTotal;
		}
		
		/* Construct bound range */
		for (int i=0;i<keepDistanceActionChance.Length;i++)
		{
			int twoI = i*2;
			keepDistanceActionBounds[i] = new KeepDistanceActionBound
			(
				keepDistanceActionBoundValue[twoI], keepDistanceActionBoundValue[twoI+1], keepDistanceActions[i],
				keepDistanceActionDuration[i]
			);
		}
	}
	
	/// <summary>
	/// Choose an action within a range (random)
	/// </summary>
	/// <returns></returns>
	KeepDistanceActionBound ChooseKeepDistanceAction()
	{
		float decision = Random.Range(0, 1f);
		
		for (int i=0;i<keepDistanceActionBounds.Length;i++)
		{
			if (decision >= keepDistanceActionBounds[i].lowBound && decision < keepDistanceActionBounds[i].highBound) return keepDistanceActionBounds[i];
		}
		
		return keepDistanceActionBounds[0];
	}
	
	public Coroutine ChangeMode(MoveMode mode)
	{
		switch (mode)
		{
			case MoveMode.MoveToTarget:
				moveMode = MoveMode.MoveToTarget;
				return HandleMoveToTarget();
			case MoveMode.KeepDistanceWithTarget:
				moveMode = MoveMode.KeepDistanceWithTarget;
				return HandleKeepDistanceWithTarget();
			default:
				return null;
		}
	}
	
	Coroutine HandleMoveToTarget()
	{
		ToggleMoveAnim(true);
		return StartCoroutine(MoveToTargetCoroutine());
	}
	
	IEnumerator MoveToTargetCoroutine()
	{
		while (true)
		{
			moveVector = customMono.Target.transform.position - transform.position;
			Move(moveVector);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	
	Coroutine HandleKeepDistanceWithTarget()
	{
		return StartCoroutine(KeepDistanceWithTargetCoroutine());
	}
	
	IEnumerator KeepDistanceWithTargetCoroutine()
	{
		float currentTime;
		while (true)
		{
			/* If bot is too close to target, move away from target */
			moveVector = customMono.Target.transform.position - transform.position;
			distanceToTarget = moveVector.magnitude;
			if (distanceToTarget < keepDistanceAmmount)
			{
				ToggleMoveAnim(true);
				do
				{
					moveVector *= -1;
					Move(moveVector);
					moveVector = customMono.Target.transform.position - transform.position;
					distanceToTarget = moveVector.magnitude;
					yield return new WaitForSeconds(Time.fixedDeltaTime);
				} while (distanceToTarget < keepDistanceAmmount);
			}
			
			ToggleMoveAnim(false);
			if (!stopPickingAction)
			{
				/* If bot is out of keep distance range, do some action */
				KeepDistanceActionBound actionBound = ChooseKeepDistanceAction();
				currentTime = 0;
				switch (actionBound.keepDistanceAction)
				{
					/* Be idle for a while */
					case KeepDistanceAction.Idle:
					{
						ToggleMoveAnim(false);
						while (currentTime < actionBound.duration)
						{	
							yield return new WaitForSeconds(Time.fixedDeltaTime);
							currentTime += Time.fixedDeltaTime;
						}
						break;
					}
					/* Move randomly in one direction for a while */
					case KeepDistanceAction.MoveRandomly:
					{
						moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
						ToggleMoveAnim(true);
						while (currentTime < actionBound.duration)
						{
							Move(moveVector);
							yield return new WaitForSeconds(Time.fixedDeltaTime);
							currentTime += Time.fixedDeltaTime;
						}
						break;
					}
					/* Stop picking action for a while */
					case KeepDistanceAction.StopPickingAction:
					{
						stopPickingAction = true;
						StartCoroutine(ContinuePickingActionCoroutine(actionBound.duration));
						break;
					}
					/* Or we can choose to move closer to target */
					case KeepDistanceAction.MoveToTarget:
					{
						Coroutine moveToTargetCoroutine = ChangeMode(MoveMode.MoveToTarget);
						StartCoroutine(ContinueKeepDistanceToTargetCoroutine(actionBound.duration, moveToTargetCoroutine));
						yield break;
					}
					default: break;
				}
			}
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	
	IEnumerator ContinuePickingActionCoroutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		stopPickingAction = false;
	}
	
	IEnumerator ContinueKeepDistanceToTargetCoroutine(float duration, Coroutine coroutine)
	{
		yield return new WaitForSeconds(duration);
		StopCoroutine(coroutine);
		ChangeMode(MoveMode.KeepDistanceWithTarget);
	}
}