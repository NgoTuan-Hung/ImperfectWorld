using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class OrbitalNemesis : SkillBase
{
    Vector2 enemyDirection;

    public override void Awake()
    {
        base.Awake();
        duration = 0.3f;
        cooldown = 0.5f;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 5f;
        modifiedAngle = 45f.DegToRad();
        /* In this skill, this will be the max distance allowed */
        maxRange = 5 * 5;
        /* In this skill, this will be the number of animation variantion for dash */
        maxAmmo = 3;
        /* In this skill, this will be the portion each variation hold in blend tree. */
        interval = 1f / (maxAmmo - 1);
        // dashSpeed *= Time.deltaTime;
        // boolhash = ...
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
                ActionUse.KeepDistance,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: Time.fixedDeltaTime)
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
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.movementActionBlocking = true;
            customMono.actionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            customMono.AnimatorWrapper.animator.SetTrigger(
                GameManager.Instance.mainSkill2TriggerHash
            );
            StartCoroutine(actionIE = StartDash());
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;
            return successResult;
        }

        return failResult;
    }

    float currentTime,
        dashSpeed = 100;

    IEnumerator StartDash()
    {
        if (customMono.botSensor.currentNearestEnemy == null)
        {
            int t_variation = Random.Range(0, maxAmmo);
            enemyDirection = GetRandomVariation(t_variation).normalized;
            SetBlend(GameManager.Instance.mainSkill2BlendHash, interval * t_variation);
        }
        else
        {
            enemyDirection =
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position
                - customMono.rotationAndCenterObject.transform.position;

            /* If we are too far from the enemy, dash closer */
            if (enemyDirection.sqrMagnitude > maxRange)
            {
                enemyDirection = enemyDirection.normalized;
                SetBlend(GameManager.Instance.mainSkill2BlendHash, 0);
            }
            else
            {
                enemyDirection = enemyDirection
                    .WithY(0)
                    .RotateZ(modifiedAngle * Random.Range(0, 2) == 0 ? 1 : -1)
                    .normalized;
                if (enemyDirection.y < 0)
                    SetBlend(GameManager.Instance.mainSkill2BlendHash, interval * 2);
                else
                    SetBlend(GameManager.Instance.mainSkill2BlendHash, interval);
            }
        }

        customMono.SetUpdateDirectionIndicator(
            enemyDirection,
            UpdateDirectionIndicatorPriority.Low
        );
        while (!customMono.animationEventFunctionCaller.mainSkill2Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill2Signal = false;

        GameEffect t_dashEffect = GameManager.Instance.orbitalNemesisDashPool.PickOne().gameEffect;
        t_dashEffect.PlaceAndLookAt(
            transform.position
                + Vector3.right.Mul(new Vector3(0.5f * (enemyDirection.x > 0 ? -1 : 1), 1, 1)),
            enemyDirection
        );
        yield return DashTo(enemyDirection);
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.movementActionBlocking = false;
        customMono.actionBlocking = false;
        customMono.currentAction = null;
    }

    Vector2 GetRandomVariation(int p_varitationIndex) =>
        p_varitationIndex switch
        {
            0 => Vector2.right.WithX(Random.Range(0, 2) == 0 ? 1 : -1),
            1 => Vector2.right.RotateZ(modifiedAngle)
                * new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, 1),
            2 => Vector2.right.RotateZ(-modifiedAngle)
                * new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, 1),
            _ => default,
        };

    IEnumerator DashTo(Vector3 p_direction)
    {
        stopwatch.Restart();

        p_direction = p_direction.normalized * dashSpeed * Time.fixedDeltaTime;
        while (stopwatch.Elapsed.TotalSeconds < duration)
        {
            transform.position +=
                (1 - EasingFunctions.OutQuint((float)stopwatch.Elapsed.TotalSeconds / duration))
                * p_direction;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotStartDash(p_direction, p_duration));
    }

    IEnumerator BotStartDash(Vector2 p_direction, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(direction: p_direction);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.movementActionBlocking = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
    }
}
