using UnityEngine;

public enum SkillType
{
    Active,
    Passive,
    Special,
}

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "ScriptableObjects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    public enum InputType
    {
        Click,
        Hold,
        HoldAndRelease,
    }

    public SkillIndicatorType skillIndicatorType = SkillIndicatorType.None;
    public InputType inputType;
    public string skillName;
    public Sprite skillImage;

    [TextArea]
    public string skillHelperDescription;
    public SkillType skillType;
}
