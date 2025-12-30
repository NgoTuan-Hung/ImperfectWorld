using System;
using UnityEngine.UI;

public class Relic : MonoSelfAware
{
    public RelicDataSO relicDataSO;
    public RelicBehavior relicBehavior;
    public Image image;

    public override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
    }

    public Relic Setup(RelicDataSO relicDataSO)
    {
        this.relicDataSO = relicDataSO;
        image.sprite = relicDataSO.icon;
        var type = GameManager.Instance.GetRelicBehaviorType(relicDataSO.relicBehavior);
        if (type != null)
            relicBehavior = Activator.CreateInstance(type) as RelicBehavior;

        return this;
    }
}
