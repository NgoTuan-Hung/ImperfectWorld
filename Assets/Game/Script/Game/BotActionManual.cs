using UnityEngine;

/// <summary>
/// Just a guide to tell the bot how to use an action.
/// For example, this action can be used to run away from
/// target, or it can also be used to get closer, depends
/// on the parameters.
/// </summary>
public class BotActionManual
{
	public ActionUse actionUse;
	public bool direction;
	public float directionMultiplier;
	public delegate void DoAction(Vector2 direction, Vector2 location);
	public DoAction doAction;
	public int botActionManualIndex;

	public BotActionManual(ActionUse actionUse, DoAction doAction, bool direction = false, float directionMultiplier = 1)
	{
		this.actionUse = actionUse;
		this.direction = direction;
		this.directionMultiplier = directionMultiplier;
		this.doAction = doAction;
	}
}