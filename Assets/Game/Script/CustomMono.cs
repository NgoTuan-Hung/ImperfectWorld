using UnityEngine;
public class CustomMono : MonoBehaviour
{
	[SerializeField] private bool isBot = true;
	private GameObject target;
	private GameObject mainComponent;
	private AnimatorWrapper animatorWrapper;
	private AnimationEventFunctionCaller animationEventFunctionCaller;
	private BotMovable botMovable;
	private BotAttack botAttack;
	public AnimatorWrapper AnimatorWrapper { get => animatorWrapper; set => animatorWrapper = value; }
	public GameObject Target { get => target; set => target = value; }
	public GameObject MainComponent { get => mainComponent; set => mainComponent = value; }
	public AnimationEventFunctionCaller AnimationEventFunctionCaller { get => animationEventFunctionCaller; set => animationEventFunctionCaller = value; }
	public BotMovable BotMovable { get => botMovable; set => botMovable = value; }
	public BotAttack BotAttack { get => botAttack; set => botAttack = value; }

	private void Awake() 
	{
		animatorWrapper = GetComponent<AnimatorWrapper>();	
		if (isBot)
		{
			target = GameObject.Find("Player");
			GetBotBaseAction();
		}
		mainComponent = transform.Find("MainComponent").gameObject;
		animationEventFunctionCaller = mainComponent.GetComponentInChildren<AnimationEventFunctionCaller>();
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
}