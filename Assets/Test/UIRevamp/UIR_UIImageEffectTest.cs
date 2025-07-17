using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIR_UIImageEffectTest : MonoBehaviour
{
    UIImageEffect uIImageEffect;
    Image cooldownIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uIImageEffect = GetComponent<UIImageEffect>();

        cooldownIndicator = transform.Find("CooldownIndicator").GetComponent<Image>();
        cooldownIndicator.fillAmount = 1f;
        /// convert this to Color: rgba(126, 126, 126, 1)
        uIImageEffect.image.color = new Color(126 / 255f, 126 / 255f, 126 / 255f, 1);
        cooldownIndicator
            .DOFillAmount(0, 3)
            .OnComplete(() =>
            {
                uIImageEffect.uIEffectTweener.Play();
                uIImageEffect.image.color = Color.white;
            });
    }

    // Update is called once per frame
    void Update() { }
}
