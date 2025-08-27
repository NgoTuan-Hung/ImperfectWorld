using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimuruCombo2 : SkillBase
{
    public static List<ObjectPool> effectPools;

    public override void Awake()
    {
        base.Awake();
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        // dashSpeed *= Time.deltaTime;
        // boolhash = ...
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
        effectPools = new()
        {
            GameManager.Instance.rimuruCombo2SlashAPool,
            GameManager.Instance.rimuruCombo2SlashBPool,
        };
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 0f;
        GetActionField<ActionIntField>(ActionFieldName.Variants).value = 2;
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
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = WaitCombo(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;

            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitCombo(Vector3 p_direction)
    {
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.Variants).value; i++)
        {
            while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.animationEventFunctionCaller.mainSkill1Signal = false;

            customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value = effectPools[i]
                .PickOne()
                .gameEffect;
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.SetParentAndLocalPosAndRot(
                    customMono.rotationAndCenterObject.transform,
                    GetActionField<ActionGameEffectField>(
                        ActionFieldName.GameEffect
                    ).value.gameEffectSO.effectLocalPosition,
                    GetActionField<ActionGameEffectField>(
                        ActionFieldName.GameEffect
                    ).value.gameEffectSO.effectLocalRotation
                );
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.SetUpCollideAndDamage(
                    customMono.allyTags,
                    GetActionField<ActionFloatField>(ActionFieldName.Damage).value
                );

            customMono.rotationAndCenterObject.transform.localScale = new(
                customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
                1,
                1
            );
            customMono.rotationAndCenterObject.transform.Rotate(
                Vector3.forward,
                Vector2.SignedAngle(
                    customMono.rotationAndCenterObject.transform.localScale,
                    customMono.GetDirection()
                )
            );
        }

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
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
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
