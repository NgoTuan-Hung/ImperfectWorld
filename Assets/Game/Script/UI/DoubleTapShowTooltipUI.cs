using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleTapShowTooltipUI : DoubleTapUI
{
    DoubleTapTooltip tooltip;
    IDoubleTapShowTooltipBehavior doubleTapShowTooltipBehavior;

    public void Init(
        IDoubleTapShowTooltipBehavior doubleTapShowTooltipBehavior,
        Vector3 tooltipOffset,
        Color tooltipColor
    )
    {
        this.doubleTapShowTooltipBehavior = doubleTapShowTooltipBehavior;
        tooltip = GameUIManager.Instance.GetNewDoubleTapTooltip().GetComponent<DoubleTapTooltip>();
        tooltip.Init(transform, tooltipOffset, tooltipColor);
        tooltip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        tooltip.gameObject.SetActive(false);
    }

    private void Start()
    {
        doubleTapEvent += ShowTooltip;
        tooltip.textContainer.doubleTapEvent +=
            doubleTapShowTooltipBehavior.GetTooltipForDescription;
        tooltip.exitButton.pointerDownEvent += CloseTooltip;
    }

    public void ShowTooltip(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
        doubleTapShowTooltipBehavior.ResetDescription();
    }

    void CloseTooltip(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }

    public void SetDescription(string description) => tooltip.descriptionTMP.text = description;

    public void SetTooltipFor(string description)
    {
        if (TooltipHelper.GenerateTooltip(description) is { Length: > 0 } result)
            tooltip.descriptionTMP.text = result;
    }

    public IEnumerator SetName(string name)
    {
        while (tooltip.nameTMP == null)
            yield return null;

        tooltip.nameTMP.text = name;
    }

    private void OnDestroy()
    {
        Destroy(tooltip.gameObject);
    }
}
