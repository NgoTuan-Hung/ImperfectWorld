using UnityEngine;
using UnityEngine.UIElements.Experimental;
public class Movable : BaseAction 
{
	protected Vector2 moveVector;
	private bool canMove = true;
	private int walkBoolHash = Animator.StringToHash("Walk");
	public bool CanMove { get => canMove; set => canMove = value; }

	public override void Awake() 
	{
		base.Awake();
	}

    public override void Start()
    {
        
    }

    
	public void Move(Vector2 direction)
	{
		customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.VeryLow);
		transform.position += (Vector3)direction.normalized * customMono.stat.moveSpeedPerFrame;
	}
	
	public void ToggleMoveAnim(bool value)
	{
		customMono.AnimatorWrapper.animator.SetBool(walkBoolHash, value);
	}
}