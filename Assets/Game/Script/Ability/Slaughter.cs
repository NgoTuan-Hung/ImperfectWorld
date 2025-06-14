using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Slaughter : SkillBase
{
    GameObject projectilePrefab;
    static ObjectPool projectilePool;

    public override void Awake()
    {
        base.Awake();
        boolHash = Animator.StringToHash("Slaughter");
        audioClip = Resources.Load<AudioClip>("AudioClip/slaughter");
        cooldown = defaultCooldown = 1f;
        damage = 5f;
        maxAmmo = 10;

        projectilePrefab = Resources.Load("SlaughterProjectile") as GameObject;
        projectilePool ??= new ObjectPool(
            projectilePrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );

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

#if UNITY_EDITOR
        onExitPlayModeEvent += () =>
        {
            projectilePool = null;
        };
#endif
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
    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking && currentAmmo > 0)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            customMono.audioSource.PlayOneShot(audioClip);
            AddAmmo(-1);
            ToggleAnim(boolHash, true);
            StartCoroutine(actionIE = EndAnimWaitCoroutine());
            customMono.currentAction = this;

            customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
            GameEffect t_projectileEffect = projectilePool.PickOne().gameEffect;
            var t_collideAndDamage = t_projectileEffect.GetBehaviour<CollideAndDamage>();
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
        }
    }

    IEnumerator EndAnimWaitCoroutine()
    {
        while (!customMono.animationEventFunctionCaller.endSlaughter)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        ToggleAnim(boolHash, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.endSlaughter = false;
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
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(boolHash, false);
        StopCoroutine(actionIE);
        customMono.animationEventFunctionCaller.endSlaughter = false;
        customMono.currentAction = null;
    }
}
