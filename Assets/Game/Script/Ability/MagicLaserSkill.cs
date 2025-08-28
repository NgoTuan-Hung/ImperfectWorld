using System.Collections;
using UnityEngine;

public class MagicLaserSkill : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 10f;
        damage = defaultDamage = 1f;
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        manaCost = 20f;
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
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new BotActionManual(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) =>
                    FireAt(
                        p_doActionParamInfo.targetCenterPosition,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = TriggerCoroutine(location, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerCoroutine(Vector2 location = default, Vector2 direction = default)
    {
        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        bool t_animatorLocalScale = customMono.AnimatorWrapper.animator.transform.localScale.x > 0;

        CollideAndDamage gameEffect =
            GameManager
                .Instance.magicLaserPool.PickOne()
                .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        gameEffect.allyTags = customMono.allyTags;
        gameEffect.collideDamage = damage;

        if (t_animatorLocalScale)
            gameEffect.transform.SetPositionAndRotation(
                location - new Vector2(6, 0),
                Quaternion.Euler(0, 0, 0)
            );
        else
            gameEffect.transform.SetPositionAndRotation(
                location + new Vector2(6, 0),
                Quaternion.Euler(0, 180, 0)
            );

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
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
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
    }
}
