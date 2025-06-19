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

    public InputType inputType;
    public string skillName;
    public Sprite skillImage;
    public Sprite skillHelperImage;

    [TextArea]
    public string skillHelperDescription;
    public Sprite skillTreeIcon;
    public SkillType skillType;
}
