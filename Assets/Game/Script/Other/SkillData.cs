using UnityEngine.UIElements;

public class SkillData
{
    public int currentUsableScrollViewIndex;
    public int dropIntoScrollViewIndex;
    public UsableSlotUIInfo usableSlotUIInfo;
    public SkillDataSO skillDataSO;
    public VisualTreeAsset usableHolderVTA;

    public SkillData(SkillDataSO skillDataSO)
    {
        this.skillDataSO = skillDataSO;
    }
}