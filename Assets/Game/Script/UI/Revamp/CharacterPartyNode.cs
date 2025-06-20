using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPartyNode : MonoBehaviour, IPointerDownHandler
{
    public Image avatarIcon;
    public RectTransform rectTransform;
    public Action<PointerEventData> pointerDownEvent = (pointerEventData) => { };

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent(eventData);
    }
}
