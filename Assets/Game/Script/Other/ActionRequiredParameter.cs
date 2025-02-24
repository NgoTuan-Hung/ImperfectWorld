using UnityEngine;

public class ActionRequiredParameter
{
	public Vector2 targetDirection;
	public Vector2 targetPosition;
	public void Refresh()
	{
		targetDirection = Vector2.zero;
		targetPosition = Vector3.zero;
	}
}