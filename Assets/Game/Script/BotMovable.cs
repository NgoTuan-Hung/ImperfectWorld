using System.Collections;
using UnityEngine;

public class BotMovable : Movable
{
	public enum MoveMode {Walk}
	private MoveMode moveMode = MoveMode.Walk;
	
	private void Start() 
	{
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
			Move(customMono.Target.transform.position - transform.position);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}