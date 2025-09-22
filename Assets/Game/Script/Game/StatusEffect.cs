using System;
using System.Collections;
using UnityEngine;

public enum StatusEffectState
{
    KnockUp,
    Stun,
    DamageEffect,
    HitColorEffect,
    Heal,
    Poison,
    Slow,
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
    Vector3 currentSpriteLocalScale;
    public float elementalEffectDuration = 0.2f;
    public Color elementalEffectColor = Color.red;
    public bool ccImmune = false;
    public float knockUpInitialVelocity = 1.5f,
        knockUpVelocity,
        knockUpAcceleration = -10f;
    public float stunTime = 0f;
    GameObject statusEffectIndicator,
        stunIndicator;
    ParticleSystem healIndicator,
        poisonIndicator;
    int totalPoison = 0;
    float poisonDamage;
    int actionBlockingFactors = 0,
        movementBlockingFactors = 0;
    float finalTakenDamage;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        statusEffectIndicator = customMono
            .directionModifier.transform.Find("StatusEffectIndicator")
            .gameObject;
        stunIndicator = statusEffectIndicator.transform.Find("StunIndicator").gameObject;
        healIndicator = statusEffectIndicator
            .transform.Find("HealIndicator")
            .GetComponent<ParticleSystem>();
        var sh = healIndicator.shape;
        sh.spriteRenderer = customMono.spriteRenderer;
        poisonIndicator = statusEffectIndicator
            .transform.Find("PoisonIndicator")
            .GetComponent<ParticleSystem>();
        sh = poisonIndicator.shape;
        sh.spriteRenderer = customMono.spriteRenderer;

