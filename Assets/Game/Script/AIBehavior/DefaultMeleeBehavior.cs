using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMeleeBehavior : BaseAIBehavior
{
    public float logicalAttackRange = 1.5f; //
    public float targetTooCloseRange = 0.5f;

    public override void Awake()
    {
        base.Awake();

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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void DoFixedUpdate()
    {
        ThinkAndPrepare();
        base.DoAction();
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
            Think();
    }

    void Think()
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
}
