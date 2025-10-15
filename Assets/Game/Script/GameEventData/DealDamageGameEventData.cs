public class DealDamageGameEventData : IGameEventData
{
    public int count = 0;
    public CustomMono dealer = null,
        target = null;
    public float damage;
}
