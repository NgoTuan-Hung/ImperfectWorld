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

public class TraderWare : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    TextMeshProUGUI priceTMP;
    public WareType type;
    List<RaycastResult> rcResults = new();
    public TraderWareResult result = TraderWareResult.None;
    Vector3 localPos;
    object ware;

    ChampionRewardUI ChampionRewardUI() => ware as ChampionRewardUI;

    private void Awake()
    {
        priceTMP = transform.Find("PriceTMP").GetComponent<TextMeshProUGUI>();
        localPos = transform.localPosition;
    }

    public void SetChampionWare(ChampionRewardUI championRewardUI)
    {
        ware = championRewardUI;
        type = WareType.Champion;
        priceTMP.text = championRewardUI.rewardCD.price.ToString();
        championRewardUI.SetAsShopWare(transform);
    }

    public void SetItemWare(Item item)
    {
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
                        ChampionRewardUI().deactivate();

                    EndBuy();
                    break;
                }
                case WareType.Item:
                {
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
}
