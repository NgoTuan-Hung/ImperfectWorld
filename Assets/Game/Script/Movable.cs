using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Movable : BaseAction 
{
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private float defaultMoveSpeed = 1f;
	protected Vector2 moveVector;
	private float moveSpeedPerFrame;
	private float fixedDeltaTime;
	private bool canMove = true;
	private int walkBoolHash = Animator.StringToHash("Walk");
	float directionIndicatorAngle;
	public float MoveSpeed { get => moveSpeed; set { moveSpeed = value; moveSpeedPerFrame = moveSpeed * fixedDeltaTime;} }
	public bool CanMove { get => canMove; set => canMove = value; }
	public float DefaultMoveSpeed { get => defaultMoveSpeed; set => defaultMoveSpeed = value; }

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
		customMono.SpriteRenderer.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
		transform.position += (Vector3)direction.normalized * moveSpeedPerFrame;
	}
	

	public void SetDefaultMoveSpeed()
	{
		MoveSpeed = DefaultMoveSpeed;
	}
	
	public void ToggleMoveAnim(bool value)
	{
		customMono.AnimatorWrapper.Animator.SetBool(walkBoolHash, value);
	}
	
	public void UpdateDirectionIndicator(Vector2 direction)
	{
		directionIndicatorAngle = Vector2.SignedAngle(Vector2.right, direction);
		customMono.DirectionIndicator.transform.rotation = Quaternion.Euler(0, 0, directionIndicatorAngle);
	}
}