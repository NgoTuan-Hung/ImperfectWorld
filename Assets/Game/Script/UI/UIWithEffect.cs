using System;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.Events;

public class UIWithEffect : MonoBehaviour
{
    public UIEffect uIEffect;
    public UIEffectTweener tweener;
    public Action onComplete = () => { };

    private void Awake()
    {
        uIEffect = GetComponent<UIEffect>();
        tweener = GetComponent<UIEffectTweener>();
    }

    public void OnComplete()
    {
        onComplete();
    }
}
