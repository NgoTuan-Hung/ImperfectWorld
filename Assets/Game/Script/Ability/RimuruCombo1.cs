using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class RimuruCombo1 : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, BotDoActionContinous, 1.35f, true);
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
        GetActionField<ActionActionField>(ActionFieldName.ComboEndAction).value = Combo1End;
        GetActionField<ActionListComboEffectField>(ActionFieldName.ComboEffects).value = new()
        {
            new(GameManager.Instance.rimuruCombo1SlashAWithColliderPool, SpawnEffectAsChild),
            new(GameManager.Instance.rimuruCombo1SlashBPool, SpawnEffectAsChild),
            new(GameManager.Instance.rimuruCombo1DashPool, SpawnEffectRelative),
        };
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 100f;
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 1.25f;
        GetActionField<ActionIntField>(ActionFieldName.Variants).value = 3;
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 0.25f;
        GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.1f;
        GetActionField<ActionListComboActionField>(ActionFieldName.ComboActions).value = new()
        {
            new(DashSmall),
            new(DashSmall),
            new(FlashSmall),
        };
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
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
            customMono.stat.reflex.FinalValue * 4.25f;
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
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        if (!customMono.actionBlocking && !customMono.movementActionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.combo1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = WaitCombo1(
                    direction
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

    public void Combo1End()
    {
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndCombo1, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.combo1BoolHash, false);
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
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.combo1BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.Combo1Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndCombo1, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        Combo1Stop();
    }

    IEnumerator DashSmall(Vector2 p_pos, Vector2 p_dir)
    {
        yield return Dash(
            p_dir,
            GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
            EasingFunctions.OutQuint
        );
    }

    IEnumerator FlashSmall(Vector2 p_pos, Vector2 p_dir)
    {
        yield return Flash(p_dir, 2, 0.08f);
    }

    public override void BotDoAction(DoActionParamInfo p_doActionParamInfo)
    {
        Trigger();
    }

    public override void BotDoActionContinous(DoActionParamInfo p_doActionParamInfo)
    {
        TriggerContinuous(default, p_doActionParamInfo.originToTargetOriginDirection);
    }
}
