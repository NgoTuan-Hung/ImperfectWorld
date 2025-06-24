using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SkillIndicatorType
{
    Direction,
    Location,
}

public class SkillUseUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    RectTransform rectTransform;
    public Image border,
        icon;
    public Action<Vector2, Vector2> holdEvent = (p_pointerPosition, p_centerToPointerDir) => { },
        pointerUpEvent = (p_pointerPosition, p_centerToPointerDir) => { };
    Vector2 centerToPointerDir = Vector2.zero,
        pointerPosition = Vector2.zero;
    public bool holding = false;
    public RectTransform skillDirectionIndicator,
        skillLocationIndicator;
    public SkillIndicatorType skillIndicatorType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        border = GetComponent<Image>();
        skillDirectionIndicator = transform
            .Find("SkillDirectionIndicator")
            .GetComponent<RectTransform>();
        skillLocationIndicator = transform
            .Find("SkillLocationIndicator")
            .GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!holding)
        {
            holding = true;
            border.color = Color.red;
            StartCoroutine(OnHold());

            switch (skillIndicatorType)
            {
                case SkillIndicatorType.Direction:
                {
                    skillDirectionIndicator.gameObject.SetActive(true);
                    break;
                }
                case SkillIndicatorType.Location:
                {
                    skillLocationIndicator.gameObject.SetActive(true);
                    break;
                }
                default:
                    break;
            }
        }
    }

    IEnumerator OnHold()
    {
        while (holding)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            holdEvent(pointerPosition, centerToPointerDir);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (holding)
        {
            holding = false;
            border.color = Color.white;
            switch (skillIndicatorType)
            {
                case SkillIndicatorType.Direction:
                {
                    skillDirectionIndicator.gameObject.SetActive(false);
                    break;
                }
                case SkillIndicatorType.Location:
                {
                    skillLocationIndicator.gameObject.SetActive(false);
                    break;
                }
                default:
                    break;
            }
            pointerUpEvent(pointerPosition, centerToPointerDir);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (skillIndicatorType)
        {
            case SkillIndicatorType.Direction:
            {
                centerToPointerDir = (
                    eventData.position - (Vector2)rectTransform.position
                ).normalized;
                skillDirectionIndicator.rotation = Quaternion.Euler(
                    0,
                    0,
                    Vector2.SignedAngle(Vector2.right, centerToPointerDir)
                );
                skillDirectionIndicator.localScale = new Vector3(
                    1,
                    centerToPointerDir.x > 0 ? 1 : -1,
                    1
                );
                break;
            }
            case SkillIndicatorType.Location:
            {
                pointerPosition = GameManager.Instance.camera.ScreenToWorldPoint(
                    eventData.position
                );
                skillLocationIndicator.position = eventData.position;
                break;
            }
            default:
                break;
        }
    }

    public void ResetEvent()
    {
        holdEvent = (p_pointerPosition, p_centerToPointerDir) => { };
        pointerUpEvent = (p_pointerPosition, p_centerToPointerDir) => { };
    }
}
