using System.Collections;
using System.Linq;
using UnityEngine;

public class SteelCyclone : SkillBase
{
    public override void Awake()
    {
        base.Awake();
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
        StatChangeRegister();
    }

    public override void Config()
    {
        GetActionField<ActionIntField>(ActionFieldName.EffectCount).value = 16;
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 1f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GetActionField<ActionFloatField>(ActionFieldName.Angle).value = (45f / 2).DegToRad();
        /* Also use damage, gameeffect */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.wisdom.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value = customMono
            .stat
            .wisdom
            .FinalValue;
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (canUse && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            StartCoroutine(CooldownCoroutine());

            for (
                int i = 0;
                i < GetActionField<ActionIntField>(ActionFieldName.EffectCount).value;
                i++
            )
            {
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect).value =
                    GameManager.Instance.kunaiPool.PickOne().gameEffect;
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                    .value.SetUpCollideAndDamage(
                        customMono.allyTags,
                        GetActionField<ActionFloatField>(ActionFieldName.Damage).value
                    );

                GetActionField<ActionGameEffectField>(
                    ActionFieldName.GameEffect
                ).value.transform.position = customMono.rotationAndCenterObject.transform.position;
                GetActionField<ActionGameEffectField>(ActionFieldName.GameEffect)
                    .value.KeepFlyingAt(
                        Vector2.right.RotateZ(
                            i * GetActionField<ActionFloatField>(ActionFieldName.Angle).value
                        ),
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
    }
}
