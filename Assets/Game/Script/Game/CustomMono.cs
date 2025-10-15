using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpdateDirectionIndicatorPriority
{
    VeryLow = 4,
    Low = 3,
    Medium = 2,
    High = 1,
    VeryHigh = 0,
}

public enum AttackType
{
    Melee,
    Ranged,
}

[RequireComponent(typeof(BotSensor), typeof(BotAIManager))]
[DefaultExecutionOrder(0)]
public class CustomMono : MonoSelfAware
{
    public CharUIData charUIData;
    public bool isControllable = true;
    public HashSet<string> allyTags = new();
    private GameObject mainComponent;
    public SpriteRenderer spriteRenderer;
    private AnimatorWrapper animatorWrapper;
    public AnimationEventFunctionCaller animationEventFunctionCaller;
    public Movable movable;
    public BotSensor botSensor;
    public BotAIManager botAIManager;
    public Stat stat;
    GameObject directionIndicator;
    float directionIndicatorAngle;
    Vector2[] updateDirectionIndicatorQueue = new Vector2[5];

    /// <summary>
    /// Just caching time for CollideAndDamage multiple collision handler.
    /// for each collider in contact, hold a timer instance for it. Remember,
    /// we should sync the list accordingly when modifying the dict.
    /// </summary>
    public Dictionary<int, MultipleCollideTimer> multipleCollideTimersDict = new();
    List<MultipleCollideTimer> multipleCollideTimersList = new();

    /// <summary>
    /// This is the interval between bot next action choosing (we only
    /// have this because of performance reason, we don't want the bot
    /// to choose action too fast right ? Well i think we might remove
    /// this in the future, let's see). Also we will need to let bot
    /// choose action faster if it is allowed to choose faster like
    /// attacking faster.
    /// </summary>
    public bool actionInterval;
    public bool movementActionInterval;

    /// <summary>
    /// You don't want other action to be executed while an animation of
    /// an action is playing, like you can't cast spell while your
    /// attack animation is playing. Well, you can still move while attacking
    /// or casting spell though, that's why we divide things into action or
    /// movement action.
    /// </summary>
    public bool actionBlocking;
    public bool movementActionBlocking;
    public GameObject fieldOfView,
        combatCollision,
        firePoint;
    public BoxCollider2D boxCollider2D,
        combatCollider2D;
    float boxColliderDefaultXSize,
        directionIndicationDefaultScale,
        firePointDefaultYPos;
    Vector2 combatCollisionDefaultSize,
        combatCollisionDefaultOffset;
    public AudioSource audioSource;
    public new Rigidbody2D rigidbody2D;
    public Action startPhase1 = () => { };
    public AudioClip attackAudioClip;
    public CharAttackInfo charAttackInfo;
    public StatusEffect statusEffect;
    public GameObject rotationAndCenterObject,
        directionModifier;
    public BaseAction currentAction,
        currentMovementAction;
    public AnimatorWrapper AnimatorWrapper
    {
        get => animatorWrapper;
        set => animatorWrapper = value;
    }
    public Skill skill;

    public override void Awake()
    {
        base.Awake();
        allyTags.Add(gameObject.tag);
        GetAllChildObject();
        GetAllComponents();
        PrepareValues();
        StatChangeRegister();
        GameManager.Instance.AddCustomMono(this);
    }

    private void OnEnable()
    {
        combatCollision.SetActive(true);
    }

