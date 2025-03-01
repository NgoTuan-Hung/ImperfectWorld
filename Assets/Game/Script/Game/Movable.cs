using System.Collections;
using UnityEngine;
public class Movable : BaseAction
{
	public Vector2 moveVector;

	public override void Awake() 
	{
		base.Awake();
		boolHash = Animator.StringToHash("Walk");
		AddActionManuals();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}
	
	public override void AddActionManuals()
	{
		base.AddActionManuals();
		botActionManuals.Add(new BotActionManual(ActionUse.GetCloser, (direction, location, nextActionChoosingIntervalProposal) => MoveTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, 1));
		botActionManuals.Add(new BotActionManual(ActionUse.GetAway, (direction, location, nextActionChoosingIntervalProposal) => MoveTo(direction, nextActionChoosingIntervalProposal), 0.5f, true, -1));
		botActionManuals.Add(new BotActionManual(ActionUse.Dodge, (direction, location, nextActionChoosingIntervalProposal) => MoveTo(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), nextActionChoosingIntervalProposal), 0.5f));
		botActionManuals.Add(new BotActionManual(ActionUse.Passive, (direction, location, nextActionChoosingIntervalProposal) => Idle(direction, nextActionChoosingIntervalProposal), 0.5f));
		botActionManuals.Add(new BotActionManual(ActionUse.Roam, (direction, location, nextActionChoosingIntervalProposal) => MoveTo(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), nextActionChoosingIntervalProposal), 1f));
		botActionManuals.Add(new BotActionManual(ActionUse.Roam, (direction, location, nextActionChoosingIntervalProposal) => Idle(direction, nextActionChoosingIntervalProposal), 1f));
	}

	public override void Start()
	{
		base.Start();
	}

	private void FixedUpdate() 
	{
		MoveByController();
	}
	
	void MoveByController()
	{
	    if (moveVector != Vector2.zero) Move(moveVector);
		else ToggleMoveAnim(false);
	}
	
	public void Move(Vector2 direction)
	{
		if (canUse && !customMono.movementActionBlocking)
		{
			if (!GetMoveBool()) ToggleMoveAnim(true);
			customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.VeryLow);
			transform.position += (Vector3)direction.normalized * customMono.stat.moveSpeedPerFrame;
		}
	}
	
	public void ToggleMoveAnim(bool value)
	{
		customMono.AnimatorWrapper.animator.SetBool(boolHash, value);
	}
	
	public bool GetMoveBool() => GetBool(boolHash);
	
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