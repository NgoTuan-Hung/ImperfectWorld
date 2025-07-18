using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SkillIndicatorType
{
    None,
    Direction,
    Location,
}

public class SkillUseUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    RectTransform rectTransform;
    public Image border,
        cooldownIndicator;
    public UIImageEffect icon;
    public Action<Vector2, Vector2> pointerDownEvent = (
            p_pointerPosition,
            p_centerToPointerDir
        ) => { },
        holdEvent = (p_pointerPosition, p_centerToPointerDir) => { },
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
            pointerDownEvent(pointerPosition, centerToPointerDir);
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
                case SkillIndicatorType.None:
                    break;
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
                case SkillIndicatorType.None:
                    break;
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
            case SkillIndicatorType.None:
                break;
            default:
                break;
        }
    }

    public void ResetEvent()
    {
        pointerDownEvent = (p_pointerPosition, p_centerToPointerDir) => { };
        holdEvent = (p_pointerPosition, p_centerToPointerDir) => { };
        pointerUpEvent = (p_pointerPosition, p_centerToPointerDir) => { };
    }

    Color halfWhite = new Color(126 / 255f, 126 / 255f, 126 / 255f, 1);

    public void StartCooldown(float time)
    {
        cooldownIndicator.fillAmount = 1;
        icon.image.color = halfWhite;
        cooldownIndicator
            .DOFillAmount(0, time)
            .OnComplete(() =>
            {
                icon.uIEffectTweener.Play(true);
                icon.image.color = Color.white;
            });
    }
}
