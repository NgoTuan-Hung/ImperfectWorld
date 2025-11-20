using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChampionRewardUI : DoubleTapUI, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static Vector3 dragScale = new Vector3(0.25f, 0.25f, 1f);
    Vector3 originalAnchorLoc;
    RectTransform rectTransform;
    TextMeshProUGUI tooltipTMP;
    GameObject tooltip;
    PointerDownUI tooltipCloseButton;
    ChampionReward championReward;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    Image image;
    UIEffect uIEffect;

    public void SetReward(ChampionReward cR)
    {
        gameObject.SetActive(true);
        championReward = cR;
        image.sprite = cR
            .prefab.transform.Find("DirectionModifier/MainComponent")
            .GetComponent<SpriteRenderer>()
            .sprite;
        StartCoroutine(EntranceIE());
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
        rectTransform = (RectTransform)transform;
        originalAnchorLoc = rectTransform.anchoredPosition3D;
        tooltip = transform.Find("Tooltip").gameObject;
        tooltipTMP = tooltip
            .transform.Find("Background/TooltipSV/Viewport/Content")
            .GetComponent<TextMeshProUGUI>();
        tooltipCloseButton = tooltip.transform.Find("Background/X").GetComponent<PointerDownUI>();
        uIEffect = GetComponent<UIEffect>();
        image = GetComponent<Image>();

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        doubleTapEvent += DoubletapShowTooltip;
        tooltipCloseButton.pointerDownEvent += CloseTooltip;
    }

    void DoubletapShowTooltip(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        // tooltipTMP.text = TooltipHelper.GenerateTooltip(statUpgrade.description);
    }

    void CloseTooltip(PointerEventData eventData) => tooltip.SetActive(false);

    public void OnBeginDrag(PointerEventData eventData)
    {
        uIEffect.LoadPreset(GameManager.Instance.championRewardSelectedEffectPreset);
        GameUIManager.Instance.ShowOnlyChampionRewardUI(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GameManager.Instance.camera.ScreenToWorldPoint(
            eventData.position.WithZ(GameUIManager.Instance.planeDistance)
        );

        rcResults.Clear();
        EventSystem.current.RaycastAll(eventData, rcResults);

        attachedTo = null;

        for (int i = 0; i < rcResults.Count; i++)
        {
            if (rcResults[i].gameObject.CompareTag("ChampInfoPanel"))
            {
                attachedTo = rcResults[i].gameObject.GetComponent<ChampInfoPanel>();
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (attachedTo != null)
        {
            // GameManager.Instance.UpgradeStat(attachedTo.owner, statUpgrade);
            GameUIManager.Instance.FinishReward();
            uIEffect.Clear();
            gameObject.SetActive(false);
        }
        else
        {
            uIEffect.Clear();
            rectTransform.DOAnchorPos(originalAnchorLoc, 0.5f).SetEase(Ease.OutQuint);
            GameUIManager.Instance.ShowChampionRewardUIs();
        }
    }
}
