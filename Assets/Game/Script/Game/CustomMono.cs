using System;
using System.Collections.Generic;
using UnityEngine;
public enum UpdateDirectionIndicatorPriority {VeryLow = 4, Low = 3, Medium = 2, High = 1, VeryHigh = 0}
public class CustomMono : MonoBehaviour, IComparable<CustomMono>
{
	public bool isBot = true;
	Dictionary<string, bool> alliesTag = new Dictionary<string, bool>();
	private GameObject target;
	private GameObject mainComponent;
	private SpriteRenderer spriteRenderer;
	private AnimatorWrapper animatorWrapper;
	public AnimationEventFunctionCaller animationEventFunctionCaller;
	public Movable movable;
	public Attackable attackable;
	public MovementIntelligence movementIntelligence;
	public ActionIntelligence actionIntelligence;
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
	/// Just caching time for CollideAndDamage multiple collision handler
	/// </summary>
	public float multipleCollideCurrentTime = 0f;
	public bool actionInterval = false;
	public bool movementActionInterval = false;
	/// <summary>
	/// You don't want other action to be executed while an animation of 
	/// an action is playing, like you can't cast spell while your 
	/// attack animation is playing. Well, you can still move while attacking
	/// or casting spell though, that's why we divide things into action or
	/// movement action.
	/// </summary>
	public bool actionBlocking = false;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject Target { get => target; set => target = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public Dictionary<string, bool> AlliesTag { get => alliesTag; set => alliesTag = value; }
	public GameObject DirectionIndicator { get => directionIndicator; set => directionIndicator = value; }
	public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

	private void Awake() 
	{
		GameManager.Instance.AddCustomMono(this);
		alliesTag[gameObject.tag] = true;
		GetAllChildObject();
		GetAllComponents();
		if (isBot)
		{
			target = GameObject.Find("Player");
		}
		else
		{
			playerMovable = GetComponent<PlayerMovable>();
		}
		mainComponent = transform.Find("MainComponent").gameObject;
		animationEventFunctionCaller = mainComponent.GetComponentInChildren<AnimationEventFunctionCaller>();
	}
	
	void GetAllComponents()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animatorWrapper = GetComponent<AnimatorWrapper>();
		stat = GetComponent<Stat>();
		movable = GetComponent<Movable>();
		attackable = GetComponent<Attackable>();
		movementIntelligence = GetComponent<MovementIntelligence>();
		actionIntelligence = GetComponent<ActionIntelligence>();
	}
	
	void GetAllChildObject()
	{
		directionIndicator = transform.Find("DirectionIndicator").gameObject;
	}
	
	private void Start() 
	{
		//
	}
	
	private void FixedUpdate() 
	{
		if (multipleCollideCurrentTime > 0) multipleCollideCurrentTime -= Time.fixedDeltaTime;
		UpdateDirectionIndicator();	
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
				SpriteRenderer.transform.localScale = new Vector3(updateDirectionIndicatorQueue[i].x > 0 ? 1 : -1, 1, 1);
				directionIndicatorAngle = Vector2.SignedAngle(Vector2.right, updateDirectionIndicatorQueue[i]);
				DirectionIndicator.transform.rotation = Quaternion.Euler(0, 0, directionIndicatorAngle);
				break;
			}
		}
		
		for (int i=0;i<updateDirectionIndicatorQueue.Length;i++) updateDirectionIndicatorQueue[i] = Vector2.zero;
	}
}