using System.Collections;
using System.Collections.Generic;
using System.Text;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ChampionRewardType
{
    Sacrifice,
    Select,
    None,
}

public class ChampionRewardUI : DoubleTapUI, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static Vector3 dragScale = new Vector3(0.25f, 0.25f, 1f);
    Vector3 originalAnchorLoc;
    RectTransform rectTransform;
    TextMeshProUGUI nameTMP,
        statTMP,
        abilityTMP,
        offerTMP;
    GameObject infoPanel,
        container,
        statSV,
        abilitySV,
        offerSV,
        activeContent;
    PointerDownUI tooltipCloseButton;
    ChampionReward championReward;
    Skill rewardSkill;
    ChampionData rewardCD;
    ChampionRewardType championRewardType = ChampionRewardType.None;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    Image image;
    UIEffect uIEffect;
    public InfoPanelTabButton selectedIPTB,
        xIPTB,
        statIPTB,
        abilityIPTB,
        offerIPTB;

    public void SetReward(ChampionReward cR)
    {
        gameObject.SetActive(true);
        championReward = cR;
        rewardSkill = cR.prefab.GetComponent<Skill>();
        rewardCD = cR.prefab.GetComponent<CustomMono>().championData;
        image.sprite = cR
            .prefab.transform.Find("DirectionModifier/MainComponent")
            .GetComponent<SpriteRenderer>()
            .sprite;
        SetStatText();
        SetAbilityText();
        StartCoroutine(EntranceIE());
    }

    private void SetAbilityText()
    {
        for (int i = 1; i < rewardSkill.skillDataSOs.Count; i++)
        {
            abilityTMP.text = "";
            abilityTMP.text += $"{rewardSkill.skillDataSOs[i].skillDescription}\n\n";
        }
    }

    private void SetStatText()
    {
        nameTMP.text = "Bruh";
        StringBuilder sb = new();

        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.HP, $": {rewardCD.healthPoint}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.HPREGEN,
                $": {rewardCD.healthRegen}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.MP, $": {rewardCD.manaPoint}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.MPREGEN,
                $": {rewardCD.manaRegen}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.MIGHT, $": {rewardCD.might}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.REFLEX, $": {rewardCD.reflex}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.WISDOM, $": {rewardCD.wisdom}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.ASPD,
                $": {rewardCD.attackSpeed}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.ARMOR, $": {rewardCD.armor}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(ComplexTextID.MSPD, $": {rewardCD.moveSpeed}")
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.DMGMOD,
                $": {rewardCD.damageModifier}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.OMNIVAMP,
                $": {rewardCD.omnivamp}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.ATK,
                $": {rewardCD.attackDamage}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.CRIT,
                $": {rewardCD.critChance}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.CRITMOD,
                $": {rewardCD.critDamageModifier}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.DMGREDUC,
                $": {rewardCD.damageReduction}"
            )
        );
        sb.AppendLine(
            GameManager.Instance.ConstructComplexText(
                ComplexTextID.ATKRANGE,
                $": {rewardCD.attackRange}"
            )
        );

        statTMP.text = sb.ToString();
    }

    IEnumerator EntranceIE()
    {
        yield return Random.Range(0, 0.25f);
        rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(
            rectTransform.anchoredPosition.y + 150f
        );
        rectTransform.DOAnchorPos(originalAnchorLoc, 0.5f).SetEase(Ease.OutBack);
    }

    private void Awake()
    {
        GetChildAndComponents();
        RegisterEvents();
        InitTabButtons();
    }

    private void InitTabButtons()
    {
        var tabButtons = GetComponentsInChildren<InfoPanelTabButton>(true);
        activeContent = statSV;
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
                case "Offer":
                {
                    offerIPTB = iptb;
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
                infoPanel.SetActive(false);
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
            case "Offer":
            {
                SwitchTab(iptb);
                SwitchActiveContent(offerSV);
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

    void GetChildAndComponents()
    {
        rectTransform = (RectTransform)transform;
        originalAnchorLoc = rectTransform.anchoredPosition3D;
        uIEffect = GetComponent<UIEffect>();
        image = GetComponent<Image>();
        infoPanel = transform.Find("InfoPanel").gameObject;
        container = infoPanel.transform.Find("Container").gameObject;
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statSV = container.transform.Find("StatSV").gameObject;
        abilitySV = container.transform.Find("AbilitySV").gameObject;
        offerSV = container.transform.Find("OfferSV").gameObject;
        statTMP = statSV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
        abilityTMP = abilitySV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
        offerTMP = offerSV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
    }

    private void RegisterEvents()
    {
        doubleTapEvent += DoubletapShowInfoPanel;
    }

    void DoubletapShowInfoPanel(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
        // tooltipTMP.text = TooltipHelper.GenerateTooltip(statUpgrade.description);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        uIEffect.LoadPreset(GameManager.Instance.championRewardSelectedEffectPreset);
        GameUIManager.Instance.ShowOnlyChampionRewardUI(this);
        GameUIManager.Instance.TurnOnChampionRewardSelectZone();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GameManager.Instance.camera.ScreenToWorldPoint(
            eventData.position.WithZ(GameUIManager.Instance.planeDistance)
        );

        rcResults.Clear();
        EventSystem.current.RaycastAll(eventData, rcResults);

        championRewardType = ChampionRewardType.None;

        for (int i = 0; i < rcResults.Count; i++)
        {
            if (rcResults[i].gameObject.CompareTag("ChampInfoPanel"))
            {
                attachedTo = rcResults[i].gameObject.GetComponent<ChampInfoPanel>();
                championRewardType = ChampionRewardType.Sacrifice;
                break;
            }
            else if (rcResults[i].gameObject.CompareTag("ChampionRewardSelectZone"))
            {
                championRewardType = ChampionRewardType.Select;
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.TurnOffChampionRewardSelectZone();
        switch (championRewardType)
        {
            case ChampionRewardType.Sacrifice:
            {
                GameManager.Instance.SacrificeChampionRewardAsStat(rewardCD, attachedTo.owner);
                GameUIManager.Instance.FinishReward();
                gameObject.SetActive(false);
                break;
            }
            case ChampionRewardType.Select:
            {
                GameManager.Instance.SpawnChampionForPlayer(championReward.prefab);
                GameUIManager.Instance.FinishReward();
                gameObject.SetActive(false);
                break;
            }
            case ChampionRewardType.None:
            {
                rectTransform.DOAnchorPos(originalAnchorLoc, 0.5f).SetEase(Ease.OutQuint);
                GameUIManager.Instance.ShowChampionRewardUIs();

                break;
            }
            default:
                break;
        }
        uIEffect.Clear();
    }
}
