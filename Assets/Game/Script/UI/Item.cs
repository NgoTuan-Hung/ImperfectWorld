using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemState
{
    Inventory,
    Equipped,
    None,
}

public class Item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemDataSO itemDataSO;
    public RectTransform rectTransform;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    public ItemState state = ItemState.Inventory;
    DoubleTapUI doubleTapUI;
    ItemTooltip itemTooltip;
    public Dictionary<string, object> fields = new();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        doubleTapUI = GetComponent<DoubleTapUI>();
        RegisterCallback();
    }

    private void Start()
    {
        itemTooltip = Instantiate(GameManager.Instance.itemTooltipPrefab)
            .GetComponent<ItemTooltip>();
        itemTooltip.Init(this);
        itemTooltip.gameObject.SetActive(false);
    }

    private void RegisterCallback()
    {
        doubleTapUI.doubleTapEvent = (evt) =>
        {
            itemTooltip.gameObject.SetActive(true);
            itemTooltip.ResetText(itemDataSO.itemDescription);
        };
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
            attachedTo.AttachItem(this);
            state = ItemState.Equipped;
        }
        else
        {
            GameUIManager.Instance.AddToInventory(this);
            state = ItemState.Inventory;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (state)
        {
            case ItemState.Inventory:
            {
                GameUIManager.Instance.RemoveFromInventory(this);
                state = ItemState.None;
                break;
            }
            case ItemState.Equipped:
            {
                attachedTo.DetachItem(this);
                attachedTo = null;
                state = ItemState.None;
                break;
            }
            default:
                break;
        }
    }
}
