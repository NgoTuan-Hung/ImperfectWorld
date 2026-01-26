public enum ItemBehaviourType
{
    KuraiKōraBehaviour,
    PhoenixHeartBehaviour,
    MoonCleaverBehaviour,
    BlinkspineScepterBehaviour,
    MercuryGraspBehaviour,
}

public interface IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }

    /// <summary>
    /// Set customMono who use this behaviour and item who owns this behaviour
    /// </summary>
    /// <param name="customMono"></param>
    /// <param name="item"></param>
    public void OnAttach(CustomMono customMono, Item item);
    public void OnDetach();
}
