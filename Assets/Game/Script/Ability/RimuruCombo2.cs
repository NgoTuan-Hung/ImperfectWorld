using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimuruCombo2 : SkillBase
{
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
        GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value = Combo2End;
        GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects).value = new()
        {
            new(GameManager.Instance.rimuruCombo2SlashAPool, SpawnEffectAsChild),
            new(GameManager.Instance.rimuruCombo2SlashBPool, SpawnEffectAsChild),
        };
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 0f;
        GetActionField<ActionIntField>(ActionFieldName.Variants).value = 2;
        ConfigCombo2();
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
            ToggleAnim(GameManager.Instance.combo2BoolHash, true);
            StartCoroutine(actionIE = WaitCombo2(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;

            return successResult;
        }

        return failResult;
    }

    public void Combo2End()
    {
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.endCombo2 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.combo2BoolHash, false);
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
        ToggleAnim(GameManager.Instance.combo2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.combo2Signal = false;
        customMono.animationEventFunctionCaller.endCombo2 = false;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
