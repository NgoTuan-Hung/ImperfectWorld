using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class DashSkill : SkillBase
{
    public int totalEffect = 10;
    public float dashSpeed = 2f;
    public float effectLifeTime = 0.5f;
    public float spawnEffectInterval;

    public override void Awake()
    {
        base.Awake();
        duration = 0.3f;
        cooldown = 8f;

        spawnEffectInterval = duration / totalEffect;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
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
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.movementActionBlocking)
        {
            canUse = false;
            customMono.movementActionBlocking = true;
            StartCoroutine(actionIE = Dashing(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    public IEnumerator Dashing(Vector3 direction)
    {
        GameEffect gameEffect;
        /* Dash explode vfx */
        gameEffect = GameManager.Instance.dashExplodePool.PickOne().gameEffect;
        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        gameEffect.transform.parent = customMono.rotationAndCenterObject.transform;
        gameEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        direction = direction.normalized;

        float currentTime = 0;
        for (int i = 0; i < totalEffect; i++)
        {
            gameEffect = GameManager.Instance.dashEffectPool.PickOne().gameEffect;
            gameEffect.animateObjects[0].spriteRenderer.sprite = customMono.spriteRenderer.sprite;
            gameEffect.animateObjects[0].transform.localScale = customMono
                .directionModifier
                .transform
                .localScale;
            gameEffect.transform.position = customMono.transform.position;

            transform.position +=
                (1 - EasingFunctions.OutQuint(currentTime / duration)) * dashSpeed * direction;

            yield return new WaitForSeconds(spawnEffectInterval);
            currentTime += spawnEffectInterval;
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
