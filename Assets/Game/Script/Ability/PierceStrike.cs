using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class PierceStrike : SkillBase
{
    bool secondPhase = false;
    IEnumerator secondPhaseHandlerIE,
        cooldownIE;
    bool phaseOneFinish = false;
    float secondPhaseDeadline;
    public ActionResult additionalPhaseWithConditionResult;

    public override void Awake()
    {
        base.Awake();
        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.4f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        secondPhaseDeadline = 3f;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.203f;
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 0.8f;
        additionalPhaseWithConditionResult = new(
            true,
            ActionResultType.AdditionalPhaseWithCondition,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value,
            secondPhaseDeadline
        );
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 20f;
        /* Also use current time, damage , actionie, actionie1*/
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value = customMono
            .stat
            .might
            .FinalValue;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (secondPhase)
        {
            if (phaseOneFinish && !customMono.actionBlocking && !customMono.movementActionBlocking)
            {
                canUse = false;
                customMono.actionBlocking = true;
                customMono.movementActionBlocking = true;
                StopCoroutine(secondPhaseHandlerIE);
                StartCoroutine(
                    GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                        StartDash(direction)
                );
                StartCoroutine(cooldownIE = CooldownCoroutine());
                ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
                customMono.currentAction = this;
                secondPhase = false;
                return successResult;
            }
        }
        else
        {
            if (
                customMono.stat.currentManaPoint.Value
                < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
            )
                return failResult;
            else if (canUse && !customMono.actionBlocking)
            {
                canUse = false;
                customMono.actionBlocking = true;
                customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
                ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
                StartCoroutine(
                    GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                        TriggerIE(location, direction)
                );
                StartCoroutine(cooldownIE = CooldownCoroutine());
                customMono.currentAction = this;
                phaseOneFinish = false;
                customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                    ActionFieldName.ManaCost
                ).value;
                return additionalPhaseWithConditionResult;
            }
        }

        return failResult;
    }

    void SetupCAD(CollideAndDamage p_cAD) => p_cAD.dealDamageEvent = ChangePhase;

    IEnumerator TriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        SpawnBasicCombatEffectAsChild(
            p_direction,
            GameManager.Instance.pierceStrikePool.PickOneGameEffect(),
            SetupCAD
        );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        phaseOneFinish = true;
        customMono.currentAction = null;
    }

    IEnumerator StartDash(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        StartCoroutine(
            GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE1).value =
                SecondPhaseTriggerIE(p_direction: p_direction)
        );
        SpawnNormalEffect(
            GameManager.Instance.vanishEffectPool.PickOneGameEffect(),
            transform.position
        );
        yield return Dash(
            p_direction,
            GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
            EasingFunctions.OutQuint
        );
    }

    IEnumerator SecondPhaseTriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
    {
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        SpawnBasicCombatEffectAsChild(
            p_direction,
            GameManager.Instance.pierceStrikeSecondPhasePool.PickOneGameEffect()
        );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
        botActionManuals[0].actionUse = ActionUse.RangedDamage;
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotStartDash(p_direction, p_duration));
    }

    IEnumerator BotStartDash(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    void ChangePhase(float p_damageDealt)
    {
        secondPhase = true;
        secondPhaseHandlerIE = SecondPhaseHandlerIE();
        StartCoroutine(secondPhaseHandlerIE);
        additionalPhaseWithConditionResult.conditionMetCallback(additionalPhaseWithConditionResult);
        StopCoroutine(cooldownIE);
        botActionManuals[0].actionUse = ActionUse.GetCloser;
    }

    IEnumerator SecondPhaseHandlerIE()
    {
        yield return new WaitForSeconds(secondPhaseDeadline);
        secondPhase = false;
        botActionManuals[0].actionUse = ActionUse.RangedDamage;
        StartCoroutine(cooldownIE = CooldownCoroutine());
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE1).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);

        botActionManuals[0].actionUse = ActionUse.RangedDamage;
    }
}
