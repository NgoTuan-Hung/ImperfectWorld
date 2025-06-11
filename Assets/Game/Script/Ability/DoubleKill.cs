using System.Collections;
using UnityEngine;

public class DoubleKill : SkillBase
{
    PoisonInfo poisonInfo;

    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 10f;
        poisonInfo = new(5, 10);
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
            customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
            ToggleAnim(GameManager.Instance.attackBoolHash, true);
            StartCoroutine(actionIE = WaitSpawnArrow(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator WaitSpawnArrow(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.attack)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.attack = false;
        GameEffect t_arrow;
        if (Random.Range(0, 2) == 0)
        {
            t_arrow = GameManager.Instance.elementalLeafRangerPoisonArrowPool.PickOne().gameEffect;
        }
        else
        {
            t_arrow = GameManager.Instance.elementalLeafRangerPoisonArrowPool.PickOne().gameEffect;
        }
        t_arrow.collideAndDamage.allyTags = customMono.allyTags;
        t_arrow.collideAndDamage.collideDamage = damage;
        t_arrow.collideAndDamage.poisonInfo = poisonInfo;
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
        customMono.stat.SetDefaultMoveSpeed();
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
        customMono.stat.SetDefaultMoveSpeed();
        ToggleAnim(GameManager.Instance.attackBoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.attack = false;
        customMono.animationEventFunctionCaller.endAttack = false;
    }
}
