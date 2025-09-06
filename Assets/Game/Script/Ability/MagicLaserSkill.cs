using System.Collections;
using UnityEngine;

public class MagicLaserSkill : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        successResult = new(true, ActionResultType.Cooldown, cooldown);
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
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 10f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 20f;
        /* Also use damage and gaemeeffect */
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
            customMono.stat.wisdom.FinalValue * 0.2f;
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    FireAt(
                        p_doActionParamInfo.targetCenterPosition,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override ActionResult Trigger(
        Vector2 p_location = default,
        Vector2 p_direction = default
    )
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = TriggerCoroutine(p_location, p_direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerCoroutine(Vector2 p_location = default, Vector2 p_direction = default)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill1AS.signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;

        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
            GameManager.Instance.magicLaserPool.PickOneGameEffect();
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value.transform.position =
            p_location;
        GetActionField<ActionGameEffectField>(
            ActionFieldName.GameEffect
        ).value.transform.localScale = GetActionField<ActionGameEffectField>(
            ActionFieldName.GameEffect
        )
            .value.transform.localScale.WithX(customMono.GetDirection().x > 0 ? 1 : -1);

        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
    }

    public void FireAt(Vector2 location, float duration)
    {
        StartCoroutine(FireAtCoroutine(location, duration));
    }

    IEnumerator FireAtCoroutine(Vector2 location, float duration)
    {
        customMono.actionInterval = true;
        Trigger(p_location: location);
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;
        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
    }
}
