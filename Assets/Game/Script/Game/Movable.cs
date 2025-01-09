using System.Collections;
using UnityEngine;
public class Movable : BaseAction
{
	protected Vector2 moveVector;
	private bool canMove = true;
	private int walkBoolHash = Animator.StringToHash("Walk");
	public bool CanMove { get => canMove; set => canMove = value; }

	public override void Awake() 
	{
		base.Awake();
		AddActionManuals();
	}

	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.GetCloser, (direction, location) => MoveTo(direction, 0.5f), true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.GetAway, (direction, location) => MoveTo(direction, 0.5f), true, -1));
		botActionManuals.Add(new BotActionManual(ActionUse.Dodge, (direction, location) => MoveTo(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 0.5f)));
		botActionManuals.Add(new BotActionManual(ActionUse.Passive, (direction, location) => Idle(direction, 0.5f)));
	}

	public override void Start()
	{
		
	}

	
	public void Move(Vector2 direction)
	{
		if (!GetMoveBool()) ToggleMoveAnim(true);
		customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.VeryLow);
		transform.position += (Vector3)direction.normalized * customMono.stat.moveSpeedPerFrame;
	}
	
	public void ToggleMoveAnim(bool value)
	{
		customMono.AnimatorWrapper.animator.SetBool(walkBoolHash, value);
	}
	
	public bool GetMoveBool() => GetBool(walkBoolHash);
	
	public void MoveTo(Vector2 direction, float duration)
	{
		StartCoroutine(MoveToCoroutine(direction, duration));
	}
	
	IEnumerator MoveToCoroutine(Vector2 direction, float duration)
	{
		float currentTime = 0;
		customMono.movementActionInterval = true;
		while (currentTime < duration)
		{
			customMono.movable.Move(direction);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			currentTime += Time.fixedDeltaTime;
		}
		customMono.movementActionInterval = false;
	}
	
	public void Idle(Vector2 direction, float duration)
	{
		StartCoroutine(IdleCoroutine(direction, duration));
	}
	
	IEnumerator IdleCoroutine(Vector2 direction, float duration)
	{
		customMono.movementActionInterval = true;
		customMono.movable.ToggleMoveAnim(false);
		yield return new WaitForSeconds(duration);
		
		customMono.movementActionInterval = false;
	}
}