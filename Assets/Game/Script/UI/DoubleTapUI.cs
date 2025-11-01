using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleTapUI : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> doubleTapEvent = (p_eventData) => { };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
            doubleTapEvent(eventData);
    }
}
