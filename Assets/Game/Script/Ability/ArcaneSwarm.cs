using System.Collections;
using UnityEngine;

public class ArcaneSwarm : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        cooldown = 0.5f;
        /* In this skill, this will be the number of projectile variant we select - 1*/
        maxAmmo = 6;
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
                    BotTrigger(p_doActionParamInfo.nextActionChoosingIntervalProposal),
                new(nextActionChoosingIntervalProposal: Time.fixedDeltaTime)
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
        damage = customMono.stat.wisdom.FinalValue * 3f;
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse)
        {
            canUse = false;
            FireHoming();
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;

            return successResult;
        }

        return failResult;
    }

    void FireHoming()
    {
        GameEffect t_projectile;
        CollideAndDamage t_collideAndDamage;

        t_projectile = ChooseProjectile(Random.Range(1, maxAmmo)).PickOne().gameEffect;
        t_collideAndDamage = (CollideAndDamage)
            t_projectile.GetBehaviour(EGameEffectBehaviour.CollideAndDamage);
        t_collideAndDamage.allyTags = customMono.allyTags;
        t_collideAndDamage.collideDamage = damage;

        if (customMono.botSensor.currentNearestEnemy == null)
        {
            t_collideAndDamage.transform.position =
                customMono.transform.position
                + new Vector3(
                    Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.x,
                    Random.Range(-0.5f, .5f) * customMono.fieldOfView.transform.localScale.y,
                    0
                );
        }
        else
            t_collideAndDamage.transform.position = customMono
                .botSensor
                .currentNearestEnemy
                .rotationAndCenterObject
                .transform
                .position;

        customMono.currentAction = null;
    }

    ObjectPool ChooseProjectile(int p_projectileID) =>
        p_projectileID switch
        {
            1 => GameManager.Instance.arcaneSwarmSlash1Pool,
            2 => GameManager.Instance.arcaneSwarmSlash2Pool,
            3 => GameManager.Instance.arcaneSwarmSlash3Pool,
            4 => GameManager.Instance.arcaneSwarmSlash4Pool,
            5 => GameManager.Instance.arcaneSwarmSlash5Pool,
            _ => null,
        };

    void BotTrigger(float p_duration)
    {
        StartCoroutine(BotTriggerIE(p_duration));
    }

    IEnumerator BotTriggerIE(float p_duration)
    {
        customMono.actionInterval = true;
        Trigger();
        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
    }
}
