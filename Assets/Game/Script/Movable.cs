using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Movable : BaseAction 
{
	private float moveSpeed = 1f;
	private float moveSpeedPerFrame;
	private float fixedDeltaTime;
	private bool canMove = true;
	public float MoveSpeed { get => moveSpeed; set { moveSpeed = value; moveSpeedPerFrame = moveSpeed * fixedDeltaTime;} }

	public bool CanMove { get => canMove; set => canMove = value; }
	
	public override void Awake() 
	{
		base.Awake();
		fixedDeltaTime = Time.fixedDeltaTime;
		moveSpeedPerFrame = moveSpeed * fixedDeltaTime;
	}

	private void FixedUpdate() 
	{
		
	}
	
	public void Move(Vector2 direction)
	{
		customMono.MainComponent.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
		transform.position += (Vector3)direction.normalized * moveSpeedPerFrame;
	}
}