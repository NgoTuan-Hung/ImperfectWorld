using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A sensor system for bot, bot can see nearby enemies (field of view), select nearest one,
/// and so on.
/// </summary>
[DefaultExecutionOrder(-1)]
public class BotSensor : CustomMonoPal
{
    CustomMono onTriggerCM;
    float currentNearestEnemySqrDistance = Mathf.Infinity;
    public CustomMono currentNearestEnemy = null,
        /* detect enemy is the unknown
        enemy we detected from the radar (GameManager) and not yet been seen. */
        detectEnemy;
    public Action<CustomMono> nearestEnemyChanged = (person) => { };
    public Action<CustomMono> someOneExitView = (person) => { };
    public List<CustomMono> enemiesWeSee = new();

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
    public float distanceToNearestEnemy;

    public override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        originToTargetOriginDirection = Vector2.one;
        centerToTargetCenterDirection = Vector2.one;
        targetOriginPosition = Vector3.zero;
        targetCenterPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        ResetField();
        EnemySensing();
    }

    void EnemySensing()
    {
        if (currentNearestEnemy == null)
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
            /* IMPORTANT */
            SetOriginToTargetOriginDirection(
                currentNearestEnemy.transform.position - transform.position,
                ModificationPriority.VeryLow
            );
            SetCenterToTargetCenterDirection(
                currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryLow
            );
            SetFirePointToTargetCenterDirection(
                currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.firePoint.transform.position,
                ModificationPriority.VeryLow
            );
            SetTargetOriginPosition(
                currentNearestEnemy.transform.position,
                ModificationPriority.VeryLow
            );
            SetTargetCenterPosition(
                currentNearestEnemy.rotationAndCenterObject.transform.position,
                ModificationPriority.VeryLow
            );

            /* IMPORTANT */
            distanceToNearestEnemy = originToTargetOriginDirection.magnitude;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (
            (onTriggerCM = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject))
            != null
        )
        {
            /* if we see a new enemy, remember him. */
            if (!customMono.allyTags.Contains(onTriggerCM.tag))
                enemiesWeSee.Add(onTriggerCM);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (
            (onTriggerCM = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject))
            != null
        )
        {
            /* Erase enemy from see list.. */
            enemiesWeSee.Remove(onTriggerCM);
            someOneExitView(onTriggerCM);

            /* If the current nearest enemy is this one, erase it as well. */
            if (onTriggerCM.Equals(currentNearestEnemy))
            {
                currentNearestEnemy = null;
                currentNearestEnemySqrDistance = float.MaxValue;
                nearestEnemyChanged(null);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (
            (onTriggerCM = GameManager.Instance.GetCustomMono(other.transform.parent.gameObject))
            != null
        )
        {
            if (!customMono.allyTags.Contains(onTriggerCM.transform.tag))
            {
                /* We can compare squared distance instead of distance because it's faster yet
                still gives the same result. */
                float t_enemySqrDistance = (
                    transform.position - onTriggerCM.transform.position
                ).sqrMagnitude;

                /* if we see a closer enemy, put him as the nearest. Else, update the distance of
                the nearest enemy instead. */
                if (t_enemySqrDistance < currentNearestEnemySqrDistance)
                {
                    currentNearestEnemySqrDistance = t_enemySqrDistance;
                    currentNearestEnemy = onTriggerCM;
                    nearestEnemyChanged(currentNearestEnemy);
                }
                else if (onTriggerCM.Equals(currentNearestEnemy))
                    currentNearestEnemySqrDistance = t_enemySqrDistance;
            }
        }
    }

    /// <summary>
    /// Should be called somewhere else to guarantee the order.
    /// </summary>
    public void ResetField()
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
}
