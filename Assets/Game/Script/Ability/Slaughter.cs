using System.Collections;
using UnityEngine;

public class Slaughter : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        audioClip = Resources.Load<AudioClip>("AudioClip/slaughter");
        cooldown = defaultCooldown = 1f;
        maxAmmo = 10;
        manaCost = 5f;

        AddActionManuals();
    }

    IEnumerator RefillAmmo()
    {
        while (true)
        {
            if (currentAmmo < maxAmmo)
            {
                yield return new WaitForSeconds(cooldown);
                AddAmmo(1);
            }
            else
                yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public override void AddAmmo(int ammount)
    {
        base.AddAmmo(ammount);
        if (currentAmmo > 0)
            botActionManuals[0].actionChanceAjuster = 100;
        else
            botActionManuals[0].actionChanceAjuster = 0;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        currentAmmo = maxAmmo;
        StartCoroutine(RefillAmmo());
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        damage = customMono.stat.might.FinalValue;
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(ActionUse.RangedDamage, DoAuto, new(nextActionChoosingIntervalProposal: 0))
        );
    }

    /* The logic of this ability is we can fire projectile whenever we have ammo,
    if there are no ammo, we can't fire, if ammo isn't full, we will reload it
    automatically (RefillAmmo coroutine). */
    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (customMono.stat.currentManaPoint.Value < manaCost)
            return failResult;
        else if (canUse && !customMono.actionBlocking && currentAmmo > 0)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            customMono.audioSource.PlayOneShot(audioClip);
            AddAmmo(-1);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(actionIE = EndAnimWaitCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= manaCost;

            customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
            GameEffect t_projectileEffect = GameManager
                .Instance.slaughterProjectilePool.PickOne()
                .gameEffect;
            var t_collideAndDamage =
                t_projectileEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
                as CollideAndDamage;
            t_collideAndDamage.allyTags = customMono.allyTags;
            t_collideAndDamage.collideDamage = damage;
            t_projectileEffect.transform.position = customMono.firePoint.transform.position;
            t_projectileEffect.transform.rotation = Quaternion.Euler(
                0,
                0,
                Vector2.SignedAngle(Vector2.right, direction)
            );

            /* Place the projectile slightly above our current fire direction so:
            | (place it here instead)
            |
            |
            (fire point)---------------------------------> (fire direction)
            |
            |
            | (or place it here)*/
            t_projectileEffect.transform.position +=
                t_projectileEffect.transform.TransformDirection(Vector3.up).normalized
                * Random.Range(-0.3f, 0.3f);

            t_projectileEffect.KeepFlyingAt(direction);

            return successResult;
        }

        return failResult;
    }

    IEnumerator EndAnimWaitCoroutine()
    {
        while (!customMono.animationEventFunctionCaller.endMainSkill1)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        canUse = true;
        customMono.currentAction = null;
    }

    public override void DoAuto(DoActionParamInfo p_doActionParamInfo)
    {
        Trigger(default, p_doActionParamInfo.centerToTargetCenterDirection);
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        canUse = true;
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.endMainSkill1 = false;
        customMono.currentAction = null;
    }
}
