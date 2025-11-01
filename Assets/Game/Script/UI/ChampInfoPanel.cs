using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ChampInfoPanel : MonoBehaviour
{
    CustomMono owner;
    GameObject container,
        statSV,
        statContent,
        abilitySV,
        activeContent,
        tooltip;
    TextMeshProUGUI nameTMP,
        hpTMP,
        mpTMP,
        mightTMP,
        reflexTMP,
        wisdomTMP,
        aspdTMP,
        armorTMP,
        omnivampTMP,
        aTKTMP,
        critTMP,
        critModTMP,
        abilityTMP,
        tooltipTMP;
    public static Vector3 offset = new(1.33942f, 2.614366f);
    ChampInfoPanelTabButton selectedCIPTB;

    private void Awake()
    {
        container = transform.Find("Container").gameObject;
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statSV = container.transform.Find("StatSV").gameObject;
        statContent = statSV.transform.Find("Viewport/Content").gameObject;
        hpTMP = statContent.transform.Find("HP").GetComponent<TextMeshProUGUI>();
        mpTMP = statContent.transform.Find("MP").GetComponent<TextMeshProUGUI>();
        mightTMP = statContent.transform.Find("Might").GetComponent<TextMeshProUGUI>();
        reflexTMP = statContent.transform.Find("Reflex").GetComponent<TextMeshProUGUI>();
        wisdomTMP = statContent.transform.Find("Wisdom").GetComponent<TextMeshProUGUI>();
        aspdTMP = statContent.transform.Find("ASPD").GetComponent<TextMeshProUGUI>();
        armorTMP = statContent.transform.Find("Armor").GetComponent<TextMeshProUGUI>();
        omnivampTMP = statContent.transform.Find("Omnivamp").GetComponent<TextMeshProUGUI>();
        aTKTMP = statContent.transform.Find("ATK").GetComponent<TextMeshProUGUI>();
        critTMP = statContent.transform.Find("Crit").GetComponent<TextMeshProUGUI>();
        critModTMP = statContent.transform.Find("CritMod").GetComponent<TextMeshProUGUI>();
        abilitySV = container.transform.Find("AbilitySV").gameObject;
        abilityTMP = abilitySV
            .transform.Find("Viewport/Content")
            .transform.GetComponent<TextMeshProUGUI>();
        activeContent = statSV;
        tooltip = transform.Find("Tooltip").gameObject;
        tooltipTMP = tooltip
            .transform.Find("Background/TooltipSV/Viewport/Content")
            .GetComponent<TextMeshProUGUI>();
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
        foreach (
            ChampInfoPanelTabButton ciptb in GetComponentsInChildren<ChampInfoPanelTabButton>()
        )
        {
            if (ciptb.name.Equals("Stat"))
                selectedCIPTB = ciptb;
            ciptb.click += () => OnTabClick(ciptb);
        }
    }

    void OnTabClick(ChampInfoPanelTabButton p_ciptb)
    {
        switch (p_ciptb.name)
        {
            case "X":
            {
                gameObject.SetActive(false);
                break;
            }
            case "Stat":
            {
                SwitchTab(p_ciptb);
                SwitchActiveContent(statSV);
                break;
            }
            case "Ability":
            {
                SwitchTab(p_ciptb);
                SwitchActiveContent(abilitySV);
                break;
            }
            case "Other":
            {
                SwitchTab(p_ciptb);
                break;
            }
            default:
                break;
        }
    }

    void SwitchTab(ChampInfoPanelTabButton p_ciptb)
    {
        if (selectedCIPTB != null)
        {
            selectedCIPTB.ChangeToNormal();
        }
        selectedCIPTB = p_ciptb;
        selectedCIPTB.ChangeToHighLight();
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
        p_owner.stat.manaPoint.finalValueChangeEvent += MPChange;
        p_owner.stat.might.finalValueChangeEvent += MightChange;
        p_owner.stat.reflex.finalValueChangeEvent += ReflexChange;
        p_owner.stat.wisdom.finalValueChangeEvent += WisdomChange;
        p_owner.stat.attackSpeed.finalValueChangeEvent += ASPDChange;
        p_owner.stat.armor.finalValueChangeEvent += ArmorChange;
        p_owner.stat.omnivamp.finalValueChangeEvent += OmnivampChange;
        p_owner.stat.attackDamage.finalValueChangeEvent += ATKChange;
        p_owner.stat.critChance.finalValueChangeEvent += CritChange;
        p_owner.stat.critDamageModifier.finalValueChangeEvent += CritModChange;
    }

    void HPChange()
    {
        hpTMP.text = $"HP: {owner.stat.healthPoint.FinalValue} ❤️";
    }

    void MPChange()
    {
        mpTMP.text = $"MP: {owner.stat.manaPoint.FinalValue} 💙";
    }

    void MightChange()
    {
        mightTMP.text = $"MIGHT: {owner.stat.might.FinalValue} 💪";
    }

    void ReflexChange()
    {
        reflexTMP.text = $"REFLEX: {owner.stat.reflex.FinalValue} ⚡";
    }

    void WisdomChange()
    {
        wisdomTMP.text = $"WISDOM: {owner.stat.wisdom.FinalValue} 🧠";
    }

    void ASPDChange()
    {
        aspdTMP.text = $"ASPD: {owner.stat.attackSpeed.FinalValue} ⚔️";
    }

    void ArmorChange()
    {
        armorTMP.text = $"ARMOR: {owner.stat.armor.FinalValue} 🛡️";
    }

    void OmnivampChange()
    {
        omnivampTMP.text = $"OMNIVAMP: {owner.stat.omnivamp.FinalValue} 🍃";
    }

    void ATKChange()
    {
        aTKTMP.text = $"ATK: {owner.stat.attackDamage.FinalValue} 🗡️";
    }

    void CritChange()
    {
        critTMP.text = $"CRIT: {owner.stat.critChance.FinalValue} 🎯";
    }

    void CritModChange()
    {
        critModTMP.text = $"CRITMOD: {owner.stat.critDamageModifier.FinalValue} 💢";
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }
}
