using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ChampionRewardResult
{
    Sacrifice,
    Select,
    None,
}

public enum ChampionRewardUIState
{
    Reward,
    ShopWare,
}

public class ChampionRewardUI
    : MonoSelfAware,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IPointerClickHandler
{
    ChampionRewardUIState state = ChampionRewardUIState.Reward;
    Vector3 originalAnchorLoc;
    RectTransform rectTransform;
    TextMeshProUGUI nameTMP,
        statTMP,
        abilityTMP,
        offerTMP,
        tooltipTMP;
    GameObject infoPanel,
        container,
        statSV,
        abilitySV,
        offerSV,
        activeContent,
        tooltip;
    PointerDownUI tooltipCloseButton;
    public ChampionReward championReward;
    Skill rewardSkill;
    public ChampionData rewardCD;
    ChampionRewardResult championRewardResult = ChampionRewardResult.None;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    Image image;
    UIEffect uIEffect;
    public InfoPanelTabButton selectedIPTB,
        xIPTB,
        statIPTB,
        abilityIPTB,
        offerIPTB;
    DoubleTapUI statDTU,
        abilityDTU,
        offerDTU;

    public void SetAsReward(Transform parent, Vector2 localPos)
    {
        originalAnchorLoc = localPos;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
        transform.localScale = Vector3.one;
        image.raycastTarget = true;
        StartCoroutine(EntranceIE());
        SwitchState(ChampionRewardUIState.Reward);
    }

    public void SetAsShopWare(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        transform.localScale = GameManager.Instance.screenSpaceToWorldSpaceUIScale;
        image.raycastTarget = false;
        SwitchState(ChampionRewardUIState.ShopWare);
    }

    public void Init(ChampionReward cR)
    {
        championReward = cR;
        rewardSkill = cR.prefab.GetComponent<Skill>();
        rewardCD = cR.prefab.GetComponent<CustomMono>().championData;
        image.sprite = cR
            .prefab.transform.Find("DirectionModifier/MainComponent")
            .GetComponent<SpriteRenderer>()
            .sprite;
        SetStatText();
        SetAbilityText();
        SetOfferText();
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
        nameTMP.text = championReward.prefab.name;
        statTMP.text = rewardCD.GetPrecomputeData().statDescription;
    }

    private void SetOfferText()
    {
        offerTMP.text = rewardCD.GetPrecomputeData().offerDescription;
    }

    IEnumerator EntranceIE()
    {
        yield return Random.Range(0, 0.25f);
        rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(
            rectTransform.anchoredPosition.y + 150f
        );
        yield return rectTransform
            .DOAnchorPos(originalAnchorLoc, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(ShowInfoPanelAfterEntrance);
    }

    void ShowInfoPanelAfterEntrance()
    {
        infoPanel.SetActive(true);
        OnTabClick(statIPTB);
    }

    public override void Awake()
    {
        base.Awake();
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
        tooltip = infoPanel.transform.Find("Tooltip").gameObject;
        tooltipTMP = tooltip
            .transform.Find("Background/TooltipSV/Viewport/Content")
            .GetComponent<TextMeshProUGUI>();
        tooltipCloseButton = tooltip.transform.Find("Background/X").GetComponent<PointerDownUI>();
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statSV = container.transform.Find("StatSV").gameObject;
        abilitySV = container.transform.Find("AbilitySV").gameObject;
        offerSV = container.transform.Find("OfferSV").gameObject;
        statTMP = statSV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
        abilityTMP = abilitySV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
        offerTMP = offerSV.transform.Find("Viewport/Content").GetComponent<TextMeshProUGUI>();
        statDTU = statTMP.GetComponent<DoubleTapUI>();
        abilityDTU = abilityTMP.GetComponent<DoubleTapUI>();
        offerDTU = offerTMP.GetComponent<DoubleTapUI>();
    }

    private void RegisterEvents()
    {
        doubleTapEvent += DoubleTapShowInfoPanel;
        statDTU.doubleTapEvent += ShowTooltipForStat;
        abilityDTU.doubleTapEvent += ShowTooltipForAbility;
        offerDTU.doubleTapEvent += ShowTooltipForOffer;
        tooltipCloseButton.pointerDownEvent += CloseTooltip;
    }

    void DoubleTapShowInfoPanel(PointerEventData eventData) => infoPanel.SetActive(true);

    void ShowTooltipForStat(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        tooltipTMP.text = TooltipHelper.GenerateTooltip(statTMP.text);
    }

    void ShowTooltipForAbility(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        tooltipTMP.text = TooltipHelper.GenerateTooltip(abilityTMP.text);
    }

    void ShowTooltipForOffer(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        tooltipTMP.text = TooltipHelper.GenerateTooltip(offerTMP.text);
    }

    void CloseTooltip(PointerEventData eventData) => tooltip.SetActive(false);

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

        championRewardResult = ChampionRewardResult.None;
        bool breakLoop = false;
        for (int i = 0; i < rcResults.Count; i++)
        {
            switch (rcResults[i].gameObject.tag)
            {
                case "ChampInfoPanel":
                    attachedTo = rcResults[i].gameObject.GetComponent<ChampInfoPanel>();
                    championRewardResult = ChampionRewardResult.Sacrifice;
                    breakLoop = true;
                    break;
                case "ChampionRewardSelectZone":
                    championRewardResult = ChampionRewardResult.Select;
                    breakLoop = true;
                    break;
                default:
                    break;
            }

            if (breakLoop)
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.TurnOffChampionRewardSelectZone();
        switch (championRewardResult)
        {
            case ChampionRewardResult.Sacrifice:
            {
                GameManager.Instance.SacrificeChampionRewardAsStat(rewardCD, attachedTo.owner);
                GameUIManager.Instance.FinishChampionReward();
                gameObject.SetActive(false);
                break;
            }
            case ChampionRewardResult.Select:
            {
                GameManager.Instance.RewardChampion(championReward.prefab);
                GameUIManager.Instance.FinishChampionReward();
                gameObject.SetActive(false);
                break;
            }
            case ChampionRewardResult.None:
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

    public Action<PointerEventData> doubleTapEvent = (p_eventData) => { };
    public float lastClickTime = 0f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickTime - lastClickTime < 0.25f)
            doubleTapEvent(eventData);
        lastClickTime = eventData.clickTime;
    }

    void SwitchState(ChampionRewardUIState championRewardUIState)
    {
        state = championRewardUIState;
    }
}
