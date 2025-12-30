using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DoubleTapShowTooltipUI : DoubleTapUI
{
    protected TextMeshProUGUI nameTMP,
        descriptionTMP;
    DoubleTapUI textContainer;
    GameObject tooltip;
    PointerDownUI exitButton;

#if false
    AC
        MB own
        ctor()

    DTSTU : AC{
        tmp n, dsc;
        dtu tc;
        tt;
        eb;

        ctor : b()
            tt = own.
    }
#endif

    private void Awake()
    {
        tooltip = transform.Find("Tooltip").gameObject;
        nameTMP = tooltip.transform.Find("NameBackground/Name").GetComponent<TextMeshProUGUI>();
        textContainer = tooltip
            .transform.Find("TooltipSV/Viewport/Content")
            .GetComponent<DoubleTapUI>();
        descriptionTMP = textContainer.GetComponent<TextMeshProUGUI>();
        exitButton = tooltip.transform.Find("X").GetComponent<PointerDownUI>();
        doubleTapEvent += ShowTooltip;
        textContainer.doubleTapEvent += GetTooltipForDescription;
        exitButton.pointerDownEvent += CloseTooltip;
    }

    void ShowTooltip(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        ResetDescription();
    }

    void CloseTooltip(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }

    public virtual void ResetDescription() { }

    public virtual void GetTooltipForDescription(PointerEventData pointerDownEvent) { }

    public virtual void SetupTooltip() { }
}
