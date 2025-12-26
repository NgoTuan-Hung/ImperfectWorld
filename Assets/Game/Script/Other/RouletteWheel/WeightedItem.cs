public class WeightedItem<T>
{
    public T Item { get; }
    public int Weight { get; }

    public WeightedItem(T item, int weight)
    {
        Item = item;
        Weight = weight;
    }
}
