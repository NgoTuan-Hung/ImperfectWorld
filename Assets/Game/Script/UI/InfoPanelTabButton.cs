using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoPanelTabButton : MonoBehaviour, IPointerDownHandler
{
    public Color highLightColor;
    public static Color transparent = new(0, 0, 0, 0);
    TextMeshProUGUI tMP;
    Image underline;
    public Action click = () => { };

    private void Awake()
    {
        tMP = GetComponent<TextMeshProUGUI>();
        underline = GetComponentInChildren<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        click();
    }

    public void ChangeToHighLight()
    {
        tMP.color = highLightColor;
        underline.color = highLightColor;
    }

    public void ChangeToNormal()
    {
        tMP.color = Color.white;
        underline.color = transparent;
    }
}
