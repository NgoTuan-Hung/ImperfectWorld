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
    public Action<GameEffect, Action<float>> effectAction = NoAction;

    public ComboEffect() { }

    public ComboEffect(ObjectPool effectPool, Action<GameEffect, Action<float>> effectAction)
    {
        this.effectPool = effectPool;
        this.effectAction = effectAction;
    }

    static void NoAction(GameEffect p_gameEffect, Action<float> p_dealDamageEvent) { }
}
