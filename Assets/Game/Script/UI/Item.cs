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
        animator.SetFloat(GameManager.Instance.itemCycleOffsetFloatHash, Random.Range(0, 1f));
        switch (itemDataSO.itemTier)
        {
            case ItemTier.Normal:
                uIEffect.Clear();
                animator.SetFloat(GameManager.Instance.itemTierBlendHash, 0);
                break;
            case ItemTier.Rare:
                uIEffect.LoadPreset(GameManager.Instance.rareItemEffectPreset);
                animator.SetFloat(GameManager.Instance.itemTierBlendHash, 1);
                break;
            case ItemTier.Epic:
                uIEffect.LoadPreset(GameManager.Instance.epicItemEffectPreset);
                animator.SetFloat(GameManager.Instance.itemTierBlendHash, 1);
                break;
            case ItemTier.Legendary:
            default:
                break;
        }
    }

    public void SetAsReward(Transform parent, Vector2 localPos)
    {
        transform.SetParent(parent, false);
        StartCoroutine(EntranceIE(localPos));
        SwitchState(ItemState.Reward);
    }

    IEnumerator EntranceIE(Vector2 localPos)
    {
        yield return Random.Range(0, 0.25f);
        rectTransform.anchoredPosition = localPos + new Vector2(0, 150f);
        rectTransform.DOAnchorPos(localPos, 0.5f).SetEase(Ease.OutBack);
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

    public void SwitchState(ItemState state)
    {
        this.state = state;
    }
}
