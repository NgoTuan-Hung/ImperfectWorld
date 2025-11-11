using UnityEngine;

public partial class CustomMono
{
    public void EquipItem(Item item)
    {
        item.itemDataSO.statBuffs.ForEach(sB =>
        {
            switch (sB.statBuffType)
            {
                case StatBuffType.Health:
                    stat.healthPoint.AddModifier(sB.modifier);
                    break;
                case StatBuffType.Mana:
                    stat.manaPoint.AddModifier(sB.modifier);
                    break;
                case StatBuffType.Might:
                    stat.might.AddModifier(sB.modifier);
                    break;
                case StatBuffType.Reflex:
                    stat.reflex.AddModifier(sB.modifier);
                    break;
                case StatBuffType.Wisdom:
                    stat.wisdom.AddModifier(sB.modifier);
                    break;
                case StatBuffType.Damage:
                    stat.attackDamage.AddModifier(sB.modifier);
                    break;
                default:
                    break;
            }
        });

        item.itemDataSO.itemBehaviours.ForEach(iB =>
        {
            var behaviorComp =
                gameObject.AddComponent(GameManager.Instance.MapItemBehaviour(iB))
                as IItemBehaviour;
            behaviorComp.OnAttach(this, item);
        });
    }

    public void UnEquipItem(Item item)
    {
        item.itemDataSO.statBuffs.ForEach(sB =>
        {
            switch (sB.statBuffType)
            {
                case StatBuffType.Health:
                    stat.healthPoint.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.Mana:
                    stat.manaPoint.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.Might:
                    stat.might.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.Reflex:
                    stat.reflex.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.Wisdom:
                    stat.wisdom.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.Damage:
                    stat.attackDamage.RemoveModifier(sB.modifier);
                    break;
                default:
                    break;
            }
        });

        item.itemDataSO.itemBehaviours.ForEach(iB =>
        {
            var behaviorComp =
                gameObject.GetComponent(GameManager.Instance.MapItemBehaviour(iB))
                as IItemBehaviour;
            behaviorComp.OnDetach();
            Destroy((MonoBehaviour)behaviorComp);
        });
    }
}
