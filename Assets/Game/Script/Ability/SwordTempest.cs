using System.Collections;
using UnityEngine;

public class SwordTempest : SkillBase
{
    public override void Awake()
    {
        base.Awake();
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
                ActionUse.MeleeDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
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
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 25f;
        /* Also damage, actionIE, gameeffect */
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
            customMono.stat.reflex.FinalValue * 2.5f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
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
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    WaitSpawnSlashSignal(direction)
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

    IEnumerator WaitSpawnSlashSignal(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill3Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.swordTempestSlash1Pool.PickOneGameEffect();
        SpawnEffectAsChild(
            p_direction,
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value
        );
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill3Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.swordTempestSlash2Pool.PickOneGameEffect();
        SpawnEffectAsChild(
            p_direction,
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value
        );
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                2 * GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill3Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.swordTempestSlash1Pool.PickOneGameEffect();
        SpawnEffectAsChild(
            p_direction,
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value
        );
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                3 * GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill3Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.swordTempestSlash3Pool.PickOneGameEffect();
        SpawnEffectAsChild(
            p_direction,
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value
        );
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                4 * GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill3)
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.currentAction = null;
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotTriggerIE(p_direction, p_duration));
    }

    IEnumerator BotTriggerIE(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
