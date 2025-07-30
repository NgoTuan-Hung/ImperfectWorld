using System.Collections;
using UnityEngine;

public class NuclearBomb : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 200f;
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
            new(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.targetOriginPosition,
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

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        base.WhileWaiting(p_direction);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(GameManager.Instance.mainSkill3BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnExplosion(location));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnExplosion(Vector3 p_location)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        CollideAndDamage t_nuclearBomb =
            GameManager
                .Instance.nuclearBombExplosionPool.PickOne()
                .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        t_nuclearBomb.allyTags = customMono.allyTags;
        t_nuclearBomb.collideDamage = damage;
        t_nuclearBomb.transform.position = p_location;

        while (!customMono.animationEventFunctionCaller.endMainSkill3)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.currentAction = null;
    }

    void BotTrigger(Vector2 p_location, float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_location, p_duration));
    }

    IEnumerator BotTriggerIE(Vector2 p_location, float p_duration)
    {
        customMono.actionInterval = true;
        Trigger(location: p_location);
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        customMono.currentAction = null;
    }
}
