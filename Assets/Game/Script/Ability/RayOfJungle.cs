using System.Collections;
using UnityEngine;

public class RayOfJungle : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 20f;
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
                        p_doActionParamInfo.centerToTargetCenterDirection,
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

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.stat.MoveSpeed = customMono.stat.actionMoveSpeedReduced;
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = TriggerIE(location, direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
        }
    }

    IEnumerator TriggerIE(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill1Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.rotationAndCenterObject.transform.localRotation = Quaternion.identity;
        GameEffect t_rayOfJungleBeam = GameManager
            .Instance.rayOfJungleBeamPool.PickOne()
            .gameEffect;
        t_rayOfJungleBeam.transform.parent = customMono.rotationAndCenterObject.transform;
        t_rayOfJungleBeam.transform.SetLocalPositionAndRotation(
            t_rayOfJungleBeam.effectLocalPosition,
            Quaternion.Euler(t_rayOfJungleBeam.effectLocalRotation)
        );
        t_rayOfJungleBeam.transform.localScale = Vector3.one;
        t_rayOfJungleBeam.collideAndDamage.allyTags = customMono.allyTags;
        t_rayOfJungleBeam.collideAndDamage.collideDamage = damage;
        customMono.rotationAndCenterObject.transform.localScale = new(
            customMono.directionModifier.transform.localScale.x > 0 ? 1 : -1,
            1,
            1
        );
        customMono.rotationAndCenterObject.transform.Rotate(
            Vector3.forward,
            Vector2.SignedAngle(
                customMono.rotationAndCenterObject.transform.localScale.WithY(0),
                p_direction
            )
        );

        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.actionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.stat.SetDefaultMoveSpeed();
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotTriggerIE(p_direction, p_duration));
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
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill1Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.stat.SetDefaultMoveSpeed();
    }
}
