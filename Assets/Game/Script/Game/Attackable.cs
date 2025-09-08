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

        if (customMono.attackType == AttackType.Melee)
        {
            Attack = MeleeAttack;
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

        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
    }

    public override void Start()
    {
        base.Start();

        StatChangeRegister();
    }

    public override void Config()
    {
        if (customMono.attackType == AttackType.Melee)
            GetActionField<ActionFloatField>(ActionFieldName.Damage).value = 5f;
        else
            GetActionField<ActionFloatField>(ActionFieldName.Damage).value = 2.5f;

        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value =
            defaultCooldown =
            defaultStateSpeed =
                1f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );

        /* also damage, actionIE */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        /* Attack speed change => animator_state_speed *= attack_speed
                               => attack_cooldown /= attack_speed */
        customMono.stat.attackSpeed.finalValueChangeEvent += () =>
        {
            customMono.AnimatorWrapper.animator.SetFloat(
                "AttackAnimSpeed",
                defaultStateSpeed * customMono.stat.attackSpeed.FinalValue
            );
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value =
                defaultCooldown / customMono.stat.attackSpeed.FinalValue;
            botActionManuals[0].doActionParamInfo.nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.attackSpeed.FinalValue;

            botActionManuals[0].actionChanceAjuster = (int)(
                (customMono.stat.attackSpeed.FinalValue - 1) * 60
            );
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
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(boolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    MeleeAttackCoroutine(attackDirection)
            );
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
                .Instance.attackColliderPool.PickOne()
                .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        attackCollider.GameEffect.gameEffectSO.collideAndDamageSO.spawnedEffectOnCollide =
            customMono.meleeCollisionEffectSO;
        attackCollider.allyTags = customMono.allyTags;
        attackCollider.transform.position = customMono.firePoint.transform.position;
        attackCollider.collideDamage = GetActionField<ActionFloatField>(
            ActionFieldName.Damage
        ).value;
        attackCollider.GameEffect.rigidbody2D.AddForce(
            attackDirection.normalized * colliderForce,
            ForceMode2D.Impulse
        );

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
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
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(boolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    RangedAttackCoroutine(attackDirection)
            );
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
            .Instance.poolLink[customMono.longRangeProjectileEffectSO]
            .PickOne()
            .gameEffect;
        var t_collideAndDamage =
            t_projectileEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
            as CollideAndDamage;
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = GetActionField<ActionFloatField>(
            ActionFieldName.Damage
        ).value;
        t_projectileEffect.transform.position = customMono.firePoint.transform.position;
        t_projectileEffect.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(Vector2.right, attackDirection)
        );

        t_projectileEffect.KeepFlyingAt(attackDirection);

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
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
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.animationEventFunctionCaller.attack = false;
    }
}
