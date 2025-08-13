using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class BladeOfMinhKhai : SkillBase
{
    IEnumerator actionIE1;

    public override void Awake()
    {
        base.Awake();
        duration = 0.203f;
        cooldown = 5f;
        lifeStealPercent = 0.25f;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 10f;
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
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetAway,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(
                    nextActionChoosingIntervalProposal: 0.5f,
                    isDirectionModify: true,
                    directionModifier: -1
                )
            )
        );
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        damage = customMono.stat.reflex.FinalValue * 2f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = StartDash(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;
            return successResult;
        }

        return failResult;
    }

    float currentTime,
        dashSpeed = 40;

    IEnumerator StartDash(Vector3 p_direction)
    {
        currentTime = 0;

        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        GameEffect vanishEffect = GameManager.Instance.vanishEffectPool.PickOne().gameEffect;
        vanishEffect.transform.position = transform.position;
        StartCoroutine(actionIE1 = WaitSpawnSlashSignal(p_direction));

        p_direction = p_direction.normalized * dashSpeed * Time.fixedDeltaTime;
        while (currentTime < duration)
        {
            transform.position +=
                (1 - EasingFunctions.OutQuint(currentTime / duration)) * p_direction;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }
    }

    IEnumerator WaitSpawnSlashSignal(Vector3 p_direction)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;

        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;

        GameEffect t_slashEffect = GameManager
            .Instance.bladeOfMinhKhaiSlashEffectPool.PickOne()
            .gameEffect;
        t_slashEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_slashEffect.transform.SetLocalPositionAndRotation(
            t_slashEffect.gameEffectSO.effectLocalPosition,
            Quaternion.Euler(t_slashEffect.gameEffectSO.effectLocalRotation)
        );

        var t_collideAndDamage = (CollideAndDamage)
            t_slashEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);

        /* This is needed because it will change parent eventually */
        t_slashEffect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = damage;
        t_collideAndDamage.dealDamageEvent = LifeSteal;
        customMono.rotationAndCenterObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationAndCenterObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(
                customMono.rotationAndCenterObject.transform.localScale,
                p_direction
            )
        );

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
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

    public override void LifeSteal(float damageDealt)
    {
        customMono.stat.currentHealthPoint.Value += damageDealt * lifeStealPercent;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        StopCoroutine(actionIE1);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.currentAction = null;
    }
}
