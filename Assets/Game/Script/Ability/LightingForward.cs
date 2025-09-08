using System.Collections;
using UnityEngine;

public class LightingForward : SkillBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.originToTargetOriginDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.4f)
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
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 5;
        /* Distance between each lighting */
        GetActionField<ActionFloatField>(ActionFieldName.Distance).value = 2f;
        GetActionField<ActionFloatField>(ActionFieldName.Interval).value = 0.1f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 20f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
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
            customMono.stat.wisdom.FinalValue * 2.75f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        base.WhileWaiting(p_direction);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
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
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    WaitSpawnLighting(direction)
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnLighting(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill1AS.signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;
        StartCoroutine(SpawnLightingIE(p_direction));

        while (!customMono.animationEventFunctionCaller.mainSkill1AS.end)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.currentAction = null;
    }

    IEnumerator SpawnLightingIE(Vector3 p_direction)
    {
        Vector3 t_lightingOrigin = transform.position;
        for (int i = 0; i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value; i++)
        {
            SpawnNormalEffect(
                GameManager.Instance.lightingForwardLightingPool.PickOneGameEffect(),
                t_lightingOrigin
                    + i
                        * GetActionField<ActionFloatField>(ActionFieldName.Distance).value
                        * p_direction.normalized,
                default,
                true
            );

            yield return new WaitForSeconds(
                GetActionField<ActionFloatField>(ActionFieldName.Interval).value
            );
        }
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_direction, p_duration));
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
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;
        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
    }
}
