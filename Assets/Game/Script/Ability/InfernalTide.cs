using System.Collections;
using UnityEngine;

public class InfernalTide : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 1f;
        currentAmmo = maxAmmo = 5;
        // maxRange = 2f;
        interval = 0.1f;
        successResult = new(true, true, cooldown);
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
                ActionUse.MeleeDamage,
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
            StartCoroutine(actionIE = WaitSpawnFlame(direction));
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitSpawnFlame(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        StartCoroutine(SpawnFlameIE(p_direction));

        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        while (!customMono.animationEventFunctionCaller.mainSkill3Signal)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.mainSkill3Signal = false;

        transform.position -= p_direction.normalized * 1.5f;
        CollideAndDamage t_fan =
            GameManager
                .Instance.gameEffectPool.PickOne()
                .gameEffect.Init(GameManager.Instance.infernalTideFanSO)
                .GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        t_fan.allyTags = customMono.allyTags;
        t_fan.collideDamage = damage * 10;
        t_fan.transform.SetPositionAndRotation(
            customMono.firePoint.transform.position,
            Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, p_direction))
        );

        while (!customMono.animationEventFunctionCaller.endMainSkill3)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.currentAction = null;
    }

    IEnumerator SpawnFlameIE(Vector3 p_direction)
    {
        customMono.rotationAndCenterObject.transform.rotation = Quaternion.Euler(
            customMono.rotationAndCenterObject.transform.rotation.eulerAngles.WithZ(
                Vector2.SignedAngle(Vector2.right, p_direction)
            )
        );

        currentAmmo = 0;
        while (currentAmmo < maxAmmo)
        {
            CollideAndDamage t_flame =
                GameManager
                    .Instance.gameEffectPool.PickOne()
                    .gameEffect.Init(GameManager.Instance.infernalTideFlameSO)
                    .GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
            t_flame.allyTags = customMono.allyTags;
            t_flame.collideDamage = damage;
            t_flame.transform.position =
                customMono.rotationAndCenterObject.transform.TransformPoint(
                    Vector3.right.WithY(
                        currentAmmo % 2 == 0 ? -currentAmmo / 2 : (currentAmmo + 1) / 2
                    )
                );

            yield return new WaitForSeconds(interval);
            currentAmmo++;
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
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(GameManager.Instance.mainSkill3BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.mainSkill3Signal = false;
        customMono.animationEventFunctionCaller.endMainSkill3 = false;
        customMono.currentAction = null;
    }
}
