using System;
using System.Collections.Generic;
using UnityEngine;
public class CustomMono : MonoBehaviour, IComparable<CustomMono>
{
	[SerializeField] private bool isBot = true;
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
	private Stat stat;
	GameObject directionIndicator;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject Target { get => target; set => target = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public AnimationEventFunctionCaller AnimationEventFunctionCaller { get => animationEventFunctionCaller; set => animationEventFunctionCaller = value; }
	public BotMovable BotMovable { get => botMovable; set => botMovable = value; }
	public BotAttack BotAttack { get => botAttack; set => botAttack = value; }
	public Stat Stat { get => stat; set => stat = value; }
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
}