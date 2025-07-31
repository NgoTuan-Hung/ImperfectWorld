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
/// Basically just a sensor manager for bot, so the bot can detect enemy, see how
/// far away is the enemy, and so on. Also it is an ai decision maker base on the
/// environment/situtation.
/// </summary>
public class MyBotPersonality : CustomMonoPal
{
    /// <summary>
    /// Origin is the position of a CustomMono
    /// </summary>
    public Vector2 originToTargetOriginDirection,
        /* Center is the position of RotationAndCenterObject of a CustomMono */
        centerToTargetCenterDirection,
        firePointToTargetCenterDirection;
    public Vector3 targetOriginPosition,
        targetCenterPosition;

    /// <summary>
    /// OTTOD = Origin To Target Origin Direction, CTTCD = Center To Target Center Direction,
    /// FPTTCD = Fire Point To Target Center Direction, ...
    /// </summary>
    public int current_OTTOD_ChangePriority = (int)ModificationPriority.VeryLow,
        current_CTTCD_ChangePriority = (int)ModificationPriority.VeryLow,
        current_FPTTCD_ChangePriority = (int)ModificationPriority.VeryLow,
        current_TOP_ChangePriority = (int)ModificationPriority.VeryLow,
        current_TCP_ChangePriority = (int)ModificationPriority.VeryLow;
    public float distanceToTarget;
    public float logicalAttackRange = 1f;
    public float targetTooCloseRange = 1f;
    public MyBotCombatBehaviour myBotCombatBehaviour;
    Action combatThinking;

    /* Target enemy is the enemy we are currently seeing and targeting, detect enemy is the unknown
    enemy we detected from the radar (GameManager) and not yet been seen. */
    CustomMono targetEnemy,
        detectEnemy;
    Action forceUsingAction = () => { };
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        if (myBotCombatBehaviour == MyBotCombatBehaviour.Melee)
            combatThinking = MeleeThinking;
        else
            combatThinking = RangedThinking;

        customMono.someOneExitView += (person) =>
        {
            if (targetEnemy != null)
            {
                if (targetEnemy.Equals(person))
                {
                    targetEnemy = null;
                    SetOriginToTargetOriginDirection(Vector2.one, ModificationPriority.VeryLow);
                    SetCenterToTargetCenterDirection(Vector2.one, ModificationPriority.VeryLow);
                    SetTargetOriginPosition(Vector3.zero, ModificationPriority.VeryLow);
                    SetTargetCenterPosition(Vector3.zero, ModificationPriority.VeryLow);
                }
            }
        };

