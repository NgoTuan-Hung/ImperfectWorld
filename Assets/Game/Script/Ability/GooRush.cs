using System;
using System.Collections;
using UnityEngine;

public class GooRush : SkillBase
{
    SlowInfo slowInfo;
    FloatStatModifier aspdBuff;

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
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = GameManager
            .Instance
            .largePositiveNumber;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.5f;
        slowInfo = new(new(-0.5f, ModifierType.Multiplicative), 5.5f);
        aspdBuff = new(0.3f, ModifierType.Additive);
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        /* Also use actionie, gameeffect */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        slowInfo.totalSlow.value = Math.Clamp(
            -0.5f - customMono.stat.might.FinalValue * 0.01f,
            -1,
            0
        );

        aspdBuff.value = 0.3f + customMono.stat.reflex.FinalValue * 0.01f;
    }

    public override ActionResult Trigger(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        if (customMono.stat.currentManaPoint.Value < customMono.stat.manaPoint.FinalValue)
            return failResult;
        if (!customMono.actionBlocking && !customMono.movementActionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
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
        customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return null;

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.gooRushSmokePool.PickOneGameEffect();
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.GetCollideAndDamage()
            .SetupSlow(customMono, slowInfo);
        SpawnEffectAsChild(
            Vector2.right,
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value
        );

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return null;

        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        var nearestAlly = GameManager.Instance.FindNearestAlly(customMono);
        if (nearestAlly == null)
            customMono.statusEffect.BuffAttackSpeed(aspdBuff, 3.5f);
        else
        {
            ToggleAnim(GameManager.Instance.jumpBoolHash, true);
            var jumpDest =
                nearestAlly.transform.position + VectorExtension.RandomXYNormalized() * 1.5f;
            var jumpPoint = transform.position;
            float currentTime = 0;
            while (currentTime < GetActionField<ActionFloatField>(ActionFieldName.Duration).value)
            {
                transform.position = Vector3.Lerp(
                    jumpPoint,
                    jumpDest,
                    currentTime / GetActionField<ActionFloatField>(ActionFieldName.Duration).value
                );

                yield return new WaitForSeconds(Time.fixedDeltaTime);
                currentTime += Time.fixedDeltaTime;
            }

            nearestAlly.statusEffect.BuffAttackSpeed(aspdBuff, 3.5f);
            ToggleAnim(GameManager.Instance.jumpBoolHash, false);
        }

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.currentAction = null;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        ToggleAnim(GameManager.Instance.jumpBoolHash, false);
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
