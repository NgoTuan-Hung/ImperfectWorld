using UnityEngine.EventSystems;

public interface IDoubleTapShowTooltipBehavior
{
    public void ResetDescription();

    public void GetTooltipForDescription(PointerEventData pointerDownEvent);

    public void SetupTooltip();
}
