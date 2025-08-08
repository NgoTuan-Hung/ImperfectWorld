using System.Collections;
using UnityEngine;

public class DoubleKill : SkillBase
{
    PoisonInfo poisonInfo;
    SlowInfo slowInfo;

    public override void Awake()
    {
        base.Awake();
        cooldown = 1f;
        poisonInfo = new(5, 0);
        slowInfo = new(new(-0.3f, FloatStatModifierType.Multiplicative), 1f);
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 5f;
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
                        p_doActionParamInfo.firePointToTargetCenterDirection,
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
        damage = customMono.stat.wisdom.FinalValue * 1.1f;
        poisonInfo.poisonDamage = customMono.stat.wisdom.FinalValue * 0.25f;
        slowInfo.totalSlow.value = -(customMono.stat.wisdom.FinalValue * 0.01f + 0.3f);
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
            ToggleAnim(GameManager.Instance.attackBoolHash, true);
            StartCoroutine(actionIE = WaitSpawnArrow(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnArrow(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.attack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.attack = false;
        GameEffect t_arrow;
        CollideAndDamage t_collideAndDamage;
        if (Random.Range(0, 2) == 0)
        {
            t_arrow = GameManager.Instance.elementalLeafRangerPoisonArrowPool.PickOne().gameEffect;
            t_collideAndDamage = (CollideAndDamage)
                t_arrow.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
            t_collideAndDamage.poisonInfo = poisonInfo;
        }
        else
        {
            t_arrow = GameManager.Instance.elementalLeafRangerVineArrowPool.PickOne().gameEffect;
            t_collideAndDamage = (CollideAndDamage)
                t_arrow.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
            t_collideAndDamage.slowInfo = slowInfo;
        }
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = damage;
        t_arrow.transform.position = customMono.firePoint.transform.position;
        t_arrow.transform.rotation = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(Vector2.right, p_direction)
        );
        t_arrow.KeepFlyingAt(p_direction);

        while (!customMono.animationEventFunctionCaller.endAttack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endAttack = false;
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.currentAction = null;
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
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.attack = false;
        customMono.animationEventFunctionCaller.endAttack = false;
        customMono.currentAction = null;
    }
}
