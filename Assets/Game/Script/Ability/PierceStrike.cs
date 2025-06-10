using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class PierceStrike : SkillBase
{
    bool secondPhase = false;
    IEnumerator secondPhaseHandlerIE,
        actionIE1;
    bool phaseOneFinish = false;
    float currentTime,
        speed,
        secondPhaseDeadline;

    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 20f;
        secondPhaseDeadline = 3f;
        duration = 0.203f;
        speed = 40f;
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
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.4f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (secondPhase)
        {
            if (phaseOneFinish && !customMono.actionBlocking && !customMono.movementActionBlocking)
            {
                canUse = false;
                customMono.actionBlocking = true;
                customMono.movementActionBlocking = true;
                StopCoroutine(secondPhaseHandlerIE);
                StartCoroutine(actionIE = StartDash(direction));
                ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
                customMono.currentAction = this;
                secondPhase = false;
            }
        }
        else
        {
            if (canUse && !customMono.actionBlocking)
            {
                canUse = false;
                customMono.actionBlocking = true;
                customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
                ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
                StartCoroutine(actionIE = TriggerIE(location, direction));
                StartCoroutine(CooldownCoroutine());
                customMono.currentAction = this;
                phaseOneFinish = false;
            }
        }
    }

    IEnumerator TriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        GameEffect t_pierceStrikeEffect = GameManager
            .Instance.pierceStrikePool.PickOne()
            .gameEffect;
        t_pierceStrikeEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_pierceStrikeEffect.transform.SetLocalPositionAndRotation(
            t_pierceStrikeEffect.effectLocalPosition,
            Quaternion.Euler(t_pierceStrikeEffect.effectLocalRotation)
        );
        t_pierceStrikeEffect.transform.localScale = Vector3.one;
        t_pierceStrikeEffect.collideAndDamage.allyTags = customMono.allyTags;
        t_pierceStrikeEffect.collideAndDamage.collideDamage = damage;
        t_pierceStrikeEffect.collideAndDamage.dealDamageEvent = ChangePhase;
        customMono.rotationAndCenterObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationAndCenterObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(
                customMono.rotationAndCenterObject.transform.localScale.WithY(0),
                p_direction
            )
        );

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.stat.SetDefaultMoveSpeed();
        phaseOneFinish = true;
    }

    IEnumerator StartDash(Vector3 p_direction)
    {
        currentTime = 0;

        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        GameEffect vanishEffect = GameManager.Instance.vanishEffectPool.PickOne().gameEffect;
        vanishEffect.transform.position = transform.position;
        StartCoroutine(actionIE1 = SecondPhaseTriggerIE(p_direction: p_direction));

        p_direction = p_direction.normalized * speed * Time.fixedDeltaTime;
        while (currentTime < duration)
        {
            transform.position +=
                (1 - EasingFunctions.OutQuint(currentTime / duration)) * p_direction;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    IEnumerator SecondPhaseTriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;

        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        GameEffect t_pierceStrikeSecondPhase = GameManager
            .Instance.pierceStrikeSecondPhasePool.PickOne()
            .gameEffect;
        t_pierceStrikeSecondPhase.transform.parent = customMono.rotationAndCenterObject.transform;
        t_pierceStrikeSecondPhase.transform.SetLocalPositionAndRotation(
            t_pierceStrikeSecondPhase.effectLocalPosition,
            Quaternion.Euler(t_pierceStrikeSecondPhase.effectLocalRotation)
        );
        t_pierceStrikeSecondPhase.transform.localScale = Vector3.one;
        t_pierceStrikeSecondPhase.collideAndDamage.allyTags = customMono.allyTags;
        t_pierceStrikeSecondPhase.collideAndDamage.collideDamage = damage;
        customMono.rotationAndCenterObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationAndCenterObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(
                customMono.rotationAndCenterObject.transform.localScale.WithY(0),
                p_direction
            )
        );

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
    }

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

    void ChangePhase(float p_damageDealt)
    {
        secondPhase = true;
        secondPhaseHandlerIE = SecondPhaseHandlerIE();
        StartCoroutine(secondPhaseHandlerIE);
    }

    IEnumerator SecondPhaseHandlerIE()
    {
        yield return new WaitForSeconds(secondPhaseDeadline);
        secondPhase = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        if (actionIE1 != null)
            StopCoroutine(actionIE1);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.stat.SetDefaultMoveSpeed();
    }
}
