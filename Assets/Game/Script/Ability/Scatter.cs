using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Scatter : SkillBase
{
    ActionWaitInfo actionWaitInfo = new();
    GameEffect scatterChargeGameEffect;
    public static List<List<float>> arrowAnglesAtPhases;
    public long realInterval;
    SpriteRenderer scatterArrowPhaseIcon;
    public static Vector3 punch;
    public static float punchDuration = 0.5f,
        elasticity = 1;
    public int vibrato = 10;
    Tweener iconTweener;

    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        boolHash = Animator.StringToHash("Charge");
        audioClip = Resources.Load<AudioClip>("AudioClip/scatter-release");
        actionWaitInfo.releaseBoolHash = Animator.StringToHash("Release");
        damage = defaultDamage = 30f;
        /* In this skill ammo mean phase */
        currentAmmo = 0;
        maxAmmo = 3;
        realInterval = (long)(0.5 * 1000);

        arrowAnglesAtPhases ??= new()
        {
            new List<float> { 0f.DegToRad() },
            new List<float> { -30f.DegToRad(), 30f.DegToRad() },
            new List<float> { 0f.DegToRad(), -30f.DegToRad(), 30f.DegToRad() },
        };
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        scatterArrowPhaseIcon = transform
            .Find("ScatterArrowPhaseIcon")
            .GetComponent<SpriteRenderer>();
        punch = new(0.2f, 0.2f);
        punchDuration = 0.5f;
        elasticity = 0;
        vibrato = 10;

        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        actionWaitInfo.stillWaiting = false;
    }

    public override void Start()
    {
#if UNITY_EDITOR
        onExitPlayModeEvent += () => arrowAnglesAtPhases = null;
#endif
        base.Start();
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

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (actionWaitInfo.stillWaiting)
        {
            actionWaitInfo.stillWaiting = false;
            return successResult;
        }

        return failResult;
    }

    public override ActionResult StartAndWait()
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
        }

        return failResult;
    }

    IEnumerator WaitingCoroutine()
    {
        scatterChargeGameEffect = GameManager.Instance.scatterChargePool.PickOne().gameEffect;
        scatterChargeGameEffect.Follow(transform);

        stopwatch.Restart();
        currentAmmo = 0;
        while (actionWaitInfo.stillWaiting)
        {
            yield return new WaitForEndOfFrame();

            if (currentAmmo < maxAmmo)
            {
                if (stopwatch.ElapsedMilliseconds > realInterval)
                {
                    currentAmmo++;
                    HandleIcon();

                    stopwatch.Restart();
                }
            }
        }

        stopwatch.Stop();
        scatterChargeGameEffect.deactivate();
        StartCoroutine(CooldownCoroutine());
        scatterArrowPhaseIcon.gameObject.SetActive(false);
        iconTweener?.Kill();

        if (currentAmmo > 0)
        {
            ToggleAnim(actionWaitInfo.releaseBoolHash, true);
            ToggleAnim(boolHash, false);
            customMono.audioSource.PlayOneShot(audioClip);

            GameEffect t_scatterFlashEffect = GameManager
                .Instance.scatterFlashPool.PickOne()
                .gameEffect;
            t_scatterFlashEffect.transform.position = customMono.firePoint.transform.position;

            GameEffect t_scatterArrowGameEffect;
            CollideAndDamage t_collideAndDamage;
            arrowAnglesAtPhases[currentAmmo - 1]
                .ForEach(arrowAngle =>
                {
                    t_scatterArrowGameEffect = GameManager
                        .Instance.scatterArrowPool.PickOne()
                        .gameEffect;
                    t_collideAndDamage =
                        t_scatterArrowGameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
                        as CollideAndDamage;
                    t_collideAndDamage.allyTags = customMono.allyTags;
                    t_collideAndDamage.collideDamage = damage;

                    t_scatterArrowGameEffect.transform.SetPositionAndRotation(
                        customMono.firePoint.transform.position,
                        Quaternion.Euler(
                            0,
                            0,
                            Vector2.SignedAngle(
                                Vector2.right,
                                actionWaitInfo.finalDirection.RotateZ(arrowAngle)
                            )
                        )
                    );

                    t_scatterArrowGameEffect.KeepFlyingForward();
                });

            while (!customMono.animationEventFunctionCaller.endRelease)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.actionBlocking = false;
            ToggleAnim(actionWaitInfo.releaseBoolHash, false);
            customMono.animationEventFunctionCaller.endRelease = false;
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }
        else
        {
            customMono.actionBlocking = false;
            ToggleAnim(boolHash, false);
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }

        customMono.currentAction = null;
    }

    private void HandleIcon()
    {
        scatterArrowPhaseIcon.gameObject.SetActive(true);
        switch (currentAmmo)
        {
            case 1:
            {
                scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
                scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(0.33f);
                scatterArrowPhaseIcon
                    .transform.DOPunchScale(punch, punchDuration, vibrato, elasticity)
                    .SetEase(Ease.OutQuart);
                // .OnComplete(() => spriteRenderer.enabled = false);
                break;
            }
            case 2:
            {
                scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
                scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(0.66f);
                scatterArrowPhaseIcon
                    .transform.DOPunchScale(punch, punchDuration, vibrato * 2, elasticity)
                    .SetEase(Ease.OutQuart);
                // .OnComplete(() => spriteRenderer.enabled = false);
                break;
            }
            case 3:
            {
                scatterArrowPhaseIcon.transform.localScale = Vector3.one * 0.1f;
                scatterArrowPhaseIcon.color = ((Vector4)scatterArrowPhaseIcon.color).WithW(1);
                iconTweener = scatterArrowPhaseIcon
                    .transform.DOPunchScale(punch, punchDuration, vibrato * 3, elasticity)
                    .SetEase(Ease.OutQuart)
                    .SetLoops(-1);
                break;
            }

            default:
                break;
        }
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        actionWaitInfo.finalDirection = p_direction;
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
