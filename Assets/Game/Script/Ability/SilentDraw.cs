using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilentDraw : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 100f;
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 5;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        /* Also use damage, actionie, gameeff */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.reflex.FinalValue * 1.25f;
    }

    public override void TriggerContinuous(
        Vector2 p_location = default,
        Vector2 p_direction = default
    )
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        if (!customMono.actionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
                    direction: direction
                )
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;

            return successResult;
        }

        return failResult;
    }

    public IEnumerator TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return null;

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        var count = GetActionField<ActionIntField>(ActionFieldName.EffectCount).value;
        for (int i = 0; i < count; i++)
        {
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
                GameManager.Instance.jinseiKunaiPool.PickOneGameEffect();
            GetActionField<ActionGameEffectField>(
                ActionFieldName.GameEffect
            ).value.transform.position = customMono.rotationAndCenterObject.transform.position;
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.transform.RotateToDirection(direction);
            GetActionField<ActionGameEffectField>(
                ActionFieldName.GameEffect
            ).value.transform.position +=
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value.transform.up
                * (i % 2 == 0 ? i / 4 : (i + 1) / -4);
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.SetUpCollideAndDamage(
                    customMono,
                    GetActionField<ActionFloatField>(ActionFieldName.Damage).value
                );
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.KeepFlyingForward();
        }

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return null;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }

    public override void BotDoAction(DoActionParamInfo p_doActionParamInfo)
    {
        Trigger(direction: p_doActionParamInfo.centerToTargetCenterDirection);
    }

    public override void BotDoActionContinous(DoActionParamInfo p_doActionParamInfo)
    {
        TriggerContinuous(default, p_doActionParamInfo.originToTargetOriginDirection);
    }
}
