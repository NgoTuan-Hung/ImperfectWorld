using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventChoiceButton : InteractiveButtonUI
{
    MysteryEventDataSO mysteryEventDataSO;
    public bool isAvailable = false;
    CanvasGroup canvasGroup;
    TextMeshProUGUI textMeshProUGUI;
    int choiceIndex;

    public override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        pointerClickEvent += OnSelected;
    }

    public void ShowButton()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1).SetEase(Ease.OutQuint);
    }

    public void SetupChoice(int index, MysteryEventDataSO mysteryEventDataSO)
    {
        choiceIndex = index;
        this.mysteryEventDataSO = mysteryEventDataSO;
        textMeshProUGUI.text = mysteryEventDataSO.choices[index];
        isAvailable = true;
    }

    public void ResetChoice()
    {
        gameObject.SetActive(false);
        isAvailable = false;
    }

    void OnSelected(PointerEventData pointerEventData) =>
        GameManager.Instance.MysteryEventChoiceSelectedCallback(choiceIndex, mysteryEventDataSO);
}
