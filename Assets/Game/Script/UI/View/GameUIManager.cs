using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GameUIManager : MonoSingleton<GameUIManager>
{
	public enum LayerUse
	{
		MainView = 0,
		Config = 1,
		DynamicUI = 2
	}
	public static Dictionary<int, VisualElement> helpers = new Dictionary<int, VisualElement>();

	// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	// static void Init()
	// {
	// 	helpers = new Dictionary<string, VisualElement>();    
	// }

	public static void AddHelper(int key, VisualElement helper)
	{
		helpers.Add(key, helper);
	}

	public static VisualElement GetHelper(int key)
	{
		return helpers[key];
	}

	public static void ChangeAllHelperOpacity(float opacity)
	{
		foreach (var helper in helpers.Values)
		{
			helper.style.opacity = opacity;
		}
	}

	UIDocument mainUIDocument;
	VisualElement root;
	List<VisualElement> layers;

	private MainView mainView;
	private ConfigView configView; 
	[SerializeField] private VisualTreeAsset configMenuVTA;
	VisualElement configMenu;
	GameObject radialProgressPrefab;
	ObjectPool radialProgressPool;
	[SerializeField] private Vector3 radialProgressOffset = new Vector3(-0.5f, 1.5f, 0);
	[SerializeField] private float radialProgressPositionLerpTime = 0.5f;
	public MainView MainView { get => mainView; set => mainView = value; }
	public ConfigView ConfigView { get => configView; set => configView = value; }
	public List<VisualElement> Layers { get => layers; set => layers = value; }

	private void Awake() 
	{
		LoadAssets();
		InitPools();
		EnhancedTouchSupport.Enable();
		mainUIDocument = GetComponent<UIDocument>();
		root = mainUIDocument.rootVisualElement;

		layers = root.Query<VisualElement>(classes: "layer").ToList();
		layers.Sort((ve1, ve2) => ve1.name.CompareTo(ve2.name));
		InitDefaultLayer();
		AddLayerEvent();

		GetViewComponents();
		InstantiateView();
		InitViewComponents();
		
		HandleSafeArea();
	}
	
	void InitPools()
	{
		radialProgressPool = new ObjectPool(radialProgressPrefab, 100, new PoolArgument(typeof(RadialProgress), PoolArgument.WhereComponent.Child));
	}
	
	void LoadAssets()
	{
		radialProgressPrefab = Resources.Load<GameObject>("RadialProgress");
	}

	private void GetViewComponents()
	{
		mainView = GetComponent<MainView>();
		configView = GetComponent<ConfigView>();
		mainView.GameUIManager = configView.GameUIManager = this;
	}

	private void InstantiateView()
	{
		configMenu = configMenuVTA.Instantiate();
		configMenu.name = "config__menu-root";
		configMenu.style.flexGrow = 1;
		layers[1].Q(classes:"safe-area").Add(configMenu);
	}


	private void InitViewComponents()
	{
		mainView.Init();
		configView.Init();
	}

	public void HandleSafeArea()
	{
		/* Calculate the safe area so UIs don't touch unreachable parts of the screen */
		Rect safeArea = Screen.safeArea;
		Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
		Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));
		root.Query<VisualElement>(classes: "safe-area").ForEach((ve) => 
		{
			ve.style.paddingLeft = leftTop.x;
			ve.style.paddingTop = leftTop.y;
			ve.style.paddingRight = rightBottom.x;
			ve.style.paddingBottom = rightBottom.y;
			/*  */
		});
	}

	public void InitDefaultLayer()
	{
		layers[0].style.left = 0;
		layers[0].style.top = 0;

		for (int i = 1; i < layers.Count; i++)
		{
			layers[i].style.left = 99999f;
			layers[i].style.top = 99999f;
		}
	}

	public void AddLayerEvent()
	{
		layers[(int)LayerUse.MainView].RegisterCallback<PointerDownEvent>((evt) => 
		{

		});
	}

	public void ActivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 0;
		layers[layerIndex].style.top = 0;
	}

	public void DeactivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 99999f;
		layers[layerIndex].style.top = 99999f;
	}
	
	public void ActivateOnlyLayer(LayerUse layerUse)
	{
		for (int i = 0; i < layers.Count; i++)
		{
			layers[i].style.left = 99999f;
			layers[i].style.top = 99999f;
		}
		
		layers[(int)layerUse].style.left = 0;
		layers[(int)layerUse].style.top = 0;
	}
	
	public void ActivateOnlyLayers(List<LayerUse> layerUses)
	{
		for (int i = 0; i < layers.Count; i++)
		{
			layers[i].style.left = 99999f;
			layers[i].style.top = 99999f;
		}
		
		for (int i = 0; i < layerUses.Count; i++)
		{
			layers[(int)layerUses[i]].style.left = 0;
			layers[(int)layerUses[i]].style.top = 0;
		}
	}

	public VisualElement GetLayer(int layerIndex)
	{
		return layers[layerIndex];
	}
	
	public RadialProgress CreateAndHandleRadialProgressFollowing(Transform transform)
	{
		PoolObject radialProgressPoolObject = radialProgressPool.PickOne();
		RadialProgress radialProgress = radialProgressPoolObject.radialProgress;
		StartCoroutine(HandleRadialProgressFollowing(transform, radialProgressPoolObject.gameObject));
		return radialProgress;
	}
	
	public IEnumerator HandleRadialProgressFollowing(Transform transform, GameObject radialProgress)
	{
		Vector2 newVector2Position = transform.position + radialProgressOffset, prevVector2Position, expectedVector2Position;
		float currentTime;
		
		while (true)
		{
			prevVector2Position = newVector2Position;
			/* Check current health bar position */
			newVector2Position = transform.position + radialProgressOffset;
			
			/* Start lerping position for specified duration if position change detected.*/
			if (prevVector2Position != newVector2Position)
			{
				currentTime = 0;
				while (currentTime < radialProgressPositionLerpTime + Time.fixedDeltaTime)
				{
					expectedVector2Position = Vector2.Lerp(prevVector2Position, newVector2Position, currentTime / radialProgressPositionLerpTime);
					radialProgress.transform.position = new Vector2(expectedVector2Position.x, expectedVector2Position.y);
					
					yield return new WaitForSeconds(currentTime += Time.fixedDeltaTime);
				}
			}
			
			radialProgress.transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}