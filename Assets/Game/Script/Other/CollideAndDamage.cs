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
    public new Rigidbody2D rigidbody2D;

    public enum CollideType
    {
        Single,
        Multiple,
    }

    public CollideType collideType = CollideType.Single;
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
    public OneTimeContactInteraction oneTimeContactInteraction = OneTimeContactInteraction.None;

    public enum PushEnemyOnCollideType
    {
        Random,
        BothSide,
    };

    public PushEnemyOnCollideType pushEnemyOnCollideType = PushEnemyOnCollideType.Random;
    public float pushEnemyOnCollideForce = 1f;
    public bool deactivateOnCollide = false;
    public bool spawnEffectOnCollide = false;
    public EffectPool collisionEffectPoolType;
    public ObjectPool collisionEffectPool;
    public float stunDuration;
    public float healAmmount;
    public PoisonInfo poisonInfo;
    public SlowInfo slowInfo;

    public void Awake()
    {
        switch (oneTimeContactInteraction)
        {
            case OneTimeContactInteraction.Push:
            {
                switch (pushEnemyOnCollideType)
                {
                    case PushEnemyOnCollideType.Random:
                    {
                        onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                        {
                            p_customMono.rigidbody2D.AddForce(
                                pushEnemyOnCollideForce
                                    * new Vector2(
                                        Random.Range(-1, 1),
                                        Random.Range(-1, 1)
                                    ).normalized,
                                ForceMode2D.Impulse
                            );
                        };
                        break;
                    }
                    case PushEnemyOnCollideType.BothSide:
                    {
                        onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                        {
                            p_customMono.rigidbody2D.AddForce(
                                pushEnemyOnCollideForce
                                    * (Random.Range(-1, 1) == 0 ? 1 : -1)
                                    * transform.TransformDirection(Vector3.up).normalized,
                                ForceMode2D.Impulse
                            );
                        };
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
                onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                {
                    p_customMono.statusEffect.KnockUp();
                };
                break;
            }
            case OneTimeContactInteraction.Stun:
            {
                onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                    p_customMono.statusEffect.Stun(stunDuration);
                break;
            }
            case OneTimeContactInteraction.Heal:
            {
                onTriggerEnterWithAllyCM += (p_customMono, p_collider2D) =>
                {
                    p_customMono.statusEffect.Heal(healAmmount);
                };
                break;
            }
            case OneTimeContactInteraction.Poison:
            {
                onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                {
                    p_customMono.statusEffect.Poison(poisonInfo);
                };
                break;
            }
            case OneTimeContactInteraction.Slow:
            {
                onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                {
                    p_customMono.statusEffect.Slow(slowInfo);
                };
                break;
            }
            default:
                break;
        }

        if (collideType == CollideType.Single)
        {
            onTriggerEnterWithEnemyCM += (p_customMono, other) =>
            {
                p_customMono.statusEffect.GetHit(collideDamage);
                dealDamageEvent(collideDamage);
            };
        }
        else
        {
            onTriggerStayWithEnemyCM += (p_customMono, other) =>
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
            };
        }

        onTriggerEnter2D += (p_collider2D) =>
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
        };

        onTriggerStay2D += (p_collider2D) =>
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
        };
    }

    private void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () =>
        {
            collisionEffectPool = null;
        };
#endif

        if (spawnEffectOnCollide)
        {
            if (!collisionEffectPoolType.Equals(EffectPool.LaterDecide))
                collisionEffectPool = GameManager.Instance.GetEffectPool(collisionEffectPoolType);

            if (collideType == CollideType.Single)
            {
                onTriggerEnterWithEnemyCM += (p_customMono, p_collider2D) =>
                    SpawnCollisionEffectOnEnemy(p_customMono, p_collider2D);
            }
            else
            {
                onTriggerStayWithEnemyCM += (p_customMono, p_collider2D) =>
                    SpawnCollisionEffectOnEnemy(p_customMono, p_collider2D);
            }
        }

        if (deactivateOnCollide)
            onTriggerEnterWithEnemyCM += (p_customMono, collider2D) => GameEffect.deactivate();
    }

    float t_randomBias;

    public void SpawnCollisionEffectOnEnemy(CustomMono p_customMono, Collider2D p_collider2D)
    {
        GameEffect t_collisionEffect = collisionEffectPool.PickOne().gameEffect;
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
    }
}
