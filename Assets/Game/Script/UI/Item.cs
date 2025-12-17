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

public class Item : MonoSelfAware, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemDataSO itemDataSO;
    public RectTransform rectTransform;
    List<RaycastResult> rcResults = new();
    ChampInfoPanel attachedTo = null;
    public ItemState state = ItemState.Inventory;
    DoubleTapUI doubleTapUI;
    ItemTooltip itemTooltip;
    public Dictionary<string, object> fields = new();
    UIEffect uIEffect;
    Animator animator;
    Image image;
    Vector2 rewardPos;
    float itemCycleOffset = 0,
        itemTierBlend = 0;

    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        doubleTapUI = GetComponent<DoubleTapUI>();
        uIEffect = GetComponent<UIEffect>();
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        RegisterCallback();
        InitTooltip();
    }

    private void OnEnable()
    {
        animator.SetFloat(GameManager.Instance.itemCycleOffsetFloatHash, itemCycleOffset);
        animator.SetFloat(GameManager.Instance.itemTierBlendHash, itemTierBlend);
    }

    void InitTooltip()
    {
        itemTooltip = Instantiate(GameManager.Instance.itemTooltipPrefab)
            .GetComponent<ItemTooltip>();
        itemTooltip.Init(this);
        itemTooltip.gameObject.SetActive(false);
    }

    public override void Start()
    {
        base.Start();
    }

    public void Init(ItemDataSO itemDataSO)
    {
        this.itemDataSO = itemDataSO;
        image.sprite = itemDataSO.icon;
        itemTooltip.Setup();
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

    private void RegisterCallback()
    {
        doubleTapUI.doubleTapEvent = ShowTooltip;
    }

    public void ShowTooltip(PointerEventData pointerEventData)
    {
        ShowTooltip();
    }

    void ShowTooltip()
    {
        itemTooltip.gameObject.SetActive(true);
        itemTooltip.ResetText(itemDataSO.itemDescription);
    }

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
        SwitchState(ItemState.Inventory);
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
        itemTooltip.gameObject.SetActive(false);
    }

    public void ShowReward()
    {
        gameObject.SetActive(true);
        itemTooltip.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        Destroy(itemTooltip);
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
}
