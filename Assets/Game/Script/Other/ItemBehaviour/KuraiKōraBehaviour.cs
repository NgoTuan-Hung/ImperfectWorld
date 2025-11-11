using UnityEngine;

public class KuraiKōraBehaviour : MonoBehaviour, IItemBehaviour
{
    public CustomMono CustomMono { get; set; }
    public Item Item { get; set; }
    float rand;
    AttackGameEventData attackGameEventData;

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
        attackGameEventData = iGameEventData.As<AttackGameEventData>();
        rand = Random.Range(0f, 1f);
        if (rand < 0.25f)
        {
            attackGameEventData.target.statusEffect.Stun(1.2f);
        }
    }
}
