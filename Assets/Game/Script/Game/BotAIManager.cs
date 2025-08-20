using UnityEngine;

public partial class BotAIManager : CustomMonoPal
{
    public override void Awake()
    {
        base.Awake();
        aiBehavior = gameObject.AddComponent(behaviorMap[useBotAIBehavior]) as BaseAIBehavior;
    }
}
