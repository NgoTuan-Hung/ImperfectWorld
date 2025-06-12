using System;
using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class AirRollSkill : SkillBase
{
    Vector2 p1,
        p2,
        p3,
        mid;
    float t,
        oneMinusT,
        moveLength,
        landDelay = 0.5f;
    int landBoolHash;
    float angle = 90f.DegToRad(); /* -90 is ok too */

    public override void Awake()
    {
        base.Awake();
        duration = 1f;
        moveLength = Time.fixedDeltaTime / duration;
        boolHash = Animator.StringToHash("AirRoll");
        landBoolHash = Animator.StringToHash("Land");
        audioClip = Resources.Load<AudioClip>("AudioClip/air-roll-landing");
        cooldown = 0f;

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

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.statusEffect.ccImmune = true;
            ToggleAnim(boolHash, true);
            actionIE = TriggerCoroutine(location, direction);
            StartCoroutine(actionIE);
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
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
        p2 = p2.RotateZ(angle) + mid;

        customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);

        t = 0;
        while (t < duration)
        {
            oneMinusT = 1 - t;
            /* quadratic bezier formula */
            transform.position = oneMinusT * oneMinusT * p1 + 2 * t * oneMinusT * p2 + t * t * p3;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            t += moveLength;
        }

        /* After we finish jumping, we can delay to land for a short time. */
        ToggleAnim(boolHash, false);
        ToggleAnim(landBoolHash, true);
        customMono.audioSource.PlayOneShot(audioClip);
        customMono.statusEffect.ccImmune = false;

        yield return new WaitForSeconds(landDelay);
        ToggleAnim(landBoolHash, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.currentAction = null;
    }

    public void AirRollTo(Vector2 direction, Vector2 location, float duration)
    {
        botIE = AirRollToCoroutine(direction, location, duration);
        StartCoroutine(botIE);
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
        ToggleAnim(boolHash, false);
        StopCoroutine(actionIE);
        customMono.currentAction = null;
    }
}
