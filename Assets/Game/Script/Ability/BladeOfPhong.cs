using System.Collections;
using UnityEngine;

public class BladeOfPhong : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 1f;
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
                (direction, location, nextActionChoosingIntervalProposal) =>
                    BotTrigger(direction, nextActionChoosingIntervalProposal),
                0.4f
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
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            StartCoroutine(actionIE = WaitSpawnTornadoSignal(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator WaitSpawnTornadoSignal(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill2Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
        GameEffect t_tornado = GameManager
            .Instance.bladeOfPhongTornadoEffectPool.PickOne()
            .gameEffect;
        t_tornado.collideAndDamage.allyTags = customMono.allyTags;
        t_tornado.collideAndDamage.collideDamage = damage;
        t_tornado.transform.position = transform.position;
        t_tornado.KeepFlyingAt(p_direction);

        while (!customMono.animationEventFunctionCaller.endMainSkill2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill2 = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
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
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill2 = false;
    }
}
