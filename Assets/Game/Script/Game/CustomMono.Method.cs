using UnityEngine;

public partial class CustomMono
{
    public void EquipItem(Item item)
    {
        item.itemDataSO.statBuffs.ForEach(sB =>
        {
            switch (sB.statBuffType)
            {
                case StatBuffType.HP:
                    stat.healthPoint.AddModifier(sB.modifier);
                    break;
                case StatBuffType.MP:
                    stat.manaPoint.AddModifier(sB.modifier);
                    break;
                case StatBuffType.MIGHT:
                    stat.might.AddModifier(sB.modifier);
                    break;
                case StatBuffType.REFLEX:
                    stat.reflex.AddModifier(sB.modifier);
                    break;
                case StatBuffType.WISDOM:
                    stat.wisdom.AddModifier(sB.modifier);
                    break;
                case StatBuffType.ATK:
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
                case StatBuffType.HP:
                    stat.healthPoint.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.MP:
                    stat.manaPoint.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.MIGHT:
                    stat.might.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.REFLEX:
                    stat.reflex.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.WISDOM:
                    stat.wisdom.RemoveModifier(sB.modifier);
                    break;
                case StatBuffType.ATK:
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
