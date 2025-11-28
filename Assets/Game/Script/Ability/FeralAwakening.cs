using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// OnEnable should run after Stat.OnEnable
/// since it will clear all modifiers
/// </summary>
[DefaultExecutionOrder(1)]
public class FeralAwakening : SkillBase
{
    bool used = false;
    static ChampionData meleeCD,
        rangedCD;
    FloatStatModifier aspdBuff,
        atkRangeBuff,
        hpBuff;
    AnimatorController feralAwakeningScreenEffectAC;

    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
        meleeCD =
            meleeCD != null
                ? meleeCD
                : Resources.Load<ChampionData>("ScriptableObject/ChampionData/MukaiMeleeCD");
        rangedCD =
            rangedCD != null
                ? rangedCD
                : Resources.Load<ChampionData>("ScriptableObject/ChampionData/MukaiRangedCD");
        feralAwakeningScreenEffectAC =
            feralAwakeningScreenEffectAC != null
                ? feralAwakeningScreenEffectAC
                : Resources.Load<AnimatorController>(
                    "AnimatorController/FeralAwakeningScreenEffectAC"
                );
    }

    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LateOnEnable());
    }

    IEnumerator LateOnEnable()
    {
        yield return null;
        SwitchStance(false);
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
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );

        GameManager.Instance.battleEndCallback += ResetUse;
        customMono.stat.beforeDeathCallback += Proc;
        aspdBuff = new(2f, FloatStatModifierType.Additive);
        atkRangeBuff = new(8.75f, FloatStatModifierType.Additive);
        hpBuff = new(1000f, FloatStatModifierType.Additive);
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
        aspdBuff.value = 2f + customMono.stat.might.FinalValue * 0.01f;
        hpBuff.value = 1000f + customMono.stat.might.FinalValue * 10f;
    }

    void Proc()
    {
        if (
            !used
            && customMono.stat.currentHealthPoint.Value
                <= customMono.stat.healthPoint.FinalValue / 2
        )
        {
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    TransformIE()
            );
        }
    }

    IEnumerator TransformIE()
    {
        used = true;
        while (customMono.actionBlocking)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        GameUIManager.Instance.ApplyScreenEffect(feralAwakeningScreenEffectAC);
        customMono.actionBlocking = true;
        customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
        customMono.currentAction = this;
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill2Signal
            )
        )
            yield return null;

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);
        SwitchStance(true);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill2)
        )
            yield return null;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.currentAction = null;
        GameUIManager.Instance.DisableScreenEffect();
    }

    void ResetUse() => used = false;

    public override void BotDoAction(DoActionParamInfo p_doActionParamInfo)
    {
        Trigger(p_customMono: p_doActionParamInfo.target);
    }

    void SwitchStance(bool melee)
    {
        if (melee)
        {
            customMono.championData = meleeCD;
            (customMono.skill.skillBases[0] as Attack).SwitchAttackType(AttackType.Melee);
            customMono.stat.attackSpeed.AddModifier(aspdBuff);
            customMono.stat.healthPoint.AddModifier(hpBuff);
            customMono.stat.currentHealthPoint.Value += hpBuff.value;
            ToggleAnim(GameManager.Instance.meleeStanceBoolHash, true);
        }
        else
        {
            customMono.championData = rangedCD;
            (customMono.skill.skillBases[0] as Attack).SwitchAttackType(AttackType.Ranged);
            customMono.stat.attackRange.AddModifier(atkRangeBuff);
            ToggleAnim(GameManager.Instance.meleeStanceBoolHash, false);
        }
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill2Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill2, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        GameUIManager.Instance.DisableScreenEffect();
    }

    private void OnDestroy()
    {
        GameManager.Instance.battleEndCallback -= ResetUse;
    }
}
