public class DealDamageGameEventData : IGameEventData
{
    public int count = 0;
    public CustomMono dealer = null,
        target = null;
    public float damage;

    public void Setup(CustomMono p_dealer, CustomMono p_target, float p_damage)
    {
        dealer = p_dealer;
        target = p_target;
        damage = p_damage;
        count++;
    }
}
