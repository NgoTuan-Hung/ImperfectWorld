using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ChampInfoPanel : MonoBehaviour
{
    //
#if false
    CustomMono.Start
        GenerateAndBindChampUI => GUIM.GenerateAndBindChampUI

    GUIM.GenerateAndBindChampUI
        champUI = instantiate
        ChampUIDict.Add(customMono, champUI)

    ChampUI
        customMono
        Init()
            customMomo = cM
            name = name
            ValueChangeRegister
        FixUpdate
            transform.pos = cM.pos
#endif
    CustomMono owner;
    GameObject container,
        statContent;
    TextMeshProUGUI nameTMP,
        hpTMP,
        mpTMP,
        mightTMP,
        reflexTMP,
        wisdomTMP,
        aspdTMP,
        armorTMP,
        omnivampTMP;
    public static Vector3 offset = new(1.33942f, 2.614366f);
    ChampInfoPanelTabButton selectedCIPTB;

    private void Awake()
    {
        container = transform.Find("Container").gameObject;
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statContent = container.transform.Find("StatSV/Viewport/Content").gameObject;
        hpTMP = statContent.transform.Find("HP").GetComponent<TextMeshProUGUI>();
        mpTMP = statContent.transform.Find("MP").GetComponent<TextMeshProUGUI>();
        mightTMP = statContent.transform.Find("Might").GetComponent<TextMeshProUGUI>();
        reflexTMP = statContent.transform.Find("Reflex").GetComponent<TextMeshProUGUI>();
        wisdomTMP = statContent.transform.Find("Wisdom").GetComponent<TextMeshProUGUI>();
        aspdTMP = statContent.transform.Find("ASPD").GetComponent<TextMeshProUGUI>();
        armorTMP = statContent.transform.Find("Armor").GetComponent<TextMeshProUGUI>();
        omnivampTMP = statContent.transform.Find("Omnivamp").GetComponent<TextMeshProUGUI>();
        InitTabButtons();
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
                break;
            }
            case "Ability":
            {
                SwitchTab(p_ciptb);
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

    public void Init(CustomMono p_owner)
    {
        owner = p_owner;
        nameTMP.text = owner.name;
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
        mightTMP.text = $"Might: {owner.stat.might.FinalValue} 💪";
    }

    void ReflexChange()
    {
        reflexTMP.text = $"Reflex: {owner.stat.reflex.FinalValue} ⚡";
    }

    void WisdomChange()
    {
        wisdomTMP.text = $"Wisdom: {owner.stat.wisdom.FinalValue} 🧠";
    }

    void ASPDChange()
    {
        aspdTMP.text = $"ASPD: {owner.stat.attackSpeed.FinalValue} ⚔️";
    }

    void ArmorChange()
    {
        armorTMP.text = $"Armor: {owner.stat.armor.FinalValue} 🛡️";
    }

    void OmnivampChange()
    {
        omnivampTMP.text = $"Omnivamp: {owner.stat.omnivamp.FinalValue} 🍃";
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }
}
