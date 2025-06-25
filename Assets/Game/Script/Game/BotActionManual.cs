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
    public delegate void DoAction(DoActionParamInfo p_doActionParamInfo);
    public DoAction doAction;
    public DoActionParamInfo doActionParamInfo;
    public int botActionManualIndex;
    public bool actionNeedWait;
    public Func<bool> startAndWait = () =>
    {
        return true;
    };
    public Action<Vector2, Vector2> whileWaiting = (targetLocation, targetDirection) => { };
    public int actionChanceAjuster = 0;

    public BotActionManual(
        ActionUse actionUse,
        DoAction doAction,
        DoActionParamInfo doActionParamInfo,
        bool actionNeedWait = false,
        Func<bool> startAndWait = null,
        Action<Vector2, Vector2> whileWaiting = null
    )
    {
        this.actionUse = actionUse;
        this.doAction = doAction;
        this.doActionParamInfo = doActionParamInfo;
        this.actionNeedWait = actionNeedWait;
        this.startAndWait = startAndWait;
        this.whileWaiting = whileWaiting;
    }
}
