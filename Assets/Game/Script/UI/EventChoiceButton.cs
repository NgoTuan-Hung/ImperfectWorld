using DG.Tweening;
using TMPro;
using UnityEngine;

public class EventChoiceButton : InteractiveButtonUI
{
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
    }

    public void ShowButton()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1).SetEase(Ease.OutQuint);
    }

    public void SetupChoice(int index, string choiceText)
    {
        choiceIndex = index;
        textMeshProUGUI.text = choiceText;
        isAvailable = true;
    }

    public void ResetChoice()
    {
        gameObject.SetActive(false);
        isAvailable = false;
    }
}
