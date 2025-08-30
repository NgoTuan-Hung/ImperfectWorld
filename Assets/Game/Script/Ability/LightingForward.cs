using System.Collections;
using UnityEngine;

public class LightingForward : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        currentAmmo = maxAmmo = 5;
        maxRange = 2f;
        interval = 0.1f;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 20f;
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
        StatChangeRegister();
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.wisdom.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        damage = customMono.stat.wisdom.FinalValue * 2.75f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        base.WhileWaiting(p_direction);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnLighting(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnLighting(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill1AS.signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;
        StartCoroutine(SpawnLightingIE(p_direction));

        while (!customMono.animationEventFunctionCaller.mainSkill1AS.end)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.currentAction = null;
    }

    IEnumerator SpawnLightingIE(Vector3 p_direction)
    {
        int currentSpawn = 0;
        Vector3 t_lightingOrigin = transform.position;
        while (currentSpawn < maxAmmo)
        {
            CollideAndDamage t_lighting =
                GameManager
                    .Instance.lightingForwardLightingPool.PickOne()
                    .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
                as CollideAndDamage;
            t_lighting.allyTags = customMono.allyTags;
            t_lighting.collideDamage = damage;
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
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1AS.signal = false;
        customMono.animationEventFunctionCaller.mainSkill1AS.end = false;
    }
}
