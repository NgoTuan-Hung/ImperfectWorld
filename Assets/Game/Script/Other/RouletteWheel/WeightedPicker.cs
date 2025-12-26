using System;
using System.Collections.Generic;
using System.Linq;

public class WeightedPicker<T>
{
    private readonly List<WeightedItem<T>> items;
    private readonly int totalWeight;
    private readonly Random rng = new();

    public WeightedPicker(IEnumerable<WeightedItem<T>> items)
    {
        this.items = items.ToList();

        totalWeight = this.items.Sum(i => i.Weight);

        if (totalWeight <= 0)
            throw new ArgumentException("Total weight must be greater than zero.");
    }

    public T Pick()
    {
        int roll = rng.Next(totalWeight);
        int cumulative = 0;

        foreach (var item in items)
        {
            cumulative += item.Weight;
            if (roll < cumulative)
                return item.Item;
        }

        // Safety fallback
        return items[0].Item;
    }
}
