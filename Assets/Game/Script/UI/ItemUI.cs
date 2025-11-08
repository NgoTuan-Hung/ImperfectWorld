using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemState
{
    Inventory,
    Equipped,
    None,
}

public class ItemUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemDataSO itemDataSO;
    public RectTransform rectTransform;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    public ItemState state = ItemState.Inventory;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
