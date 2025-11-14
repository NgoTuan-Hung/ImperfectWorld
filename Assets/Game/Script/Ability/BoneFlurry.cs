using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoneFlurry : SkillBase
{
    float chance = 0.35f;
    float[] damageVariant = { 1.25f, 1.5f, 1.75f };

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
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 1.25f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        (customMono.skill.skillBases[0] as Attack).endAttackAction += Proc;
        /* Also use actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        chance = Math.Clamp(0.35f + customMono.stat.might.FinalValue * 0.01f, 0, 1);
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

        StartCoroutine(BuffIE());
        customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
            ActionFieldName.ManaCost
        ).value;

        return successResult;
    }

    IEnumerator BuffIE()
    {
        var buff = 1 + customMono.stat.might.FinalValue * 0.01f;
        for (int i = 0; i < 3; i++)
            damageVariant[i] += buff;
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 3; i++)
            damageVariant[i] -= buff;
    }

    public void Proc(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        if (!customMono.actionBlocking)
        {
            var selection = Random.Range(0f, 1f);

            if (selection < chance)
            {
                customMono.actionBlocking = true;
                customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
                ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
                StartCoroutine(
                    GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                        TriggerIE(p_customMono: p_customMono)
                );
                customMono.currentAction = this;
            }
        }
    }

    public IEnumerator TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        for (int i = 0; i < 3; i++)
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

            p_customMono.statusEffect.GetHit(
                customMono,
                customMono.stat.attackDamage.FinalValue * damageVariant[i]
            );

            SpawnEffectAtLoc(
                p_customMono.rotationAndCenterObject.transform.position,
                GameManager.Instance.bladeOfVuImpactPool.PickOneGameEffect()
            );
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
