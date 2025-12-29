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

    public override void Awake()
    {
        base.Awake();
        pausableScript.Setup(NoFixedUpdate, DoFixedUpdate);
    }

    public virtual void DoFixedUpdate() { }

    public virtual void FixedUpdate()
    {
        pausableScript.fixedUpdate();
    }

    public static void NoFixedUpdate() { }
}
