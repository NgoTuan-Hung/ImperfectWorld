using System;
using System.Collections;
using UnityEngine;

public enum StatusEffectState
{
    KnockUp = 0,
    Stun = 1,
}

public class StatusEffect : CustomMonoPal
{
    public int currentStates = 0;
    public float osTime = 0,
        damp = 0.5f;
    public float speed = 10f,
        scaleRange = 0.25f;
    public int frequency = 3;
    float currentDamageTime = 0;
    public float damageDuration = 0.25f;
    bool damageInEffect = false;
    Vector3 currentSpriteLocalScale;
    public float elementalEffectDuration = 0.2f;
    public Color elementalEffectColor = Color.red;
    bool hitColorInEffect = false;
    public bool ccImmune = false;
    public float knockUpInitialVelocity = 1.5f,
        knockUpVelocity,
        knockUpAcceleration = -10f;
    public float stunTime = 0f;
    GameObject stunIndicator;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        stunIndicator = customMono.directionModifier.transform.Find("StunIndicator").gameObject;
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
            if (CheckEffect(StatusEffectState.KnockUp))
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
        ToggleEffect(StatusEffectState.KnockUp);
        knockUpVelocity = knockUpInitialVelocity;
        customMono.currentAction?.ActionInterrupt();
        while (customMono.directionModifier.transform.localPosition.y >= 0)
        {
            customMono.directionModifier.transform.localPosition += Vector3.up * knockUpVelocity;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            knockUpVelocity += knockUpAcceleration * Time.fixedDeltaTime;
        }

        customMono.directionModifier.transform.localPosition =
            customMono.directionModifier.transform.localPosition.WithY(0);
        RemoveEffect(StatusEffectState.KnockUp);
        // fallingIndicator.gameObject.SetActive(false);
        if (!CheckEffect(StatusEffectState.Stun))
        {
            customMono.actionBlocking = false;
            customMono.movementActionBlocking = false;
        }
    }

    public void Stun(float p_duration)
    {
        if (ccImmune) { }
        else
        {
            if (CheckEffect(StatusEffectState.Stun))
            {
                stunTime += p_duration;
            }
            else
                StartCoroutine(StunIE(p_duration));
        }
    }

    IEnumerator StunIE(float p_duration)
    {
        customMono.actionBlocking = true;
        customMono.movementActionBlocking = true;
        ToggleEffect(StatusEffectState.Stun);
        customMono.currentAction?.ActionInterrupt();
        stunTime = p_duration;
        stunIndicator.SetActive(true);
        while (stunTime > 0)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            stunTime -= Time.fixedDeltaTime;
        }
        RemoveEffect(StatusEffectState.Stun);
        stunIndicator.SetActive(false);
        if (!CheckEffect(StatusEffectState.KnockUp))
        {
            customMono.actionBlocking = false;
            customMono.movementActionBlocking = false;
        }
    }

    bool CheckEffect(StatusEffectState p_statusEffectState) =>
        (currentStates & 0x1 << (int)p_statusEffectState) == 0x1 << (int)p_statusEffectState;

    void ToggleEffect(StatusEffectState p_statusEffectState)
    {
        currentStates |= 0x1 << (int)p_statusEffectState;
    }

    void RemoveEffect(StatusEffectState p_statusEffectState)
    {
        currentStates &= ~(0x1 << (int)p_statusEffectState);
    }
}
