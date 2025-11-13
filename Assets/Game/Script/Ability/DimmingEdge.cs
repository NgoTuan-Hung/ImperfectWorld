using System.Collections;
using UnityEngine;

public class DimmingEdge : SkillBase
{
    FloatStatModifier damageDebuff;

    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
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
        /* Stun duration */
        // successResult = new(
        //     true,
        //     ActionResultType.Cooldown,
        //     GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        // );
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 1.25f;
        damageDebuff = new(-0.25f, FloatStatModifierType.Multiplicative);
        /* Debuff duration */
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 5f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 100f;

        /* Also use actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        // customMono.stat.attackSpeed.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
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
            || (
                customMono.stat.currentManaPoint.Value
                < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
            )
        )
            return failResult;
        else if (!customMono.actionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
                    p_location,
                    p_direction,
                    p_customMono
                )
            );
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerIE(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        customMono.SetUpdateDirectionIndicator(
            p_customMono.rotationAndCenterObject.transform.position
                - customMono.rotationAndCenterObject.transform.position,
            UpdateDirectionIndicatorPriority.Low
        );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        p_customMono.statusEffect.Weaken(
            damageDebuff,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value
        );

        SpawnEffectAtLoc(
            p_customMono.rotationAndCenterObject.transform.position,
            GameManager.Instance.dimmingEdgeEffectPool.PickOneGameEffect()
        );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

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
        Trigger(p_customMono: p_doActionParamInfo.target);
    }
}
