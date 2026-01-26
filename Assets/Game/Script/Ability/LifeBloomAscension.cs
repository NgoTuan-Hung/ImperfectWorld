using System.Collections;
using UnityEngine;

public class LifeBloomAscension : SkillBase
{
    float healAmount;

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
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 10f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        /* Also use target, actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.wisdom.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        healAmount = customMono.stat.wisdom.FinalValue * 10.75f;
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
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE()
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
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return null;

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value =
            GameManager.Instance.FindLowestHPAlly(
                customMono,
                GetActionField<ActionFloatField>(ActionFieldName.Range).value
            );
        if (GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value == null)
            GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value = customMono;

        GameManager.Instance.wanderMagicianHealEffectPool.PickOneGameEffect().transform.position =
            GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value.transform.position;
        customMono.SetUpdateDirectionIndicator(
            GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value.transform.position
                - customMono.transform.position,
            UpdateDirectionIndicatorPriority.Low
        );
        GetActionField<ActionCustomMonoField>(ActionFieldName.Target)
            .value.statusEffect.Heal(healAmount);

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
