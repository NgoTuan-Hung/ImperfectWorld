using System;
using System.Collections.Generic;

public enum BotAIBehavior
{
    NewAIBehavior,
}

public partial class BotAIManager
{
    public BotAIBehavior useBotAIBehavior = BotAIBehavior.NewAIBehavior;
    static Dictionary<BotAIBehavior, Type> behaviorMap = new()
    {
        { BotAIBehavior.NewAIBehavior, typeof(NewAIBehavior) },
    };

    public BaseAIBehavior aiBehavior;
}
