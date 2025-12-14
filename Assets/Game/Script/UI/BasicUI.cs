using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasicUI : MonoSelfAware
{
    public Image image;

    public override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
    }

    public void LocalMoveTo(Vector3 localPos, float duration, Ease ease, Action finishCallback)
    {
        transform
            .DOLocalMove(localPos, duration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                finishCallback?.Invoke();
                deactivate();
            });
    }
}
