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
    public Action<DoActionParamInfo> botDoAction = (p_doActionParamInfo) => { };
    public Action<DoActionParamInfo> botDoActionContinuous = (p_doActionParamInfo) => { };
    public float continousDuration;
    public bool requireContinuous = false;

    public BotActionManual(
        Action<DoActionParamInfo> botDoAction,
        Action<DoActionParamInfo> botDoActionContinuous,
        float continousDuration = 0,
        bool requireContinuous = false
    )
    {
        this.botDoAction = botDoAction;
        this.botDoActionContinuous = botDoActionContinuous;
        this.continousDuration = continousDuration;
        this.requireContinuous = requireContinuous;
    }
}
