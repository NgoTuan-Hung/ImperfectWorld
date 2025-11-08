using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "ScriptableObjects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public List<StatBuff> statBuffs;

    [TextArea(1, 15)]
    public string itemDescription;
}
