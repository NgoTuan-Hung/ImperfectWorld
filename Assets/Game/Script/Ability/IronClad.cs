using System.Collections;
using UnityEngine;

public class IronClad : SkillBase
{
    public float armorBuff;

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
        /* Debuff duration */
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 100f;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 5f;

        /* Also use actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        // customMono.stat.attackSpeed.finalValueChangeEvent += RecalculateStat;

        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        armorBuff = 10 + customMono.stat.might.FinalValue;
    }

    public override ActionResult Trigger(
        Vector2 p_location = default,
        Vector2 p_direction = default,
        CustomMono p_customMono = null
    )
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
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
        SpawnEffectAsChild(Vector2.zero, GameManager.Instance.ironCladVFXPool.PickOneGameEffect());
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        customMono.statusEffect.BuffArmor(
            armorBuff,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value
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
        Trigger();
    }
}
