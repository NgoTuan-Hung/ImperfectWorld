using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Attack : SkillBase
{
    Func<Vector2, Vector2, CustomMono, IEnumerator> triggerIE;
    AttackGameEventData attackGED = new();
    public DealDamageGameEventData dealDamageGED = new();
    public Action<Vector2, Vector2, CustomMono> endAttackAction = (_, _, _) => { };

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 1f;
        /* Stun duration */
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = customMono
            .charAttackInfo
            .attackRange;

        GetActionField<ActionFloatField>(ActionFieldName.Blend).value =
            1f
            / (
                (customMono.charAttackInfo.variant - 1) == 0
                    ? 1
                    : customMono.charAttackInfo.variant - 1
            );

        switch (customMono.charAttackInfo.attackType)
        {
            case CharAttackInfo.AttackType.Melee:
            {
                triggerIE = MeleeTriggerIE;
                break;
            }
            case CharAttackInfo.AttackType.Ranged:
            {
                triggerIE = RangedTriggerIE;
                break;
            }
            default:
                break;
        }

        /* Also use damage, actionie, selectedVariant */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.attackSpeed.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();

        customMono.AnimatorWrapper.animator.SetFloat(
            "AttackAnimSpeed",
            defaultStateSpeed * customMono.stat.attackSpeed.FinalValue
        );

        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value =
            1 / customMono.stat.attackSpeed.FinalValue;
    }

    public override ActionResult Trigger(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        if (
            Vector2.Distance(customMono.transform.position, p_customMono.transform.position)
            > GetActionField<ActionFloatField>(ActionFieldName.Range).value
        )
            return failResult;
        else if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.attackBoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = triggerIE(
                    p_location,
                    p_direction,
                    p_customMono
                )
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator MeleeTriggerIE(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value = Random.Range(
            0,
            customMono.charAttackInfo.variant
        );
        SetBlend(
            GameManager.Instance.attackBlendHash,
            GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value
                * GetActionField<ActionFloatField>(ActionFieldName.Blend).value
        );

        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.Attack))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Attack, false);

        if (
            customMono.charAttackInfo.CheckMeleeAttackEffect(
                GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value
            )
        )
            SpawnEffectAsChild(
                p_direction,
                customMono.charAttackInfo.GetMeleeAttackEffect(
                    GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value
                )
            );

        dealDamageGED.damage = p_customMono.statusEffect.GetHit(CalculateAttackDamage());

        dealDamageGED.dealer = attackGED.attacker = customMono;
        dealDamageGED.count = attackGED.count++;
        dealDamageGED.target = attackGED.target = p_customMono;
        GameManager.Instance.GetSelfEvent(customMono, GameEventType.Attack).action(attackGED);
        GameManager
            .Instance.GetSelfEvent(customMono, GameEventType.DealDamage)
            .action(dealDamageGED);

        SpawnEffectAtLoc(
            p_customMono.rotationAndCenterObject.transform.position,
            customMono.charAttackInfo.GetMeleeImpactEffect(
                GetActionField<ActionIntField>(ActionFieldName.SelectedVariant).value
            )
        );

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndAttack))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndAttack, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        customMono.currentAction = null;
        endAttackAction(p_location, p_direction, p_customMono);
    }

    IEnumerator RangedTriggerIE(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.Attack))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Attack, false);

        dealDamageGED.dealer = attackGED.attacker = customMono;
        customMono
            .charAttackInfo.GetRangedProjectileEffect()
            .FireAsRangedAttackEffect(
                customMono.rotationAndCenterObject.transform.position,
                CalculateAttackDamage(),
                p_customMono,
                customMono.charAttackInfo.rangedImpactEffectSO,
                attackGED,
                dealDamageGED
            );

        while (!customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndAttack))
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndAttack, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        customMono.currentAction = null;
        endAttackAction(p_location, p_direction, p_customMono);
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Attack, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndAttack, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }

    float CalculateAttackDamage() =>
        CalculateFinalDamage(
            customMono.stat.attackDamage.FinalValue
                * (
                    1
                    + (
                        Random.Range(0, 1f) < customMono.stat.critChance.FinalValue
                            ? customMono.stat.critDamageModifier.FinalValue
                            : 0
                    )
                )
        );
}
