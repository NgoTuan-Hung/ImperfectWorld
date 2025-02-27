using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : ViewBase
{
	VisualTreeAsset scrollViewLockVTA;
	[SerializeField] private AudioClip scrollSound;
	[SerializeField] private AudioSource audioSource;
	bool optionExpandButtonExpanded = true, charSelectionExpandButtonExpanded = true;
	public delegate void AddSkillToScrollViewDelegate(Touch touch, Vector2 direction);

	public void Init() 
	{
		base.Init();
		snapInterval = snapTime * snapIntervalPortion;
		audioSource.clip = scrollSound;

		HandleExpandButton();
		PopulateOptions();
	}

	public override void InitIndividualView(IndividualView p_individualView, CharUIData p_charUIData)
	{
		HandleUsableHolderScrollView(p_individualView);
		HandleJoyStickView(p_individualView);
		HandleScrollLockExpandLock(p_individualView);
		HandleExpandButton(p_individualView);
	}

	public override void LoadAllTemplate()
	{
		scrollViewLockVTA = Resources.Load<VisualTreeAsset>("ScrollViewLock");
	}

	public VisualElement root, optionExpandButton, options, mainViewLayer, mainViewCharSelectionContainer
	, mainViewCharSelection, mainViewCharSelectionExpandButton;
	public override void GetAllRequiredVisualElements()
	{
		var uiDocument = GetComponent<UIDocument>();
		root = uiDocument.rootVisualElement;
		
		mainViewLayer = gameUIManager.layers[(int)GameUIManager.LayerUse.MainView];
		
		/* Option Views */
		optionExpandButton = mainViewLayer.Q<VisualElement>(name: "main-view__option-expand-button");
		options = mainViewLayer.Q<VisualElement>(name: "main-view__options");
		
		mainViewCharSelectionContainer = mainViewLayer.Q("main-view__char-selection-container");
		mainViewCharSelection = mainViewCharSelectionContainer.Q("main-view__char-selection");
		mainViewCharSelectionExpandButton = mainViewCharSelectionContainer.Q("main-view__char-selection-expand-button");
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
				case "OpenCharInfo": 
				{
					visualElement.RegisterCallback<PointerDownEvent>(evt => 
					{
						gameUIManager.ActivateLayer((int)GameUIManager.LayerUse.Info);
					});
					break;
				}
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

	public void HandleExpandButton()
	{
		optionExpandButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
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
		
		mainViewCharSelectionExpandButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			if (charSelectionExpandButtonExpanded)
			{
				charSelectionExpandButtonExpanded = false;
				mainViewCharSelectionExpandButton.AddToClassList("main-view__expand-button-collapsed");
				mainViewCharSelection.AddToClassList("main-view__char-selection-hide");
			}
			else
			{
				charSelectionExpandButtonExpanded = true;
				mainViewCharSelectionExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				mainViewCharSelection.RemoveFromClassList("main-view__char-selection-hide");
			}
		});
	}
	
	public void HandleExpandButton(IndividualView p_individualView)
	{
		p_individualView.scrollLockExpandButton.RegisterCallback<PointerDownEvent>((evt) =>
		{
			if (p_individualView.lockExpandLocked) return;
			if (p_individualView.scrollLockExpandButtonExpanded)
			{
				p_individualView.scrollLockExpandButtonExpanded = false;
				p_individualView.scrollLockExpandButton.AddToClassList("main-view__expand-button-collapsed");
				p_individualView.scrollLockParent.AddToClassList("scroll-lock-view__lock-parent-collapsed");
			}
			else
			{
				p_individualView.scrollLockExpandButtonExpanded = true;
				p_individualView.scrollLockExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				p_individualView.scrollLockParent.RemoveFromClassList("scroll-lock-view__lock-parent-collapsed");
			}
		});
	}
	
	private void HandleScrollLockExpandLock(IndividualView p_individualView)
	{
		p_individualView.scrollLockExpandLock.RegisterCallback<PointerDownEvent>(evt => 
		{
			if (p_individualView.lockExpandLocked) p_individualView.scrollLockExpandLock.AddToClassList("scroll-lock-view__lock-expand-unlocked");
			else p_individualView.scrollLockExpandLock.RemoveFromClassList("scroll-lock-view__lock-expand-unlocked");
			p_individualView.lockExpandLocked = !p_individualView.lockExpandLocked;
		});
	}

	/// <summary>
	/// Handle scroll view lock
	/// . Lock state: Lock -> Unlocked -> AutoLocked -> Lock -> ...
	/// </summary>
	public void HandleScrollLock(UsableScrollViewUIInfo p_usableScrollViewUIInfo)
	{	
		switch (p_usableScrollViewUIInfo.scrollViewLockState)
		{
			case ScrollViewLockState.Locked:
			{
				p_usableScrollViewUIInfo.scrollViewLock.AddToClassList("scroll-lock-view__lock-unlocked");
				p_usableScrollViewUIInfo.scrollViewLockState = ScrollViewLockState.Unlocked;
				break;
			}
			case ScrollViewLockState.Unlocked:
			{
				p_usableScrollViewUIInfo.scrollViewLock.RemoveFromClassList("scroll-lock-view__lock-unlocked");
				p_usableScrollViewUIInfo.scrollViewLock.AddToClassList("scroll-lock-view__lock-auto-lock");
				p_usableScrollViewUIInfo.scrollViewLockState = ScrollViewLockState.AutoLocked;
				break;
			}
			case ScrollViewLockState.AutoLocked:
			{
				p_usableScrollViewUIInfo.scrollViewLock.RemoveFromClassList("scroll-lock-view__lock-auto-lock");
				p_usableScrollViewUIInfo.scrollViewLockState = ScrollViewLockState.Locked;
				break;
			}
			default: break;
		}
	}
	
	/// <summary>
	/// Add the skill to UI and handle input logic of the skill.
	/// </summary>
	/// <param name="skillDataSO"></param>
	/// <param name="trigger"></param>
	/// <param name="startHoldingCallback"></param>
	/// <param name="stillHoldingCallback"></param>
	public void AddSkillToScrollView(SkillDataSO skillDataSO, AddSkillToScrollViewDelegate trigger
	, Func<bool> startHoldingCallback = null, Action<Vector2> stillHoldingCallback = null)
	{
		// var newSkillHolder = skillHolderTemplate.Instantiate();
		// skillScrollViews[skillDataSO.skillButtonIndex].contentContainer.Add(newSkillHolder);
		// SkillHolderView t_skillHolderView = HandleSkillHolderView(newSkillHolder, skillDataSO.skillButtonIndex);
		
		// /* Add image for skill holder */
		// newSkillHolder.Q<VisualElement>("skill-holder-in").style.backgroundImage = new StyleBackground(skillDataSO.skillImage);
		
		// var skillTooltip = new SkillTooltipView(skillTooltipTemplate.Instantiate(), skillDataSO.skillName, skillDataSO.skillHelperImage, skillDataSO.skillHelperDescription, skillTooltipSS).visualElement;
		// newSkillHolder.GetLayer().Add(skillTooltip);
		// skillTooltip.style.position = new StyleEnum<Position>(Position.Absolute);
		// skillTooltip.style.visibility = Visibility.Hidden;
		// GameUIManager.AddHelper(newSkillHolder.GetHashCode(), skillTooltip);

		// newSkillHolder.AddToClassList("has-helper");
		// newSkillHolder.AddToClassList("helper-type-skill-info");
		
		
		// /* Only allow tooltip for current visible element in scroll view */
		// if (skillScrollViews[skillDataSO.skillButtonIndex].contentContainer.IndexOf(newSkillHolder)
		// != skillScrollViewUIInfos[skillDataSO.skillButtonIndex].SkillScrollViewNewIndex)
		// 	newSkillHolder.AddToClassList("helper-invisible");
		
		// /* Handle event here */
		// switch (skillDataSO.inputType)
		// {
		// 	case SkillDataSo.InputType.Click:
		// 		newSkillHolder.RegisterCallback<PointerDownEvent>((evt) => 
		// 		{
		// 			var touch = TouchExtension.GetTouchOverlapVisualElement(newSkillHolder, root.panel);
		// 			trigger(touch, default);
		// 		});
		// 		break;
		// 	case SkillDataSo.InputType.Hold:
		// 		newSkillHolder.RegisterCallback<PointerDownEvent>((evt) => 
		// 		{
		// 			var touch = TouchExtension.GetTouchOverlapVisualElement(newSkillHolder, root.panel);
		// 			StartCoroutine(SkillHolderHoldCoroutine(touch, t_skillHolderView, trigger));
		// 		});
		// 		break;
		// 	case SkillDataSo.InputType.HoldAndRelease:
		// 		newSkillHolder.RegisterCallback<PointerDownEvent>((evt) => 
		// 		{
		// 			var touch = TouchExtension.GetTouchOverlapVisualElement(newSkillHolder, root.panel);
		// 			StartCoroutine(SkillHolderHoldAndReleaseCoroutine(touch, t_skillHolderView, trigger, startHoldingCallback, stillHoldingCallback));
		// 		});
		// 		break;
		// 	default:
		// 		break;
		// }
	}
	
	/// <summary>
	/// Handle holding the skill holder button and trigger action at releasing.
	/// </summary>
	/// <param name="touch"></param>
	/// <param name="skillHolderView"></param>
	/// <param name="trigger"></param>
	/// <returns></returns>
	IEnumerator SkillHolderHoldAndReleaseCoroutine(TouchInfo p_touchInfo, SkillHolderView skillHolderView, AddSkillToScrollViewDelegate trigger,
	Func<bool> startHoldingCallback, Action<Vector2> stillHoldingCallback)
	{
		/* Direction will be a vector from button middle point to our current touch */
		Vector2 t_direction = p_touchInfo.panelPosition - skillHolderView.midPos;
		t_direction.Scale(VectorExtension.inverseY);
		
		startHoldingCallback?.Invoke();
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{
			p_touchInfo.UpdateSelf();
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			t_direction = p_touchInfo.panelPosition - skillHolderView.midPos;
			t_direction.Scale(VectorExtension.inverseY);
			
			stillHoldingCallback?.Invoke(t_direction);
		}
		
		trigger(p_touchInfo.touch, t_direction);
	}
	
	/// <summary>
	/// Handle triggering action while we still holding the button.
	/// </summary>
	/// <param name="touch"></param>
	/// <param name="skillHolderView"></param>
	/// <param name="trigger"></param>
	/// <returns></returns>
	IEnumerator SkillHolderHoldCoroutine(TouchInfo p_touchInfo, SkillHolderView skillHolderView, AddSkillToScrollViewDelegate trigger)
	{
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{
			p_touchInfo.UpdateSelf();

			Vector2 t_direction = p_touchInfo.panelPosition - skillHolderView.midPos;
			t_direction.Scale(VectorExtension.inverseY);
			trigger(p_touchInfo.touch, t_direction);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	public void InitUsableHolder(IndividualView p_individualView, VisualTreeAsset p_usableHolderVTA, UsableSlotUIInfo p_usableSlotUIInfo, Texture2D p_usableHolderImage)
	{
		VisualElement t_usableHolder = p_usableHolderVTA.Instantiate();
		t_usableHolder = t_usableHolder.ElementAt(0);
		t_usableHolder.Q(classes: "usable-holder-in").style.backgroundImage = new StyleBackground(p_usableHolderImage);
		t_usableHolder.style.position = Position.Absolute;
		t_usableHolder.style.visibility = Visibility.Hidden;
		t_usableHolder.transform.position = VectorExtension.veryFar;
		p_individualView.usableScrollViewHolder.Add(t_usableHolder);
		p_usableSlotUIInfo.usableHolder = t_usableHolder;

		t_usableHolder.RegisterCallback<PointerMoveEvent>((evt) => 
		{
		    // 		/* Check When locked is set to true, lock the scroll view. Also scroll happen at 
			// 		scrollview.contentContainer.PointerMoveEvent(Bubble Up phase) so we can block it here */
			if (p_individualView.usableScrollViewUIInfos[p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex].scrollViewLockState == ScrollViewLockState.Locked)
				evt.StopPropagation();
		});
	}
	
	public void AddUsableHolderToScrollView(IndividualView p_individualView, UsableSlotUIInfo p_usableSlotUIInfo, int addIndex)
	{
		p_usableSlotUIInfo.usableHolder.style.position = Position.Relative;
		p_usableSlotUIInfo.usableHolder.style.visibility = Visibility.Visible;
		p_usableSlotUIInfo.usableHolder.transform.position = Vector3.zero;
		try
		{
			p_individualView.usableHolderScrollViews[p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex].Insert(addIndex, p_usableSlotUIInfo.usableHolder);
		}
		catch (Exception)
		{
			p_individualView.usableHolderScrollViews[p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex].Add(p_usableSlotUIInfo.usableHolder);
		}
	}
	
	public void HideUsableHolder(IndividualView p_individualView, VisualElement p_usableHolder)
	{
		p_usableHolder.style.position = Position.Absolute;
		p_usableHolder.style.visibility = Visibility.Hidden;
		p_usableHolder.transform.position = VectorExtension.veryFar;
		p_individualView.usableScrollViewHolder.Add(p_usableHolder);
	}
	
	/// <summary>
	/// Populate the skill slots info
	/// </summary>
	public void HandleUsableHolderScrollView(IndividualView p_individualView)
	{	
		/* Handle scroll logic, scrolling, snapping */
		for (int i=0;i<p_individualView.usableHolderScrollViews.Count;i++)
		{
			/* For each scroll view, we assign a lock to it */
			UsableScrollViewUIInfo t_usableScrollViewUIInfo = new
			(
				p_individualView.usableHolderScrollViews[i], null, scrollViewLockVTA.Instantiate().ElementAt(0), ScrollViewLockState.Locked
			);

			t_usableScrollViewUIInfo.scrollViewLock.RegisterCallback<PointerDownEvent>((evt) => 
			{
				evt.StopPropagation();
				HandleScrollLock(t_usableScrollViewUIInfo);
			});
			
			p_individualView.scrollLockParent.Insert(i, t_usableScrollViewUIInfo.scrollViewLock.parent);

			p_individualView.usableHolderScrollViews[i].verticalScroller.valueChanged += evt => UsableScrollViewValueChanged(t_usableScrollViewUIInfo);
			
			p_individualView.usableHolderScrollViews[i].RegisterCallback<PointerDownEvent>((evt) => 
			{
				UsableScrollViewPointerDown(evt.position, t_usableScrollViewUIInfo);
			}, TrickleDown.TrickleDown);
			
			/* Used to determine some final style of scroll view (height,...)*/
			p_individualView.usableHolderScrollViews[i].RegisterCallback<GeometryChangedEvent>
			(
				(evt) => SkillScrollViewGeometryChanged(t_usableScrollViewUIInfo)
			);

			p_individualView.usableScrollViewUIInfos.Add(t_usableScrollViewUIInfo);
		}
	}
	
	/// <summary>
	/// Mainly handle blocking scroll event and cache reference to skill holder with some
	/// additional infos
	/// </summary>
	/// <param name="skillHolder"></param>
	/// <param name="scrollViewIndex"></param>
	// SkillHolderView HandleSkillHolderView(VisualElement skillHolder, int scrollViewIndex)
	// {
	// 	/* We will mainly use this for referencing the middle point of the scroll view, which is
	// 	very important for dealing with event like touching, holding a skill, middle point will
	// 	be updated at ScrollView GeometryChanged event */
	// 	SkillHolderView skillHolderView = new SkillHolderView(skillHolder, skillScrollViews[scrollViewIndex]);
	// 	skillHolder.RegisterCallback<PointerMoveEvent>((evt) => 
	// 	{
	// 		/* Check When locked is set to true, lock the scroll view. Also scroll happen at 
	// 		scrollview.contentContainer.PointerMoveEvent(Bubble Up phase) so we can block it here */
	// 		if (skillScrollViewUIInfos[scrollViewIndex].ScrollViewLockState == ScrollViewLockState.Locked) evt.StopPropagation();
	// 	});
		
	// 	skillScrollViewUIInfos[scrollViewIndex].skillHolderViews.Add(skillHolderView);
	// 	return skillHolderView;
	// }

	/// <summary>
	/// Mostly used to play sound if scroll view scroll passed an element
	/// </summary>
	/// <param name="skillScrollViewUIInfo"></param>
	public void UsableScrollViewValueChanged(UsableScrollViewUIInfo usableScrollViewUIInfo)
	{
		usableScrollViewUIInfo.newChildIndex = (int)Math.Floor(usableScrollViewUIInfo.scrollView.verticalScroller.value / usableScrollViewUIInfo.scrollViewHeight + 0.5f);
		if (usableScrollViewUIInfo.newChildIndex != usableScrollViewUIInfo.previousChildIndex) audioSource.Play();

		usableScrollViewUIInfo.previousChildIndex = usableScrollViewUIInfo.newChildIndex;
	}

	public void UsableScrollViewPointerDown(Vector2 p_pointerPosition, UsableScrollViewUIInfo p_usableScrollViewUIInfo)
	{
		if (p_usableScrollViewUIInfo.scrollViewLockState == ScrollViewLockState.Locked) return;
		if (p_usableScrollViewUIInfo.scrollSnapCoroutine != null) StopCoroutine(p_usableScrollViewUIInfo.scrollSnapCoroutine);
		p_usableScrollViewUIInfo.scrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
		TouchInfo t_touchInfo = TouchExtension.GetTouchInfoAt(p_pointerPosition, gameUIManager.root);
		p_usableScrollViewUIInfo.scrollSnapCoroutine = StartCoroutine(HandleScrollSnap(t_touchInfo, p_usableScrollViewUIInfo));
	}
	
	void SkillScrollViewGeometryChanged(UsableScrollViewUIInfo p_usableScrollViewUIInfo)
	{
		// var t_skillScrollView = p_usableScrollViewUIInfo.ScrollView;
		p_usableScrollViewUIInfo.scrollViewHeight = p_usableScrollViewUIInfo.scrollView.resolvedStyle.height;
		p_usableScrollViewUIInfo.distanceToSnap = p_usableScrollViewUIInfo.scrollViewHeight * distanceToSnapScale;
		
		/* Update mid position of all skill holders in this scroll view */
		// skillScrollViewUIInfo.skillHolderViews.ForEach(skillHolderView => 
		// {
		// 	skillHolderView.midPos = t_skillScrollView.worldBound.position + t_skillScrollView.worldBound.size / 2;
		// });
	}

	[SerializeField] private float snapTime = 0.3f;
	[SerializeField] private float snapIntervalPortion = 0.1f;
	private float snapInterval;
	[SerializeField] private float distanceToSnapScale = 0.5f;
	private float defaultScrollDecelerationRate = 0.135f;

	public IEnumerator HandleScrollSnap(TouchInfo p_touchInfo, UsableScrollViewUIInfo p_usableScrollViewUIInfo)
	{
		/* snap logic only happens when we release the touch */
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{
			p_touchInfo.UpdateSelf();
			
			yield return new WaitForSeconds(Time.deltaTime);
		}

		float prevPosition = float.MaxValue; 
		float finalPosition, currentPosition;
		int finalIndex;

		/* snap logic only happens when the scroll speed is low enough */
		while (Math.Abs(p_usableScrollViewUIInfo.scrollView.verticalScroller.value - prevPosition) > p_usableScrollViewUIInfo.distanceToSnap)
		{
			prevPosition = p_usableScrollViewUIInfo.scrollView.verticalScroller.value;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		} p_usableScrollViewUIInfo.scrollView.scrollDecelerationRate = 0f;

		/* snap logic:
		- Grab the element that the center of the scroll view is inside
		- Lerp from the current scroll position to the element's position
		- Snap to the element for more accurate snapping (Use unity internal function ScrollTo)
		 */
		currentPosition = p_usableScrollViewUIInfo.scrollView.verticalScroller.value;
		finalIndex = (int)Math.Floor(p_usableScrollViewUIInfo.scrollView.verticalScroller.value/p_usableScrollViewUIInfo.scrollViewHeight + 0.5f);
		finalPosition = finalIndex * p_usableScrollViewUIInfo.scrollViewHeight;

		float currentTime = 0, progress = 0;
		while (progress < 1.0)
		{
			progress = currentTime / snapTime;
			p_usableScrollViewUIInfo.scrollView.verticalScroller.value = Mathf.Lerp(currentPosition, finalPosition, progress);
			yield return new WaitForSeconds(snapInterval);
			currentTime += snapInterval;
		}
		p_usableScrollViewUIInfo.scrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
		p_usableScrollViewUIInfo.scrollView.ScrollTo(p_usableScrollViewUIInfo.scrollView.contentContainer.ElementAt(finalIndex));
		
		/* If we choose auto lock scroll view, we can handle it here, right after scrolling and snapping
		is done */
		if (p_usableScrollViewUIInfo.scrollViewLockState == ScrollViewLockState.AutoLocked) HandleScrollLock(p_usableScrollViewUIInfo);
	}

	public void HandleJoyStickView(IndividualView p_individualView)
	{
		p_individualView.joyStickOuter.RegisterCallback<GeometryChangedEvent>((evt) => 
		{
			PrepareValue(p_individualView);
		});

		p_individualView.joyStickInner.RegisterCallback<GeometryChangedEvent>((evt) => 
		{
			PrepareValue(p_individualView);
		});

		
		p_individualView.joyStickOuter.RegisterCallback<PointerDownEvent>((evt) => 
		{
			TouchInfo t_touchInfo = TouchExtension.GetTouchInfoAt(evt.position, gameUIManager.root);
			StartCoroutine(MoveJoystick(p_individualView, t_touchInfo));
		});
	}

	public void PrepareValue(IndividualView p_individualView)
	{
		p_individualView.outerRadius = p_individualView.joyStickOuter.resolvedStyle.width / 2f;
		p_individualView.outerRadiusSqr = p_individualView.outerRadius * p_individualView.outerRadius;
		p_individualView.joyStickCenterPosition = new Vector2(p_individualView.joyStickOuter.worldBound.position.x + p_individualView.outerRadius, p_individualView.joyStickOuter.worldBound.position.y + p_individualView.outerRadius);
		p_individualView.innerRadius = p_individualView.joyStickInner.resolvedStyle.width / 2f;
		p_individualView.joyStickInnerDefaultPosition = new Vector3(p_individualView.outerRadius - p_individualView.innerRadius, p_individualView.outerRadius - p_individualView.innerRadius, p_individualView.joyStickInner.transform.position.z);
		p_individualView.joyStickInner.transform.position = p_individualView.joyStickInnerDefaultPosition;
		p_individualView.joystickInnerOffset = new Vector2(p_individualView.innerRadius, p_individualView.innerRadius);
	}

	/// <summary>
	/// Handle joystick inner circle movement
	/// </summary>
	/// <param name="p_individualView"></param>
	/// <param name="p_pointerPosition"></param>
	public IEnumerator MoveJoystick(IndividualView p_individualView, TouchInfo p_touchInfo)
	{
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{
			p_touchInfo.UpdateSelf();

			p_individualView.centerToTouch = p_touchInfo.panelPosition - p_individualView.joyStickCenterPosition;
			/* Ensure touch is inside the circle */
			p_individualView.centerToTouch *= Math.Min(1f, p_individualView.outerRadius / p_individualView.centerToTouch.magnitude);
			/* Custom event will be executed here*/
			p_individualView.joyStickMoveEvent?.Invoke(p_individualView.centerToTouch);

			/* Make inner circle follow touch position within circle bound */
			p_individualView.joyStickInner.transform.position = p_individualView.joyStickOuter.WorldToLocal
			(
				p_individualView.joyStickCenterPosition + p_individualView.centerToTouch - p_individualView.joystickInnerOffset
			);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		p_individualView.joyStickInner.transform.position = p_individualView.joyStickInnerDefaultPosition;
	}
	
	/// <summary>
	/// Handle clicking/holding attack button
	/// </summary>
	// void HandleAttackButton()
	// {
	// 	/* Access attack button from predefined index */
	// 	SkillHolderView t_skillHolderView = skillScrollViewUIInfos[GameManager.Instance.attackButtonScrollViewIndex]
	// 	.skillHolderViews[GameManager.Instance.attackButtonIndex];
		
	// 	t_skillHolderView.root.RegisterCallback<PointerDownEvent>((evt) => 
	// 	{
	// 		clickAttackButtonEvent();
	// 		Touch touch = TouchExtension.GetTouchOverlapVisualElement(t_skillHolderView.root, root.panel);
	// 		StartCoroutine(AttackButtonHoldingCoroutine(touch, t_skillHolderView));
	// 	});
	// }
	
	// IEnumerator AttackButtonHoldingCoroutine(Touch touch, SkillHolderView skillHolderView)
	// {
	// 	Vector2 touchPos;
	// 	while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
	// 	{
	// 		touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
	// 		/* Direction of holding will be a vector from button middle point to our current touch */
	// 		attackDirection = touchPos - skillHolderView.midPos;
	// 		attackDirection.Scale(inverseY);
	// 		holdAttackButtonEvent(attackDirection);
	// 		yield return new WaitForSeconds(Time.fixedDeltaTime);
	// 	}
	// }
}