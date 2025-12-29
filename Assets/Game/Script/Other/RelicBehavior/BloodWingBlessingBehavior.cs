public class BloodWingBlessingBehavior : RelicBehavior
{
    public override void PerFloorCallback()
    {
        GameManager.Instance.HealAllPlayerAlliesByPercentage(0.1f);
    }
}
