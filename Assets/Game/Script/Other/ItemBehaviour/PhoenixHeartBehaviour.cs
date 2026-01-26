using UnityEngine;

public class PhoenixHeartBehaviour : MonoBehaviour, IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }

    public void OnAttach(CustomMono customMono, Item item)
    {
        CustomMono = customMono;
        Item = item;
        if (!Item.fields.ContainsKey("phoenixHeartBehaviourFieldCreated"))
        {
            Item.fields["phoenixHeartBehaviourFieldCreated"] = true;
            Item.fields["used"] = false;
        }

        customMono.stat.beforeDeathCallback += ReviveHandler;
        GameManager.Instance.battleEndCallback += ResetUse;
    }

    void ResetUse() => Item.fields["used"] = false;

    public void OnDetach()
    {
        CustomMono.stat.beforeDeathCallback -= ReviveHandler;
        GameManager.Instance.battleEndCallback -= ResetUse;
    }

    public void ReviveHandler()
    {
        if (!(bool)Item.fields["used"] && CustomMono.stat.currentHealthPoint.Value <= 0)
        {
            CustomMono.stat.currentHealthPoint.Value = CustomMono.stat.healthPoint.FinalValue;
            CustomMono.stat.currentManaPoint.Value = CustomMono.stat.manaPoint.FinalValue;
            Item.fields["used"] = true;
        }
    }
}
