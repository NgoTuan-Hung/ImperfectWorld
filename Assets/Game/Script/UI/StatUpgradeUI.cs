using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatUpgradeUI : DoubleTapUI, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static Vector3 dragScale = new Vector3(0.25f, 0.25f, 1f);
    Vector3 originalAnchorLoc,
        originalScale;
    RectTransform rectTransform;
    TextMeshProUGUI textMeshProUGUI,
        tooltipTMP;
    GameObject tooltip;
    PointerDownUI tooltipCloseButton;
    StatUpgrade statUpgrade;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    Tween scaleTween;

    public void SetUpgrade(StatUpgrade statUpgrade)
    {
        gameObject.SetActive(true);
        this.statUpgrade = statUpgrade;
        textMeshProUGUI.text = statUpgrade.description;
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
        originalScale = rectTransform.localScale;
        textMeshProUGUI = transform.Find("TextContainer/Text").GetComponent<TextMeshProUGUI>();
        tooltip = transform.Find("Tooltip").gameObject;
        tooltipTMP = tooltip
            .transform.Find("Background/TooltipSV/Viewport/Content")
            .GetComponent<TextMeshProUGUI>();
        tooltipCloseButton = tooltip.transform.Find("Background/X").GetComponent<PointerDownUI>();

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        doubleTapEvent += DoubletapShowTooltip;
        tooltipCloseButton.pointerDownEvent += CloseTooltip;
    }

    void DoubletapShowTooltip(PointerEventData eventData)
    {
        ShowTooltip();
    }

    void ShowTooltip()
    {
        tooltip.SetActive(true);
        tooltipTMP.text = TooltipHelper.GenerateTooltip(statUpgrade.description);
    }

    void CloseTooltip(PointerEventData eventData) => tooltip.SetActive(false);

    public void OnBeginDrag(PointerEventData eventData)
    {
        scaleTween = rectTransform.DOScale(dragScale, 0.5f).SetEase(Ease.OutQuint);
        GameUIManager.Instance.ShowOnlyStatUpgradeUI(this);
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
            GameManager.Instance.UpgradeStat(attachedTo.owner, statUpgrade);
            GameUIManager.Instance.FinishReward();
            scaleTween.Kill();
            rectTransform.localScale = originalScale;
            gameObject.SetActive(false);
        }
        else
        {
            rectTransform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuint);
            rectTransform.DOAnchorPos(originalAnchorLoc, 0.5f).SetEase(Ease.OutQuint);
            GameUIManager.Instance.ShowStatUpgradeUIs();
        }
    }
}
