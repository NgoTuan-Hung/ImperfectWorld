using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemState
{
    Inventory,
    Equipped,
    Reward,
    ShopWare,
    None,
}

public class Item
    : MonoSelfAware,
        IDragHandler,
        IEndDragHandler,
        IBeginDragHandler,
        IDoubleTapShowTooltipBehavior
{
    public ItemDataSO itemDataSO;
    public RectTransform rectTransform;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    public ItemState state = ItemState.Inventory;
    public Dictionary<string, object> fields = new();
    UIEffect uIEffect;
    Animator animator;
    Image image;
    Vector2 rewardPos;
    float itemCycleOffset = 0,
        itemTierBlend = 0;
    DoubleTapShowTooltipUI doubleTapShowTooltipUI;

    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        uIEffect = GetComponent<UIEffect>();
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        doubleTapShowTooltipUI = GetComponent<DoubleTapShowTooltipUI>();
        InitComponents();
        RegisterCallback();
    }

    private void InitComponents()
    {
        doubleTapShowTooltipUI.Init(this, new(0.24f, 0.22f), GameManager.Instance.itemTooltipColor);
    }

    private void OnEnable()
    {
        animator.SetFloat(GameManager.Instance.itemCycleOffsetFloatHash, itemCycleOffset);
        animator.SetFloat(GameManager.Instance.itemTierBlendHash, itemTierBlend);
    }

    public void Init(ItemDataSO itemDataSO)
    {
        this.itemDataSO = itemDataSO;
        image.sprite = itemDataSO.icon;
        SetupTooltip();
        itemCycleOffset = Random.Range(0, 1f);
        animator.SetFloat(GameManager.Instance.itemCycleOffsetFloatHash, itemCycleOffset);
        switch (itemDataSO.itemTier)
        {
            case ItemTier.Normal:
                uIEffect.Clear();
                itemTierBlend = 0;
                break;
            case ItemTier.Rare:
                uIEffect.LoadPreset(GameManager.Instance.rareItemEffectPreset);
                itemTierBlend = 1;
                break;
            case ItemTier.Epic:
                uIEffect.LoadPreset(GameManager.Instance.epicItemEffectPreset);
                itemTierBlend = 1;
                break;
            case ItemTier.Legendary:
            default:
                break;
        }

        animator.SetFloat(GameManager.Instance.itemTierBlendHash, itemTierBlend);
        image.raycastTarget = true;
        transform.localScale = Vector3.one;
        SwitchState(ItemState.None);
    }

    public void SetAsReward(Transform parent, Vector2 localPos)
    {
        transform.SetParent(parent, false);
        rewardPos = localPos;
        StartCoroutine(EntranceIE(localPos));
        SwitchState(ItemState.Reward);
    }

    public void SetAsShopWare(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        transform.localScale = GameManager.Instance.screenSpaceToWorldSpaceUIScale;
        image.raycastTarget = false;
        SwitchState(ItemState.ShopWare);
    }

    public void SetAsEquipped()
    {
        SwitchState(ItemState.Equipped);
    }

    public void SetAsInventory()
    {
        SwitchState(ItemState.Inventory);
    }

    public void SetAsNone() => SwitchState(ItemState.None);

    IEnumerator EntranceIE(Vector2 localPos)
    {
        yield return Random.Range(0, 0.25f);
        rectTransform.anchoredPosition = localPos + new Vector2(0, 150f);
        rectTransform.DOAnchorPos(localPos, 0.5f).SetEase(Ease.OutBack).OnComplete(ShowTooltip);
    }

    private void RegisterCallback() { }

    public void OnDrag(PointerEventData eventData)
    {
        if (state == ItemState.Equipped)
            if (!attachedTo.owner.CompareTag("Team1"))
                return;

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
        switch (state)
        {
            case ItemState.Inventory:
            {
                EquipOrToInventory();
                break;
            }
            case ItemState.Equipped:
            {
                if (!attachedTo.owner.CompareTag("Team1"))
                    return;
                EquipOrToInventory();
                break;
            }
            case ItemState.Reward:
            {
                EquipOrToReward();
                break;
            }
            case ItemState.None:
            {
                EquipOrToInventory();
                break;
            }
            default:
                break;
        }
    }

    void EquipOrToInventory()
    {
        if (attachedTo != null)
        {
            if (attachedTo.owner.stat.EquipItem(this))
                return;
        }

        GameUIManager.Instance.AddToInventory(this);
        SetAsInventory();
    }

    void EquipOrToReward()
    {
        if (attachedTo != null)
        {
            if (attachedTo.owner.stat.EquipItem(this))
            {
                GameUIManager.Instance.FinishItemReward(this);
                return;
            }
        }

        rectTransform.DOAnchorPos(rewardPos, 0.5f).SetEase(Ease.OutQuint);
        GameUIManager.Instance.ShowItemRewardUIs();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (state)
        {
            case ItemState.Inventory:
            {
                GameUIManager.Instance.RemoveFromInventory(this);
                break;
            }
            case ItemState.Equipped:
            {
                if (!attachedTo.owner.CompareTag("Team1"))
                    return;
                attachedTo.owner.stat.UnEquipItem(this);
                break;
            }
            case ItemState.Reward:
            {
                GameUIManager.Instance.ShowOnlyItemReward(this);
                break;
            }
            default:
                break;
        }
    }

    public void SwitchState(ItemState state)
    {
        this.state = state;
    }

    public void HideReward()
    {
        gameObject.SetActive(false);
    }

    public void ShowReward()
    {
        gameObject.SetActive(true);
    }

    public void GetBought()
    {
        GameUIManager.Instance.AddToInventory(this);
        image.raycastTarget = true;
        transform.localScale = Vector3.one;
        SetAsInventory();
    }

    public void Attach(ChampInfoPanel champInfoPanel)
    {
        attachedTo = champInfoPanel;
    }

    public void Detach()
    {
        attachedTo = null;
    }

    public void ResetDescription()
    {
        doubleTapShowTooltipUI.SetDescription(itemDataSO.itemDescription);
    }

    public void GetTooltipForDescription(PointerEventData pointerDownEvent)
    {
        doubleTapShowTooltipUI.SetTooltipFor(itemDataSO.itemDescription);
    }

    public void SetupTooltip()
    {
        StartCoroutine(doubleTapShowTooltipUI.SetName(itemDataSO.itemName));
    }

    public void ShowTooltip() => doubleTapShowTooltipUI.ShowTooltip(null);
}
