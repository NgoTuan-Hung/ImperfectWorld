using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum OneTimeContactInteraction
{
    None,
    Push,
    KnockUp,
    Stun,
    Heal,
    Poison,
    Slow,
}

public class CollideAndDamage : MonoEditor, IGameEffectBehaviour
{
    public HashSet<string> allyTags = new();
    public float collideDamage = 1f;
    public float multipleCollideInterval = 0.05f;
    Action<Collider2D> onTriggerEnter2D = (other) => { };
    Action<Collider2D> onTriggerStay2D = (other) => { };

    /// <summary>
    /// After getting customMono from collision, you can do
    /// any thing with it (add force, etc.)
    /// </summary>
    Action<CustomMono, Collider2D> onTriggerEnterWithEnemyCM = (p_customMono, collider2D) => { };
    Action<CustomMono, Collider2D> onTriggerStayWithEnemyCM = (p_customMono, collider2D) => { };
    Action<CustomMono, Collider2D> onTriggerEnterWithAllyCM = (p_customMono, collider2D) => { };
    public Action<float> dealDamageEvent = (damageDealt) => { };
    public float pushEnemyOnCollideForce = 1f;
    public Vector3 pushDirection = Vector3.zero;
    public float stunDuration;
    public float healAmmount;
    public PoisonInfo poisonInfo;
    public SlowInfo slowInfo;

    public void Awake() { }

    public override void Start()
    {
        base.Start();
    }

    float t_randomBias;

    public void SpawnCollisionEffectOnEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        GameEffect t_collisionEffect = GameManager
            .Instance.poolLink[GameEffect.gameEffectSO.collideAndDamageSO.spawnedEffectOnCollide]
            .PickOneGameEffect();

        t_randomBias = Random.Range(0, 1f);
        t_collisionEffect.transform.position =
            p_collider2D.bounds.center * t_randomBias
            + (1 - t_randomBias) * (Vector3)p_collider2D.ClosestPoint(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        onTriggerStay2D(other);
    }

    public GameEffect GameEffect { get; set; }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;

