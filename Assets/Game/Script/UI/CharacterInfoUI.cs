using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    public GameObject skillSVContent;
    public CharacterPartyNode characterPartyNode;
    public EnhancedOnScreenStick enhancedOnScreenStick;
    public GameObject tooltips,
        skillSlots,
        skillAndItemUseButtons;
    public List<SkillSlotUI> skillSlotUIs;
    public List<SkillUseUI> skillUseUIs;
    public SkillNodeUI queueSkillNodeUI;

    private void Awake()
    {
        skillSlotUIs = skillSlots.GetComponentsInChildren<SkillSlotUI>().ToList();
    }
}
