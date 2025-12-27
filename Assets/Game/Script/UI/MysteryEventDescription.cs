using System;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MysteryEventDescription : PointerDownUI
{
    TextMeshProUGUI textMeshProUGUI;
    public TMPWriter tMPWriter;
    public Action onFinishWriter = () => { };

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        tMPWriter = GetComponent<TMPWriter>();
        pointerDownEvent += StopWriter;
    }

    void StopWriter(PointerEventData eventData)
    {
        tMPWriter.SkipWriter();
    }

    public void FinishWriter()
    {
        onFinishWriter();
    }
}
