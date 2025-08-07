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
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 25f;
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
        damage = customMono.stat.reflex.FinalValue * 2.5f;
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
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnSlashSignal(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;

            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnSlashSignal(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;

        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        GameEffect t_swordTempestSlashEffect = GameManager
            .Instance.swordTempestSlash1Pool.PickOne()
            .gameEffect;
        t_swordTempestSlashEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlashEffect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlashEffect.gameEffectSO.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlashEffect.gameEffectSO.effectLocalRotation)
        );

        var t_collideAndDamage =
            t_swordTempestSlashEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;

        /* This is needed because it will change parent eventually */
        t_swordTempestSlashEffect.transform.localScale = Vector3.one;
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
        t_swordTempestSlashEffect = GameManager
            .Instance.swordTempestSlash2Pool.PickOne()
            .gameEffect;
        t_swordTempestSlashEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlashEffect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlashEffect.gameEffectSO.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlashEffect.gameEffectSO.effectLocalRotation)
        );

        t_collideAndDamage =
            t_swordTempestSlashEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;

        /* This is needed because it will change parent eventually */
        t_swordTempestSlashEffect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 2 * damage;

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        t_swordTempestSlashEffect = GameManager
            .Instance.swordTempestSlash1Pool.PickOne()
            .gameEffect;
        t_swordTempestSlashEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlashEffect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlashEffect.gameEffectSO.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlashEffect.gameEffectSO.effectLocalRotation)
        );

        t_collideAndDamage =
            t_swordTempestSlashEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;

        /* This is needed because it will change parent eventually */
        t_swordTempestSlashEffect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 3 * damage;

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        t_swordTempestSlashEffect = GameManager
            .Instance.swordTempestSlash3Pool.PickOne()
            .gameEffect;
        t_swordTempestSlashEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        t_swordTempestSlashEffect.transform.SetLocalPositionAndRotation(
            t_swordTempestSlashEffect.gameEffectSO.effectLocalPosition,
            Quaternion.Euler(t_swordTempestSlashEffect.gameEffectSO.effectLocalRotation)
        );

        t_collideAndDamage =
            t_swordTempestSlashEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;

        /* This is needed because it will change parent eventually */
        t_swordTempestSlashEffect.transform.localScale = Vector3.one;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = 4 * damage;

        while (!customMono.animationEventFunctionCaller.endMainSkill3)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
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
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
