using System.Collections;
using UnityEngine;

public class WoodCry : SkillBase
{
    float healAmmount = 50f;

    public override void Awake()
    {
        base.Awake();
        cooldown = 10f;
        damage = defaultDamage = 10f;

        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.magickaChangeEvent.action += () =>
        {
            // magicka 0 -> 100
            damage = defaultDamage + customMono.stat.Magicka * 0.1f;
        };
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    FireAt(
                        p_doActionParamInfo.targetOriginPosition,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            StartCoroutine(actionIE = TriggerCoroutine(location, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator TriggerCoroutine(Vector2 location = default, Vector2 direction = default)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill2Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
        // bool t_animatorLocalScale = customMono.AnimatorWrapper.animator.transform.localScale.x > 0;

        GameEffect gameEffect = GameManager.Instance.woodCryArrowPool.PickOne().gameEffect;
        gameEffect.collideAndDamage.allyTags = customMono.allyTags;
        gameEffect.collideAndDamage.collideDamage = damage;
        gameEffect.collideAndDamage.healAmmount = healAmmount;
        gameEffect.transform.position = location;

        while (!customMono.animationEventFunctionCaller.endMainSkill2)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.animationEventFunctionCaller.endMainSkill2 = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.currentAction = null;
    }

    public void FireAt(Vector2 location, float duration)
    {
        StartCoroutine(FireAtCoroutine(location, duration));
    }

    IEnumerator FireAtCoroutine(Vector2 location, float duration)
    {
        customMono.actionInterval = true;
        Trigger(location: location);
        yield return new WaitForSeconds(duration);

        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill2Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill2 = false;
        customMono.currentAction = null;
    }
}
