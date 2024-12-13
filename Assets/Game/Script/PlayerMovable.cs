
using UnityEngine;

public class PlayerMovable : Movable
{
	Vector2 moveVector, inverseY = new Vector2(1, -1);
	private void Start() 
	{
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