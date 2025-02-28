using UnityEngine.UIElements;

public class SkillData
{
    public int currentUsableScrollViewIndex;
    public int dropIntoScrollViewIndex;
    public UsableSlotUIInfo usableSlotUIInfo;
    public SkillDataSO skillDataSO;
    public VisualTreeAsset usableHolderVTA;
    public SkillBase skillBase;

    public SkillData(SkillDataSO p_skillDataSO, SkillBase p_skillBase)
    {
        skillDataSO = p_skillDataSO;
        skillBase = p_skillBase;
    }
}