using System.Collections;
using Kryz.Tweening;
using UnityEngine;

public class StormFangMerge : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        duration = 1f;
        cooldown = 5f;
        damage = defaultDamage = 2f;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
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
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
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
        if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            StartCoroutine(actionIE = StartSpinning(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    float currentTime,
        travelSpeed = 40;

    IEnumerator StartSpinning(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        // GameEffect vanishEffect = GameManager
        //     .Instance.gameEffectPool.PickOne()
        //     .gameEffect.Init(GameManager.Instance.vanishEffectSO);
        // vanishEffect.transform.position = transform.position;
        // StartCoroutine(actionIE1 = WaitSpawnSlashSignal(p_direction));

        GameManager
            .Instance.PickGameEffectAndInit(GameManager.Instance.stormFangMergeProgressSO)
            .transform.position = customMono.rotationAndCenterObject.transform.position;

        customMono.boxCollider2D.enabled = false;
        customMono.combatCollider2D.enabled = false;
        customMono.spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.5f);

        CollideAndDamage t_stormFangMergeBlades =
            GameManager
                .Instance.PickGameEffectAndInit(GameManager.Instance.stormFangMergeBladesSO)
                .GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        t_stormFangMergeBlades.transform.parent = customMono.rotationAndCenterObject.transform;
        t_stormFangMergeBlades.transform.localPosition = Vector3.zero;
        t_stormFangMergeBlades.allyTags = customMono.allyTags;
        t_stormFangMergeBlades.collideDamage = damage;

        p_direction = p_direction.normalized * travelSpeed * Time.fixedDeltaTime;
        currentTime = 0;
        while (currentTime < duration)
        {
            transform.position +=
                // (1 - EasingFunctions.InQuint(currentTime / duration)) * p_direction;
                EasingFunctions.InQuint(currentTime / duration) * p_direction;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }

        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.spriteRenderer.enabled = true;
        customMono.boxCollider2D.enabled = true;
        customMono.combatCollider2D.enabled = true;

        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.currentAction = null;
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
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.currentAction = null;
    }
}