        var p_collideAndDamageSO = GameEffect.gameEffectSO.collideAndDamageSO;
        #region Contact Action
        switch (p_collideAndDamageSO.oneTimeContactInteraction)
        {
            case OneTimeContactInteraction.Push:
            {
                switch (p_collideAndDamageSO.pushEnemyOnCollideType)
                {
                    case CollideAndDamageSO.PushEnemyOnCollideType.Random:
                    {
                        onTriggerEnterWithEnemyCM += PushEnemyRandom;
                        break;
                    }
                    case CollideAndDamageSO.PushEnemyOnCollideType.BothSide:
                    {
                        onTriggerEnterWithEnemyCM += PushEnemyBothSide;
                        break;
                    }
                    case CollideAndDamageSO.PushEnemyOnCollideType.LaterDecide:
                    {
                        onTriggerEnterWithEnemyCM += PushEnemyLaterDecide;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
                break;
            }
            case OneTimeContactInteraction.KnockUp:
            {
                onTriggerEnterWithEnemyCM += KnockUpEnemy;
                break;
            }
            case OneTimeContactInteraction.Stun:
            {
                onTriggerEnterWithEnemyCM += StunEnemy;

                break;
            }
            case OneTimeContactInteraction.Heal:
            {
                onTriggerEnterWithAllyCM += HealAlly;
                break;
            }
            case OneTimeContactInteraction.Poison:
            {
                onTriggerEnterWithEnemyCM += PoisonEnemy;
                break;
            }
            case OneTimeContactInteraction.Slow:
            {
                onTriggerEnterWithEnemyCM += SlowEnemy;
                break;
            }
            default:
                break;
        }
        #endregion

        #region Dealing Damage
        if (p_collideAndDamageSO.collideType == CollideAndDamageSO.CollideType.Single)
            onTriggerEnterWithEnemyCM += DealDamageOnTriggerEnter;
        else
            onTriggerStayWithEnemyCM += DealDamageOnTriggerStay;

        #endregion


        onTriggerEnter2D += OnTriggerEnter2DLogic;
        onTriggerStay2D += OnTriggerStay2DLogic;

        if (p_collideAndDamageSO.spawnEffectOnCollide)
        {
            if (p_collideAndDamageSO.collideType == CollideAndDamageSO.CollideType.Single)
            {
                onTriggerEnterWithEnemyCM += SpawnCollisionEffectOnEnemy;
            }
            else
            {
                onTriggerStayWithEnemyCM += SpawnCollisionEffectOnEnemy;
            }
        }

        if (p_collideAndDamageSO.deactivateOnCollide)
            onTriggerEnterWithEnemyCM += DeactivateOnCollide;
    }

    /// <summary>
    /// Decide logic to apply for colliding with different types of object
    /// </summary>
    /// <param name="p_collider2D"></param>
    void OnTriggerEnter2DLogic(Collider2D p_collider2D)
    {
        if (p_collider2D.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(
                p_collider2D.transform.parent.gameObject
            );
            if (t_customMono != null)
            {
                if (!allyTags.Contains(t_customMono.tag))
                {
                    onTriggerEnterWithEnemyCM(t_customMono, p_collider2D);
                }
                else
                {
                    onTriggerEnterWithAllyCM(t_customMono, p_collider2D);
                }
            }
        }
    }

    /// <summary>
    /// Decide logic to apply for colliding with different types of object
    /// </summary>
    /// <param name="p_collider2D"></param>
    void OnTriggerStay2DLogic(Collider2D p_collider2D)
    {
        if (p_collider2D.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(
                p_collider2D.transform.parent.gameObject
            );
            if (t_customMono != null)
            {
                if (!allyTags.Contains(t_customMono.tag))
                {
                    onTriggerStayWithEnemyCM(t_customMono, p_collider2D);
                }
                else { }
            }
        }
    }

    void DealDamageOnTriggerEnter(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.GetHit(collideDamage);
        dealDamageEvent(collideDamage);
    }

    void DealDamageOnTriggerStay(CustomMono p_customMono, Collider2D p_collider2D)
    {
        /* Check if the next collision is allowed, if yes, reset the timer
                for this customMono and decrease its health, otherwise do nothing.
                The timer progression is handled in CustomMono.FixedUpdate. */

        try
        {
            if (p_customMono.multipleCollideTimersDict[GetHashCode()].currentTime <= 0)
            {
                p_customMono.multipleCollideTimersDict[GetHashCode()].currentTime =
                    multipleCollideInterval;
                p_customMono.statusEffect.GetHit(collideDamage);
                dealDamageEvent(collideDamage);
            }
        }
        catch (KeyNotFoundException)
        {
            p_customMono.AddMultipleCollideTimer(GetHashCode(), multipleCollideInterval);
            p_customMono.statusEffect.GetHit(collideDamage);
            dealDamageEvent(collideDamage);
        }
    }

    void PushEnemyRandom(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.rigidbody2D.AddForce(
            pushEnemyOnCollideForce
                * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized,
            ForceMode2D.Impulse
        );
    }

    void PushEnemyBothSide(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.rigidbody2D.AddForce(
            pushEnemyOnCollideForce
                * (Random.Range(-1, 1) == 0 ? 1 : -1)
                * transform.TransformDirection(Vector3.up).normalized,
            ForceMode2D.Impulse
        );
    }

    void PushEnemyLaterDecide(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.rigidbody2D.AddForce(
            pushEnemyOnCollideForce * pushDirection.normalized,
            ForceMode2D.Impulse
        );
    }

    void KnockUpEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.KnockUp();
    }

    void StunEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.Stun(stunDuration);
    }

    void HealAlly(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.Heal(healAmmount);
    }

    void PoisonEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.Poison(poisonInfo);
    }

    void SlowEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        p_customMono.statusEffect.Slow(slowInfo);
    }

    void DeactivateOnCollide(CustomMono p_customMono, Collider2D p_collider2D) =>
        GameEffect.deactivate();
}
