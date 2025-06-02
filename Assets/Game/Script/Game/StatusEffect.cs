using System;
using System.Collections;
using UnityEngine;

public class StatusEffect : CustomMonoPal
{
    public float osTime = 0,
        damp = 0.5f;
    public float speed = 1f,
        scaleRange = 0.5f;
    public int frequency = 1;
    float currentDamageTime = 0;
    public float damageDuration = 0.5f;
    bool damageInEffect = false;
    Vector3 currentSpriteLocalScale;
    public float elementalEffectDuration = 0.2f;
    public Color elementalEffectColor = Color.red;
    bool hitColorInEffect = false;
    public bool ccImmune = false;
    bool isKnockingUp = false;
    public float knockUpInitialVelocity = 1.5f,
        knockUpVelocity,
        knockUpAcceleration = -10f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public void GetHit()
    {
        if (damageInEffect)
            currentDamageTime = 0;
        else
            StartCoroutine(DamageEffect());
        if (!hitColorInEffect)
            StartCoroutine(HitColorEffect());
    }

    IEnumerator DamageEffect()
    {
        damageInEffect = true;
        currentDamageTime = 0;
        currentSpriteLocalScale = customMono.spriteRenderer.transform.localScale;
        while (currentDamageTime < damageDuration)
        {
            osTime = speed * currentDamageTime;
            customMono.spriteRenderer.transform.localScale =
                currentSpriteLocalScale
                + (float)(Math.Exp(-damp * osTime) * Math.Cos(frequency * osTime))
                    * scaleRange
                    * Vector3.one;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentDamageTime += Time.deltaTime;
        }

        damageInEffect = false;
        customMono.spriteRenderer.transform.localScale = new Vector3(
            customMono.stat.Size,
            customMono.stat.Size,
            1
        );
    }

    IEnumerator HitColorEffect()
    {
        hitColorInEffect = true;
        customMono.spriteRenderer.color = elementalEffectColor;

        yield return new WaitForSeconds(elementalEffectDuration);
        customMono.spriteRenderer.color = Color.white;
        hitColorInEffect = false;
    }

    public void KnockUp()
    {
        if (ccImmune) { }
        else
        {
            customMono.currentAction?.ActionInterrupt();
            if (isKnockingUp)
                knockUpVelocity += knockUpInitialVelocity;
            else
                StartCoroutine(KnockUpIE());
        }
    }

    IEnumerator KnockUpIE()
    {
        // fallingIndicator.gameObject.SetActive(true);
        customMono.actionBlocking = true;
        customMono.movementActionBlocking = true;
        isKnockingUp = true;
        knockUpVelocity = knockUpInitialVelocity;
        while (customMono.spriteRenderer.transform.localPosition.y >= 0)
        {
            customMono.spriteRenderer.transform.localPosition += Vector3.up * knockUpVelocity;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            knockUpVelocity += knockUpAcceleration * Time.fixedDeltaTime;
        }

        customMono.spriteRenderer.transform.localPosition =
            customMono.spriteRenderer.transform.localPosition.WithY(0);
        isKnockingUp = false;
        // fallingIndicator.gameObject.SetActive(false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
    }
}
