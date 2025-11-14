public class AttackGameEventData : IGameEventData
{
    public int count = 0;
    public CustomMono attacker = null,
        target = null;

    public void Setup(CustomMono p_attacker, CustomMono p_target)
    {
        attacker = p_attacker;
        target = p_target;
        count++;
    }
}
