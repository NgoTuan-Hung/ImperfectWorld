public class GameEventData
{
    public float maxValue;
    public float currentValue;
    public CustomMono owner;

    public GameEventData(CustomMono owner)
    {
        this.owner = owner;
    }
}
