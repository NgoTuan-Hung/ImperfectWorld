using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI textName,
        textDescription;
    public PointerDownUI equipButton,
        unequipButton,
        hideButton;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        hideButton.pointerDownEvent = HideTooltip;
    }

    void HideTooltip(PointerEventData p_pointerEventData) => gameObject.SetActive(false);
}
