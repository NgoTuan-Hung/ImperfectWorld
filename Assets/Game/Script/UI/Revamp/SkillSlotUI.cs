using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour, IPointerDownHandler
{
    public Image border,
        icon;
    public Action<PointerEventData> pointerDownEvent = (p_eventData) => { };
    public SkillUseUI skillUseUI;
    public SkillNodeUI skillNodeUI;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent(eventData);
    }
}
