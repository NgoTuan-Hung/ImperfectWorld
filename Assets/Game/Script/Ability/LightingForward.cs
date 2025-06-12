using System.Collections;
using UnityEngine;

public class LightingForward : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 20f;
        currentAmmo = maxAmmo = 5;
        maxRange = 2f;
        interval = 0.1f;
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
            new(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.originToTargetOriginDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.4f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override void WhileWaiting(Vector2 vector2)
    {
        base.WhileWaiting(vector2);
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnLighting(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator WaitSpawnLighting(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        StartCoroutine(SpawnLightingIE(p_direction));

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.currentAction = null;
    }

    IEnumerator SpawnLightingIE(Vector3 p_direction)
    {
        int currentSpawn = 0;
        Vector3 t_lightingOrigin = transform.position;
        while (currentSpawn < maxAmmo)
        {
            GameEffect t_lighting = GameManager
                .Instance.lightingForwardLightingPool.PickOne()
                .gameEffect;
            t_lighting.collideAndDamage.allyTags = customMono.allyTags;
            t_lighting.collideAndDamage.collideDamage = damage;
            t_lighting.transform.position =
                t_lightingOrigin + currentSpawn * maxRange * p_direction.normalized;

            yield return new WaitForSeconds(interval);
            currentSpawn++;
        }
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_direction, p_duration));
    }

    IEnumerator BotTriggerIE(Vector2 p_direction, float p_duration)
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
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.currentAction = null;
    }
}
