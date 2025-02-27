using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IndividualView
{
	public bool scrollLockExpandButtonExpanded = true, lockExpandLocked = true;
	GameUIManager gameUIManager;
	public float innerRadius, outerRadius, outerRadiusSqr;
	public Vector2 joyStickCenterPosition, centerToTouch, joystickInnerOffset;
	public Vector3 joyStickInnerDefaultPosition;
	public delegate void JoyStickMoveEvent(Vector2 value);
	public int currentSkillTree = 0;
	/// <summary>
	/// You can add your custom event here whenever joystick is moved, function will be populated with a vector2
	/// </summary>
	public JoyStickMoveEvent joyStickMoveEvent;
	public void Init(CharUIData p_charUIData, Action p_switchCharAction)
	{
		gameUIManager = GameUIManager.Instance;
		HandleCharSelection(p_charUIData, p_switchCharAction);
	}
	
	public List<VisualElement> usableSlotScrollView, skillTrees = new(), movableElements = new();
	/* Main view */
	public List<ScrollView> usableHolderScrollViews;
	public List<UsableScrollViewUIInfo> usableScrollViewUIInfos = new();
	public VisualElement usableScrollViewHolder, scrollLockParent, scrollLockExpandButton, scrollLockExpandLock, joystickHolder, joyStickOuter, joyStickInner
	
	/* Char info view */
	, charInfoContentSkillItem, charInfoContentStat, charInfoSkillItem, skillTreeNext, skillTreePrevious, charInfoItemSkillSlot, inventory, charInfoItemAccessories
	, accessoriesLabel, charInfoAccessories;
	
	public void GetAllRequiredVisualElements()
	{
		/* Main view */
		usableScrollViewHolder = gameUIManager.usableScrollViewHolderVTA.Instantiate().ElementAt(0);
		usableHolderScrollViews = usableScrollViewHolder.Query<ScrollView>(classes: "main-view__skill-scroll-view").ToList();
		scrollLockParent = usableScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__lock-parent");
		scrollLockExpandButton = usableScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__expand-button");
		scrollLockExpandLock = scrollLockParent.Q<VisualElement>(name: "scroll-lock-view__lock-expand");
		
		joystickHolder = gameUIManager.joystickVTA.Instantiate().ElementAt(0);
		joyStickOuter = joystickHolder.ElementAt(0);
		joyStickInner = joyStickOuter.ElementAt(0);
		
		gameUIManager.layers[(int)GameUIManager.LayerUse.MainView].Add(usableScrollViewHolder);
		gameUIManager.layers[(int)GameUIManager.LayerUse.MainView].Add(joystickHolder);
		
		/* Char info view */
		charInfoContentSkillItem = gameUIManager.contentSkillItemVTA.Instantiate().ElementAt(0);
		charInfoContentStat = gameUIManager.contentStatVTA.Instantiate().ElementAt(0);
		
		charInfoContentSkillItem.transform.position = VectorExtension.veryFar;
		charInfoContentSkillItem.style.visibility = Visibility.Hidden;
		charInfoContentStat.transform.position = VectorExtension.veryFar;
		charInfoContentStat.style.visibility = Visibility.Hidden;
		
		charInfoSkillItem = charInfoContentSkillItem.Q("char-info__skill-item");
		charInfoItemSkillSlot = charInfoSkillItem.Q("char-info__item-skill-slot");
		
		charInfoItemAccessories = charInfoSkillItem.Q("char-info__item-accessories");
		accessoriesLabel = charInfoItemAccessories.Q("char-info__accessories-label");
		charInfoAccessories = charInfoItemAccessories.Q("char-info__accessories");
		
		skillTreeNext = charInfoSkillItem.Q( "skill-tree__next");
		skillTreePrevious = charInfoSkillItem.Q( "skill-tree__previous");
		
		usableSlotScrollView = charInfoItemSkillSlot.Query(classes: "usable-slot-scrollview").ToList();
		inventory = charInfoItemSkillSlot.Q("inventory");
		
		gameUIManager.charInfoView.root.Add(charInfoContentSkillItem);
		gameUIManager.charInfoView.root.Add(charInfoContentStat);
		
		/* Config view */
		movableElements.AddRange(usableScrollViewHolder.Query(classes: "dynamic-ui__movable").ToList());
		movableElements.Add(joyStickOuter);
	}
	
	public void HandleCharSelection(CharUIData p_charUIData, Action p_switchCharAction)
	{
		VisualElement t_charSelectionChar = gameUIManager.charSelectionCharVTA.Instantiate().ElementAt(0);
		t_charSelectionChar.Q(classes: "char-selection__char-image").style.backgroundImage = p_charUIData.charImage;
		t_charSelectionChar.Q<CustomProgressUI>(classes: "char-selection__char-hp").Progress = p_charUIData.currentHP;
		t_charSelectionChar.Q<CustomProgressUI>(classes: "char-selection__char-xp").Progress = p_charUIData.currentXP;
		t_charSelectionChar.Q<Label>(classes: "char-selection__char-info").text = p_charUIData.name + " - LV" + p_charUIData.currentLevel;
		
		gameUIManager.mainView.mainViewCharSelection.Add(t_charSelectionChar);
		t_charSelectionChar.RegisterCallback<PointerDownEvent>((evt) => p_switchCharAction());
	}
	
	public void Show()
	{
		joystickHolder.transform.position = Vector2.zero;
		joystickHolder.style.visibility = Visibility.Visible;
		
		usableScrollViewHolder.transform.position = Vector2.zero;
		usableScrollViewHolder.style.visibility = Visibility.Visible;
		
		movableElements.ForEach(moveableElement => gameUIManager.FireGeometryChangedEvent(moveableElement));
	}
	
	public void Hide()
	{
		joystickHolder.transform.position = VectorExtension.veryFar;
		joystickHolder.style.visibility = Visibility.Hidden;
		
		usableScrollViewHolder.transform.position = VectorExtension.veryFar;
		usableScrollViewHolder.style.visibility = Visibility.Hidden;
		
		movableElements.ForEach(moveableElement => gameUIManager.FireGeometryChangedEvent(moveableElement));
	}
}