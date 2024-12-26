using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum ScrollViewLockState {Locked = 0, Unlocked = 1, AutoLocked = 2}

public class MainView : ViewBase
{
	[SerializeField] private VisualTreeAsset skillHolderTemplate;
	[SerializeField] private VisualTreeAsset skillTooltipTemplate;
	[SerializeField] private VisualTreeAsset healthBarTemplate;
	// [SerializeField] private VisualTreeAsset helperLensTemplate;
	StyleSheet skillTooltipSS;
	[SerializeField] private AudioClip scrollSound;
	[SerializeField] private AudioSource audioSource;
	bool optionExpandButtonExpanded = true, scrollLockExpandButtonExpanded = true, 
	lockExpandLocked = true;
	public Action clickAttackButtonEvent = () => { };
	public Action<Vector2> holdAttackButtonEvent = (vector2) => { };
	Vector2 attackDirection, inverseY = new Vector2(1, -1);

	public void Init() 
	{
		FindAllVisualElements();
		snapInterval = snapTime * snapIntervalPortion;
		audioSource.clip = scrollSound;
		skillTooltipSS = Resources.Load<StyleSheet>("SkillTooltipSS");

		HelperLensDragAndDropManipulator dragAndDropManipulator = new HelperLensDragAndDropManipulator(helperLensRoot, skillTooltipTemplate);

		HandleSkillView();
		HandleJoyStickView();
		HandleScrollLockExpandLock();
		HandleExpandButton();
		PopulateOptions();
		HandleAttackButton();
	}

	List<ScrollView> skillScrollViews; List<SkillScrollViewUIInfo> skillScrollViewUIInfos = new List<SkillScrollViewUIInfo>();
	VisualElement root, helperLensRoot, optionExpandButton, options, mainViewLayer, skillScrollViewHolder
	, scrollLockParent, scrollLockExpandButton, scrollLockExpandLock, joyStickHolder, joyStickOuter, joyStickInner;
	void FindAllVisualElements()
	{
		var uiDocument = GetComponent<UIDocument>();
		root = uiDocument.rootVisualElement;
		
		mainViewLayer = GameUIManager.Instance.Layers[(int)GameUIManager.LayerUse.MainView];
		
		/* Option Views */
		optionExpandButton = mainViewLayer.Q<VisualElement>(name: "main-view__option-expand-button");
		options = mainViewLayer.Q<VisualElement>(name: "main-view__options");
		
		/* ScrollView */
		skillScrollViewHolder = mainViewLayer.Q<VisualElement>(name: "main-view__skill-scroll-view-holder");
		skillScrollViews = skillScrollViewHolder.Query<ScrollView>(classes: "main-view__skill-scroll-view").ToList();
		scrollLockParent = skillScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__lock-parent");
		scrollLockExpandButton = skillScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__expand-button");
		scrollLockExpandLock = scrollLockParent.Q<VisualElement>(name: "scroll-lock-view__lock-expand");
		
		/* JoyStick */
		joyStickHolder = mainViewLayer.Q<VisualElement>(name: "JoyStickHolder");
		joyStickOuter = joyStickHolder.ElementAt(0);
		joyStickInner = joyStickOuter.ElementAt(0);
		
		/* Create a helper lens and assign drag and drop logic to it */
		helperLensRoot = root.Q<VisualElement>("helper-lens");
		helperLensRoot.style.position = Position.Absolute;
	}

	void PopulateOptions()
	{
		List<MainViewOptionData> mainViewOptionDatas = Resources.LoadAll<MainViewOptionData>("UI/MainViewOptionData").ToList();
		
		mainViewOptionDatas.ForEach(mainViewOptionData => 
		{
			VisualElement visualElement = new();
			visualElement.AddToClassList("main-view__option-button");
			visualElement.style.backgroundImage = mainViewOptionData.icon;

			switch(mainViewOptionData.functionName)
			{
				case "": break;
				case "OpenSetting": 
				{
					visualElement.RegisterCallback<PointerDownEvent>(evt => 
					{
						/* Used to block touch screen swipe event */
						evt.StopPropagation();
						gameUIManager.ActivateLayer((int)GameUIManager.LayerUse.Config);
					});
					break;
				}
				default: break;
			}
			
			options.Add(visualElement);
		});
	}

