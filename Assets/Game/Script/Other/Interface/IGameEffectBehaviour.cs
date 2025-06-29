public interface IGameEffectBehaviour
{
    public GameEffect GameEffect { get; set; }
    public void Initialize(GameEffect gameEffect);
    public void Disable();
    public void Enable(GameEffectSO p_gameEffectSO);
}