        customMono.nearestEnemyChanged += (person) => targetEnemy = person;
    }

    private void OnEnable()
    {
        originToTargetOriginDirection = Vector2.zero;
        centerToTargetCenterDirection = Vector2.zero;
        targetOriginPosition = Vector3.zero;
        targetCenterPosition = Vector3.zero;
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

    void ResetField()
    {
        current_CTTCD_ChangePriority = (int)ModificationPriority.VeryLow;
        current_OTTOD_ChangePriority = (int)ModificationPriority.VeryLow;
        current_TCP_ChangePriority = (int)ModificationPriority.VeryLow;
        current_TOP_ChangePriority = (int)ModificationPriority.VeryLow;
    }

    public void SetOriginToTargetOriginDirection(
        Vector2 p_direction,
        ModificationPriority p_priority
    )
    {
        if ((int)p_priority <= current_OTTOD_ChangePriority)
        {
            originToTargetOriginDirection = p_direction == Vector2.zero ? Vector2.one : p_direction;
            current_OTTOD_ChangePriority = (int)p_priority;
        }
    }

    public void SetCenterToTargetCenterDirection(
        Vector2 p_direction,
        ModificationPriority p_priority
    )
    {
        if ((int)p_priority <= current_CTTCD_ChangePriority)
        {
            centerToTargetCenterDirection = p_direction == Vector2.zero ? Vector2.one : p_direction;
            current_CTTCD_ChangePriority = (int)p_priority;
        }
    }

    public void SetFirePointToTargetCenterDirection(
        Vector2 p_direction,
        ModificationPriority p_priority
    )
    {
        if ((int)p_priority <= current_FPTTCD_ChangePriority)
        {
            firePointToTargetCenterDirection =
                p_direction == Vector2.zero ? Vector2.one : p_direction;
            current_FPTTCD_ChangePriority = (int)p_priority;
        }
    }

    public void SetTargetOriginPosition(Vector3 p_position, ModificationPriority p_priority)
    {
        if ((int)p_priority <= current_TOP_ChangePriority)
        {
            targetOriginPosition = p_position;
            current_TOP_ChangePriority = (int)p_priority;
        }
    }

    public void SetTargetCenterPosition(Vector3 p_position, ModificationPriority p_priority)
    {
        if ((int)p_priority <= current_TCP_ChangePriority)
        {
            targetCenterPosition = p_position;
            current_TCP_ChangePriority = (int)p_priority;
        }
    }

    void ThinkAndPrepare()
    {
        ResetField();
        forceUsingAction();
        if (targetEnemy == null)
        {
            if (detectEnemy == null)
                detectEnemy = GameManager.Instance.GetRandomPlayerAlly();
            SetOriginToTargetOriginDirection(
                detectEnemy.transform.position - transform.position,
                ModificationPriority.VeryLow
            );
            SetCenterToTargetCenterDirection(
                detectEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryLow
            );
        }
        else
        {
            ThinkAboutNumbers();
        }

        if (targetEnemy == null)
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Roam, 1);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.Roam, 1);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 5);
        }
        else
        {
            combatThinking();
        }
    }

    void MeleeThinking()
    {
        if (distanceToTarget > logicalAttackRange)
        {
            customMono.movementIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
            customMono.movementIntelligence.PreSumActionChance(ActionUse.Dodge, 5);
            customMono.actionIntelligence.PreSumActionChance(ActionUse.GetCloser, 30);
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
            p_originToTargetOriginDirection: originToTargetOriginDirection,
            p_centerToTargetCenterDirection: centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: firePointToTargetCenterDirection,
            p_targetOriginPosition: targetOriginPosition,
            p_targetCenterPosition: targetCenterPosition
        );
        customMono.actionIntelligence.ExecuteAnyActionThisFrame(
            customMono.actionInterval,
            p_originToTargetOriginDirection: originToTargetOriginDirection,
            p_centerToTargetCenterDirection: centerToTargetCenterDirection,
            p_firePointToTargetCenterDirection: firePointToTargetCenterDirection,
            p_targetOriginPosition: targetOriginPosition,
            p_targetCenterPosition: targetCenterPosition
        );
    }

    void ThinkAboutNumbers()
    {
        /* IMPORTANT */
        SetOriginToTargetOriginDirection(
            targetEnemy.transform.position - transform.position,
            ModificationPriority.VeryLow
        );
        SetCenterToTargetCenterDirection(
            targetEnemy.rotationAndCenterObject.transform.position
                - customMono.rotationAndCenterObject.transform.position,
            ModificationPriority.VeryLow
        );
        SetFirePointToTargetCenterDirection(
            targetEnemy.rotationAndCenterObject.transform.position
                - customMono.firePoint.transform.position,
            ModificationPriority.VeryLow
        );
        SetTargetOriginPosition(targetEnemy.transform.position, ModificationPriority.VeryLow);
        SetTargetCenterPosition(
            targetEnemy.rotationAndCenterObject.transform.position,
            ModificationPriority.VeryLow
        );

        /* IMPORTANT */
        distanceToTarget = originToTargetOriginDirection.magnitude;
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

        if (distanceToTarget > targetTooCloseRange)
        {
            if (distanceToTarget < logicalAttackRange)
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
        Action t_forceUsingAction = () =>
        {
            SetOriginToTargetOriginDirection(
                targetEnemy.transform.position - transform.position,
                ModificationPriority.VeryHigh
            );
            SetCenterToTargetCenterDirection(
                targetEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            SetTargetOriginPosition(targetPositionParam, ModificationPriority.VeryHigh);
            SetTargetCenterPosition(
                targetEnemy.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryHigh
            );
            customMono.actionIntelligence.PreSumActionChance(actionUse, 9999);
        };

        forceUsingAction += t_forceUsingAction;
        StartCoroutine(ForceUsingActionCoroutine(t_forceUsingAction, duration));
    }

    IEnumerator ForceUsingActionCoroutine(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);

        forceUsingAction -= action;
    }
}
