using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour, IPointerDownHandler
{
    public Image icon;
    public Action<PointerEventData> pointerDownEvent = (eventData) => { };

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent(eventData);
    }
}
