public class PoisonInfo
{
    public CustomMono owner;
    public int totalPoison;
    public float poisonDamage;

    public PoisonInfo(CustomMono owner, int totalPoison, float poisonDamage)
    {
        this.totalPoison = totalPoison;
        this.poisonDamage = poisonDamage;
    }
}