    void GetAllComponents()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animatorWrapper = GetComponent<AnimatorWrapper>();
        stat = GetComponent<Stat>();
        movable = GetComponent<Movable>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        combatCollider2D = combatCollision.GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        statusEffect = GetComponent<StatusEffect>();
        skill = GetComponent<Skill>();
        botSensor = GetComponent<BotSensor>();
        botAIManager = GetComponent<BotAIManager>();
    }

    void GetAllChildObject()
    {
        directionModifier = transform.Find("DirectionModifier").gameObject;
        firePoint = directionModifier.transform.Find("FirePoint").gameObject;
        mainComponent = directionModifier.transform.Find("MainComponent").gameObject;
        animationEventFunctionCaller = mainComponent.GetComponent<AnimationEventFunctionCaller>();
        directionIndicator = transform.Find("DirectionIndicator").gameObject;
        fieldOfView = transform.Find("FieldOfView").gameObject;
        combatCollision = transform.Find("CombatCollision").gameObject;
        rotationAndCenterObject = transform.Find("RotationAndCenterObject").gameObject;
    }

    void PrepareValues()
    {
        boxColliderDefaultXSize = boxCollider2D.size.x;
        combatCollisionDefaultSize = combatCollider2D.size;
        combatCollisionDefaultOffset = combatCollider2D.offset;
        directionIndicationDefaultScale = directionIndicator.transform.localScale.x;
        firePointDefaultYPos = firePoint.transform.localPosition.y;
    }

    void StatChangeRegister()
    {
        stat.sizeChangeEvent.action += () =>
        {
            spriteRenderer.transform.localScale = new Vector3(stat.Size, stat.Size, 1);
            boxCollider2D.size = new Vector2(
                boxColliderDefaultXSize * stat.Size,
                boxCollider2D.size.y
            );
            combatCollider2D.size = combatCollisionDefaultSize * stat.Size;
            combatCollider2D.offset =
                combatCollisionDefaultOffset
                - new Vector2(0, (combatCollisionDefaultSize - combatCollider2D.size).y / 2);
            directionIndicator.transform.localScale = new Vector3(
                directionIndicationDefaultScale * stat.Size,
                directionIndicationDefaultScale * stat.Size,
                1
            );
            firePoint.transform.localPosition = new Vector3(
                firePoint.transform.localPosition.x,
                firePointDefaultYPos * stat.Size,
                firePoint.transform.localPosition.z
            );
        };
    }

    public override void Start()
    {
        if (isControllable)
            GameManager.Instance.InitializeControllableCharacterRevamp(this);

        startPhase1();
    }

    public void PauseBot()
    {
        botAIManager.aiBehavior.pausableScript.pauseFixedUpdate();
        movable.pausableScript.resumeFixedUpdate();
    }

    public void ResumeBot()
    {
        botAIManager.aiBehavior.pausableScript.resumeFixedUpdate();
        movable.pausableScript.pauseFixedUpdate();
    }

    private void FixedUpdate()
    {
        UpdateDirectionIndicator();

        for (int i = 0; i < multipleCollideTimersList.Count; i++)
            if (multipleCollideTimersList[i].currentTime > 0)
                multipleCollideTimersList[i].currentTime -= Time.fixedDeltaTime;
    }

    public void AddMultipleCollideTimer(int key, float time)
    {
        multipleCollideTimersList.Add(new MultipleCollideTimer(time));
        multipleCollideTimersDict.Add(key, multipleCollideTimersList[^1]);
    }

    /// <summary>
    /// Higher priority will be updated
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="priority"></param>
    public void SetUpdateDirectionIndicator(
        Vector2 direction,
        UpdateDirectionIndicatorPriority priority
    )
    {
        updateDirectionIndicatorQueue[(int)priority] = direction;
    }

    void UpdateDirectionIndicator()
    {
        for (int i = 0; i < updateDirectionIndicatorQueue.Length; i++)
        {
            if (updateDirectionIndicatorQueue[i] != Vector2.zero)
            {
                directionModifier.transform.localScale = new Vector3(
                    updateDirectionIndicatorQueue[i].x * directionModifier.transform.localScale.x
                    < 0
                        ? -directionModifier.transform.localScale.x
                        : directionModifier.transform.localScale.x,
                    directionModifier.transform.localScale.y,
                    directionModifier.transform.localScale.z
                );
                directionIndicatorAngle = Vector2.SignedAngle(
                    Vector2.right,
                    updateDirectionIndicatorQueue[i]
                );
                directionIndicator.transform.rotation = Quaternion.Euler(
                    0,
                    0,
                    directionIndicatorAngle
                );
                break;
            }
        }

        for (int i = 0; i < updateDirectionIndicatorQueue.Length; i++)
            updateDirectionIndicatorQueue[i] = Vector2.zero;
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveCustomMono(this);
    }

    /// <summary>
    /// Get the direction (of the arrow with the circle around characters)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirection() => directionIndicator.transform.right;
}
