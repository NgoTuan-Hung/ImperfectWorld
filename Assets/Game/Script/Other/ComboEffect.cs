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
    public Action<GameEffect, Action<CollideAndDamage>> effectAction = NoAction;

    public ComboEffect() { }

    public ComboEffect(
        ObjectPool effectPool,
        Action<GameEffect, Action<CollideAndDamage>> effectAction
    )
    {
        this.effectPool = effectPool;
        this.effectAction = effectAction;
    }

    static void NoAction(GameEffect p_gameEffect, Action<CollideAndDamage> p_setupCAD) { }
}
