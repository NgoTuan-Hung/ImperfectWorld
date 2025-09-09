using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class BladeOfMinhKhai : SkillBase
{
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
            new BotActionManual(
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetAway,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(
                    nextActionChoosingIntervalProposal: 0.5f,
                    isDirectionModify: true,
                    directionModifier: -1
                )
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
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.1f;
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        GetActionField<ActionFloatField>(ActionFieldName.LifeStealPercent).value = 0.25f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 10f;
        GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0;
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 0.3f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        // also actionIE and actionIE1
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
            customMono.stat.reflex.FinalValue * 2f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(
        Vector2 p_location = default,
        Vector2 p_direction = default
    )
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = MyDash(
                    p_direction
                )
            );
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE1).value =
                    WaitSpawnSlashSignal(p_direction)
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

    IEnumerator MyDash(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        GameEffect vanishEffect = GameManager.Instance.vanishEffectPool.PickOne().gameEffect;
        vanishEffect.transform.position = transform.position;

        yield return Dash(
            p_direction,
            GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
            EasingFunctions.OutQuint
        );
    }

    IEnumerator WaitSpawnSlashSignal(Vector3 p_direction)
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
            GameManager.Instance.bladeOfMinhKhaiSlashEffectPool.PickOneGameEffect(),
            SetupCAD
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
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(BotStartDash(p_direction, p_duration));
    }

    IEnumerator BotStartDash(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(p_direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    void SetupCAD(CollideAndDamage p_cAD) => p_cAD.dealDamageEvent = LifeSteal;

    public override void LifeSteal(float damageDealt)
    {
        customMono.stat.currentHealthPoint.Value +=
            damageDealt * GetActionField<ActionFloatField>(ActionFieldName.LifeStealPercent).value;
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
    }
}
