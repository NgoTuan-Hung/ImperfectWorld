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
        damage = defaultDamage = 50f;
        lifeStealPercent = 0.25f;
        // dashSpeed *= Time.deltaTime;
        // boolhash = ...

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
                ActionUse.GetCloser,
                (direction, location, nextActionChoosingIntervalProposal) =>
                    BotTrigger(direction, nextActionChoosingIntervalProposal),
                0.5f,
                true,
                1
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetAway,
                (direction, location, nextActionChoosingIntervalProposal) =>
                    BotTrigger(direction, nextActionChoosingIntervalProposal),
                0.5f,
                true,
                -1
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void WhileWaiting(Vector2 vector2)
    {
        customMono.SetUpdateDirectionIndicator(vector2, UpdateDirectionIndicatorPriority.Low);
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.bladeOfMinhKhaiBoolHash, true);
            StartCoroutine(actionIE = StartDash(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
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
        while (!customMono.animationEventFunctionCaller.bladeOfMinhKhaiSpawnSlash)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.bladeOfMinhKhaiSpawnSlash = false;

        customMono.rotationObject.transform.localRotation = Quaternion.identity;
        GameEffect t_slashEffect = GameManager
            .Instance.bladeOfMinhKhaiSlashEffectPool.PickOne()
            .gameEffect;
        t_slashEffect.transform.parent = customMono.rotationObject.transform;
        t_slashEffect.transform.SetLocalPositionAndRotation(
            t_slashEffect.effectLocalPosition,
            Quaternion.Euler(t_slashEffect.effectLocalRotation)
        );
        /* This is needed because it will change parent eventually */
        t_slashEffect.transform.localScale = Vector3.one;
        t_slashEffect.collideAndDamage.allyTags = customMono.allyTags;
        t_slashEffect.collideAndDamage.collideDamage = damage;
        t_slashEffect.collideAndDamage.dealDamageEvent = LifeSteal;
        customMono.rotationObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(customMono.rotationObject.transform.localScale, p_direction)
        );

        while (!customMono.animationEventFunctionCaller.endBladeOfMinhKhai)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endBladeOfMinhKhai = false;
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.bladeOfMinhKhaiBoolHash, false);
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
        customMono.stat.Health += damageDealt * lifeStealPercent;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.bladeOfMinhKhaiBoolHash, false);
        StopCoroutine(actionIE);
        StopCoroutine(actionIE1);
        customMono.animationEventFunctionCaller.bladeOfMinhKhaiSpawnSlash = false;
        customMono.animationEventFunctionCaller.endBladeOfMinhKhai = false;
    }
}
