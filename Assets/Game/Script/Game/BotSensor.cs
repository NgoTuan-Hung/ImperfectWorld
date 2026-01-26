using UnityEngine;

/// <summary>
/// A sensor system for bot, bot can see nearby enemies (field of view), select nearest one,
/// and so on.
/// </summary>
[DefaultExecutionOrder(-1)]
public class BotSensor : CustomMonoPal
{
    public CustomMono currentNearestEnemy = null;

    /// <summary>
    /// Origin is the position of a CustomMono
    /// </summary>
    public Vector2 originToTargetOriginDirection,
        /* Center is the position of RotationAndCenterObject of a CustomMono */
        centerToTargetCenterDirection,
        firePointToTargetCenterDirection;
    public Vector3 targetOriginPosition,
        targetCenterPosition;
    public float distanceToNearestEnemy;

    /// <summary>
    /// A centralized storage for sensor data
    /// </summary>
    DoActionParamInfo doActionParamInfo = new();
    public PausableScript pausableScript = new();

    public override void Awake()
    {
        base.Awake();
        pausableScript.Setup(EmptyFixedUpdate, DoFixedUpdate);
    }

    void SetTargetToCenterMap()
    {
        SetOriginToTargetOriginDirection(Vector3.zero - transform.position);
        SetCenterToTargetCenterDirection(
            Vector3.zero - customMono.rotationAndCenterObject.transform.position
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
        EnemySensing();
    }

    void EmptyFixedUpdate() { }

    /// <summary>
    /// Update info about currentNearestEnemy, if there is none, target center map.
    /// </summary>
    void EnemySensing()
    {
        currentNearestEnemy = GameManager.Instance.GetNearestEnemy(customMono);
        if (currentNearestEnemy == null)
        {
            SetTargetToCenterMap();
        }
        else
        {
            SetOriginToTargetOriginDirection(
                currentNearestEnemy.transform.position - transform.position
            );
            SetCenterToTargetCenterDirection(
                currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.rotationAndCenterObject.transform.position
            );
            SetFirePointToTargetCenterDirection(
                currentNearestEnemy.rotationAndCenterObject.transform.position
                    - customMono.firePoint.transform.position
            );
            SetTargetOriginPosition(currentNearestEnemy.transform.position);
            SetTargetCenterPosition(currentNearestEnemy.rotationAndCenterObject.transform.position);

            distanceToNearestEnemy = originToTargetOriginDirection.magnitude;
        }
    }

    void SetOriginToTargetOriginDirection(Vector2 p_direction)
    {
        originToTargetOriginDirection = p_direction == Vector2.zero ? Vector2.one : p_direction;
    }

    void SetCenterToTargetCenterDirection(Vector2 p_direction)
    {
        centerToTargetCenterDirection = p_direction == Vector2.zero ? Vector2.one : p_direction;
    }

    void SetFirePointToTargetCenterDirection(Vector2 p_direction)
    {
        firePointToTargetCenterDirection = p_direction == Vector2.zero ? Vector2.one : p_direction;
    }

    void SetTargetOriginPosition(Vector3 p_position)
    {
        targetOriginPosition = p_position;
    }

    void SetTargetCenterPosition(Vector3 p_position)
    {
        targetCenterPosition = p_position;
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
