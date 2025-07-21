using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScatterArrowIconTest : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    bool allow = true;

    public Vector3 punch = new Vector3(1.5f, 1.5f, 0);
    public float duration = 0.5f,
        elasticity = 1;
    public int vibrato = 10;

    // Update is called once per frame
    void Update()
    {
        if (allow && Keyboard.current.kKey.isPressed)
        {
            allow = false;
            StartCoroutine(AllowIE());
            spriteRenderer.enabled = true;
            transform.localScale = Vector3.one * 0.1f;
            transform
                .DOPunchScale(punch, duration, vibrato, elasticity)
                .SetEase(Ease.OutQuart)
                // .OnComplete(() => spriteRenderer.enabled = false);
                .SetLoops(-1);
        }
    }

    IEnumerator AllowIE()
    {
        yield return new WaitForSeconds(0.5f);
        allow = true;
    }
}
