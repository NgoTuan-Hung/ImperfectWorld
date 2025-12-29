using UnityEngine;

public enum RelicType
{
    None,
    PerFloor,
}

public enum ERelicBehavior
{
    None,
    BloodWingBlessingBehavior,
}

[CreateAssetMenu(fileName = "RelicDataSO", menuName = "ScriptableObjects/RelicDataSO", order = 0)]
public class RelicDataSO : ScriptableObject
{
    public string relicName;
    public string description;
    public Sprite icon;
    public RelicType relicType;
    public ERelicBehavior relicBehavior;
}