	void HandleExpandButton()
	{
		optionExpandButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			/* Used to block touch screen swipe event */
			evt.StopPropagation();
			if (optionExpandButtonExpanded)
			{
				optionExpandButtonExpanded = false;
				optionExpandButton.AddToClassList("main-view__expand-button-collapsed");
				options.AddToClassList("main-view__options-collapsed");
			}
			else
			{
				optionExpandButtonExpanded = true;
				optionExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				options.RemoveFromClassList("main-view__options-collapsed");
			}
		});
		
		scrollLockExpandButton.RegisterCallback<PointerDownEvent>((evt) =>
		{
			evt.StopPropagation();
			if (lockExpandLocked) return;
			if (scrollLockExpandButtonExpanded)
			{
				scrollLockExpandButtonExpanded = false;
				scrollLockExpandButton.AddToClassList("main-view__expand-button-collapsed");
				scrollLockParent.AddToClassList("scroll-lock-view__lock-parent-collapsed");
			}
			else
			{
				scrollLockExpandButtonExpanded = true;
				scrollLockExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				scrollLockParent.RemoveFromClassList("scroll-lock-view__lock-parent-collapsed");
			}
		});
	}
	
	void HandleScrollLockExpandLock()
	{
		scrollLockExpandLock.RegisterCallback<MouseDownEvent>(evt => 
		{
			if (lockExpandLocked) scrollLockExpandLock.AddToClassList("scroll-lock-view__lock-expand-unlocked");
			else scrollLockExpandLock.RemoveFromClassList("scroll-lock-view__lock-expand-unlocked");
			lockExpandLocked = !lockExpandLocked;
			
		});
	}

	[SerializeField] private VisualTreeAsset scrollViewLockVTA;
	/// <summary>
	/// Handle scroll view lock (mostly skill)
	/// . Lock state: Lock -> Unlocked -> AutoLocked -> Lock -> ...
	/// </summary>
	void HandleScrollLock(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{	
		switch (skillScrollViewUIInfo.ScrollViewLockState)
		{
			case ScrollViewLockState.Locked:
			{
				skillScrollViewUIInfo.ScrollViewLock.AddToClassList("scroll-lock-view__lock-unlocked");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Unlocked;
				break;
			}
			case ScrollViewLockState.Unlocked:
			{
				skillScrollViewUIInfo.ScrollViewLock.RemoveFromClassList("scroll-lock-view__lock-unlocked");
				skillScrollViewUIInfo.ScrollViewLock.AddToClassList("scroll-lock-view__lock-auto-lock");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.AutoLocked;
				break;
			}
			case ScrollViewLockState.AutoLocked:
			{
				skillScrollViewUIInfo.ScrollViewLock.RemoveFromClassList("scroll-lock-view__lock-auto-lock");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Locked;
				break;
			}
			default: break;
		}
	}
	
	public void AddSkillToScrollView(SkillDataSo skillDataSO, Action<Touch> trigger)
	{
		var newSkillHolder = skillHolderTemplate.Instantiate();
		skillScrollViews[skillDataSO.skillButtonIndex].contentContainer.Add(newSkillHolder);
		HandleSkillHolderView(newSkillHolder, skillDataSO.skillButtonIndex);
		
		/* Add image for skill holder */
		newSkillHolder.Q<VisualElement>("skill-holder-in").style.backgroundImage = new StyleBackground(skillDataSO.skillImage);
		
		var skillTooltip = new SkillTooltipView(skillTooltipTemplate.Instantiate(), skillDataSO.skillName, skillDataSO.skillHelperImage, skillDataSO.skillHelperDescription, skillTooltipSS).visualElement;
		newSkillHolder.GetLayer().Add(skillTooltip);
		skillTooltip.style.position = new StyleEnum<Position>(Position.Absolute);
		skillTooltip.style.visibility = Visibility.Hidden;
		GameUIManager.AddHelper(newSkillHolder.GetHashCode(), skillTooltip);

		newSkillHolder.AddToClassList("has-helper");
		newSkillHolder.AddToClassList("helper-type-skill-info");
		
		
		/* Only allow tooltip for current visible element in scroll view */
		if (skillScrollViews[skillDataSO.skillButtonIndex].contentContainer.IndexOf(newSkillHolder)
		!= skillScrollViewUIInfos[skillDataSO.skillButtonIndex].SkillScrollViewNewIndex)
			newSkillHolder.AddToClassList("helper-invisible");
		
		/* Handle event here */
		switch (skillDataSO.inputType)
		{
			case SkillDataSo.InputType.Click:
				newSkillHolder.RegisterCallback<PointerDownEvent>((evt) => 
				{
					var touch = TouchExtension.GetTouchOverlapVisualElement(newSkillHolder, root.panel);
					trigger(touch);
				});
				break;
			case SkillDataSo.InputType.Hold:
				break;
			default:
				break;
		}
	}
	
	/// <summary>
	/// Populate the skill slots info
	/// </summary>
	void HandleSkillView()
	{	
		/* Load datas from scriptable object and create skill ui.
		Also handle tooltip of each skill*/
		List<SkillData> skillDatas = Resources.LoadAll<SkillData>("SkillData").ToList();
		
		skillDatas.ForEach(skillData => 
		{
			// Handle add skill to scroll view here
			var newSkillHolder = skillHolderTemplate.Instantiate();
			skillScrollViews[skillData.skillButtonIndex].contentContainer.Add(newSkillHolder);
		});

		/* Handle scroll logic, scrolling, snapping */
		for (int i=0;i<skillScrollViews.Count;i++)
		{	
			SkillScrollViewUIInfo skillScrollViewUIInfo = new SkillScrollViewUIInfo(skillScrollViews[i], i, null);
			skillScrollViewUIInfos.Add(skillScrollViewUIInfo);
			
			/* For each scroll view, we assign a lock to it */
			skillScrollViewUIInfo.ScrollViewLock = scrollViewLockVTA.Instantiate().ElementAt(0);
			skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Locked;
			skillScrollViewUIInfo.ScrollViewLock.RegisterCallback<PointerDownEvent>((evt) => 
			{
				/* Used to block touch screen swipe event */
				evt.StopPropagation();
				HandleScrollLock(skillScrollViewUIInfo);
			});
			
			/* Add reference for all skill holders in this scroll view */
			var t_children = skillScrollViews[i].contentContainer.Children();
			foreach (VisualElement skillHolder in t_children) HandleSkillHolderView(skillHolder, i);
			
			scrollLockParent.Insert(i, skillScrollViewUIInfo.ScrollViewLock.parent);

			skillScrollViews[i].verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollViewUIInfo);
			
			skillScrollViews[i].RegisterCallback<PointerDownEvent>((evt) => 
			{
				SkillScrollViewPointerDown(skillScrollViewUIInfo);
			}, TrickleDown.TrickleDown);
			
			skillScrollViews[i].RegisterCallback<PointerDownEvent>((evt) => 
			{
				/* Used to block touch screen swipe event */
				evt.StopPropagation();
			});
			
			/* Used to determine some final style of scroll view (height,...)*/
			skillScrollViews[i].RegisterCallback<GeometryChangedEvent>
			(
				(evt) => SkillScrollViewGeometryChanged(skillScrollViewUIInfo)
			);
		}
	}
	
	/// <summary>
	/// Mainly handle blocking scroll event and cache reference to skill holder with some
	/// additional infos
	/// </summary>
	/// <param name="skillHolder"></param>
	/// <param name="scrollViewIndex"></param>
	void HandleSkillHolderView(VisualElement skillHolder, int scrollViewIndex)
	{
		/* We will mainly use this for referencing the middle point of the scroll view, which is
		very important for dealing with event like touching, holding a skill, middle point will
		be updated at ScrollView GeometryChanged event */
		SkillHolderView skillHolderView = new SkillHolderView(skillHolder, skillScrollViews[scrollViewIndex]);
		skillHolder.RegisterCallback<PointerMoveEvent>((evt) => 
		{
			/* Check When locked is set to true, lock the scroll view. Also scroll happen at 
			scrollview.contentContainer.PointerMoveEvent(Bubble Up phase) so we can block it here */
			if (skillScrollViewUIInfos[scrollViewIndex].ScrollViewLockState == ScrollViewLockState.Locked) evt.StopPropagation();
		});
		
		skillScrollViewUIInfos[scrollViewIndex].skillHolderViews.Add(skillHolderView);
	}

	/// <summary>
	/// Mostly used to play sound if scroll view scroll passed a element
	/// </summary>
	/// <param name="skillScrollViewUIInfo"></param>
	void SkillScrollViewEvent(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
		skillScrollViewUIInfo.SkillScrollViewNewIndex = (int)Math.Floor(skillScrollViewUIInfo.ScrollView.verticalScroller.value / skillScrollViewUIInfo.ScrollViewHeight + 0.5f);
		if (skillScrollViewUIInfo.SkillScrollViewNewIndex != skillScrollViewUIInfo.SkillScrollViewPreviousIndex)
		{
			audioSource.Play();
			skillScrollViewUIInfo.ScrollView.contentContainer.ElementAt(skillScrollViewUIInfo.SkillScrollViewNewIndex).RemoveFromClassList("helper-invisible");
			skillScrollViewUIInfo.ScrollView.contentContainer.ElementAt(skillScrollViewUIInfo.SkillScrollViewPreviousIndex).AddToClassList("helper-invisible");
		}

		skillScrollViewUIInfo.SkillScrollViewPreviousIndex = skillScrollViewUIInfo.SkillScrollViewNewIndex;
	}

	void SkillScrollViewPointerDown(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
		if (skillScrollViewUIInfo.ScrollViewLockState == ScrollViewLockState.Locked) return;
		if (skillScrollViewUIInfo.ScrollSnapCoroutine != null) StopCoroutine(skillScrollViewUIInfo.ScrollSnapCoroutine);
		skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
		skillScrollViewUIInfo.ScrollSnapCoroutine = StartCoroutine(HandleScrollSnap(skillScrollViewUIInfo));
	}
	
	void SkillScrollViewGeometryChanged(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
		var t_skillScrollView = skillScrollViewUIInfo.ScrollView;
		skillScrollViewUIInfo.ScrollViewHeight = t_skillScrollView.resolvedStyle.height;
		skillScrollViewUIInfo.DistanceToSnap = skillScrollViewUIInfo.ScrollViewHeight * distanceToSnapScale;
		
		/* Update mid position of all skill holders in this scroll view */
		skillScrollViewUIInfo.skillHolderViews.ForEach(skillHolderView => 
		{
			skillHolderView.midPos = t_skillScrollView.worldBound.position + t_skillScrollView.worldBound.size / 2;
		});
	}

	[SerializeField] private float snapTime = 0.3f;
	[SerializeField] private float snapIntervalPortion = 0.1f;
	private float snapInterval;
	[SerializeField] private float distanceToSnapScale = 0.5f;
	private float defaultScrollDecelerationRate = 0.135f;

	IEnumerator HandleScrollSnap(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
		/* Find any first touch that overlaps the skill scroll view */
		Touch associatedTouch = TouchExtension.GetTouchOverlapVisualElement(skillScrollViewUIInfo.ScrollView, root.panel);
		
		if (associatedTouch.Equals(default)) yield break;

		/* snap logic only happens when we release the touch */
		while (associatedTouch.phase != UnityEngine.InputSystem.TouchPhase.Ended) yield return new WaitForSeconds(Time.deltaTime);

		float prevPosition = float.MaxValue; 
		float finalPosition, currentPosition;
		int finalIndex;

		/* snap logic only happens when the scroll speed is low enough */
		while (Math.Abs(skillScrollViewUIInfo.ScrollView.verticalScroller.value - prevPosition) > skillScrollViewUIInfo.DistanceToSnap)
		{
			prevPosition = skillScrollViewUIInfo.ScrollView.verticalScroller.value;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		} skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = 0f;

		/* snap logic:
		- Grab the element that the center of the scroll view is inside
		- Lerp from the current scroll position to the element's position
		- Snap to the element for more accurate snapping (Use unity internal function ScrollTo)
		 */
		currentPosition = skillScrollViewUIInfo.ScrollView.verticalScroller.value;
		finalIndex = (int)Math.Floor(skillScrollViewUIInfo.ScrollView.verticalScroller.value/skillScrollViewUIInfo.ScrollViewHeight + 0.5f);
		finalPosition = finalIndex * skillScrollViewUIInfo.ScrollViewHeight;

		float currentTime = 0, progress;
		while (true)
		{
			progress = currentTime / snapTime;
			if (progress > 1.01f) break;
			skillScrollViewUIInfo.ScrollView.verticalScroller.value = Mathf.Lerp(currentPosition, finalPosition, progress);
			yield return new WaitForSeconds(snapInterval);
			currentTime += snapInterval;
		}
		skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
		skillScrollViewUIInfo.ScrollView.ScrollTo(skillScrollViewUIInfo.ScrollView.contentContainer.Children().ElementAt(finalIndex));
		
		/* If we choose auto lock scroll view, we can handle it here, right after scrolling and snapping
		is done */
		if (skillScrollViewUIInfo.ScrollViewLockState == ScrollViewLockState.AutoLocked) HandleScrollLock(skillScrollViewUIInfo);
	}

	/// <summary>
	/// Assign a health bar to a specific transform
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="camera"></param>
	public void InstantiateAndHandleHealthBar(Transform transform, Camera camera)
	{
		var healthBar = healthBarTemplate.Instantiate();
		healthBar.style.flexGrow = 0f;
		healthBar.style.position = Position.Absolute;
		gameUIManager.GetLayer((int)GameUIManager.LayerUse.MainView).Add(healthBar);
		StartCoroutine(HandleHealthBarFloating(transform, healthBar, camera));
	}

	[SerializeField] private Vector3 healthBarOffset = new Vector3(-0.5f, 1.5f, 0);
	[SerializeField] private float healthBarPositionLerpTime = 0.5f;
	/// <summary>
	/// Handle floating health bar position every frame. Health bar will follow the transform with a little offset
	/// and the health bar movement will be smoothed every specified duration (healthBarPositionLerpTime).
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="healthBar"></param>
	/// <param name="camera"></param>
	/// <returns></returns>
	IEnumerator HandleHealthBarFloating(Transform transform, VisualElement healthBar, Camera camera)
	{
		Vector2 newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera)
		, prevVector2Position, expectedVector2Position;
		float currentTime;
		
		while (true)
		{
			prevVector2Position = newVector2Position;
			/* Check current health bar position */
			newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera);
			
			/* Start lerping position for specified duration if position change detected. Note that we only lerp on screen space position.*/
			if (prevVector2Position != newVector2Position)
			{
				currentTime = 0;
				while (currentTime < healthBarPositionLerpTime + Time.fixedDeltaTime)
				{
					expectedVector2Position = Vector2.Lerp(prevVector2Position, newVector2Position, currentTime / healthBarPositionLerpTime);
					healthBar.transform.position = new Vector2(expectedVector2Position.x, expectedVector2Position.y);
					
					yield return new WaitForSeconds(currentTime += Time.fixedDeltaTime);
				}
			}
			
			healthBar.transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	float innerRadius, outerRadius, outerRadiusSqr; Vector2 joyStickCenterPosition, touchPos, centerToTouch; Vector3 joyStickInnerDefaultPosition;
	public delegate void JoyStickMoveEvent(Vector2 value);
	/// <summary>
	/// You can add your custom event here whenever joystick is moved, function will be populated with a vector2
	/// </summary>
	public JoyStickMoveEvent joyStickMoveEvent;
	void HandleJoyStickView()
	{	
		prepareJoyStickStartValue += PrepareJoyStickStartValue;
		joyStickHolder.parent.RegisterCallback<GeometryChangedEvent>((evt) => PrepareValue());

		joyStickOuter.RegisterCallback<PointerDownEvent>((evt) => 
		{
			/* Used to block touch screen swipe event */
			evt.StopPropagation();
			Touch touch = TouchExtension.GetTouchOverlapVisualElement(joyStickOuter, root.panel);
			if (touch.Equals(default)) return;
			touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			// Check if touch inside the circle

			centerToTouch = touchPos - joyStickCenterPosition;
			if (centerToTouch.sqrMagnitude < outerRadiusSqr) StartCoroutine(HandleJoyStick(touch));            
		});
	}
	
	void PrepareJoyStickStartValue()
	{
		outerRadius = joyStickOuter.resolvedStyle.width / 2f;
		outerRadiusSqr = outerRadius * outerRadius;
		innerRadius = joyStickInner.resolvedStyle.width / 2f;
		joyStickInnerDefaultPosition = new Vector3(outerRadius - innerRadius, outerRadius - innerRadius, joyStickInner.transform.position.z);
		joyStickInner.transform.position = joyStickInnerDefaultPosition;
		prepareJoyStickStartValue -= PrepareJoyStickStartValue;
	}

	Action prepareJoyStickStartValue;
	void PrepareValue()
	{
		prepareJoyStickStartValue?.Invoke();
		joyStickCenterPosition = new Vector2(joyStickOuter.worldBound.position.x + outerRadius, joyStickOuter.worldBound.position.y + outerRadius);
	}

	/// <summary>
	/// Handle joystick inner circle movement
	/// </summary>
	/// <param name="touch"></param>
	/// <returns></returns>
	IEnumerator HandleJoyStick(Touch touch)
	{
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			/* Ensure touch is inside the circle */
			centerToTouch *= Math.Min(1f, outerRadius / centerToTouch.magnitude);
			/* Custom event will be executed here*/
			joyStickMoveEvent?.Invoke(centerToTouch);

			/* Make inner circle follow touch position within circle bound */
			joyStickInner.transform.position = joyStickOuter.WorldToLocal
			(
				joyStickCenterPosition + centerToTouch - new Vector2(innerRadius, innerRadius)
			);
 
			yield return new WaitForSeconds(Time.deltaTime);
			touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			centerToTouch = touchPos - joyStickCenterPosition;
		}

		joyStickInner.transform.position = joyStickInnerDefaultPosition;
		joyStickMoveEvent?.Invoke(Vector2.zero);
	}
	
	/// <summary>
	/// Handle clicking/holding attack button
	/// </summary>
	void HandleAttackButton()
	{
		/* Access attack button from predefined index */
		SkillHolderView t_skillHolderView = skillScrollViewUIInfos[GameManager.Instance.attackButtonScrollViewIndex]
		.skillHolderViews[GameManager.Instance.attackButtonIndex];
		
		t_skillHolderView.root.RegisterCallback<PointerDownEvent>((evt) => 
		{
			clickAttackButtonEvent();
			Touch touch = TouchExtension.GetTouchOverlapVisualElement(t_skillHolderView.root, root.panel);
			StartCoroutine(AttackButtonHoldingCoroutine(touch, t_skillHolderView));
		});
	}
	
	IEnumerator AttackButtonHoldingCoroutine(Touch touch, SkillHolderView skillHolderView)
	{
		Vector2 touchPos;
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			/* Direction of holding will be a vector from button middle point to our current touch */
			attackDirection = touchPos - skillHolderView.midPos;
			attackDirection.Scale(inverseY);
			holdAttackButtonEvent(attackDirection);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
}