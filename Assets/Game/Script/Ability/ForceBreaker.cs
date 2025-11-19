using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBreaker : SkillBase
{
    List<ObjectPool> effectPools;

    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, BotDoActionContinous, 1.7f, true);
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
        effectPools = new()
        {
            GameManager.Instance.forceBreakerFirstPunchPool,
            GameManager.Instance.forceBreakerKickPool,
            GameManager.Instance.forceBreakerSecondPunchPool,
            null,
        };

        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 1.25f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        /* Also use damage, actionie */
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
            customMono.stat.reflex.FinalValue * 5.25f;
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
        if (customMono.stat.currentManaPoint.Value < customMono.stat.manaPoint.FinalValue)
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
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= customMono.stat.manaPoint.FinalValue;

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
        for (int i = 0; i < effectPools.Count; i++)
        {
            while (
                !customMono.animationEventFunctionCaller.GetSignalVals(
                    EAnimationSignal.MainSkill1Signal
                )
            )
                yield return null;

            customMono.animationEventFunctionCaller.SetSignal(
                EAnimationSignal.MainSkill1Signal,
                false
            );

            if (effectPools[i] != null)
                SpawnBasicCombatEffectAsChild(
                    customMono.GetDirection(),
                    effectPools[i].PickOneGameEffect()
                );

            switch (i)
            {
                case 2:
                {
                    var t_wave = GameManager.Instance.forceBreakerWavePool.PickOneGameEffect();
                    (
                        t_wave.GetBehaviour(EGameEffectBehaviour.PullingMissile) as PullingMissile
                    ).allyTags = customMono.allyTags;
                    t_wave.KeepFlyingAt(transform.position, customMono.GetDirection(), true);
                    break;
                }
                case 3:
                {
                    GameManager
                        .Instance.forceBreakerProjectilePool.PickOneGameEffect()
                        .FireAsBasicCombatProjectile(
                            customMono,
                            GetActionField<ActionFloatField>(ActionFieldName.Damage).value,
                            customMono.rotationAndCenterObject.transform.position,
                            direction
                        );
                    break;
                }
                default:
                    break;
            }
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
