using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernalTideFanReceiver : MonoBehaviour, IGameEffectBehaviour
{
    Action<Collider2D> onTriggerEnter2D = (Collider2D other) => { };

    public GameEffect GameEffect { get; set; }

    public void Disable()
    {
        onTriggerEnter2D = DisabledOnTriggerEnter2D;
    }

    public void Enable(GameEffectSO p_gameEffectSO)
    {
        onTriggerEnter2D = OnTriggerEnter2DLogic;
    }

    public void Initialize(GameEffect gameEffect)
    {
        GameEffect = gameEffect;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter2D(other);
    }

    private void DisabledOnTriggerEnter2D(Collider2D other) { }

    GameObject collideWithGO;
    static readonly int spawnAmmount = 5;
    static readonly float spawnDistance = 1f;
    static readonly float spawnInterval = 0.1f;

    private void OnTriggerEnter2DLogic(Collider2D other)
    {
        if (GameManager.Instance.colliderOwner.TryGetValue(other.GetHashCode(), out collideWithGO))
        {
            if (collideWithGO.CompareTag("InfernalTideFan"))
            {
                StartCoroutine(SpawnFlameIE(other.transform.right));

                onTriggerEnter2D = DisabledOnTriggerEnter2D;
            }
        }
    }

    IEnumerator SpawnFlameIE(Vector2 p_direction)
    {
        CollideAndDamage t_flame;
        for (int i = 0; i < spawnAmmount; i++)
        {
            t_flame =
                GameManager
                    .Instance.gameEffectPool.PickOne()
                    .gameEffect.Init(GameManager.Instance.infernalTideFlameNoReceiverSO)
                    .GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
            t_flame.transform.position =
                transform.position + (i * spawnDistance * p_direction).AsVector3();
            t_flame.allyTags = (
                GameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage
            ).allyTags;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
