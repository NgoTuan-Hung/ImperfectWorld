using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class OrbitalNemesis : SkillBase
{
    public override void Awake()
    {
        base.Awake();
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
            new BotActionManual(
                ActionUse.KeepDistance,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
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
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 0.1f;
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0.5f;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 5f;
        GetActionField<ActionFloatField>(ActionFieldName.Angle).value = 45f.DegToRad();
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = 5 * 5;
        /* animation variantions for dash */
        GetActionField<ActionIntField>(ActionFieldName.Variants).value = 3;
        /* In this skill, this will be the portion each variation hold in blend tree. */
        GetActionField<ActionFloatField>(ActionFieldName.Blend).value =
            1f / (GetActionField<ActionIntField>(ActionFieldName.Variants).value - 1);
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 2f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        /* Also use current time, direction */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.movementActionBlocking = true;
            customMono.actionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            customMono.AnimatorWrapper.animator.SetTrigger(
                GameManager.Instance.mainSkill2TriggerHash
            );
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = StartDash()
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

    IEnumerator StartDash()
    {
        if (customMono.botSensor.currentNearestEnemy == null)
        {
            int t_variation = Random.Range(
                0,
                GetActionField<ActionIntField>(ActionFieldName.Variants).value
            );
            GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                GetRandomVariation(t_variation).normalized;
            SetBlend(
                GameManager.Instance.mainSkill2BlendHash,
                GetActionField<ActionFloatField>(ActionFieldName.Blend).value * t_variation
            );
        }
        else
        {
            GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position
                - customMono.rotationAndCenterObject.transform.position;

            /* If we are too far from the enemy, dash closer */
            if (
                GetActionField<ActionVector3Field>(ActionFieldName.Direction).value.sqrMagnitude
                > GetActionField<ActionFloatField>(ActionFieldName.Range).value
            )
            {
                GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                    GetActionField<ActionVector3Field>(ActionFieldName.Direction).value.normalized;
                SetBlend(GameManager.Instance.mainSkill2BlendHash, 0);
            }
            else
            {
                GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                    GetActionField<ActionVector3Field>(ActionFieldName.Direction)
                        .value.WithY(0)
                        .AsVector2()
                        .RotateZ(
                            GetActionField<ActionFloatField>(ActionFieldName.Angle).value
                                * Random.Range(0, 2)
                            == 0
                                ? 1
                                : -1
                        )
                        .normalized;

                if (GetActionField<ActionVector3Field>(ActionFieldName.Direction).value.y < 0)
                    SetBlend(
                        GameManager.Instance.mainSkill2BlendHash,
                        GetActionField<ActionFloatField>(ActionFieldName.Blend).value * 2
                    );
                else
                    SetBlend(
                        GameManager.Instance.mainSkill2BlendHash,
                        GetActionField<ActionFloatField>(ActionFieldName.Blend).value
                    );
            }
        }

        customMono.SetUpdateDirectionIndicator(
            GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
            UpdateDirectionIndicatorPriority.Low
        );

        while (!customMono.animationEventFunctionCaller.mainSkill2Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill2Signal = false;

        GameManager
            .Instance.orbitalNemesisDashPool.PickOneGameEffect()
            .PlaceAndLookAt(
                transform.position
                    + Vector3.right.Mul(
                        new Vector3(
                            0.5f
                                * (
                                    GetActionField<ActionVector3Field>(
                                        ActionFieldName.Direction
                                    ).value.x > 0
                                        ? -1
                                        : 1
                                ),
                            1,
                            1
                        )
                    ),
                GetActionField<ActionVector3Field>(ActionFieldName.Direction).value
            );

        yield return Dash(
            GetActionField<ActionVector3Field>(ActionFieldName.Direction).value,
            GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
            EasingFunctions.OutQuint
        );
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.movementActionBlocking = false;
        customMono.actionBlocking = false;
        customMono.currentAction = null;
    }

    Vector2 GetRandomVariation(int p_varitationIndex) =>
        p_varitationIndex switch
        {
            0 => Vector2.right.WithX(Random.Range(0, 2) == 0 ? 1 : -1),
            1 => Vector2.right.RotateZ(
                GetActionField<ActionFloatField>(ActionFieldName.Angle).value
            ) * new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, 1),
            2 => Vector2.right.RotateZ(
                -GetActionField<ActionFloatField>(ActionFieldName.Angle).value
            ) * new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, 1),
            _ => default,
        };

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotStartDash(p_direction, p_duration));
    }

    IEnumerator BotStartDash(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.movementActionBlocking = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
    }
}
