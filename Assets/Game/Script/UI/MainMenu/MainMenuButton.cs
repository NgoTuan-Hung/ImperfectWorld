using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MainMenuButton : InteractiveButtonUI, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    public static Vector2 hideOffset = new(120, 0);
    Vector2 defaultAnchorPos;

    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        defaultAnchorPos = rectTransform.anchoredPosition;
    }

    public static Vector3 interactionScale = new Vector3(1.1f, 1.1f, 1);

    public Tweener FadeOut()
    {
        rectTransform
            .DOAnchorPos(rectTransform.anchoredPosition + hideOffset, 0.5f)
            .SetEase(Ease.OutQuint);
        return canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutQuint);
    }

    public Tweener FadeIn()
    {
        rectTransform.anchoredPosition = defaultAnchorPos - hideOffset;
        canvasGroup.alpha = 0;
        rectTransform.DOAnchorPos(defaultAnchorPos, 0.5f).SetEase(Ease.OutQuint);
        return canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuint);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(interactionScale, 0.5f).SetEase(Ease.OutQuint);
    }
}
