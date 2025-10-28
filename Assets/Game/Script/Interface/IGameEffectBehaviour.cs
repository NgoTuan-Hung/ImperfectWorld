public enum GameEffectBehaviourField
{
    StrikeLock,
}

public interface IGameEffectBehaviour
{
    public GameEffect GameEffect { get; set; }
    public void Initialize(GameEffect gameEffect);
}
