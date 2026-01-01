using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum WareType
{
    Champion,
    Item,
}

public enum TraderWareResult
{
    Buy,
    None,
}

public class TraderWare : DoubleTapUI, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    TextMeshProUGUI priceTMP;
    public WareType type;
    List<RaycastResult> rcResults = new();
    public TraderWareResult result = TraderWareResult.None;
    Vector3 localPos;
    object ware;

    ChampionRewardUI ChampionRewardUI() => ware as ChampionRewardUI;

    Item Item() => ware as Item;

    private void Awake()
    {
        priceTMP = transform.Find("PriceTMP").GetComponent<TextMeshProUGUI>();
        localPos = transform.localPosition;
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        doubleTapEvent += HandleDoubleTap;
    }

    void HandleDoubleTap(PointerEventData pointerEventData)
    {
        switch (type)
        {
            case WareType.Champion:
            {
                ChampionRewardUI().ShowInfoPanel(null);
                break;
            }
            case WareType.Item:
            {
                Item().ShowTooltip();
                break;
            }
            default:
                break;
        }
        transform.SetAsLastSibling();
    }

    public void SetChampionWare(ChampionRewardUI championRewardUI)
    {
        Show();
        ware = championRewardUI;
        type = WareType.Champion;
        priceTMP.text = championRewardUI.rewardCD.price.ToString();
        championRewardUI.SetAsShopWare(transform);
    }

    public void SetItemWare(Item item)
    {
        Show();
        ware = item;
        type = WareType.Item;
        priceTMP.text = item.itemDataSO.price.ToString();
        item.SetAsShopWare(transform);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameUIManager.Instance.TurnOnBuyZone();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GameManager.Instance.camera.ScreenToWorldPoint(
            eventData.position.WithZ(GameUIManager.Instance.worldSpacePlaneDistance)
        );

        rcResults.Clear();
        EventSystem.current.RaycastAll(eventData, rcResults);

        result = TraderWareResult.None;

        for (int i = 0; i < rcResults.Count; i++)
        {
            if (rcResults[i].gameObject.CompareTag("BuyZone"))
            {
                result = TraderWareResult.Buy;
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (result == TraderWareResult.Buy)
        {
            switch (type)
            {
                case WareType.Champion:
                {
                    if (GameManager.Instance.BuyChampion(ChampionRewardUI()))
                    {
                        Hide();
                    }

                    EndBuy();
                    break;
                }
                case WareType.Item:
                {
                    if (GameManager.Instance.BuyItem(Item()))
                    {
                        ware = null;
                        Hide();
                    }
                    EndBuy();
                    break;
                }
                default:
                    break;
            }
        }
        else
        {
            EndBuy();
        }
    }

    void EndBuy()
    {
        GameUIManager.Instance.TurnOffBuyZone();
        ToOriginalPosition();
    }

    void ToOriginalPosition() => transform.DOLocalMove(localPos, 0.5f).SetEase(Ease.OutQuart);

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);

    public void ScheduleClearWare()
    {
        if (ware != null)
        {
            switch (type)
            {
                case WareType.Champion:
                {
                    ChampionRewardUI().deactivate();
                    break;
                }
                case WareType.Item:
                {
                    Item().deactivate();
                    break;
                }
                default:
                    break;
            }
        }
    }
}
