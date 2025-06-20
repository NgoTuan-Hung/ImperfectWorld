using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

public enum StickType
{
    Fixed = 0,
    Floating = 1,
    Dynamic = 2,
}

[AddComponentMenu("Input/Enhanced On-Screen Stick")]
[RequireComponent(typeof(RectTransform))]
public class EnhancedOnScreenStick
    : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
{
    [SerializeField]
    StickType stickType;

    [SerializeField]
    float movementRange = 50f;

    [SerializeField, Range(0f, 1f)]
    float deadZone = 0f;

    [SerializeField]
    bool showOnlyWhenPressed;

    [SerializeField]
    RectTransform background;

    [SerializeField]
    RectTransform handle;
    public Action<Vector2> OnMove = (vector2) => { };

    public StickType StickType
    {
        get => stickType;
        set => stickType = value;
    }

    public float MovementRange
    {
        get => movementRange;
        set => movementRange = value;
    }

    public float DeadZone
    {
        get => deadZone;
        set => deadZone = value;
    }

    RectTransform rectTransform;
    public Canvas canvas;

    protected void Awake()
    {
        rectTransform = (RectTransform)transform;

        if (showOnlyWhenPressed)
            background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);

        if (stickType != StickType.Fixed)
        {
            background.localPosition = ScreenToAnchoredPosition(eventData.position);
        }

        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMove(Vector2.zero);

        handle.anchoredPosition = Vector2.zero;

        if (showOnlyWhenPressed)
            background.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var position = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera,
            background.position
        );

        var input = (eventData.position - position) / (movementRange * canvas.scaleFactor);
        var rawMagnitude = input.magnitude;
        var normalized = input.normalized;

        if (rawMagnitude < deadZone)
            input = Vector2.zero;
        else if (rawMagnitude > 1f)
            input = input.normalized;

        OnMove(input);

        if (stickType == StickType.Dynamic && rawMagnitude > 1f)
        {
            var difference = movementRange * (rawMagnitude - 1f) * normalized;
            background.anchoredPosition += difference;
        }

        handle.anchoredPosition = input * movementRange;
    }

    Vector2 ScreenToAnchoredPosition(Vector2 screenPosition)
    {
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPosition,
                canvas.worldCamera,
                out var localPoint
            )
        )
        {
            var pivotOffset = rectTransform.pivot * rectTransform.sizeDelta;
            return localPoint - (background.anchorMax * rectTransform.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    private void OnTransformParentChanged()
    {
        canvas = GetComponentInParent<Canvas>(true);
    }
}
