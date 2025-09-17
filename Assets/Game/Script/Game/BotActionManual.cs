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
    public bool requireContinuous = false;
    public delegate void DoAction(DoActionParamInfo p_doActionParamInfo);
    public DoAction doAction;
    public Action<DoActionParamInfo> botDoActionContinuous = (p_doActionParamInfo) => { };
}
