using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum UpdateDirectionIndicatorPriority {VeryLow = 4, Low = 3, Medium = 2, High = 1, VeryHigh = 0}
public class CustomMono : MonoSelfAware, IComparable<CustomMono>
{
	public CharUIData charUIData;
	public bool isControllable = true;
	public HashSet<string> allyTags = new();
	private GameObject mainComponent;
	private SpriteRenderer spriteRenderer;
	private AnimatorWrapper animatorWrapper;
	public AnimationEventFunctionCaller animationEventFunctionCaller;
	public Movable movable;
	public Attackable attackable;
	public MovementIntelligence movementIntelligence;
	public ActionIntelligence actionIntelligence;
	public MyBotPersonality myBotPersonality;
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
	public List<CustomMono> enemiesWeSee = new();
	Vector3 baseRendererLocalScale;
	BoxCollider2D boxCollider2D, combatCollider2D;
	float boxColliderDefaultXSize, directionIndicationDefaultScale, firePointDefaultYPos;
	Vector2 combatCollisionDefaultSize, combatCollisionDefaultOffset;
	public AudioSource audioSource;
	public Action<GameObject> someOneExitView = (person) => {};
	float currentNearestEnemySqrDistance = Mathf.Infinity;
	public CustomMono currentNearestEnemy = null;
	public new Rigidbody2D rigidbody2D;
	public Action<CustomMono> nearestEnemyChanged = (person) => {};
	public Action startPhase1 = () => {};
	public AudioClip attackAudioClip;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public GameObject DirectionIndicator { get => directionIndicator; set => directionIndicator = value; }
	public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

	public override void Awake() 
	{
		base.Awake();
		allyTags.Add(gameObject.tag);
		GameManager.Instance.AddCustomMono(this);
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
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animatorWrapper = GetComponent<AnimatorWrapper>();
		stat = GetComponent<Stat>();
		movable = GetComponent<Movable>();
		attackable = GetComponent<Attackable>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		combatCollider2D = combatCollision.GetComponent<BoxCollider2D>();
		audioSource = GetComponent<AudioSource>();
		rigidbody2D = GetComponent<Rigidbody2D>();
		
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
		firePointDefaultYPos = firePoint.transform.localPosition.y;
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
			firePoint.transform.localPosition = new Vector3
			(
				firePoint.transform.localPosition.x,
				firePointDefaultYPos * stat.Size,
				firePoint.transform.localPosition.z
			);
		};
	}
	
	public override void Start()
	{
		if (isControllable) GameManager.Instance.InitializeControllableCharacter(this);

		startPhase1();
	}
	
	public void PauseBot()
	{
	    myBotPersonality.pausableScript.pauseFixedUpdate();
	}
	
	public void ResumeBot()
	{
	    myBotPersonality.pausableScript.resumeFixedUpdate();
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
		/* if we see a new enemy, remember him. */
		if (!allyTags.Contains(other.transform.parent.tag)) enemiesWeSee.Add
		(
			GameManager.Instance.GetCustomMono(other.transform.parent.gameObject)
		);
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		someOneExitView(other.transform.parent.gameObject);
		
		/* Check if we saw this enemy before. */
		CustomMono t_customMono = enemiesWeSee.FirstOrDefault(customMono => 
			customMono.gameObject.Equals(other.transform.parent.gameObject)	
		);
		
		/* If we saw this enemy before, erase him. */
		if (t_customMono != null)
		{
			enemiesWeSee.Remove(t_customMono);
			
			/* If the current nearest enemy is this one, erase it as well. */
			if (t_customMono.Equals(currentNearestEnemy))
			{
				currentNearestEnemy = null;
				currentNearestEnemySqrDistance = float.MaxValue;
			}	
		}
	}
	
	private void OnTriggerStay2D(Collider2D other) 
	{
		if (!allyTags.Contains(other.transform.parent.tag))
		{
			/* We can compare squared distance instead of distance because it's faster yet
			still gives the same result. */
			float t_enemySqrDistance = (transform.position - other.transform.parent.position).sqrMagnitude;
			
			/* if we see a closer enemy, put him as the nearest. Else, update the distance of
			the nearest enemy instead. */
			if (t_enemySqrDistance < currentNearestEnemySqrDistance)
			{
				currentNearestEnemySqrDistance = t_enemySqrDistance;
				currentNearestEnemy = GameManager.Instance.GetCustomMono
				(
					other.transform.parent.gameObject
				);
				nearestEnemyChanged(currentNearestEnemy);
			} 
			else if (other.transform.parent.gameObject.Equals(currentNearestEnemy.gameObject)) currentNearestEnemySqrDistance = t_enemySqrDistance;
		}
	}
}