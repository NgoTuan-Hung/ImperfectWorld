using System;
using System.Collections;
using UnityEngine;

public class Attackable : SkillBase
{
    public float colliderForce = 5f;
    public Func<Vector2, ActionResult> Attack;

    public override void Awake()
    {
        base.Awake();
        boolHash = Animator.StringToHash("Attack");
        audioClip = customMono.attackAudioClip;
        cooldown = defaultCooldown = defaultStateSpeed = 1f;
        if (customMono.attackType == AttackType.Melee)
        {
            Attack = MeleeAttack;
            damage = defaultDamage = 15f;
            botActionManuals.Add(
                new BotActionManual(
                    ActionUse.MeleeDamage,
                    (p_doActionParamInfo) =>
                        AttackTo(
                            p_doActionParamInfo.firePointToTargetCenterDirection,
                            p_doActionParamInfo.nextActionChoosingIntervalProposal
                        ),
                    new(nextActionChoosingIntervalProposal: 0.5f)
                )
            );
        }
        else
        {
            Attack = RangedAttack;
            damage = defaultDamage = 10f;
            botActionManuals.Add(
                new BotActionManual(
                    ActionUse.RangedDamage,
                    (p_doActionParamInfo) =>
                        AttackTo(
                            p_doActionParamInfo.firePointToTargetCenterDirection,
                            p_doActionParamInfo.nextActionChoosingIntervalProposal
                        ),
                    new(nextActionChoosingIntervalProposal: 0.5f)
                )
            );
        }
        successResult.timer = true;
        successResult.cooldown = cooldown;

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
                ActionUse.Passive,
                (p_doActionParamInfo) =>
                    Idle(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new(
                ActionUse.Roam,
                (p_doActionParamInfo) =>
                    Idle(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 1)
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
        /* Attack speed change => animator_state_speed *= attack_speed
                               => attack_cooldown /= attack_speed */
        customMono.stat.attackSpeedChangeEvent.action += () =>
        {
            customMono.AnimatorWrapper.animator.SetFloat(
                "AttackAnimSpeed",
                defaultStateSpeed * customMono.stat.AttackSpeed
            );
            cooldown = defaultCooldown / customMono.stat.AttackSpeed;
            botActionManuals[0].doActionParamInfo.nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
            botActionManuals[1].doActionParamInfo.nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
            botActionManuals[2].doActionParamInfo.nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
        };
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        return Attack(direction);
    }

    public ActionResult MeleeAttack(Vector2 attackDirection)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = MeleeAttackCoroutine(attackDirection));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }
        else
            return failResult;
    }

    IEnumerator MeleeAttackCoroutine(Vector2 attackDirection)
    {
        while (!customMono.animationEventFunctionCaller.attack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.attack = false;
        customMono.audioSource.PlayOneShot(audioClip);
        customMono.SetUpdateDirectionIndicator(
            attackDirection,
            UpdateDirectionIndicatorPriority.Low
        );
        CollideAndDamage attackCollider =
            GameManager
                .Instance.gameEffectPool.PickOne()
                .gameEffect.Init(GameManager.Instance.attackColliderSO)
                .GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        attackCollider.GameEffect.currentGameEffectSO.collideAndDamageSO.spawnedEffectOnCollide =
            customMono.meleeCollisionEffectSO;
        attackCollider.allyTags = customMono.allyTags;
        attackCollider.transform.position = customMono.firePoint.transform.position;
        attackCollider.collideDamage = damage;
        attackCollider.GameEffect.rigidbody2D.AddForce(
            attackDirection.normalized * colliderForce,
            ForceMode2D.Impulse
        );

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        customMono.currentAction = null;
    }

    public ActionResult RangedAttack(Vector2 attackDirection)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = RangedAttackCoroutine(attackDirection));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }
        else
            return failResult;
    }

    IEnumerator RangedAttackCoroutine(Vector2 attackDirection)
    {
        while (!customMono.animationEventFunctionCaller.attack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.attack = false;
        customMono.audioSource.PlayOneShot(audioClip);
        customMono.SetUpdateDirectionIndicator(
            attackDirection,
            UpdateDirectionIndicatorPriority.Low
        );
        GameEffect t_projectileEffect = GameManager
            .Instance.gameEffectPool.PickOne()
            .gameEffect.Init(customMono.longRangeProjectileEffectSO);
        var t_collideAndDamage =
            t_projectileEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = damage;
        t_projectileEffect.transform.position = customMono.firePoint.transform.position;
        t_projectileEffect.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(Vector2.right, attackDirection)
        );

        t_projectileEffect.KeepFlyingAt(attackDirection, customMono.longRangeProjectileEffectSO);

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        customMono.currentAction = null;
    }

    public void AttackTo(Vector2 direction, float duration)
    {
        StartCoroutine(AttackToCoroutine(direction, duration));
    }

    IEnumerator AttackToCoroutine(Vector2 direction, float duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: direction);
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public void Idle(Vector2 direction, float duration)
    {
        StartCoroutine(IdleCoroutine(direction, duration));
    }

    IEnumerator IdleCoroutine(Vector2 direction, float duration)
    {
        customMono.actionInterval = true;
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.animationEventFunctionCaller.attack = false;
        customMono.currentAction = null;
    }
}
