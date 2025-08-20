using System;
using System.Collections.Generic;

public enum BotAIBehavior
{
    DefaultMeleeBehavior,
    DefaultRangedBehavior,
    Ruhmleyna,
}

public partial class BotAIManager
{
    public BotAIBehavior useBotAIBehavior = BotAIBehavior.DefaultMeleeBehavior;
    static Dictionary<BotAIBehavior, Type> behaviorMap = new()
    {
        { BotAIBehavior.DefaultMeleeBehavior, typeof(DefaultMeleeBehavior) },
        { BotAIBehavior.DefaultRangedBehavior, typeof(DefaultRangedBehavior) },
        { BotAIBehavior.Ruhmleyna, typeof(RuhmleynaBehavior) },
    };

    public BaseAIBehavior aiBehavior;
}
