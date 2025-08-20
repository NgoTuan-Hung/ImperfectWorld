using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyBotCombatBehaviour
{
    Melee,
    Ranged,
}

public enum ModificationPriority
{
    VeryLow = 4,
    Low = 3,
    Medium = 2,
    High = 1,
    VeryHigh = 0,
}

/// <summary>
/// Bot brain, execute actions based on various factors like sensors, conditions,...
/// Should run after BotSensor.cs .
/// </summary>
[DefaultExecutionOrder(0)]
public class MyBotPersonality : CustomMonoPal
{
    public float logicalAttackRange = 1f;
    public float targetTooCloseRange = 1f;
    public MyBotCombatBehaviour myBotCombatBehaviour;
    Action combatThinking;

    Action forceUsingAction = () => { };
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        if (myBotCombatBehaviour == MyBotCombatBehaviour.Melee)
            combatThinking = MeleeThinking;
        else
            combatThinking = RangedThinking;

        // customMono.botSensor.someOneExitView += (person) =>
        // {
        //     if (targetEnemy != null)
        //     {
        //         if (targetEnemy.Equals(person))
        //         {
        //             targetEnemy = null;
        //             customMono.botSensor.SetOriginToTargetOriginDirection(
        //                 Vector2.one,
        //                 ModificationPriority.VeryLow
        //             );
        //             customMono.botSensor.SetCenterToTargetCenterDirection(
        //                 Vector2.one,
        //                 ModificationPriority.VeryLow
        //             );
        //             customMono.botSensor.SetTargetOriginPosition(
        //                 Vector3.zero,
        //                 ModificationPriority.VeryLow
        //             );
        //             customMono.botSensor.SetTargetCenterPosition(
        //                 Vector3.zero,
        //                 ModificationPriority.VeryLow
        //             );
        //         }
        //     }
        // };
    }

    public override void Start()
    {
        base.Start();
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;
        pausableScript.resumeFixedUpdate = () => pausableScript.fixedUpdate = DoFixedUpdate;
        pausableScript.pauseFixedUpdate = () => pausableScript.fixedUpdate = () => { };
        pausableScript.resumeFixedUpdate();
    }

    private void FixedUpdate()
    {
        pausableScript.fixedUpdate();
    }

    void DoFixedUpdate()
    {
        ThinkAndPrepare();
        DoAction();
    }

    void ThinkAndPrepare()
    {
        /* Here you can add maximum chance for any action you wish to execute,
        however, remember to reset delegate afterward.*/
        forceUsingAction();

        /* If there is no nearest enemy, move to the enemy detected on radar,
        or roam instead. */
        if (customMono.botSensor.currentNearestEnemy == null)
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Roam, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 50000);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 50000);
        }
        else
        {
            combatThinking();
        }
    }

    void MeleeThinking()
    {
        if (customMono.botSensor.distanceToNearestEnemy > logicalAttackRange)
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 50000);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
        }
        else
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.MeleeDamage, 30);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.SummonShortRange, 15);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 5);
        }

        // if (customMono.attackable.onCooldown)
        // {
        // 	customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // 	customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // }

        customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 5);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 5);
    }

    void DoAction()
    {
        /* IMPORTANT */
        customMono.movementIntelligence.ExecuteAnyActionThisFrame(
            customMono.movementActionInterval,
            p_originToTargetOriginDirection: customMono.botSensor.originToTargetOriginDirection,
            p_centerToTargetCenterDirection: customMono.botSensor.centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: customMono
                .botSensor
                .firePointToTargetCenterDirection,
            p_targetOriginPosition: customMono.botSensor.targetOriginPosition,
            p_targetCenterPosition: customMono.botSensor.targetCenterPosition
        );
        customMono.actionIntelligence.ExecuteAnyActionThisFrame(
            customMono.actionInterval,
            p_originToTargetOriginDirection: customMono.botSensor.originToTargetOriginDirection,
            p_centerToTargetCenterDirection: customMono.botSensor.centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: customMono
                .botSensor
                .firePointToTargetCenterDirection,
            p_targetOriginPosition: customMono.botSensor.targetOriginPosition,
            p_targetCenterPosition: customMono.botSensor.targetCenterPosition
        );
    }

    void RangedThinking()
    {
        customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
        customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
        customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 1);

        // if (customMono.attackable.onCooldown)
        // {
        // 	customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
        // 	customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // 	customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 3);
        // 	customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 15);
        // }

        if (customMono.botSensor.distanceToNearestEnemy > targetTooCloseRange)
        {
            if (customMono.botSensor.distanceToNearestEnemy < logicalAttackRange)
            {
                customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
                customMono.movementIntelligence.PreSumActionChance(ActionUse.Passive, 1);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.Passive, 1);
            }
            else
            {
                customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
                customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
            }
        }
        else
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Dodge, 6);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetAway, 10);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.RangedDamage, 30);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.PushAway, 10);
        }
    }

    public void ForceUsingAction(ActionUse actionUse, Vector3 targetPositionParam, float duration)
    {
        void t_forceUsingAction()
        {
            customMono.botSensor.SetOriginToTargetOriginDirection(
                customMono.botSensor.currentNearestEnemy.transform.position - transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetCenterToTargetCenterDirection(
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetTargetOriginPosition(
                targetPositionParam,
                ModificationPriority.VeryHigh
            );
            customMono.botSensor.SetTargetCenterPosition(
                customMono.botSensor.currentNearestEnemy.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.actionIntelligence.PreSumActionChance(actionUse, 9999);
        }

        forceUsingAction += t_forceUsingAction;
        StartCoroutine(ForceUsingActionCoroutine(t_forceUsingAction, duration));
    }

    IEnumerator ForceUsingActionCoroutine(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);

        forceUsingAction -= action;
    }
}
