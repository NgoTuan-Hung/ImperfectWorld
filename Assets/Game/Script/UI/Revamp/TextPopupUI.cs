using System.Collections;
using DG.Tweening;
using Kryz.Tweening;
using TMPro;
using UnityEngine;

public class TextPopupUI : MonoBehaviour
{
    RectTransform rectTransform;
    TextMeshProUGUI textMeshProUGUI;
    Vector2 initialVelocity;
    static readonly float duration = 1f;
    static readonly Vector2 acceleration = new(0, -80f);
    static TMP_FontAsset damageFontAsset,
        weakenFontAsset;
    float defaultFontSize;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        defaultFontSize = textMeshProUGUI.fontSize;
        rectTransform = GetComponent<RectTransform>();
        damageFontAsset ??= Resources.Load<TMP_FontAsset>("FontAsset/DamageFA");
        weakenFontAsset ??= Resources.Load<TMP_FontAsset>("FontAsset/WeakenFA");
    }

    public IEnumerator StartPopupIE(Vector3 p_initialPos, float p_damage)
    {
        textMeshProUGUI.font = damageFontAsset;
        textMeshProUGUI.text = p_damage.ToString();
        transform.position = p_initialPos;
        float t_currentTime = 0;
        initialVelocity =
            new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(0.5f, 1f)).normalized * 20;
        float oneMinusProgress;
        while (t_currentTime < duration)
        {
            transform.position += (initialVelocity * Time.fixedDeltaTime).AsVector3();
            initialVelocity += acceleration * Time.fixedDeltaTime;
            oneMinusProgress = 1 - EasingFunctions.OutQuart(t_currentTime / duration);
            textMeshProUGUI.color = ((Vector4)textMeshProUGUI.color).WithW(oneMinusProgress);
            textMeshProUGUI.fontSize = defaultFontSize * oneMinusProgress;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            t_currentTime += Time.fixedDeltaTime;
        }
    }

    public IEnumerator StartWeakenPopup(Vector3 p_initialPos)
    {
        textMeshProUGUI.font = weakenFontAsset;
        textMeshProUGUI.text = "<shake>WEAKEN";
        transform.position = p_initialPos;
        var t_currentScale = rectTransform.localScale;
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOPunchScale(t_currentScale, 1, 2, 0);
        yield return rectTransform
            .DOMove(
                transform.position
                    + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * 2,
                1
            )
            .SetEase(Ease.OutQuint)
            .WaitForCompletion();
    }
}
