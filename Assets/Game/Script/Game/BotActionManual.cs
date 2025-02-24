using System;
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
	public bool targetDirection;
	public float targetDirectionMultiplier;
	public delegate void DoAction(Vector2 targetDirection, Vector2 targetLocation, float nextActionChoosingIntervalProposal);
	public DoAction doAction;
	public int botActionManualIndex;
	public float nextActionChoosingIntervalProposal;
	public bool actionNeedWait;
	public Func<bool> startAndWait = () => {return true;};
	public Action<Vector2> whileWaiting = (targetDirection) => {};

	public BotActionManual(ActionUse actionUse, DoAction doAction, float nextActionChoosingIntervalProposal = 0, bool targetDirection = false, float targetDirectionMultiplier = 1
	, bool actionNeedWait = false, Func<bool> startAndWait = null, Action<Vector2> whileWaiting = null)
	{
		this.actionUse = actionUse;
		this.targetDirection = targetDirection;
		this.targetDirectionMultiplier = targetDirectionMultiplier;
		this.doAction = doAction;
		this.nextActionChoosingIntervalProposal = nextActionChoosingIntervalProposal;
		this.actionNeedWait = actionNeedWait;
		this.startAndWait = startAndWait;
		this.whileWaiting = whileWaiting;
	}
}