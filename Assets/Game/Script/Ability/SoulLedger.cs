using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulLedger : SkillBase
{
    Dictionary<CustomMono, float> ledger = new();
    AttackGameEventData attackGameEventData;
    Vector3 destination;

    public override void Awake()
    {
        base.Awake();
        botActionManual = new(BotDoAction, null);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        ledger.Clear();
    }

    public override void AddActionManuals()
    {
        base.AddActionManuals();
    }

    public override void Start()
    {
        base.Start();
        StatChangeRegister();
    }

    private void FixedUpdate()
    {
        foreach (var item in ledger)
        {
            if (item.Key.stat.currentHealthPoint.Value < item.Value)
            {
                Trigger(p_customMono: item.Key);
                break;
            }
        }
    }

    public override void Config()
    {
        GetActionField<ActionFloatField>(ActionFieldName.Range).value = GameManager
            .Instance
            .largePositiveNumber;
        successResult = new(
            true,
            ActionResultType.Cooldown,
            GetActionField<ActionFloatField>(ActionFieldName.Cooldown).value
        );
        GetActionField<ActionFloatField>(ActionFieldName.Speed).value = 4f * Time.fixedDeltaTime;
        GameManager.Instance.GetSelfEvent(customMono, GameEventType.Attack).action += CheckLedger;
        /* Also use damage, actionie */
    }

    void CheckLedger(IGameEventData iGameEventData)
    {
        attackGameEventData = iGameEventData.As<AttackGameEventData>();
        if (!ledger.ContainsKey(attackGameEventData.target))
        {
            ledger.Add(
                attackGameEventData.target,
                attackGameEventData.target.stat.healthPoint.FinalValue
                    * GetActionField<ActionFloatField>(ActionFieldName.Damage).value
            );
        }
        else
            ledger[attackGameEventData.target] +=
                attackGameEventData.target.stat.healthPoint.FinalValue
                * GetActionField<ActionFloatField>(ActionFieldName.Damage).value;
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
            0.05f + customMono.stat.reflex.FinalValue * 0.001f;
    }

    public override ActionResult Trigger(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        if (!customMono.actionBlocking && !customMono.movementActionBlocking)
        {
            customMono.actionBlocking = true;
            customMono.movementActionBlocking = true;
            customMono.statusEffect.Slow(customMono.stat.actionSlowModifier);
            ToggleAnim(GameManager.Instance.dashBoolHash, true);
            StartCoroutine(
                GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value = TriggerIE(
                    p_customMono: p_customMono
                )
            );
            customMono.currentAction = this;

            return successResult;
        }

        return failResult;
    }

    public IEnumerator TriggerIE(
        Vector2 location = default,
        Vector2 direction = default,
        CustomMono p_customMono = null
    )
    {
        destination = p_customMono.transform.position + 2 * VectorExtension.RandomXYNormalized();
        customMono.SetUpdateDirectionIndicator(
            destination - transform.position,
            UpdateDirectionIndicatorPriority.Low
        );

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                GetActionField<ActionFloatField>(ActionFieldName.Speed).value
            );
            yield return null;
        }

        customMono.SetUpdateDirectionIndicator(
            p_customMono.transform.position - transform.position,
            UpdateDirectionIndicatorPriority.Low
        );
        ToggleAnim(GameManager.Instance.dashBoolHash, false);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, true);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(
                EAnimationSignal.MainSkill1Signal
            )
        )
            yield return null;

        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);

        GameManager.Instance.ResolveDamage(customMono, p_customMono, ledger[p_customMono]);
        ledger.Remove(p_customMono);

        while (
            !customMono.animationEventFunctionCaller.GetSignalVals(EAnimationSignal.EndMainSkill1)
        )
            yield return null;

        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        customMono.currentAction = null;
    }

    public override void ActionInterrupt()
    {
        base.ActionInterrupt();
        customMono.actionBlocking = false;
        customMono.movementActionBlocking = false;
        ToggleAnim(GameManager.Instance.dashBoolHash, false);
        ToggleAnim(GameManager.Instance.mainSkill1BoolHash, false);
        StopCoroutine(GetActionField<ActionIEnumeratorField>(ActionFieldName.ActionIE).value);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.MainSkill1Signal, false);
        customMono.animationEventFunctionCaller.SetSignal(EAnimationSignal.EndMainSkill1, false);
        customMono.statusEffect.RemoveSlow(customMono.stat.actionSlowModifier);
    }
}
