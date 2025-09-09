using System.Collections;
using UnityEngine;

public class AirRollSkill : SkillBase
{
    Vector2 p1,
        p2,
        p3,
        mid;
    float oneMinusT,
        landDelay = 0.5f;

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
            new BotActionManual(
                ActionUse.AirRoll,
                (p_doActionParamInfo) =>
                    AirRollTo(
                        p_doActionParamInfo.originToTargetOriginDirection,
                        p_doActionParamInfo.targetOriginPosition,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 1f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 1f;
        GetActionField<ActionFloatField>(ActionFieldName.Interval).value =
            Time.fixedDeltaTime / GetActionField<ActionFloatField>(ActionFieldName.Duration).value;
        audioClip = Resources.Load<AudioClip>("AudioClip/air-roll-landing");
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 0f;
        GetActionField<ActionFloatField>(ActionFieldName.Angle).value = 90f.DegToRad(); /* -90 is ok too */
        /* Also actionie */
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.statusEffect.ccImmune = true;
            ToggleAnim(GameManager.Instance.airRollBoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    TriggerCoroutine(location, direction)
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerCoroutine(Vector2 location, Vector2 direction)
    {
        /* The idea is jumping along a bezier curve, with p1 is our location, p2 is the control point
        , p3 is the destination. mid is the mid point between p1 and p3, and p2-mid is basically
        a perpendicular bisector of p1 and p3, with the same length as p1-p3, which ensures
        the curve is smooth.*/
        p1 = transform.position;
        p3 = location;
        mid = (p1 + p3) / 2;
        p2 = p3 - p1;
        p2 = p2.RotateZ(GetActionField<ActionFloatField>(ActionFieldName.Angle).value) + mid;

        customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);

        GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value = 0;
        while (
            GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
            < GetActionField<ActionFloatField>(ActionFieldName.Duration).value
        )
        {
            oneMinusT = 1 - GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value;
            /* quadratic bezier formula */
            transform.position =
                oneMinusT * oneMinusT * p1
                + 2
                    * GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
                    * oneMinusT
                    * p2
                + GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
                    * GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value
                    * p3;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            GetActionField<ActionFloatField>(ActionFieldName.CurrentTime).value +=
                GetActionField<ActionFloatField>(ActionFieldName.Interval).value;
        }

        /* After we finish jumping, we can delay to land for a short time. */
        ToggleAnim(GameManager.Instance.airRollBoolHash, false);
        ToggleAnim(GameManager.Instance.landBoolHash, true);
        customMono.audioSource.PlayOneShot(audioClip);
        customMono.statusEffect.ccImmune = false;

        yield return new WaitForSeconds(landDelay);
        ToggleAnim(GameManager.Instance.landBoolHash, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.currentAction = null;
    }

    public void AirRollTo(Vector2 direction, Vector2 location, float duration)
    {
        StartCoroutine(AirRollToCoroutine(direction, location, duration));
    }

    IEnumerator AirRollToCoroutine(Vector2 direction, Vector2 location, float duration)
    {
        customMono.actionInterval = true;
        Trigger(location: location, direction: direction);
        yield return new WaitForSeconds(duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.airRollBoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
    }
}
