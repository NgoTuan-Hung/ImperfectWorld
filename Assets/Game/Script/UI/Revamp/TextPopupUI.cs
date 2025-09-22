using System.Collections;
using DG.Tweening;
using Kryz.Tweening;
using TMPEffects.Components;
using TMPro;
using UnityEngine;

public class TextPopupUI : MonoSelfAware
{
    RectTransform rectTransform;
    TextMeshProUGUI textMeshProUGUI;
    Vector2 initialVelocity;
    TMPAnimator tmpAnimator;
    static readonly float duration = 1f;
    static readonly Vector2 acceleration = new(0, -80f);
    static Material damagePopupMat,
        weakenPopupMat,
        armorBuffPopupMat;
    float defaultFontSize;
    public static Color transparentWhite = new(1, 1, 1, 0);

    public override void Awake()
    {
        base.Awake();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        defaultFontSize = textMeshProUGUI.fontSize;
        rectTransform = GetComponent<RectTransform>();
        damagePopupMat = Resources.Load<Material>("Material/rimouski sb SDF - Damage");
        weakenPopupMat = Resources.Load<Material>("Material/rimouski sb SDF - Weaken");
        armorBuffPopupMat = Resources.Load<Material>("Material/rimouski sb SDF - Armor Buff");
        tmpAnimator = GetComponent<TMPAnimator>();
    }

    public void StartDamagePopup(Vector3 p_initialPos, float p_damage) =>
        StartCoroutine(StartDamagePopupIE(p_initialPos, p_damage));

    IEnumerator StartDamagePopupIE(Vector3 p_initialPos, float p_damage)
    {
        textMeshProUGUI.fontSharedMaterial = damagePopupMat;
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
        textMeshProUGUI.color = Color.white;
        textMeshProUGUI.fontSize = defaultFontSize;
        deactivate();
    }

    public void StartWeakenPopup(Vector3 p_initialPos) =>
        StartCoroutine(StartWeakenPopupIE(p_initialPos));

    IEnumerator StartWeakenPopupIE(Vector3 p_initialPos)
    {
        tmpAnimator.StartAnimating();
        textMeshProUGUI.fontSharedMaterial = weakenPopupMat;
        textMeshProUGUI.text = "<shake>WEAKEN";
        transform.position = p_initialPos;
        yield return rectTransform
            .DOMove(
                transform.position
                    + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * 2,
                0.5f
            )
            .SetEase(Ease.OutQuint)
            .WaitForCompletion();

        yield return textMeshProUGUI
            .DOColor(transparentWhite, 0.5f)
            .SetEase(Ease.OutQuint)
            .WaitForCompletion();
        textMeshProUGUI.color = Color.white;
        deactivate();
    }

    public void StartArmorBuffPopup(Vector3 p_initialPos) =>
        StartCoroutine(StartArmorBuffPopupIE(p_initialPos));

    IEnumerator StartArmorBuffPopupIE(Vector3 p_initialPos)
    {
        textMeshProUGUI.fontSharedMaterial = armorBuffPopupMat;
        textMeshProUGUI.text = "ARMOR⬆";
        transform.position = p_initialPos;
        textMeshProUGUI.DOColor(transparentWhite, 0.5f).SetEase(Ease.InCubic);
        yield return textMeshProUGUI
            .transform.DOMove(p_initialPos + 2 * Vector3.up, 0.5f)
            .SetEase(Ease.InCubic)
            .WaitForCompletion();
        deactivate();
    }
}
