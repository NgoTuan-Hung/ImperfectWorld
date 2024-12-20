using System.Collections;
using UnityEngine;

public class BotMovable : Movable
{
	public enum MoveMode {Walk}
	private MoveMode moveMode = MoveMode.Walk;
	
	
	public override void Start() 
	{
		base.Start();
		ChangeMode(MoveMode.Walk);	
	}
	
	public void ChangeMode(MoveMode mode)
	{
		switch (mode)
		{
			case MoveMode.Walk:
				moveMode = MoveMode.Walk;
				HandleWalk();
				break;
			default:
				break;
		}
	}
	
	public void HandleWalk()
	{
		ToggleMoveAnim(true);
		StartCoroutine(WalkCoroutine());
	}
	
	IEnumerator WalkCoroutine()
	{
		while (true)
		{
			moveVector = customMono.Target.transform.position - transform.position;
			UpdateDirectionIndicator(moveVector);
			Move(moveVector);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}