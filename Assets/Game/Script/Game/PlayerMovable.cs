
using UnityEngine;

public class PlayerMovable : Movable
{
	Vector2 inverseY = new Vector2(1, -1);
	public override void Start() 
	{
		base.Start();
		GameUIManager.Instance.MainView.joyStickMoveEvent += (vector) => moveVector = vector;
	}
	private void FixedUpdate() 
	{
		HandlePlayerMove();
	}
	
	public void HandlePlayerMove()
	{
		if (moveVector != Vector2.zero)
		{
			ToggleMoveAnim(true);
			moveVector.Scale(inverseY);
			Move(moveVector);
		}
		else ToggleMoveAnim(false);
	}
}