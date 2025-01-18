using System;
using System.Collections.Generic;
using UnityEngine;
public enum UpdateDirectionIndicatorPriority {VeryLow = 4, Low = 3, Medium = 2, High = 1, VeryHigh = 0}
public class CustomMono : MonoBehaviour, IComparable<CustomMono>
{
	public bool isBot = true;
	public HashSet<string> allyTags = new();
	public GameObject target = null;
	private GameObject mainComponent;
	private SpriteRenderer spriteRenderer;
	private AnimatorWrapper animatorWrapper;
	public AnimationEventFunctionCaller animationEventFunctionCaller;
	public Movable movable;
	public Attackable attackable;
	public MovementIntelligence movementIntelligence;
	public ActionIntelligence actionIntelligence;
	public MyBotPersonality myBotPersonality;
	private PlayerMovable playerMovable;
	public PlayerMovable PlayerMovable
	{
		get {return playerMovable;}
	}
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
	public GameObject fieldOfView, combatCollision, firePoint;
	public List<GameObject> allPeopleWeSee = new();
	Vector3 baseRendererLocalScale;
	BoxCollider2D boxCollider2D, combatCollider2D;
	float boxColliderDefaultXSize, directionIndicationDefaultScale;
	Vector2 combatCollisionDefaultSize, combatCollisionDefaultOffset;
	public AudioSource audioSource;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public GameObject DirectionIndicator { get => directionIndicator; set => directionIndicator = value; }
	public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

	private void Awake() 
	{
		GameManager.Instance.AddCustomMono(this);
		allyTags.Add(gameObject.tag);
		GetAllChildObject();
		GetAllComponents();
		PrepareValues();
		StatChangeRegister();
	}
	
	private void OnEnable() 
	{
		actionBlocking = false;
		actionInterval = false;
		movementActionInterval = false;
		movementActionBlocking = false;
		combatCollision.SetActive(true);
	}
	
	void GetAllComponents()
	{
		playerMovable = GetComponent<PlayerMovable>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animatorWrapper = GetComponent<AnimatorWrapper>();
		stat = GetComponent<Stat>();
		movable = GetComponent<Movable>();
		attackable = GetComponent<Attackable>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		combatCollider2D = combatCollision.GetComponent<BoxCollider2D>();
		audioSource = GetComponent<AudioSource>();
		
		/* Bot components */
		movementIntelligence = GetComponent<MovementIntelligence>();
		actionIntelligence = GetComponent<ActionIntelligence>();
		myBotPersonality = GetComponent<MyBotPersonality>();
	}
	
	void GetAllChildObject()
	{
		mainComponent = transform.Find("MainComponent").gameObject;
		animationEventFunctionCaller = mainComponent.GetComponentInChildren<AnimationEventFunctionCaller>();
		directionIndicator = transform.Find("DirectionIndicator").gameObject;
		fieldOfView = transform.Find("FieldOfView").gameObject;
		combatCollision = transform.Find("CombatCollision").gameObject;
		firePoint = transform.Find("FirePoint").gameObject;
	}
	
	void PrepareValues()
	{
		baseRendererLocalScale = spriteRenderer.transform.localScale;
		boxColliderDefaultXSize = boxCollider2D.size.x;
		combatCollisionDefaultSize = combatCollider2D.size;
		combatCollisionDefaultOffset = combatCollider2D.offset;
		directionIndicationDefaultScale = directionIndicator.transform.localScale.x;
	}
	
	void StatChangeRegister()
	{
		stat.sizeChangeEvent.action += () =>
		{
			spriteRenderer.transform.localScale = new Vector3(stat.Size, stat.Size, 1);
			baseRendererLocalScale = spriteRenderer.transform.localScale;
			boxCollider2D.size = new Vector2(boxColliderDefaultXSize * stat.Size, boxCollider2D.size.y);
			combatCollider2D.size = combatCollisionDefaultSize * stat.Size;
			combatCollider2D.offset = combatCollisionDefaultOffset - new Vector2
			(
				0, (combatCollisionDefaultSize - combatCollider2D.size).y / 2
			);
			directionIndicator.transform.localScale = new Vector3(directionIndicationDefaultScale * stat.Size, directionIndicationDefaultScale * stat.Size, 1);
		};
	}
	
	private void Start() 
	{
		//
	}
	
	private void FixedUpdate() 
	{
		UpdateDirectionIndicator();
		
		for (int i=0;i<multipleCollideTimersList.Count;i++) 
			if (multipleCollideTimersList[i].currentTime > 0) multipleCollideTimersList[i].currentTime -= Time.fixedDeltaTime;
	}
	
	public void AddMultipleCollideTimer(int key, float time)
	{
		multipleCollideTimersList.Add(new MultipleCollideTimer(time));
		multipleCollideTimersDict.Add(key, multipleCollideTimersList[^1]);
	}
	
	void GetBaseAction()
	{
		
	}

	public int CompareTo(CustomMono other)
	{
		return gameObject.GetHashCode().CompareTo(other.gameObject.GetHashCode());
	}
	
	/// <summary>
	/// Higher priority will be updated 
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="priority"></param>
	public void SetUpdateDirectionIndicator(Vector2 direction, UpdateDirectionIndicatorPriority priority)
	{
		updateDirectionIndicatorQueue[(int)priority] = direction;
	}
	
	void UpdateDirectionIndicator()
	{
		for (int i=0;i<updateDirectionIndicatorQueue.Length;i++)
		{
			if (updateDirectionIndicatorQueue[i] != Vector2.zero)
			{
				SpriteRenderer.transform.localScale = new Vector3(updateDirectionIndicatorQueue[i].x > 0 ? 
				baseRendererLocalScale.x : -baseRendererLocalScale.x, baseRendererLocalScale.y, baseRendererLocalScale.z);
				directionIndicatorAngle = Vector2.SignedAngle(Vector2.right, updateDirectionIndicatorQueue[i]);
				DirectionIndicator.transform.rotation = Quaternion.Euler(0, 0, directionIndicatorAngle);
				break;
			}
		}
		
		for (int i=0;i<updateDirectionIndicatorQueue.Length;i++) updateDirectionIndicatorQueue[i] = Vector2.zero;
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		allPeopleWeSee.Add(other.transform.parent.gameObject);
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if (target != null)
		{
			if (target.Equals(other.transform.parent.gameObject)) target = null;
		}
		allPeopleWeSee.Remove(other.transform.parent.gameObject);
	}
	
	public void TryPickRandomTarget()
	{
		allPeopleWeSee.ForEach(p => 
		{
			if (!allyTags.Contains(p.tag)) 
			{
				target = p;
				return;
			}
		});
	}
}