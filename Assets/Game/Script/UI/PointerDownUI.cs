using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownUI : MonoBehaviour, IPointerDownHandler
{
    public Action<PointerEventData> pointerDownEvent = (p_eventData) => { };

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent(eventData);
    }
}
