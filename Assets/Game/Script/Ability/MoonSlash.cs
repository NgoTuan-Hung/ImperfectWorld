using System.Collections;
using UnityEngine;

public class MoonSlash : SkillBase
{
    static ObjectPool moonSlashPool;
    GameObject moonSlashPrefab;
    int releaseBoolHash;
    ActionWaitInfo actionWaitInfo = new();

    public override void Awake()
    {
        base.Awake();
        cooldown = 5f;
        damage = defaultDamage = 30f;
        currentAmmo = 1;
        modifiedAngle = 30f.DegToRad();
        boolHash = Animator.StringToHash("Charge");
        audioClip = Resources.Load<AudioClip>("AudioClip/moon-slash");
        actionWaitInfo.releaseBoolHash = Animator.StringToHash("Release");
        actionWaitInfo.requiredWaitTime = 0.3f;

        moonSlashPrefab = Resources.Load("MoonSlash") as GameObject;
        moonSlashPool ??= new ObjectPool(
            moonSlashPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        AddActionManuals();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        actionWaitInfo.stillWaiting = false;
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () => moonSlashPool = null;
#endif
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
        botActionManuals.Add(
            new(
                ActionUse.RangedDamage,
                (p_doActionParamInfo) => Trigger(),
                new(nextActionChoosingIntervalProposal: 0.31f),
                actionNeedWait: true,
                startAndWait: StartAndWait,
                whileWaiting: WhileWaiting
            )
        );
    }

    public override void Trigger(Vector2 location = default, Vector2 direction = default)
    {
        actionWaitInfo.stillWaiting = false;
    }

    public override bool StartAndWait()
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.ActionMoveSpeedReduceRate);
            ToggleAnim(boolHash, true);
            actionWaitInfo.stillWaiting = true;
            StartCoroutine(actionIE = WaitingCoroutine());
            customMono.currentAction = this;
            return true;
        }

        return false;
    }

    IEnumerator WaitingCoroutine()
    {
        stopwatch.Restart();
        while (actionWaitInfo.stillWaiting)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        stopwatch.Stop();
        if (stopwatch.Elapsed.TotalSeconds >= actionWaitInfo.requiredWaitTime)
        {
            ToggleAnim(actionWaitInfo.releaseBoolHash, true);
            ToggleAnim(boolHash, false);
            customMono.audioSource.PlayOneShot(audioClip);
            StartCoroutine(CooldownCoroutine());

            Vector2 moonSlashDirection;
            for (int i = 0; i < currentAmmo; i++)
            {
                GameEffect t_moonSlashGameEffect = moonSlashPool.PickOne().gameEffect;
                var t_collideAndDamage = t_moonSlashGameEffect.GetBehaviour<CollideAndDamage>();
                t_collideAndDamage.allyTags = customMono.allyTags;
                t_collideAndDamage.collideDamage = damage;
                if (i % 2 == 1)
                {
                    moonSlashDirection = actionWaitInfo.finalDirection.RotateZ(
                        (i + 1) / 2 * modifiedAngle
                    );
                    t_moonSlashGameEffect.transform.SetPositionAndRotation(
                        customMono.firePoint.transform.position,
                        Quaternion.Euler(
                            0,
                            0,
                            Vector2.SignedAngle(Vector2.right, moonSlashDirection)
                        )
                    );
                }
                else
                {
                    moonSlashDirection = actionWaitInfo.finalDirection.RotateZ(
                        -i / 2 * modifiedAngle
                    );
                    t_moonSlashGameEffect.transform.SetPositionAndRotation(
                        customMono.firePoint.transform.position,
                        Quaternion.Euler(
                            0,
                            0,
                            Vector2.SignedAngle(Vector2.right, moonSlashDirection)
                        )
                    );
                }

                t_moonSlashGameEffect.KeepFlyingAt(moonSlashDirection);
            }

            while (!customMono.animationEventFunctionCaller.endRelease)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.actionBlocking = false;
            ToggleAnim(actionWaitInfo.releaseBoolHash, false);
            customMono.animationEventFunctionCaller.endRelease = false;
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }
        else
        {
            canUse = true;
            customMono.actionBlocking = false;
            ToggleAnim(boolHash, false);
            customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        }

        customMono.currentAction = null;
    }

    public override void WhileWaiting(Vector2 vector2)
    {
        customMono.SetUpdateDirectionIndicator(vector2, UpdateDirectionIndicatorPriority.Low);
        actionWaitInfo.finalDirection = vector2;
    }

    public override void DoAuto(DoActionParamInfo p_doActionParamInfo)
    {
        StartCoroutine(DoAutoCoroutine(p_doActionParamInfo.nextActionChoosingIntervalProposal));
    }

    IEnumerator DoAutoCoroutine(float p_duration)
    {
        customMono.actionInterval = true;

        yield return new WaitForSeconds(p_duration);
        customMono.actionInterval = false;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.statusEffect.RemoveSlow(customMono.stat.ActionMoveSpeedReduceRate);
        ToggleAnim(boolHash, false);
        ToggleAnim(actionWaitInfo.releaseBoolHash, false);
        StopCoroutine(actionIE);
        actionWaitInfo.stillWaiting = false;
        stopwatch.Stop();
        customMono.animationEventFunctionCaller.endRelease = false;
        customMono.currentAction = null;
    }
}
