public class TakeDamageGameEventData : IGameEventData
{
    public float damageTaken;

    public TakeDamageGameEventData(float damageTaken)
    {
        this.damageTaken = damageTaken;
    }

    public TakeDamageGameEventData Setup(float damageTaken)
    {
        this.damageTaken = damageTaken;
        return this;
    }
}
