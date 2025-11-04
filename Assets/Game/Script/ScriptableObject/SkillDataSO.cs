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

    public string skillName;
    public Sprite skillImage;

    [TextArea(1, 15)]
    public string skillDescription;
    public SkillType skillType;
}
