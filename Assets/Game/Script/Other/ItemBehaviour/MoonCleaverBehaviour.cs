using UnityEngine;

public class MoonCleaverBehaviour : MonoBehaviour, IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }

    public void OnAttach(CustomMono customMono, Item item)
    {
        CustomMono = customMono;
        Item = item;
        GameManager.Instance.GetSelfEvent(customMono, GameEventType.Attack).action += OnAttack;
    }

    public void OnDetach()
    {
        GameManager.Instance.GetSelfEvent(CustomMono, GameEventType.Attack).action -= OnAttack;
    }

    void OnAttack(IGameEventData iGameEventData)
    {
        var effect = GameManager.Instance.moonCleaverImpactPool.PickOneGameEffect();

        effect.transform.position = iGameEventData
            .As<AttackGameEventData>()
            .target.rotationAndCenterObject.transform.position;
        effect.SetUpCollideAndDamage(CustomMono, CustomMono.stat.attackDamage.FinalValue * 0.6f);
    }
}
