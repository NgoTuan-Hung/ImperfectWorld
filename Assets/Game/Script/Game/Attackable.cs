using System;
using System.Collections;
using UnityEngine;

public class Attackable : SkillBase
{
    GameObject attackColliderPrefab;
    protected static ObjectPool attackColliderPool;
    public float colliderForce = 5f;
    public Action<Vector2> Attack;

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
            attackColliderPrefab = Resources.Load("AttackCollider") as GameObject;
            attackColliderPool ??= new ObjectPool(
                attackColliderPrefab,
                100,
                new PoolArgument(ComponentType.CollideAndDamage, PoolArgument.WhereComponent.Self)
            );
            botActionManuals.Add(
                new BotActionManual(
                    ActionUse.MeleeDamage,
                    (direction, location, nextActionChoosingIntervalProposal) =>
                        AttackTo(direction, nextActionChoosingIntervalProposal),
                    0.5f,
                    true,
                    1
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
                    (direction, location, nextActionChoosingIntervalProposal) =>
                        AttackTo(direction, nextActionChoosingIntervalProposal),
                    0.5f,
                    true,
                    1
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
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Passive,
                (direction, location, nextActionChoosingIntervalProposal) =>
                    Idle(direction, nextActionChoosingIntervalProposal),
                0.5f
            )
        );
        botActionManuals.Add(
            new(
                ActionUse.Roam,
                (direction, location, nextActionChoosingIntervalProposal) =>
                    Idle(direction, nextActionChoosingIntervalProposal),
                1f
            )
        );
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () =>
        {
            attackColliderPool = null;
        };
#endif

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
            botActionManuals[0].nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
            botActionManuals[1].nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
            botActionManuals[2].nextActionChoosingIntervalProposal =
                0.5f / customMono.stat.AttackSpeed;
        };
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        Attack(direction);
    }

    public void MeleeAttack(Vector2 attackDirection)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = MeleeAttackCoroutine(attackDirection));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
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
        CollideAndDamage attackCollider = attackColliderPool
            .PickOne(po =>
            {
                po.collideAndDamage.collisionEffectPool = GameManager.Instance.GetEffectPool(
                    customMono.meleeCollisionEP
                );
            })
            .collideAndDamage;
        attackCollider.allyTags = customMono.allyTags;
        attackCollider.transform.position = customMono.firePoint.transform.position;
        attackCollider.collideDamage = damage;
        attackCollider.rigidbody2D.AddForce(
            attackDirection.normalized * colliderForce,
            ForceMode2D.Impulse
        );

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.stat.SetDefaultMoveSpeed();
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
    }

    public void RangedAttack(Vector2 attackDirection)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = RangedAttackCoroutine(attackDirection));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
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
        GameEffect projectileEffect = GameManager
            .Instance.effectPoolDict[customMono.longRangeProjectileEP]
            .PickOne()
            .gameEffect;
        projectileEffect.collideAndDamage.allyTags = customMono.allyTags;
        projectileEffect.collideAndDamage.collideDamage = damage;
        projectileEffect.transform.position = customMono.firePoint.transform.position;
        projectileEffect.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(Vector2.right, attackDirection)
        );

        projectileEffect.KeepFlyingAt(attackDirection);

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.stat.SetDefaultMoveSpeed();
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
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
        customMono.stat.SetDefaultMoveSpeed();
        customMono.actionBlocking = false;
        ToggleAnim(boolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.animationEventFunctionCaller.attack = false;
    }
}
