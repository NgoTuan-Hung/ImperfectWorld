using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Relic : MonoSelfAware, IDoubleTapShowTooltipBehavior
{
    public RelicDataSO relicDataSO;
    public RelicBehavior relicBehavior;
    public Image image;
    DoubleTapShowTooltipUI doubleTapShowTooltipUI;

    public override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        doubleTapShowTooltipUI = GetComponent<DoubleTapShowTooltipUI>();
        InitComponents();
    }

    private void InitComponents()
    {
        doubleTapShowTooltipUI.Init(
            this,
            new(-0.20f, -0.28f),
            GameManager.Instance.relicTooltipColor
        );
    }

    public Relic Setup(RelicDataSO relicDataSO)
    {
        this.relicDataSO = relicDataSO;
        image.sprite = relicDataSO.icon;
        var type = GameManager.Instance.GetRelicBehaviorType(relicDataSO.relicBehavior);
        if (type != null)
            relicBehavior = Activator.CreateInstance(type) as RelicBehavior;
        SetupTooltip();

        return this;
    }

    public void ResetDescription()
    {
        doubleTapShowTooltipUI.SetDescription(relicDataSO.description);
    }

    public void GetTooltipForDescription(PointerEventData pointerDownEvent)
    {
        doubleTapShowTooltipUI.SetTooltipFor(relicDataSO.description);
    }

    public void SetupTooltip()
    {
        StartCoroutine(doubleTapShowTooltipUI.SetName(relicDataSO.relicName));
    }
}
