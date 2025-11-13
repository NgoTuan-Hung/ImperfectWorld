using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleTapUI : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> doubleTapEvent = (p_eventData) => { };
    public float lastClickTime = 0f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickTime - lastClickTime < 0.25f)
            doubleTapEvent(eventData);
        lastClickTime = eventData.clickTime;
    }
}
