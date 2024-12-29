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
	private AnimationEventFunctionCaller animationEventFunctionCaller;
	private BotMovable botMovable;
	private BotAttack botAttack;
	private PlayerMovable playerMovable;
	public PlayerMovable PlayerMovable
	{
		get {return playerMovable;}
	}
	public Stat stat;
	GameObject directionIndicator;
	float directionIndicatorAngle;
	Vector2[] updateDirectionIndicatorQueue = new Vector2[5];
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject Target { get => target; set => target = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public AnimationEventFunctionCaller AnimationEventFunctionCaller { get => animationEventFunctionCaller; set => animationEventFunctionCaller = value; }
	public BotMovable BotMovable { get => botMovable; set => botMovable = value; }
	public BotAttack BotAttack { get => botAttack; set => botAttack = value; }
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
			GetBotBaseAction();
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
	}
	
	void GetAllChildObject()
	{
		directionIndicator = transform.Find("DirectionIndicator").gameObject;
	}
	
	private void Start() 
	{
		
	}
	
	private void LateUpdate() 
	{
		UpdateDirectionIndicator();	
	}
	
	void GetBotBaseAction()
	{
		botMovable = GetComponent<BotMovable>();
		botAttack = GetComponent<BotAttack>();
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