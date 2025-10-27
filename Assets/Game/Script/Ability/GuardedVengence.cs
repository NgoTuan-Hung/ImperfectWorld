using System.Collections;
using UnityEngine;

public class GuardedVengence : SkillBase
{
    float damgeReductionBuff = 0.25f,
        savedTotalDamageTaken;

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
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 100f;
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 11;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 3.5f;
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
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.might.FinalValue * 5f;
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
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE()
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
        customMono.statusEffect.BuffDR(
            damgeReductionBuff,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value
        );
        savedTotalDamageTaken = customMono.statusEffect.GetTotalDamageTaken();

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
                GameManager.Instance.raikenDartPool.PickOneGameEffect();
            GetActionField<ActionGameEffectField>(
                ActionFieldName.GameEffect
            ).value.transform.position = customMono.rotationAndCenterObject.transform.position;
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.transform.SetEulerZ(i * 30f);
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.SetUpCollideAndDamage(
                    customMono,
                    (customMono.statusEffect.GetTotalDamageTaken() - savedTotalDamageTaken) * 0.3f
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
        Trigger();
    }
}
