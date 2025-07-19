using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class UIImageEffect : MonoBehaviour
{
    public Image image;
    public UIEffect uIEffect;
    public UIEffectTweener uIEffectTweener;
    public delegate void TweenerOnComplete();
    public TweenerOnComplete tweenerOnComplete;

    private void Reset()
    {
        image = GetComponent<Image>();
        uIEffect = GetComponent<UIEffect>();
        uIEffectTweener = GetComponent<UIEffectTweener>();
    }
}
