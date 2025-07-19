using System;

public enum ActionResultType
{
    None,
    Cooldown,
    AdditionalPhase,
    AdditionalPhaseWithCondition,
}

/// <summary>
/// Mostly be used by UI for showing cooldown, number of usages, etc.
/// </summary>
public class ActionResult
{
    public bool success = false;
    public ActionResultType actionResultType = ActionResultType.None;
    public float cooldown;
    public float additionalPhaseDuration;
    public Action<ActionResult> conditionMetCallback = (p_actionResult) => { };

    public ActionResult() { }

    public ActionResult(
        bool success,
        ActionResultType actionResultType,
        float cooldown = 0,
        float additionalPhaseDuration = 0
    )
    {
        this.success = success;
        this.actionResultType = actionResultType;
        this.cooldown = cooldown;
        this.additionalPhaseDuration = additionalPhaseDuration;
    }
}
