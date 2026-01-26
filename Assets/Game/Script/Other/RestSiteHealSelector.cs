using UnityEngine;

public class RestSiteHealSelector : NPC
{
    CustomMono customMono;

    public override void Awake()
    {
        base.Awake();
        interact = Heal;
    }

    public void Setup(CustomMono customMono)
    {
        this.customMono = customMono;
        transform.position = new(
            customMono.rotationAndCenterObject.transform.position.x,
            customMono.rotationAndCenterObject.transform.position.y,
            -1
        );
    }

    public void Heal()
    {
        customMono.statusEffect.Heal(customMono.stat.healthPoint.FinalValue);
        GameUIManager.Instance.FinishRestSiteHealOne();
    }
}
