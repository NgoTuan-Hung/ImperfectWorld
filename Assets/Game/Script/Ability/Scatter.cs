using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Scatter : SkillBase
{
    GameObject scatterArrowPrefab;
    static ObjectPool scatterArrowPool;
    GameObject scatterChargePrefab;
    static ObjectPool scatterChargePool;
    ActionWaitInfo actionWaitInfo = new();
    GameEffect scatterChargeGameEffect;

    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        boolHash = Animator.StringToHash("Charge");
        audioClip = Resources.Load<AudioClip>("AudioClip/scatter-release");
        actionWaitInfo.releaseBoolHash = Animator.StringToHash("Release");
        damage = defaultDamage = 30f;
        currentAmmo = 3;
        modifiedAngle = 30f.DegToRad();

        scatterArrowPrefab = Resources.Load("ScatterArrow") as GameObject;
        scatterArrowPool ??= new ObjectPool(
            scatterArrowPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );

        scatterChargePrefab = Resources.Load("ScatterCharge") as GameObject;
        scatterChargePool ??= new ObjectPool(
            scatterChargePrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );

        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        actionWaitInfo.stillWaiting = false;
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () =>
        {
            scatterArrowPool = null;
            scatterChargePool = null;
        };
#endif
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.PushAway,
                (p_doActionParamInfo) => Trigger(),
                new(nextActionChoosingIntervalProposal: 1f),
                actionNeedWait: true,
                startAndWait: StartAndWait,
                whileWaiting: WhileWaiting
            )
        );
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        actionWaitInfo.stillWaiting = false;
    }

    public override bool StartAndWait()
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(boolHash, true);
            actionWaitInfo.stillWaiting = true;
            StartCoroutine(actionIE = WaitingCoroutine());
            customMono.currentAction = this;
            return true;
        }

        return false;
    }

    IEnumerator WaitingCoroutine()
    {
        scatterChargeGameEffect = scatterChargePool.PickOne().gameEffect;
        scatterChargeGameEffect.Follow(transform);
        stopwatch.Restart();

        while (actionWaitInfo.stillWaiting)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        stopwatch.Stop();
        scatterChargeGameEffect.deactivate();
        if (stopwatch.Elapsed.TotalSeconds >= actionWaitInfo.requiredWaitTime)
        {
            ToggleAnim(actionWaitInfo.releaseBoolHash, true);
            ToggleAnim(boolHash, false);
            customMono.audioSource.PlayOneShot(audioClip);
            StartCoroutine(CooldownCoroutine());

            Vector2 arrowDirection;
            for (int i = 0; i < currentAmmo; i++)
            {
                GameEffect scatterArrowGameEffect = scatterArrowPool.PickOne().gameEffect;
                scatterArrowGameEffect.collideAndDamage.allyTags = customMono.allyTags;
                scatterArrowGameEffect.collideAndDamage.collideDamage = damage;
                if (i % 2 == 1)
                {
                    arrowDirection = actionWaitInfo.finalDirection.RotateZ(
                        (i + 1) / 2 * modifiedAngle
                    );
                    scatterArrowGameEffect.transform.SetPositionAndRotation(
                        customMono.firePoint.transform.position,
                        Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, arrowDirection))
                    );
                }
                else
                {
                    arrowDirection = actionWaitInfo.finalDirection.RotateZ(-i / 2 * modifiedAngle);
                    scatterArrowGameEffect.transform.SetPositionAndRotation(
                        customMono.firePoint.transform.position,
                        Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, arrowDirection))
                    );
                }

                scatterArrowGameEffect.KeepFlyingAt(arrowDirection);
            }

            while (!customMono.animationEventFunctionCaller.endRelease)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.actionBlocking = false;
            ToggleAnim(actionWaitInfo.releaseBoolHash, false);
            customMono.animationEventFunctionCaller.endRelease = false;
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }
        else
        {
            canUse = true;
            customMono.actionBlocking = false;
            ToggleAnim(boolHash, false);
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }

        customMono.currentAction = null;
    }

    public override void WhileWaiting(Vector2 vector2)
    {
        customMono.SetUpdateDirectionIndicator(vector2, UpdateDirectionIndicatorPriority.Low);
        actionWaitInfo.finalDirection = vector2;
    }

    public void ScatterTo(Vector2 direction, float duration)
    {
        StartCoroutine(ScatterToCoroutine(duration));
    }

    IEnumerator ScatterToCoroutine(float p_duration)
    {
        customMono.actionInterval = true;

        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(boolHash, false);
        ToggleAnim(actionWaitInfo.releaseBoolHash, false);
        StopCoroutine(actionIE);
        actionWaitInfo.stillWaiting = false;
        stopwatch.Stop();
        /* In case this is used somewhere we don't know*/
        if (scatterChargeGameEffect.gameObject.activeSelf)
            scatterChargeGameEffect.deactivate();
        customMono.animationEventFunctionCaller.endRelease = false;
        customMono.currentAction = null;
    }
}
