using System.Collections;
using UnityEngine;

public class HeliosGaze : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        cooldown = .5f;

        AddActionManuals();
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
                    BotTrigger(p_doActionParamInfo.nextActionChoosingIntervalProposal),
                new(nextActionChoosingIntervalProposal: Time.fixedDeltaTime)
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
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0.5f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 5f;
        GetActionField<ActionFloatField>(ActionFieldName.Interval).value = .5f;
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.might.FinalValue * 0.2f;
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse)
        {
            canUse = false;
            FireLaser();
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;

            return successResult;
        }

        return failResult;
    }

    void FireLaser()
    {
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value = GameManager
            .Instance.heliosGazeRayPool.PickOne()
            .gameEffect;
        GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
            .value.SetUpCollideAndDamage(
                customMono.allyTags,
                GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );

        if (customMono.botSensor.currentNearestEnemy == null)
        {
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.PlaceAndLookAt(
                    new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * 18
                        + customMono.rotationAndCenterObject.transform.position,
                    customMono.rotationAndCenterObject.transform,
                    GetActionField<ActionFloatField>(ActionFieldName.Interval).value
                );
        }
        else
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.PlaceAndLookAt(
                    new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * 9
                        + customMono
                            .botSensor
                            .currentNearestEnemy
                            .rotationAndCenterObject
                            .transform
                            .position,
                    customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform,
                    GetActionField<ActionFloatField>(ActionFieldName.Interval).value
                );

        customMono.currentAction = null;
    }

    void BotTrigger(float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_duration));
    }

    IEnumerator BotTriggerIE(float p_duration)
    {
        customMono.actionInterval = true;
        Trigger();
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
    }
}
