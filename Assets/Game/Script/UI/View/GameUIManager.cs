using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoEditorSingleton<GameUIManager>
{
	public enum LayerUse
	{
		MainView = 0,
		Info = 1,
		Config = 2,
		DynamicUI = 3
	}

	public Vector2 skillTreeSkillSize = new(80, 80), skillTreeSkillOffset = new(150, 150)
	, skillTreeContainerSize = new(2912, 1632), skillSlotSize = new(100, 100);
	/* One visual element can only be controlled by one touch at a time,
	if an element is controlled by a touch, then its value in the dictionary
	is true. */
	public Dictionary<int, bool> VisualElementTouched = new();

	public void RegisterVisualElementTouchState(VisualElement visualElement)
	{
		VisualElementTouched.Add(visualElement.GetHashCode(), false);
		visualElement.RegisterCallback<PointerDownEvent>((evt) => 
		{
			VisualElementTouched[visualElement.GetHashCode()] = true;
		});
	}
	
	public bool CheckVisualElementTouchState(VisualElement visualElement) => VisualElementTouched[visualElement.GetHashCode()];
	public void StopVisualElementTouchState(VisualElement visualElement) => VisualElementTouched[visualElement.GetHashCode()] = false;

	public UIDocument mainUIDocument;
	public VisualElement root;
	public List<VisualElement> layers;
	public MainView mainView;
	public ConfigView configView; 
	public CharInfoView charInfoView;
	public VisualTreeAsset usableScrollViewHolderVTA, joystickVTA, contentSkillItemVTA, contentStatVTA, charSelectionCharVTA;
	public IndividualView currentActiveIndividualView;
	GameObject radialProgressPrefab;
	ObjectPool radialProgressPool;

	private void Awake() 
	{
		InitPools();
		mainUIDocument = GetComponent<UIDocument>();
		root = mainUIDocument.rootVisualElement;

		layers = root.Query<VisualElement>(classes: "layer").ToList();
		layers.Sort((ve1, ve2) => ve1.name.CompareTo(ve2.name));
		InitDefaultLayer();

		LoadAllTemplate();
		GetViewComponents();
		InitViewComponents();
		
		HandleSafeArea();
	}

	public void LoadAllTemplate() 
	{
		usableScrollViewHolderVTA = Resources.Load<VisualTreeAsset>("MainView__UsableScrollViewHolder");
		joystickVTA = Resources.Load<VisualTreeAsset>("MainView__Joystick");
		contentSkillItemVTA = Resources.Load<VisualTreeAsset>("CharInfo__ContentSkillItem");
		contentStatVTA = Resources.Load<VisualTreeAsset>("CharInfo__ContentStat");
		charSelectionCharVTA = Resources.Load<VisualTreeAsset>("CharSelection__Char");
	}

	public override void Start()
	{
		base.Start();
		#if UNITY_EDITOR
		onExitPlayModeEvent += () => 
		{
			radialProgressPool = null;
		};
		#endif
	}
	List<IndividualView> individualViews = new();
	public void CheckFirstIndividualView()
	{
		if (individualViews.Count == 1)
		{
			currentActiveIndividualView = individualViews[0];
			currentActiveIndividualView.Show();
		}
	}
	
	public IndividualView AddNewIndividualView(CharUIData p_charUIData, Action p_switchCharAction)
	{
		IndividualView t_individualView = new();
		
		p_switchCharAction += () => 
		{
			currentActiveIndividualView.Hide();
			currentActiveIndividualView = t_individualView;	
			currentActiveIndividualView.Show();
			charInfoView.ResetCharInfoContent();
		};
		t_individualView.Init(p_charUIData, p_switchCharAction);
		t_individualView.GetAllRequiredVisualElements();
		mainView.InitIndividualView(t_individualView, p_charUIData);
		charInfoView.InitIndividualView(t_individualView, p_charUIData);
		configView.InitIndividualView(t_individualView, p_charUIData);
		
		t_individualView.Hide();
		individualViews.Add(t_individualView);
		
		return t_individualView;
	}
	
	public void AddTooltipHandlerForVisualElement(VisualElement p_visualElement, VisualElement p_toolTip)
	{
		p_visualElement.RegisterCallback<PointerDownEvent>((evt) => 
		{	
			if (evt.clickCount > 1)
			{	
				p_toolTip.transform.position = p_toolTip.parent.WorldToLocal(new Vector2
				(
					evt.position.x + p_toolTip.resolvedStyle.width > root.resolvedStyle.width ? evt.position.x - p_toolTip.resolvedStyle.width : evt.position.x,
					evt.position.y + p_toolTip.resolvedStyle.height > root.resolvedStyle.height ? evt.position.y - p_toolTip.resolvedStyle.height : evt.position.y
				));
				
				p_toolTip.BringToFront(); 
				p_toolTip.AddToClassList("tooltip-showup");
			}
		});
	}
	
	void InitPools()
	{
		radialProgressPrefab = Resources.Load<GameObject>("RadialProgress");
		radialProgressPool = new ObjectPool
		(
			radialProgressPrefab
			, 100
			, new PoolArgument(ComponentType.RadialProgress, PoolArgument.WhereComponent.Child)
			, new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
		);
	}

	private void GetViewComponents()
	{
		mainView = GetComponent<MainView>();
		configView = GetComponent<ConfigView>();
		charInfoView = GetComponent<CharInfoView>();
		mainView.gameUIManager = configView.gameUIManager = charInfoView.gameUIManager = this;
	}


	private void InitViewComponents()
	{
		mainView.Init();
		configView.Init();
		charInfoView.Init();
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

	public void FireGeometryChangedEvent(VisualElement p_visualElement)
	{
		using GeometryChangedEvent evt = GeometryChangedEvent.GetPooled();
		evt.target = p_visualElement;
		p_visualElement.SendEvent(evt);
	}
	
	public PoolObject CreateAndHandleRadialProgressFollowing(Transform transform)
	{
		PoolObject radialProgressPoolObject = radialProgressPool.PickOne();
		radialProgressPoolObject.gameEffect.FollowSlowly(transform);
		return radialProgressPoolObject;
	}
}