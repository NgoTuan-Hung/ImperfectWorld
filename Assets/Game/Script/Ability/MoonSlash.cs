using System.Collections;
using UnityEngine;

public class MoonSlash : SkillBase
{
    ActionWaitInfo actionWaitInfo = new();

    public override void Awake()
    {
        base.Awake();
        boolHash = Animator.StringToHash("Charge");
        audioClip = Resources.Load<AudioClip>("AudioClip/moon-slash");
        actionWaitInfo.releaseBoolHash = Animator.StringToHash("Release");
        actionWaitInfo.requiredWaitTime = 0.3f;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        actionWaitInfo.stillWaiting = false;
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        base.Config();
        GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value = new();
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 3;
        GetActionField<ActionFloatField>(ActionFieldName.Angle).value = 30f.DegToRad();
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 13f;
        /* Also use stop watch, ge, damage, direction*/
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        damage = customMono.stat.might.FinalValue * 2.3f;
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
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(boolHash, true);
            actionWaitInfo.stillWaiting = true;
            StartCoroutine(actionIE = WaitingCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator WaitingCoroutine()
    {
        GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Restart();
        while (actionWaitInfo.stillWaiting)
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Stop();
        if (
            GetActionField<ActionStopWatchField>(
                ActionFieldName.StopWatch
            ).value.Elapsed.TotalSeconds >= actionWaitInfo.requiredWaitTime
        )
        {
            ToggleAnim(actionWaitInfo.releaseBoolHash, true);
            ToggleAnim(boolHash, false);
            customMono.audioSource.PlayOneShot(audioClip);
            StartCoroutine(CooldownCoroutine());

            for (
                int i = 0;
                i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value;
                i++
            )
            {
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
                    GameManager.Instance.moonSlashPool.PickOneGameEffect();
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                    .value.SetUpCollideAndDamage(
                        customMono.allyTags,
                        GetActionField<ActionFloatField>(ActionFieldName.Damage).value
                    );

                if (i % 2 == 1)
                {
                    GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                        actionWaitInfo.finalDirection.RotateZ(
                            (i + 1)
                                / 2
                                * GetActionField<ActionFloatField>(ActionFieldName.Angle).value
                        );
                    GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                        .value.transform.SetPositionAndRotation(
                            customMono.firePoint.transform.position,
                            Quaternion.Euler(
                                0,
                                0,
                                Vector2.SignedAngle(
                                    Vector2.right,
                                    GetActionField<ActionVector3Field>(
                                        ActionFieldName.Direction
                                    ).value
                                )
                            )
                        );
                }
                else
                {
                    GetActionField<ActionVector3Field>(ActionFieldName.Direction).value =
                        actionWaitInfo.finalDirection.RotateZ(
                            -i / 2 * GetActionField<ActionFloatField>(ActionFieldName.Angle).value
                        );
                    GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                        .value.transform.SetPositionAndRotation(
                            customMono.firePoint.transform.position,
                            Quaternion.Euler(
                                0,
                                0,
                                Vector2.SignedAngle(
                                    Vector2.right,
                                    GetActionField<ActionVector3Field>(
                                        ActionFieldName.Direction
                                    ).value
                                )
                            )
                        );
                }

                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                    .value.KeepFlyingAt(
                        GetActionField<ActionVector3Field>(ActionFieldName.Direction).value
                    );
            }

            while (!customMono.animationEventFunctionCaller.endRelease)
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            customMono.actionBlocking = false;
            ToggleAnim(actionWaitInfo.releaseBoolHash, false);
            customMono.animationEventFunctionCaller.endRelease = false;
            customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        }
        else
        {
            canUse = true;
            customMono.actionBlocking = false;
            ToggleAnim(boolHash, false);
            customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
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
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        ToggleAnim(boolHash, false);
        ToggleAnim(actionWaitInfo.releaseBoolHash, false);
        StopCoroutine(actionIE);
        actionWaitInfo.stillWaiting = false;
        GetActionField<ActionStopWatchField>(ActionFieldName.StopWatch).value.Stop();
        customMono.animationEventFunctionCaller.endRelease = false;
    }
}
