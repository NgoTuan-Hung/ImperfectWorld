using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : SkillBase
{
    static ObjectPool dashEffectPool;
    GameObject dashEffectPrefab;
    public int totalEffect = 10;
    public float dashAmmountPerFrame = 0.5f;
    public float effectLifeTime = 0.5f;
    public float spawnEffectInterval;

    public override void Awake()
    {
        base.Awake();
        duration = 1f;
        cooldown = 8f;

        dashEffectPrefab = Resources.Load("DashEffect") as GameObject;
        dashEffectPool ??= new ObjectPool(
            dashEffectPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        spawnEffectInterval = duration / totalEffect;
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
                (p_doActionParamInfo) =>
                    DashTo(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.GetAway,
                (p_doActionParamInfo) =>
                    DashTo(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(
                    nextActionChoosingIntervalProposal: 0.5f,
                    isDirectionModify: true,
                    directionModifier: -1
                )
            )
        );
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.Dodge,
                (p_doActionParamInfo) =>
                    DashTo(
                        new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () => dashEffectPool = null;
#endif
    }

    public override void WhileWaiting(Vector2 vector2)
    {
        customMono.SetUpdateDirectionIndicator(vector2, UpdateDirectionIndicatorPriority.Low);
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.movementActionBlocking)
        {
            canUse = false;
            customMono.movementActionBlocking = true;
            List<PoolObject> poolObjects = dashEffectPool.PickAndPlace(
                totalEffect,
                effectActiveLocation
            );
            StartCoroutine(actionIE = Dashing(poolObjects, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    public IEnumerator Dashing(List<PoolObject> poolObjects, Vector3 direction)
    {
        GameEffect gameEffect;
        direction = direction.normalized * dashAmmountPerFrame;
        for (int i = 0; i < poolObjects.Count; i++)
        {
            gameEffect = poolObjects[i].gameEffect;
            gameEffect.spriteRenderer.sprite = customMono.spriteRenderer.sprite;
            gameEffect.spriteRenderer.transform.localScale = customMono
                .directionModifier
                .transform
                .localScale;
            gameEffect.transform.position = customMono.transform.position;

            transform.position += direction;

            yield return new WaitForSeconds(spawnEffectInterval);
        }

        customMono.movementActionBlocking = false;
        customMono.currentAction = null;
    }

    public void DashTo(Vector2 direction, float duration)
    {
        StartCoroutine(DashToCoroutine(direction, duration));
    }

    IEnumerator DashToCoroutine(Vector2 direction, float duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: direction);
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.movementActionBlocking = true;
        StopCoroutine(actionIE);
        customMono.currentAction = null;
    }
}
