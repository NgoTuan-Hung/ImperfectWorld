public class ValueChangeGameEventData : IGameEventData
{
    public float maxValue;
    public float currentValue;
    public CustomMono owner;

    public ValueChangeGameEventData(CustomMono owner)
    {
        this.owner = owner;
    }
}
