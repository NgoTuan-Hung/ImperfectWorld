using System.Collections;
using UnityEngine;

public class Slaughter : SkillBase
{
    public override void Awake()
    {
        base.Awake();
        audioClip = Resources.Load<AudioClip>("AudioClip/slaughter");
        AddActionManuals();
    }

    IEnumerator RefillAmmo()
    {
        while (true)
        {
            if (
                GetActionField<ActionIntField>(ActionFieldName.UseCount).value
                < GetActionField<ActionIntField>(ActionFieldName.MaxUseCount).value
            )
            {
                yield return new WaitForSeconds(
                    GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
                );
                AddAmmo(1);
            }
            else
                yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void AddAmmo(int ammount)
    {
        GetActionField<ActionIntField>(ActionFieldName.UseCount).value += ammount;
        if (GetActionField<ActionIntField>(ActionFieldName.UseCount).value > 0)
            botActionManuals[0].actionChanceAjuster = 100;
        else
            botActionManuals[0].actionChanceAjuster = 0;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LateEnable());
    }

    IEnumerator LateEnable()
    {
        yield return null;
        GetActionField<ActionIntField>(ActionFieldName.UseCount).value =
            GetActionField<ActionIntField>(ActionFieldName.MaxUseCount).value;
        StartCoroutine(RefillAmmo());
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 1f;
        GetActionField<ActionIntField>(ActionFieldName.MaxUseCount).value = 10;
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 5f;
        /* Also use actionie, damage, gameeffect, usageCount */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.might.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value = customMono
            .stat
            .might
            .FinalValue;
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
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (
            canUse
            && !customMono.actionBlocking
            && GetActionField<ActionIntField>(ActionFieldName.UseCount).value > 0
        )
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            customMono.audioSource.PlayOneShot(audioClip);
            AddAmmo(-1);
            ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    EndAnimWaitCoroutine()
            );
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;

            customMono.SetUpdateDirectionIndicator(direction, UpdateDirectionIndicatorPriority.Low);
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value = GameManager
                .Instance.slaughterProjectilePool.PickOne()
                .gameEffect;
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.SetUpCollideAndDamage(
                    customMono.allyTags,
                    GetActionField<ActionFloatField>(ActionFieldName.Damage).value
                );
            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.transform.SetPositionAndRotation(
                    customMono.firePoint.transform.position,
                    Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction))
                );

            /* Place the projectile slightly above our current fire direction so:
            | (place it here instead)
            |
            |
            (fire point)---------------------------------> (fire direction)
            |
            |
            | (or place it here)*/
            GetActionField<ActionGameEffectField>(
                ActionFieldName.GameEffect
            ).value.transform.position +=
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                    .value.transform.TransformDirection(Vector3.up)
                    .normalized * Random.Range(-0.3f, 0.3f);

            GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                .value.KeepFlyingAt(direction);

            return successResult;
        }

        return failResult;
    }

    IEnumerator EndAnimWaitCoroutine()
    {
        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.actionBlocking = false;
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
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
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
    }
}
