using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlacistreamBehaviour : MonoEditor, IGameEffectBehaviour
{
    public CustomMono owner;
    public float firstCollisionDamage = 1f;
    public float secondCollisionDamage = 1f;
    public float multipleCollideInterval = 0.05f;
    public Action<float> dealDamageEvent = (damageDealt) => { };
    DealDamageGameEventData dealDamageGameEventData = new();
    List<Collider2D> collidersThisTrig = new();

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
        if (other.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(other);
            if (t_customMono != null)
            {
                if (!owner.allyTags.Contains(t_customMono.tag))
                {
                    other.GetContacts(collidersThisTrig);
                    collidersThisTrig.ForEach(collider =>
                    {
                        if (collider == GameEffect.boxCollider2Ds[0])
                        {
                            dealDamageGameEventData.damage = t_customMono.statusEffect.GetHit(
                                firstCollisionDamage
                            );
                            dealDamageGameEventData.dealer = owner;
                            dealDamageGameEventData.target = t_customMono;
                            GameManager
                                .Instance.selfEvents[dealDamageGameEventData.dealer][
                                    GameEventType.DealDamage
                                ]
                                .action(dealDamageGameEventData);
                            dealDamageEvent(firstCollisionDamage);
                        }
                    });
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.parent != null)
        {
            /* Since parent will have customMono, not this */
            CustomMono t_customMono = GameManager.Instance.GetCustomMono(other);
            if (t_customMono != null)
            {
                if (!owner.allyTags.Contains(t_customMono.tag))
                {
                    other.GetContacts(collidersThisTrig);
                    collidersThisTrig.ForEach(collider =>
                    {
                        if (collider == GameEffect.boxCollider2Ds[1])
                        {
                            try
                            {
                                if (
                                    t_customMono
                                        .multipleCollideTimersDict[GetHashCode()]
                                        .currentTime <= 0
                                )
                                {
                                    t_customMono
                                        .multipleCollideTimersDict[GetHashCode()]
                                        .currentTime = multipleCollideInterval;
                                    dealDamageGameEventData.damage =
                                        t_customMono.statusEffect.GetHit(secondCollisionDamage);
                                    dealDamageGameEventData.dealer = owner;
                                    dealDamageGameEventData.target = t_customMono;
                                    GameManager
                                        .Instance.selfEvents[dealDamageGameEventData.dealer][
                                            GameEventType.DealDamage
                                        ]
                                        .action(dealDamageGameEventData);
                                    dealDamageEvent(secondCollisionDamage);
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                t_customMono.AddMultipleCollideTimer(
                                    GetHashCode(),
                                    multipleCollideInterval
                                );
                                dealDamageGameEventData.damage = t_customMono.statusEffect.GetHit(
                                    secondCollisionDamage
                                );
                                dealDamageGameEventData.dealer = owner;
                                dealDamageGameEventData.target = t_customMono;
                                GameManager
                                    .Instance.selfEvents[dealDamageGameEventData.dealer][
                                        GameEventType.DealDamage
                                    ]
                                    .action(dealDamageGameEventData);
                                dealDamageEvent(secondCollisionDamage);
                            }
                        }
                    });
                }
            }
        }
    }

    public GameEffect GameEffect { get; set; }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;
    }

    public void Setup(CustomMono owner, float firstCollisionDamage, float secondCollisionDamage)
    {
        this.owner = owner;
        this.firstCollisionDamage = firstCollisionDamage;
        this.secondCollisionDamage = secondCollisionDamage;
    }
}