        customMono.stat.currentHealthPointReachZeroEvent += StopAll;
    }

    void StopAll()
    {
        StopAllCoroutines();
        if (CheckEffect(StatusEffectState.DamageEffect))
            StopDamageEffect();
        if (CheckEffect(StatusEffectState.HitColorEffect))
            StopHitColorEffect();
        if (CheckEffect(StatusEffectState.KnockUp))
            StopKnockUp();
        if (CheckEffect(StatusEffectState.Stun))
            StopStun();
        if (CheckEffect(StatusEffectState.Poison))
            StopPoison();
    }

    public void GetHit(float p_damage)
    {
        finalTakenDamage = Math.Clamp(
            p_damage - customMono.stat.armor.FinalValue,
            0f,
            float.MaxValue
        );
        customMono.stat.currentHealthPoint.Value -= finalTakenDamage;
        GameUIManagerRevamp
            .Instance.PickOneTextPopupUI()
            .TextPopupUI.StartDamagePopup(
                customMono.rotationAndCenterObject.transform.position,
                finalTakenDamage
            );

        if (CheckEffect(StatusEffectState.DamageEffect))
            currentDamageTime = 0;
        else
            StartCoroutine(DamageEffect());
        if (!CheckEffect(StatusEffectState.HitColorEffect))
            StartCoroutine(HitColorEffect());
    }

    /// <summary>
    /// Apply a damped oscillation effect to the sprite to simulate jelly-like fluctuations.
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageEffect()
    {
        ToggleEffect(StatusEffectState.DamageEffect);
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

        StopDamageEffect();
    }

    void StopDamageEffect()
    {
        RemoveEffect(StatusEffectState.DamageEffect);
        customMono.spriteRenderer.transform.localScale = new Vector3(
            customMono.stat.Size,
            customMono.stat.Size,
            1
        );
    }

    /// <summary>
    /// Make sprite red for a short time on being hit.
    /// </summary>
    /// <returns></returns>
    IEnumerator HitColorEffect()
    {
        ToggleEffect(StatusEffectState.HitColorEffect);
        customMono.spriteRenderer.color = elementalEffectColor;

        yield return new WaitForSeconds(elementalEffectDuration);

        StopHitColorEffect();
    }

    void StopHitColorEffect()
    {
        RemoveEffect(StatusEffectState.HitColorEffect);
        customMono.spriteRenderer.color = Color.white;
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
        customMono.currentAction?.ActionInterrupt();
        ToggleEffect(StatusEffectState.KnockUp);
        BlockMovement(StatusEffectState.KnockUp);
        knockUpVelocity = knockUpInitialVelocity;
        while (customMono.directionModifier.transform.localPosition.y >= 0)
        {
            customMono.directionModifier.transform.localPosition += Vector3.up * knockUpVelocity;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            knockUpVelocity += knockUpAcceleration * Time.fixedDeltaTime;
        }

        StopKnockUp();
    }

    void StopKnockUp()
    {
        customMono.directionModifier.transform.localPosition =
            customMono.directionModifier.transform.localPosition.WithY(0);
        RemoveEffect(StatusEffectState.KnockUp);
        // fallingIndicator.gameObject.SetActive(false);

        UnblockMovement(StatusEffectState.KnockUp);
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
        customMono.currentAction?.ActionInterrupt();
        ToggleEffect(StatusEffectState.Stun);
        BlockAction(StatusEffectState.Stun);
        BlockMovement(StatusEffectState.Stun);
        stunTime = p_duration;
        stunIndicator.SetActive(true);
        while (stunTime > 0)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            stunTime -= Time.fixedDeltaTime;
        }

        StopStun();
    }

    void StopStun()
    {
        RemoveEffect(StatusEffectState.Stun);
        stunIndicator.SetActive(false);

        UnblockAction(StatusEffectState.Stun);
        UnblockMovement(StatusEffectState.Stun);
    }

    public void Heal(float p_ammount)
    {
        customMono.stat.currentHealthPoint.Value += p_ammount;
        healIndicator.Play();
    }

    public void Poison(PoisonInfo p_poisonInfo)
    {
        if (CheckEffect(StatusEffectState.Poison))
        {
            if (totalPoison < p_poisonInfo.totalPoison)
                totalPoison = p_poisonInfo.totalPoison;
            poisonDamage += p_poisonInfo.poisonDamage;
        }
        else
        {
            StartCoroutine(PoisonIE(p_poisonInfo));
        }
    }

    IEnumerator PoisonIE(PoisonInfo p_poisonInfo)
    {
        ToggleEffect(StatusEffectState.Poison);
        totalPoison = p_poisonInfo.totalPoison;
        poisonDamage = p_poisonInfo.poisonDamage;
        while (totalPoison > 0)
        {
            totalPoison--;
            GetHit(poisonDamage);
            poisonIndicator.Play();

            yield return new WaitForSeconds(0.2f);
        }

        StopPoison();
    }

    void StopPoison()
    {
        RemoveEffect(StatusEffectState.Poison);
        poisonDamage = 0;
        totalPoison = 0;
    }

    public void Slow(FloatStatModifier p_floatStatModifier)
    {
        customMono.stat.moveSpeed.AddModifier(p_floatStatModifier);
    }

    public void RemoveSlow(FloatStatModifier p_floatStatModifier)
    {
        customMono.stat.moveSpeed.RemoveModifier(p_floatStatModifier);
    }

    public void Slow(SlowInfo p_slowInfo)
    {
        StartCoroutine(SlowIE(p_slowInfo));
    }

    IEnumerator SlowIE(SlowInfo p_slowInfo)
    {
        Slow(p_slowInfo.totalSlow);
        yield return new WaitForSeconds(p_slowInfo.slowDuration);

        RemoveSlow(p_slowInfo.totalSlow);
    }

    bool CheckEffect(StatusEffectState p_statusEffectState) =>
        (currentStates & GetMask(p_statusEffectState)) == GetMask(p_statusEffectState);

    void ToggleEffect(StatusEffectState p_statusEffectState)
    {
        currentStates |= GetMask(p_statusEffectState);
    }

    void RemoveEffect(StatusEffectState p_statusEffectState)
    {
        currentStates &= ~GetMask(p_statusEffectState);
    }

    void BlockAction(StatusEffectState p_factor)
    {
        customMono.actionBlocking = true;
        actionBlockingFactors |= GetMask(p_factor);
    }

    void BlockMovement(StatusEffectState p_factor)
    {
        customMono.movementActionBlocking = true;
        movementBlockingFactors |= GetMask(p_factor);
    }

    void UnblockAction(StatusEffectState p_factor)
    {
        actionBlockingFactors &= ~GetMask(p_factor);
        if (actionBlockingFactors == 0)
            customMono.actionBlocking = false;
    }

    void UnblockMovement(StatusEffectState p_factor)
    {
        movementBlockingFactors &= ~GetMask(p_factor);
        if (movementBlockingFactors == 0)
            customMono.movementActionBlocking = false;
    }

    int GetMask(StatusEffectState p_factor) => 0x1 << (int)p_factor;

    public void Weaken(FloatStatModifier p_damageModifier, float p_duration)
    {
        StartCoroutine(WeakenIE(p_damageModifier, p_duration));
    }

    IEnumerator WeakenIE(FloatStatModifier p_damageModifier, float p_duration)
    {
        customMono.stat.damageModifier.AddModifier(p_damageModifier);
        GameUIManagerRevamp
            .Instance.PickOneTextPopupUI()
            .TextPopupUI.StartWeakenPopup(customMono.rotationAndCenterObject.transform.position);
        yield return new WaitForSeconds(p_duration);
        customMono.stat.damageModifier.RemoveModifier(p_damageModifier);
    }

    public void BuffArmor(float p_value, float p_duration)
    {
        StartCoroutine(BuffArmorIE(p_value, p_duration));
    }

    IEnumerator BuffArmorIE(float p_value, float p_duration)
    {
        GameUIManagerRevamp
            .Instance.PickOneTextPopupUI()
            .TextPopupUI.StartArmorBuffPopup(customMono.rotationAndCenterObject.transform.position);
        customMono.stat.armor.BaseValue += p_value;
        yield return new WaitForSeconds(p_duration);
        customMono.stat.armor.BaseValue -= p_value;
    }
}
