using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwinsOfCataclysm : SkillBase
{
    float variant1Chance = 0.05f,
        variant2Chance = 0.05f,
        variant3Chance;
    DealDamageGameEventData dealDamageGED = new();

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
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 0f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        (customMono.skill.skillBases[0] as Attack).endAttackAction += Trigger;
        /* Also use damage, actionie */
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
        variant1Chance = 0.05f + customMono.stat.reflex.FinalValue * 0.01f;
        variant2Chance = 0.05f + customMono.stat.might.FinalValue * 0.01f;
        variant3Chance = Math.Clamp(1 - variant1Chance - variant2Chance, 0, 1);
        var totalChance = variant1Chance + variant2Chance + variant3Chance;
        variant1Chance /= totalChance;
        variant2Chance = variant1Chance + variant2Chance / totalChance;

        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            0.75f * customMono.stat.healthPoint.FinalValue;
    }

    public new void Trigger(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        if (!customMono.actionBlocking)
        {
            var selection = Random.Range(0f, 1f);

            if (selection < variant1Chance)
            {
                customMono.actionBlocking = true;
                customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
                ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
                StartCoroutine(
                    GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                        Variant1TriggerIE(p_customMono: p_customMono)
                );
                customMono.currentAction = this;
            }
            else if (selection < variant2Chance)
            {
                customMono.actionBlocking = true;
                customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
                ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
                StartCoroutine(
                    GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                        Variant2TriggerIE(p_customMono: p_customMono)
                );
                customMono.currentAction = this;
            }
        }
    }

    public IEnumerator Variant1TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        for (int i = 1; i < 3; i++)
        {
            while (
                !customMono.animationEventFunctionCaller.GetSignalVals(
                    EAnimationSignal.MainSkill2Signal
                )
            )
                yield return null;

            customMono.animationEventFunctionCaller.SetSignal(
                EAnimationSignal.MainSkill2Signal,
                false
            );

            dealDamageGED.damage = p_customMono.statusEffect.GetHit(
                customMono.stat.attackDamage.FinalValue * i
            );
            dealDamageGED.dealer = customMono;
            dealDamageGED.target = p_customMono;
            GameManager
                .Instance.GetSelfEvent(customMono, GameEventType.DealDamage)
                .action(dealDamageGED);

            SpawnEffectAtLoc(
                p_customMono.rotationAndCenterObject.transform.position,
                GameManager.Instance.aquaenAttack1ImpactPool.PickOneGameEffect()
            );
        }

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill2)
        )
            yield return null;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.currentAction = null;
    }

    public IEnumerator Variant2TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        for (int i = 0; i < 2; i++)
        {
            while (
                !customMono.animationEventFunctionCaller.GetSignalVals(
                    EAnimationSignal.MainSkill3Signal
                )
            )
                yield return null;

            customMono.animationEventFunctionCaller.SetSignal(
                EAnimationSignal.MainSkill3Signal,
                false
            );

            if (i == 0)
            {
                dealDamageGED.damage = p_customMono.statusEffect.GetHit(
                    customMono.stat.attackDamage.FinalValue
                );

                dealDamageGED.dealer = customMono;
                dealDamageGED.target = p_customMono;
                GameManager
                    .Instance.GetSelfEvent(customMono, GameEventType.DealDamage)
                    .action(dealDamageGED);

                SpawnEffectAtLoc(
                    p_customMono.rotationAndCenterObject.transform.position,
                    GameManager.Instance.aquaenAttack1ImpactPool.PickOneGameEffect()
                );
            }
            else
            {
                SpawnBasicCombatEffectAsChild(
                    p_customMono.transform.position - customMono.transform.position,
                    GameManager.Instance.twinsOfCataclysmEffectPool.PickOneGameEffect()
                );
            }
        }

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill3)
        )
            yield return null;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.currentAction = null;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill3Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill3, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
