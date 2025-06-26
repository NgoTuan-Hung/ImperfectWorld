using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class SwordTempest : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 20f;
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
                ActionUse.MeleeDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnSlashSignal(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator WaitSpawnSlashSignal(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;

        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        GameEffect t_swordTempestSlash1Effect = GameManager
            .Instance.swordTempestSlash1Pool.PickOne()
            .gameEffect;
        t_swordTempestSlash1Effect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlash1Effect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlash1Effect.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlash1Effect.effectLocalRotation)
        );

        var t_collideAndDamage = t_swordTempestSlash1Effect.GetBehaviour<CollideAndDamage>();

        /* This is needed because it will change parent eventually */
        t_swordTempestSlash1Effect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = damage;
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

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        GameEffect t_swordTempestSlash2Effect = GameManager
            .Instance.swordTempestSlash2Pool.PickOne()
            .gameEffect;
        t_swordTempestSlash2Effect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlash2Effect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlash2Effect.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlash2Effect.effectLocalRotation)
        );

        t_collideAndDamage = t_swordTempestSlash2Effect.GetBehaviour<CollideAndDamage>();

        /* This is needed because it will change parent eventually */
        t_swordTempestSlash2Effect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 2 * damage;

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        t_swordTempestSlash1Effect = GameManager
            .Instance.swordTempestSlash1Pool.PickOne()
            .gameEffect;
        t_swordTempestSlash1Effect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlash1Effect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlash1Effect.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlash1Effect.effectLocalRotation)
        );

        t_collideAndDamage = t_swordTempestSlash1Effect.GetBehaviour<CollideAndDamage>();

        /* This is needed because it will change parent eventually */
        t_swordTempestSlash1Effect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 3 * damage;

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        GameEffect t_swordTempestSlash3Effect = GameManager
            .Instance.swordTempestSlash3Pool.PickOne()
            .gameEffect;
        t_swordTempestSlash3Effect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlash3Effect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlash3Effect.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlash3Effect.effectLocalRotation)
        );

        t_collideAndDamage = t_swordTempestSlash3Effect.GetBehaviour<CollideAndDamage>();

        /* This is needed because it will change parent eventually */
        t_swordTempestSlash3Effect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 4 * damage;

        while (!customMono.animationEventFunctionCaller.endMainSkill3)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.currentAction = null;
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotTriggerIE(p_direction, p_duration));
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
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        customMono.currentAction = null;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
    }
}
