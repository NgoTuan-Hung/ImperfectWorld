using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTooltip : MonoBehaviour, IPointerDownHandler
{
    TextMeshProUGUI textMeshProUGUI,
        nameTMP;
    PointerDownUI exitButton;
    DoubleTapUI textContainer;
    public Item owner;
    public static Vector3 offset = new Vector3(0.3f, 0.25f, 0);

    private void Awake()
    {
        nameTMP = transform.Find("NameBackground/Name").GetComponent<TextMeshProUGUI>();
        textContainer = transform.Find("TooltipSV/Viewport/Content").GetComponent<DoubleTapUI>();
        textMeshProUGUI = textContainer.GetComponent<TextMeshProUGUI>();
        exitButton = transform.Find("X").GetComponent<PointerDownUI>();
        transform.SetParent(GameUIManager.Instance.freeZone.transform);
        (transform as RectTransform).localScale = Vector3.one;
        RegisterCallback();
    }

    private void RegisterCallback()
    {
        textContainer.doubleTapEvent = (evt) =>
        {
            if (TooltipHelper.GenerateTooltip(textMeshProUGUI.text) is { Length: > 0 } result)
                textMeshProUGUI.text = result;
        };
        exitButton.pointerDownEvent = (evt) => gameObject.SetActive(false);
    }

    public void Init(Item owner)
    {
        this.owner = owner;
    }

    public void Setup()
    {
        nameTMP.text = owner.itemDataSO.itemName;
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void ResetText(string text)
    {
        textMeshProUGUI.text = text;
    }
}
