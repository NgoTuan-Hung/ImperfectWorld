using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIR_UIImageEffectTest : MonoBehaviour
{
    UIImageEffect uIImageEffect;
    Image cooldownIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        uIImageEffect = GetComponent<UIImageEffect>();

        // cooldownIndicator = transform.Find("CooldownIndicator").GetComponent<Image>();

        // _ = Test();
        // StartCoroutine(TestIE());
        StartCoroutine(Test2IE());
    }

    public bool doDis = false;

    // Update is called once per frame
    void Update() { }

    void Test()
    {
        cooldownIndicator.fillAmount = 1f;
        /// convert this to Color: rgba(126, 126, 126, 1)
        uIImageEffect.image.color = new Color(126 / 255f, 126 / 255f, 126 / 255f, 1);
        cooldownIndicator
            .DOFillAmount(0, 3)
            .OnComplete(() =>
            {
                uIImageEffect.uIEffectTweener.Play(true);
                uIImageEffect.image.color = Color.white;
            });
    }

    void Test2()
    {
        uIImageEffect.uIEffectTweener.Play(true);
    }

    IEnumerator Test2IE()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Test2();
        }
    }
}
