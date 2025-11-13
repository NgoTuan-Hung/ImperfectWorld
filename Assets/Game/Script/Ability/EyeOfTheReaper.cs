using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeOfTheReaper : SkillBase
{
    List<CustomMono> markedEnemies = new();
    float chance;

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
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = float.PositiveInfinity;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GameManager.Instance.GetSelfEvent(customMono, GameEventType.TakeDamage).action +=
            PassiveTrigger;
        /* Also use damage, actionie */
        //target
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.wisdom.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.wisdom.FinalValue * 8f;
        chance = Math.Clamp(0.25f + customMono.stat.wisdom.FinalValue * 0.005f, 0, 1);
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
        if (!customMono.actionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
                    p_customMono: p_customMono
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

    public IEnumerator TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        SpawnNormalEffect(
            GameManager.Instance.eyeOfTheReaperEffectPool.PickOneGameEffect(),
            p_customMono.transform.position,
            p_isCombat: true
        );

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
        Trigger(p_customMono: p_doActionParamInfo.target);
    }

    public void PassiveTrigger(IGameEventData takeDamageGameEventData)
    {
        var rand = Random.Range(0f, 1f);
        if (rand < chance)
        {
            GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value =
                GameManager.Instance.FindAliveEnemyNotInList(customMono, markedEnemies);

            if (GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value != null)
            {
                markedEnemies.Add(
                    GetActionField<ActionCustomMonoField>(ActionFieldName.Target).value
                );
                SpawnNormalEffect(
                    GameManager.Instance.eyeOfTheReaperEffectPool.PickOneGameEffect(),
                    GetActionField<ActionCustomMonoField>(
                        ActionFieldName.Target
                    ).value.transform.position,
                    p_isCombat: true
                );
            }
            else
                markedEnemies.Clear();
        }
    }
}
