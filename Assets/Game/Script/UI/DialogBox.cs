using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogBox : MonoBehaviour, IPointerDownHandler
{
    RectTransform rectTransform;
    public Action pointerDownEvent = () => { };
    TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textMeshProUGUI = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Show(float duration = -1)
    {
        if (duration < 0)
            ShowDialog();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void ShowDialog()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 1).SetEase(Ease.InOutBack);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent();
    }

    public void ChangeText(string text)
    {
        textMeshProUGUI.text = text;
    }
}
