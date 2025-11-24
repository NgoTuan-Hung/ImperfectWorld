public enum GameEffectBehaviourField
{
    StrikeLock,
    SlowInfo,
}

public interface IGameEffectBehaviour
{
    public GameEffect GameEffect { get; set; }
    public void Initialize(GameEffect gameEffect);
}
