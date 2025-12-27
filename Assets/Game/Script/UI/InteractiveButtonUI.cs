using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractiveButtonUI
    : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
{
    Image image;
    Color defaultImageColor;

    public virtual void Awake()
    {
        image = GetComponent<Image>();
        defaultImageColor = image.color;
    }

    public Action<PointerEventData> pointerClickEvent = (p_eventData) => { };

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        image.color = defaultImageColor * 0.5f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = defaultImageColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        pointerClickEvent(eventData);
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);
}
