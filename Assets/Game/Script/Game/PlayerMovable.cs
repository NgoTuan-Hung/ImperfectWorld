
using UnityEngine;

public class PlayerMovable : Movable
{
	Vector2 inverseY = new Vector2(1, -1);
	public override void Start() 
	{
		base.Start();
		GameUIManager.Instance.MainView.joyStickMoveEvent += (vector) => 
		{
			vector.Scale(inverseY);
			moveVector = vector;
		};
	}
	private void FixedUpdate() 
	{
		HandlePlayerMove();
	}
	
	public void HandlePlayerMove()
	{
		if (moveVector != Vector2.zero) Move(moveVector);
		else ToggleMoveAnim(false);
	}
}