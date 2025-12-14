using System.Collections.Generic;
using UnityEngine;

public enum ItemTier
{
    Normal,
    Rare,
    Epic,
    Legendary,
}

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "ScriptableObjects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public List<StatBuff> statBuffs = new();
    public List<ItemBehaviourType> itemBehaviours;

    [TextArea(1, 15)]
    public string itemDescription;
    public ItemTier itemTier;
    public int price;
}
