using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIR_UIImageEffectTest : MonoBehaviour
{
    SkillUseUI skillUseUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        skillUseUI = GetComponent<SkillUseUI>();
        // cooldownIndicator = transform.Find("CooldownIndicator").GetComponent<Image>();

        // _ = Test();
        // StartCoroutine(TestIE());
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.kKey.isPressed)
        {
            print("OK");
            Test2();
        }
    }

    void Test()
    {
        // cooldownIndicator.fillAmount = 1f;
        // /// convert this to Color: rgba(126, 126, 126, 1)
        // uIImageEffect.image.color = new Color(126 / 255f, 126 / 255f, 126 / 255f, 1);
        // cooldownIndicator
        //     .DOFillAmount(0, 3)
        //     .OnComplete(() =>
        //     {
        //         uIImageEffect.uIEffectTweener.Play(true);
        //         uIImageEffect.image.color = Color.white;
        //     });
    }

    void Test2()
    {
        skillUseUI.image.color = halfWhite;
        skillUseUI.border.uIEffectTweener.Play(true);
        StartCoroutine(OnCompleteIE());
    }

    IEnumerator OnCompleteIE()
    {
        Stopwatch stopwatch = new();
        stopwatch.Restart();
        yield return new WaitForSecondsRealtime(skillUseUI.border.uIEffectTweener.duration);

        stopwatch.Stop();
        print(stopwatch.ElapsedMilliseconds);

        ColorChange();
    }

    Color halfWhite = new(0.5f, 0.5f, 0.5f, 1);

    void ColorChange()
    {
        skillUseUI.image.color = Color.white;
        skillUseUI.border.uIEffectTweener.Stop();
        skillUseUI.border.uIEffectTweener.ResetTime();
    }
}
