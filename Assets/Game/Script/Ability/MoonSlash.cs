using System.Collections;
using UnityEngine;

public class MoonSlash : SkillBase
{
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

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        actionWaitInfo.stillWaiting = false;
        return successResult;
    }

    public override ActionResult StartAndWait()
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
            return successResult;
        }

        return failResult;
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
                GameEffect t_moonSlashGameEffect = GameManager
                    .Instance.moonSlashPool.PickOne()
                    .gameEffect;
                var t_collideAndDamage =
                    t_moonSlashGameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage)
                    as CollideAndDamage;
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

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
        actionWaitInfo.finalDirection = p_direction;
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
