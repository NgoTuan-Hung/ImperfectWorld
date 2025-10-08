using System;
using System.Collections.Generic;
using UnityEngine;

public enum ModificationPriority
{
    VeryLow = 4,
    Low = 3,
    Medium = 2,
    High = 1,
    VeryHigh = 0,
}

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
        firePointToTargetCenterDirection,
        targetPathFindingDirection;
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

    /// <summary>
    /// A centralized storage for sensor data
    /// </summary>
    DoActionParamInfo doActionParamInfo = new();
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        pausableScript.pauseFixedUpdate = () =>
        {
            pausableScript.fixedUpdate = EmptyFixedUpdate;
        };
        pausableScript.resumeFixedUpdate = () =>
        {
            pausableScript.fixedUpdate = DoFixedUpdate;
        };
    }

    void SetTargetToCenterMap()
    {
        SetOriginToTargetOriginDirection(
            Vector3.zero - transform.position,
            ModificationPriority.VeryLow
        );
        SetCenterToTargetCenterDirection(
            Vector3.zero - customMono.rotationAndCenterObject.transform.position,
            ModificationPriority.VeryLow
        );
        targetPathFindingDirection = GameManager.Instance.GetPathFindingDirectionToTarget(
            transform.position,
            Vector2.zero
        );
    }

    void SetTargetToDetectEnemy()
    {
        if (detectEnemy == null)
        {
            detectEnemy = GameManager.Instance.GetRandomEnemy(customMono.tag);
            if (detectEnemy == default)
            {
                SetTargetToCenterMap();
                return;
            }
        }

        SetOriginToTargetOriginDirection(
            detectEnemy.transform.position - transform.position,
            ModificationPriority.VeryLow
        );
        SetCenterToTargetCenterDirection(
            detectEnemy.rotationAndCenterObject.transform.position
                - customMono.rotationAndCenterObject.transform.position,
            ModificationPriority.VeryLow
        );
        targetPathFindingDirection = GameManager.Instance.GetPathFindingDirectionToTarget(
            transform.position,
            detectEnemy.transform.position
        );
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
        pausableScript.fixedUpdate();
    }

    void DoFixedUpdate()
    {
        ResetField();
        EnemySensing();
    }

    void EmptyFixedUpdate() { }

    /// <summary>
    /// Update info about currentNearestEnemy, if there is none, apply default sense.
    /// </summary>
    void EnemySensing()
    {
        if (currentNearestEnemy == null)
        {
            SetTargetToDetectEnemy();
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
            targetPathFindingDirection = GameManager.Instance.GetPathFindingDirectionToTarget(
                transform.position,
                currentNearestEnemy.transform.position
            );
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
            if (!customMono.allyTags.Contains(onTriggerCM.tag))
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

    /// <summary>
    /// Basically just gathering a bunch of sensor datas.
    /// </summary>
    public DoActionParamInfo GetDoActionParamInfo()
    {
        doActionParamInfo.originToTargetOriginDirection = originToTargetOriginDirection;
        doActionParamInfo.centerToTargetCenterDirection = centerToTargetCenterDirection;
        doActionParamInfo.firePointToTargetCenterDirection = firePointToTargetCenterDirection;
        doActionParamInfo.targetOriginPosition = targetOriginPosition;
        doActionParamInfo.targetCenterPosition = targetCenterPosition;
        doActionParamInfo.target = currentNearestEnemy;
        return doActionParamInfo;
    }
}
