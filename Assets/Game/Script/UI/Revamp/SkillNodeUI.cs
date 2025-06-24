using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour, IPointerDownHandler
{
    public Image icon;
    public Action<PointerEventData> pointerDownEvent = (eventData) => { };
    public SkillDataSO skillDataSO;
    public SkillSlotUI equippedSlot;
    public SkillBase skillBase;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent(eventData);
    }
}
