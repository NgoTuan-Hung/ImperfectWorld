using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChampInfoPanel : MonoBehaviour, IPointerDownHandler
{
    public CustomMono owner;
    GameObject container,
        statSV,
        statContent,
        abilitySV,
        itemSV,
        itemSVContent,
        activeContent,
        tooltip;
    TextMeshProUGUI nameTMP,
        hpTMP,
        hpRegenTMP,
        mpTMP,
        mpRegenTMP,
        mightTMP,
        reflexTMP,
        wisdomTMP,
        aspdTMP,
        armorTMP,
        mspdTMP,
        dmgmodTMP,
        omnivampTMP,
        aTKTMP,
        critTMP,
        critModTMP,
        atkrangeTMP,
        abilityTMP,
        tooltipTMP;
    public static Vector3 offset = new(1.33942f, 2.614366f);
    public InfoPanelTabButton selectedIPTB,
        xIPTB,
        statIPTB,
        abilityIPTB,
        itemIPTB;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        container = transform.Find("Container").gameObject;
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statSV = container.transform.Find("StatSV").gameObject;
        statContent = statSV.transform.Find("Viewport/Content").gameObject;
        hpTMP = statContent.transform.Find("HP").GetComponent<TextMeshProUGUI>();
        hpRegenTMP = statContent.transform.Find("HPRegen").GetComponent<TextMeshProUGUI>();
        mpTMP = statContent.transform.Find("MP").GetComponent<TextMeshProUGUI>();
        mpRegenTMP = statContent.transform.Find("MPRegen").GetComponent<TextMeshProUGUI>();
        mightTMP = statContent.transform.Find("Might").GetComponent<TextMeshProUGUI>();
        reflexTMP = statContent.transform.Find("Reflex").GetComponent<TextMeshProUGUI>();
        wisdomTMP = statContent.transform.Find("Wisdom").GetComponent<TextMeshProUGUI>();
        aspdTMP = statContent.transform.Find("ASPD").GetComponent<TextMeshProUGUI>();
        armorTMP = statContent.transform.Find("Armor").GetComponent<TextMeshProUGUI>();
        mspdTMP = statContent.transform.Find("MSPD").GetComponent<TextMeshProUGUI>();
        dmgmodTMP = statContent.transform.Find("DMGMOD").GetComponent<TextMeshProUGUI>();
        omnivampTMP = statContent.transform.Find("Omnivamp").GetComponent<TextMeshProUGUI>();
        aTKTMP = statContent.transform.Find("ATK").GetComponent<TextMeshProUGUI>();
        critTMP = statContent.transform.Find("Crit").GetComponent<TextMeshProUGUI>();
        critModTMP = statContent.transform.Find("CritMod").GetComponent<TextMeshProUGUI>();
        atkrangeTMP = statContent.transform.Find("ATKRange").GetComponent<TextMeshProUGUI>();
        abilitySV = container.transform.Find("AbilitySV").gameObject;
        abilityTMP = abilitySV
            .transform.Find("Viewport/Content")
            .transform.GetComponent<TextMeshProUGUI>();
        itemSV = container.transform.Find("ItemSV").gameObject;
        itemSVContent = itemSV.transform.Find("Viewport/Content").gameObject;
        activeContent = statSV;
        tooltip = transform.Find("Tooltip").gameObject;
        tooltipTMP = tooltip
            .transform.Find("Background/TooltipSV/Viewport/Content")
            .GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        InitTabButtons();
        AddEvents();
        InitOtherButtons();
    }

    private void InitOtherButtons()
    {
        tooltip.transform.Find("Background/X").GetComponent<PointerDownUI>().pointerDownEvent = (
            evt
        ) => tooltip.SetActive(false);
    }

    private void AddEvents()
    {
        abilityTMP.GetComponent<DoubleTapUI>().doubleTapEvent = (evt) =>
        {
            tooltip.SetActive(true);
            tooltipTMP.text = TooltipHelper.GenerateTooltip(abilityTMP.text);
        };

        tooltipTMP.GetComponent<DoubleTapUI>().doubleTapEvent = (evt) =>
        {
            if (TooltipHelper.GenerateTooltip(tooltipTMP.text) is { Length: > 0 } result)
                tooltipTMP.text = result;
        };
    }

    private void InitTabButtons()
    {
        var tabButtons = GetComponentsInChildren<InfoPanelTabButton>();
        foreach (InfoPanelTabButton iptb in tabButtons)
        {
            switch (iptb.name)
            {
                case "X":
                {
                    xIPTB = iptb;
                    break;
                }
                case "Stat":
                {
                    statIPTB = iptb;
                    selectedIPTB = iptb;
                    break;
                }
                case "Ability":
                {
                    abilityIPTB = iptb;
                    break;
                }
                case "Item":
                {
                    itemIPTB = iptb;
                    break;
                }
                default:
                    break;
            }

            iptb.click += () => OnTabClick(iptb);
        }
    }

    public void OnTabClick(InfoPanelTabButton iptb)
    {
        switch (iptb.name)
        {
            case "X":
            {
                gameObject.SetActive(false);
                break;
            }
            case "Stat":
            {
                SwitchTab(iptb);
                SwitchActiveContent(statSV);
                break;
            }
            case "Ability":
            {
                SwitchTab(iptb);
                SwitchActiveContent(abilitySV);
                break;
            }
            case "Item":
            {
                SwitchTab(iptb);
                SwitchActiveContent(itemSV);
                break;
            }
            default:
                break;
        }
    }

    void SwitchTab(InfoPanelTabButton iptb)
    {
        if (selectedIPTB != null)
        {
            selectedIPTB.ChangeToNormal();
        }
        selectedIPTB = iptb;
        selectedIPTB.ChangeToHighLight();
    }

    void SwitchActiveContent(GameObject newActiveContent)
    {
        activeContent.SetActive(false);
        activeContent = newActiveContent;
        activeContent.SetActive(true);
    }

    public void Init(CustomMono p_owner)
    {
        owner = p_owner;
        nameTMP.text = owner.name;
        for (int i = 1; i < owner.skill.skillDataSOs.Count; i++)
        {
            abilityTMP.text += $"{owner.skill.skillDataSOs[i].skillDescription}\n\n";
        }
        DataBinding(p_owner);
    }

    private void DataBinding(CustomMono p_owner)
    {
        p_owner.stat.healthPoint.finalValueChangeEvent += HPChange;
        p_owner.stat.healthRegen.finalValueChangeEvent += HPRegenChange;
        p_owner.stat.manaPoint.finalValueChangeEvent += MPChange;
        p_owner.stat.manaRegen.finalValueChangeEvent += MPRegenChange;
        p_owner.stat.might.finalValueChangeEvent += MightChange;
        p_owner.stat.reflex.finalValueChangeEvent += ReflexChange;
        p_owner.stat.wisdom.finalValueChangeEvent += WisdomChange;
        p_owner.stat.attackSpeed.finalValueChangeEvent += ASPDChange;
        p_owner.stat.armor.finalValueChangeEvent += ArmorChange;
        p_owner.stat.moveSpeed.finalValueChangeEvent += MoveSpeedChange;
        p_owner.stat.damageModifier.finalValueChangeEvent += DamageModifierChange;
        p_owner.stat.omnivamp.finalValueChangeEvent += OmnivampChange;
        p_owner.stat.attackDamage.finalValueChangeEvent += ATKChange;
        p_owner.stat.critChance.finalValueChangeEvent += CritChange;
        p_owner.stat.critDamageModifier.finalValueChangeEvent += CritModChange;
        p_owner.stat.attackRange.finalValueChangeEvent += AttackRangeChange;
    }

    void HPChange()
    {
        hpTMP.text = $"HP: {owner.stat.healthPoint.FinalValue:F2} ❤️";
    }

    void HPRegenChange()
    {
        hpRegenTMP.text = $"HPREGEN: {owner.stat.healthRegen.FinalValue:F2} 🌿";
    }

    void MPChange()
    {
        mpTMP.text = $"MP: {owner.stat.manaPoint.FinalValue:F2} 💙";
    }

    void MPRegenChange()
    {
        mpRegenTMP.text = $"MPREGEN: {owner.stat.manaRegen.FinalValue:F2} 💧";
    }

    void MightChange()
    {
        mightTMP.text = $"MIGHT: {owner.stat.might.FinalValue:F2} 💪";
    }

    void ReflexChange()
    {
        reflexTMP.text = $"REFLEX: {owner.stat.reflex.FinalValue:F2} ⚡";
    }

    void WisdomChange()
    {
        wisdomTMP.text = $"WISDOM: {owner.stat.wisdom.FinalValue:F2} 🧠";
    }

    void ASPDChange()
    {
        aspdTMP.text = $"ASPD: {owner.stat.attackSpeed.FinalValue:F2} ⚔️";
    }

    void ArmorChange()
    {
        armorTMP.text = $"ARMOR: {owner.stat.armor.FinalValue:F2} 🛡️";
    }

    void MoveSpeedChange()
    {
        mspdTMP.text = $"MSPD: {owner.stat.moveSpeed.FinalValue:F2} 🏃";
    }

    void DamageModifierChange()
    {
        dmgmodTMP.text = $"DMGMOD: {owner.stat.damageModifier.FinalValue:F2} 💥";
    }

    void OmnivampChange()
    {
        omnivampTMP.text = $"OMNIVAMP: {owner.stat.omnivamp.FinalValue:F2} 🍃";
    }

    void ATKChange()
    {
        aTKTMP.text = $"ATK: {owner.stat.attackDamage.FinalValue:F2} 🗡️";
    }

    void CritChange()
    {
        critTMP.text = $"CRIT: {owner.stat.critChance.FinalValue:F2} 🎯";
    }

    void CritModChange()
    {
        critModTMP.text = $"CRITMOD: {owner.stat.critDamageModifier.FinalValue:F2} 💢";
    }

    void AttackRangeChange()
    {
        atkrangeTMP.text = $"ATKRANGE: {owner.stat.attackRange.FinalValue:F2} 🎯";
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }

    public void AttachItem(Item itemUI)
    {
        itemUI.transform.SetParent(itemSVContent.transform, false);
        itemUI.rectTransform.anchoredPosition = Vector2.zero;
        itemUI.Attach(this);
        itemUI.SetAsEquipped();
    }

    public void DetachItem(Item itemUI)
    {
        itemUI.transform.SetParent(GameUIManager.Instance.freeZone.transform);
        itemUI.Detach();
        itemUI.SetAsNone();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void ShowIfOwnerIsActivated()
    {
        if (owner.gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void ActivateGlassEffect()
    {
        canvasGroup.alpha = 0.3f;
    }

    public void DeactivateGlassEffect()
    {
        canvasGroup.alpha = 1f;
    }
}
