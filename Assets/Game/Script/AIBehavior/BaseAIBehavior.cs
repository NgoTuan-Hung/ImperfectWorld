using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Bot brain, execute actions based on various factors like sensors, conditions,...
/// Should run after BotSensor.cs .
/// </summary>
[DefaultExecutionOrder(0)]
public class BaseAIBehavior : CustomMonoPal
{
    public Action forceUsingAction = () => { };
    public PausableScript pausableScript = new();

    public override void Start()
    {
        base.Start();
        StartCoroutine(LateStart());
    }

    /// <summary>
    /// This script can be paused and resumed, just need to
    /// </summary>
    /// <returns></returns>
    IEnumerator LateStart()
    {
        yield return null;
        pausableScript.resumeFixedUpdate = ResumeFixedUpdate;
        pausableScript.pauseFixedUpdate = PauseFixedUpdate;
        pausableScript.resumeFixedUpdate();
    }

    public virtual void DoFixedUpdate() { }

    public virtual void FixedUpdate()
    {
        pausableScript.fixedUpdate();
    }

    void PauseFixedUpdate() => pausableScript.fixedUpdate = NoFixedUpdate;

    void ResumeFixedUpdate() => pausableScript.fixedUpdate = DoFixedUpdate;

    public static void NoFixedUpdate() { }
}
