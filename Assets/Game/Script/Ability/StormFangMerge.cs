using System.Collections;
using System.Diagnostics;
using Kryz.Tweening;
using UnityEngine;

public class StormFangMerge : SkillBase
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
            new BotActionManual(
                ActionUse.GetCloser,
                (p_doActionParamInfo) =>
                    BotTrigger(
                        p_doActionParamInfo.centerToTargetCenterDirection,
                        p_doActionParamInfo.nextActionChoosingIntervalProposal
                    ),
                new(nextActionChoosingIntervalProposal: 0.5f)
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
        GetActionField<ActionFloatField>(ActionFieldName.Duration).value = 2f;
        GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value = 5f;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value = 30f;
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 0.8f;
        /* Also damage, currentTIme, actionie */
    }

    public override void StatChangeRegister()
    {
        base.StatChangeRegister();
        customMono.stat.reflex.finalValueChangeEvent += RecalculateStat;
    }

    public override void RecalculateStat()
    {
        base.RecalculateStat();
        GetActionField<ActionFloatField>(ActionFieldName.Damage).value =
            customMono.stat.reflex.FinalValue * 0.25f;
    }

    public override void WhileWaiting(Vector2 p_location = default, Vector2 p_direction = default)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);
    }

    public override ActionResult Trigger(Vector2 location = default, Vector2 direction = default)
    {
        if (
            customMono.stat.currentManaPoint.Value
            < GetActionField<ActionFloatField>(ActionFieldName.ManaCost).value
        )
            return failResult;
        else if (canUse && !customMono.movementActionBlocking && !customMono.actionBlocking)
        {
            canUse = false;
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            ToggleAnim(GameManager.Instance.mainSkill2BoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value =
                    StartSpinning(direction)
            );
            StartCoroutine(CooldownCoroutine());
            customMono.currentAction = this;
            customMono.stat.currentManaPoint.Value -= GetActionField<ActionFloatField>(
                ActionFieldName.ManaCost
            ).value;
            return successResult;
        }

        return failResult;
    }

    IEnumerator StartSpinning(Vector3 p_direction)
    {
        customMono.SetUpdateDirectionIndicator(p_direction, UpdateDirectionIndicatorPriority.Low);

        GameManager.Instance.stormFangMergeProgressPool.PickOne().gameEffect.transform.position =
            customMono.rotationAndCenterObject.transform.position;

        customMono.boxCollider2D.enabled = false;
        customMono.combatCollider2D.enabled = false;
        customMono.spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.5f);

        CollideAndDamage t_stormFangMergeBlades =
            GameManager
                .Instance.stormFangMergeBladesPool.PickOne()
                .gameEffect.GetBehaviour(EGameEffectBehaviour.CollideAndDamage) as CollideAndDamage;
        t_stormFangMergeBlades.transform.parent = customMono.rotationAndCenterObject.transform;
        t_stormFangMergeBlades.transform.localPosition = Vector3.zero;
        t_stormFangMergeBlades.allyTags = customMono.allyTags;
        t_stormFangMergeBlades.collideDamage = GetActionField<ActionFloatField>(
            ActionFieldName.Damage
        ).value;

        yield return Dash(
            p_direction,
            GetActionField<ActionFloatField>(ActionFieldName.Speed).value,
            GetActionField<ActionFloatField>(ActionFieldName.Duration).value,
            EasingFunctions.InQuint
        );

        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        customMono.spriteRenderer.enabled = true;
        customMono.boxCollider2D.enabled = true;
        customMono.combatCollider2D.enabled = true;

        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        customMono.currentAction = null;
    }

    void BotTrigger(Vector2 p_direction, float p_duration)
    {
        StartCoroutine(botIE = BotStartDash(p_direction, p_duration));
    }

    IEnumerator BotStartDash(Vector2 p_direction, float p_duration)
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
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill2BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
    }
}
