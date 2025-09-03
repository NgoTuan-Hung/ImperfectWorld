using System;

public class ComboEffect
{
    /// <summary>
    /// Use this to spawn Game Effect you want
    /// </summary>
    public ObjectPool effectPool;

    /// <summary>
    /// Use this to do something with the effect inside combo
    /// </summary>
    public Action<BaseAction, GameEffect, Action<float>> effectAction = NoAction;

    public ComboEffect() { }

    public ComboEffect(
        ObjectPool effectPool,
        Action<BaseAction, GameEffect, Action<float>> effectAction
    )
    {
        this.effectPool = effectPool;
        this.effectAction = effectAction;
    }

    static void NoAction(
        BaseAction p_baseAction,
        GameEffect p_gameEffect,
        Action<float> p_dealDamageEvent
    ) { }
}
