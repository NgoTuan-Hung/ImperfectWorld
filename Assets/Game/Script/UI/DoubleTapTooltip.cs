using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoubleTapTooltip : MonoBehaviour, IPointerDownHandler
{
    public Vector3 offset;
    public Transform follower;
    public Image image,
        nameBackground;
    public TextMeshProUGUI nameTMP,
        descriptionTMP;
    public DoubleTapUI textContainer;
    public PointerDownUI exitButton;

    private void Awake()
    {
        image = GetComponent<Image>();
        nameBackground = transform.Find("NameBackground").GetComponent<Image>();
        nameTMP = nameBackground
            .transform.Find("NameSV/Viewport/Name")
            .GetComponent<TextMeshProUGUI>();
        textContainer = transform.Find("TooltipSV/Viewport/Content").GetComponent<DoubleTapUI>();
        descriptionTMP = textContainer.GetComponent<TextMeshProUGUI>();
        exitButton = transform.Find("X").GetComponent<PointerDownUI>();
    }

    private void FixedUpdate()
    {
        transform.position = follower.position + offset;
    }

    public void Init(Transform follower, Vector3 tooltipOffset, Color tooltipColor)
    {
        this.follower = follower;
        offset = tooltipOffset;
        image.color = tooltipColor;
        nameBackground.color = tooltipColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}
