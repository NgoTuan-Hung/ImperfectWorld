using System.Collections;
using System.Linq;
using UnityEngine;

public class SteelCyclone : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        maxAmmo = 16;
        cooldown = 1f;
        damage = defaultDamage = 5f;
        /* Get sprite list from res */
        successResult = new(true, ActionResultType.Cooldown, cooldown);
        modifiedAngle = (45f / 2).DegToRad();
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
                new(nextActionChoosingIntervalProposal: 0.45f)
            )
        );
    }

    public override void Start()
    {
        base.Start();
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            StartCoroutine(CooldownCoroutine());

            GameEffect t_kunai;
            CollideAndDamage t_collideAndDamage;
            for (int i = 0; i < maxAmmo; i++)
            {
                t_kunai = GameManager.Instance.kunaiPool.PickOne().gameEffect;

                t_collideAndDamage =
                    t_kunai.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
                t_collideAndDamage.allyTags = customMono.allyTags;
                t_collideAndDamage.collideDamage = damage;

                t_kunai.transform.position = customMono.rotationAndCenterObject.transform.position;
                t_kunai.KeepFlyingAt(
                    Vector2.right.RotateZ(i * modifiedAngle),
                    true,
                    EasingType.OutQuint
                );
            }

            customMono.actionBlocking = false;
            return successResult;
        }

        return failResult;
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
        customMono.currentAction = null;
    }
}
