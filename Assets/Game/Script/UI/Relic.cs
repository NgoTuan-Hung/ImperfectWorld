using System;

public class Relic : MonoSelfAware
{
    public RelicDataSO relicDataSO;
    public RelicBehavior relicBehavior;

    public void Setup(RelicDataSO relicDataSO)
    {
        this.relicDataSO = relicDataSO;
        var type = GameManager.Instance.GetRelicBehaviorType(relicDataSO.relicBehavior);
        if (type != null)
            relicBehavior = Activator.CreateInstance(type) as RelicBehavior;
    }
}
