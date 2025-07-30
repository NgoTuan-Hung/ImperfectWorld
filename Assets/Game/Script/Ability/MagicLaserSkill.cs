using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MagicLaserSkill : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 10f;
        damage = defaultDamage = 1f;
        boolHash = Animator.StringToHash("CastingMagic");
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Start()
    {
        base.Start();
        actionClip = customMono.AnimatorWrapper.GetAnimationClip("CastingMagic");
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
                        p_doActionParamInfo.targetCenterPosition,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
            )
        );
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = TriggerCoroutine(location, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator TriggerCoroutine(Vector2 location = default, Vector2 direction = default)
    {
        while (!customMono.animationEventFunctionCaller.castingMagic)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.castingMagic = false;
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

        while (!customMono.animationEventFunctionCaller.endCastingMagic)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.endCastingMagic = false;
        ToggleAnim(boolHash, false);
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
        ToggleAnim(boolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.castingMagic = false;
        customMono.animationEventFunctionCaller.endCastingMagic = false;
        customMono.currentAction = null;
    }
}
