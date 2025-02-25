using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CharInfoView : ViewBase
{
	UIDocument uIDocument;
	VisualTreeAsset activeSkillVTA, passiveSkillVTA, skillHolderVTA, skillHolderSpecialVTA, skillTreeVTA
	, skillSlotVTA, skillTooltipVTA, itemSlotVTA, itemSlotUsableVTA, itemHolderVTA, itemTooltipVTA;
	Vector2 skillSlotDragOffset;
	Dictionary<int, UsableSlotUIInfo> usableSlotUIInfoDict = new();
	int skillTreeNumPointerMoving = 0;

	public override void Init()
	{
		base.Init();
		
		charInfoExitButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			gameUIManager.DeactivateLayer((int)GameUIManager.LayerUse.Info);
		});
		
		HandleCharInfoMenu();
		
		skillSlotDragOffset = gameUIManager.skillSlotSize / 2;
	}

	public override void InitIndividualView(IndividualView p_individualView, CharUIData p_charUIData)
	{
		ChangeSkillTreeHandler(p_individualView);
		InventorySwipeBlocking(p_individualView);
		PlayAccessoriesLabelAnimation(p_individualView);
		
		PopulateItem(p_individualView, p_charUIData);
	}
	
	public override void LoadAllTemplate()
	{
		activeSkillVTA = Resources.Load<VisualTreeAsset>("ActiveSkill");
		passiveSkillVTA = Resources.Load<VisualTreeAsset>("PassiveSkill");
		skillTreeVTA = Resources.Load<VisualTreeAsset>("SkillTree");
		skillSlotVTA = Resources.Load<VisualTreeAsset>("SkillSlot");
		skillTooltipVTA = Resources.Load<VisualTreeAsset>("SkillTooltip");
		skillHolderVTA = Resources.Load<VisualTreeAsset>("SkillHolder");
		skillHolderSpecialVTA = Resources.Load<VisualTreeAsset>("SkillHolderSpecial");
		itemSlotVTA = Resources.Load<VisualTreeAsset>("ItemSlot");
		itemSlotUsableVTA = Resources.Load<VisualTreeAsset>("ItemSlotUsable");
		itemHolderVTA = Resources.Load<VisualTreeAsset>("ItemHolder");
		itemTooltipVTA = Resources.Load<VisualTreeAsset>("ItemTooltip");
	}

	public VisualElement root, charInfoContentUpgrade, charInfoExitButton, currentCharInfoContent;
	ScrollView  charInfoMenu;
	public override void GetAllRequiredVisualElements()
	{
		uIDocument = GetComponent<UIDocument>();
		root = uIDocument.rootVisualElement.Q<VisualElement>(name: "char-info__root");
		charInfoMenu = root.Q<ScrollView>("char-info__menu");
		charInfoContentUpgrade = root.Q("char-info__content-upgrade");
		// skillTreeContainer = root.Q<VisualElement>(name: "skill-tree__container");
		
		charInfoExitButton = root.Q(classes: "exit-button__black");
	}
	
	void PlayAccessoriesLabelAnimation(IndividualView p_individualView)
	{
		p_individualView.accessoriesLabel.schedule.Execute(() => p_individualView.accessoriesLabel.ToggleInClassList("char-info__accessories-label-morph")).Every(3000);
	}
	
	void HandleCharInfoMenu()
	{
		List<CharInfoMenuItem> charInfoMenuItems = Resources.LoadAll<CharInfoMenuItem>("UI/CharInfoData").ToList();
		currentCharInfoContent = charInfoContentUpgrade;
		charInfoMenuItems.ForEach(charInfoMenuItem => 
		{
			Label t_charInfoMenuItem = new(charInfoMenuItem.functionality);
			t_charInfoMenuItem.AddToClassList("char-info__menu-item");
			
			Action changeCurrentCharInfoContent = () => {};
			
			switch (charInfoMenuItem.functionality)
			{
				case "Skill": changeCurrentCharInfoContent = () => currentCharInfoContent = gameUIManager.currentActiveIndividualView.charInfoContentSkillItem;
					break;
				case "Stat": changeCurrentCharInfoContent = () => currentCharInfoContent = gameUIManager.currentActiveIndividualView.charInfoContentStat;
					break;
				case "Upgrade": changeCurrentCharInfoContent = () => currentCharInfoContent = charInfoContentUpgrade;
					break;
				default:
					break;
			}
			
			t_charInfoMenuItem.RegisterCallback<PointerDownEvent>((evt) => SetCharInfoContent(changeCurrentCharInfoContent));
			
			
			charInfoMenu.Add(t_charInfoMenuItem);
		});
	}
	
	public void SetCharInfoContent(Action p_changeCurrentCharInfoContent)
	{
		currentCharInfoContent.style.visibility = Visibility.Hidden;
		currentCharInfoContent.transform.position = VectorExtension.veryFar;
		p_changeCurrentCharInfoContent();
		currentCharInfoContent.style.visibility = Visibility.Visible;
		currentCharInfoContent.transform.position = Vector3.zero;
	}
	
	public void ResetCharInfoContent() => SetCharInfoContent(() => currentCharInfoContent = charInfoContentUpgrade);
	
	/// <summary>
	/// Handle changing skill tree when we have multiple skill trees
	/// (due to skill tree is out of free space and can not put any 
	/// more skill into it), we can jump between next or previous
	/// skill tree.
	/// </summary>
	void ChangeSkillTreeHandler(IndividualView p_individualView)
	{
		p_individualView.skillTreeNext.RegisterCallback<PointerDownEvent>((evt) => 
		{
			p_individualView.skillTrees[p_individualView.currentSkillTree].style.visibility = Visibility.Hidden;
			p_individualView.skillTrees[p_individualView.currentSkillTree].transform.position = VectorExtension.veryFar;
			p_individualView.currentSkillTree = (p_individualView.currentSkillTree + 1) % p_individualView.skillTrees.Count;
			p_individualView.skillTrees[p_individualView.currentSkillTree].style.visibility = Visibility.Visible;
			p_individualView.skillTrees[p_individualView.currentSkillTree].transform.position = Vector3.zero;
		});

		p_individualView.skillTreePrevious.RegisterCallback<PointerDownEvent>((evt) =>
		{
			p_individualView.skillTrees[p_individualView.currentSkillTree].style.visibility = Visibility.Hidden;
			p_individualView.skillTrees[p_individualView.currentSkillTree].transform.position = VectorExtension.veryFar;
			p_individualView.currentSkillTree = (p_individualView.currentSkillTree + p_individualView.skillTrees.Count - 1) % p_individualView.skillTrees.Count;
			p_individualView.skillTrees[p_individualView.currentSkillTree].style.visibility = Visibility.Visible;
			p_individualView.skillTrees[p_individualView.currentSkillTree].transform.position = Vector3.zero;
		});
	}
	
	void InventorySwipeBlocking(IndividualView p_individualView)
	{
		p_individualView.inventory.RegisterCallback<PointerMoveEvent>((evt) => 
		{
			if (skillTreeNumPointerMoving != 0) evt.StopPropagation();
		});
	}
	
	
	public void PopulateSkillTree(IndividualView p_individualView, List<SkillUIData> p_skillUIDatas, List<SkillData> p_skillDatas)
	{
		/* Calculate how many skill can we put into a skill tree in a row or column taking offset into account
		, skill tree size and skill size must be defined inside GameUIManager whenever you change its size in 
		UI Builder. */
		int maxSkillPerRow = (int)gameUIManager.skillTreeContainerSize.x / (int)(gameUIManager.skillTreeSkillSize.x + gameUIManager.skillTreeSkillOffset.x)
		, maxSkillPerCol = (int)gameUIManager.skillTreeContainerSize.y / (int)(gameUIManager.skillTreeSkillSize.y + gameUIManager.skillTreeSkillOffset.y)
		, colOffset = 0, col;
		
		VisualElement t_skillTree = skillTreeVTA.Instantiate().ElementAt(0)
		, t_skillTreeContainer = t_skillTree.Q<VisualElement>(classes: "skill-tree__container");
		NewSkillTreeHandler(p_individualView, t_skillTree, t_skillTreeContainer);
		
		for (int i=0;i<p_skillUIDatas.Count;i++)
		{				
			SkillData t_skillData = p_skillDatas[i];

			VisualElement t_skill, skillTooltip = HandleTooltipView
			(
				p_individualView, skillTooltipVTA.Instantiate().ElementAt(0), p_skillUIDatas[i].skillName
				, p_skillUIDatas[i].skillHelperImage, p_skillUIDatas[i].skillHelperDescription
			);
			
			if (p_skillUIDatas[i].skillType == SkillType.Passive)
			{
				t_skill = passiveSkillVTA.Instantiate().ElementAt(0);
			}
			else
			{
				VisualElement t_skillSlot = skillSlotVTA.Instantiate();
				t_skillSlot = t_skillSlot.ElementAt(0);
				t_skillSlot.Q(classes: "usable-slot-in").style.backgroundImage = p_skillUIDatas[i].skillImage;
				t_skillSlot.style.visibility = Visibility.Hidden;
				gameUIManager.AddTooltipHandlerForVisualElement(t_skillSlot, skillTooltip);
				p_individualView.charInfoSkillItem.Add(t_skillSlot);
				
				SkillUIInfo t_skillUIInfo = new() {skillUIData = p_skillUIDatas[i], usableSlotUIInfo = new(t_skillSlot)};
				usableSlotUIInfoDict.Add(t_skillSlot.GetHashCode(), t_skillUIInfo.usableSlotUIInfo);
				
				if (p_skillUIDatas[i].skillType == SkillType.Special) t_skillUIInfo.usableHolderVTA = skillHolderSpecialVTA;
				else t_skillUIInfo.usableHolderVTA = skillHolderVTA;
				
				t_skill = activeSkillVTA.Instantiate().ElementAt(0);
				gameUIManager.mainView.InitUsableHolder
				(
					p_individualView, t_skillUIInfo.usableHolderVTA, t_skillUIInfo.usableSlotUIInfo
					, t_skillUIInfo.skillUIData.skillImage
				);
				
				t_skill.RegisterCallback<PointerDownEvent>((evt) => 
				{
					TouchInfo touchInfo = TouchExtension.GetTouchInfoAt(evt.position, uIDocument.rootVisualElement);
					StartCoroutine(DraggingUsableSlotCoroutine(p_individualView, touchInfo, t_skillUIInfo.usableSlotUIInfo
					, (p_scrollViewIndex, p_dropIntoScrollViewIndex) => 
					{
					    t_skillData.currentUsableScrollViewIndex = p_scrollViewIndex;
						t_skillData.dropIntoScrollViewIndex = p_dropIntoScrollViewIndex;
					}));
				});

				t_skillData.usableSlotUIInfo = t_skillUIInfo.usableSlotUIInfo;
			}
			
			gameUIManager.AddTooltipHandlerForVisualElement(t_skill, skillTooltip);
			
			t_skill.Q(classes: "skill-tree__skill-icon").style.backgroundImage = p_skillUIDatas[i].skillTreeIcon;
			t_skill.style.left = (i % maxSkillPerRow) * (gameUIManager.skillTreeSkillSize.x + gameUIManager.skillTreeSkillOffset.x) + gameUIManager.skillTreeSkillOffset.x;
			col = (i / maxSkillPerRow) - colOffset;
			
			if (col + 1 > maxSkillPerCol)
			{
				t_skillTree = skillTreeVTA.Instantiate().ElementAt(0);
				t_skillTree.style.visibility = Visibility.Hidden;
				t_skillTree.transform.position = VectorExtension.veryFar;
				t_skillTreeContainer = t_skillTree.Q<VisualElement>(classes: "skill-tree__container");
				NewSkillTreeHandler(p_individualView, t_skillTree, t_skillTreeContainer);
				colOffset += maxSkillPerCol;
				col -= maxSkillPerCol;
			}
			
			t_skill.style.top = col * (gameUIManager.skillTreeSkillSize.y + gameUIManager.skillTreeSkillOffset.y) + gameUIManager.skillTreeSkillOffset.y;
			
			t_skillTreeContainer.Add(t_skill);
		}
	}

	void PopulateItem(IndividualView p_individualView, CharUIData p_charUIData)
	{
		long fixedDeltaTimeInMilli = (long)(Time.fixedDeltaTime * 1000);
		List<ItemData> itemDatas = p_charUIData.itemDatas;
		
		for (int i=0;i<itemDatas.Count;i++)
		{
			VisualElement t_item, t_itemSlot;
			ItemUIInfo t_itemUIInfo;
			
			VisualElement itemTooltip = HandleTooltipView
			(
				p_individualView, skillTooltipVTA.Instantiate().ElementAt(0), itemDatas[i].itemName
				, itemDatas[i].itemHelperImage, itemDatas[i].itemHelperDescription
			);

			t_itemUIInfo = new();
			
			if (itemDatas[i].itemType == ItemType.Usable)
			{
				t_itemUIInfo.slotVTA = itemSlotUsableVTA;
				t_itemUIInfo.usableHolderVTA = itemHolderVTA;
				t_itemUIInfo.itemData = itemDatas[i];
				t_itemSlot = t_itemUIInfo.slotVTA.Instantiate().ElementAt(0);
				t_itemSlot.Q(classes: "item-slot-in").style.backgroundImage = t_itemUIInfo.itemData.itemImage;
				t_itemSlot.style.visibility = Visibility.Hidden;
				t_itemUIInfo.usableSlotUIInfo = new(t_itemSlot);
				p_individualView.charInfoSkillItem.Add(t_itemSlot);
				gameUIManager.AddTooltipHandlerForVisualElement(t_itemSlot, itemTooltip);
				
				t_item = t_itemUIInfo.slotVTA.Instantiate().ElementAt(0);
				t_item.Q(classes: "item-slot-in").style.backgroundImage = t_itemUIInfo.itemData.itemImage;
				
				usableSlotUIInfoDict.Add(t_itemSlot.GetHashCode(), t_itemUIInfo.usableSlotUIInfo);
				
				gameUIManager.mainView.InitUsableHolder
				(
					p_individualView, itemHolderVTA, t_itemUIInfo.usableSlotUIInfo, t_itemUIInfo.itemData.itemImage
				);
				
				t_item.RegisterCallback<PointerDownEvent>((evt) => 
				{
					TouchInfo touchInfo = TouchExtension.GetTouchInfoAt(evt.position, uIDocument.rootVisualElement);
					StartCoroutine(DraggingUsableSlotCoroutine(p_individualView, touchInfo, t_itemUIInfo.usableSlotUIInfo, (i1, i2) => {}));
				});
			}
			else
			{	
				t_itemUIInfo.slotVTA = itemSlotVTA;
				t_itemUIInfo.itemData = itemDatas[i];
				t_itemSlot = t_itemUIInfo.slotVTA.Instantiate().ElementAt(0);
				t_itemSlot.Q(classes: "item-slot-in").style.backgroundImage = itemDatas[i].itemImage;
				t_itemSlot.transform.position = VectorExtension.veryFar;
				t_itemSlot.style.visibility = Visibility.Hidden;
				t_itemSlot.style.position = Position.Absolute;
				t_itemSlot.RegisterCallback<PointerDownEvent>((evt) => 
				{
					t_itemSlot.transform.position = VectorExtension.veryFar;
					t_itemSlot.style.visibility = Visibility.Hidden;
					t_itemSlot.style.position = Position.Absolute;
					p_individualView.charInfoSkillItem.Add(t_itemSlot);
				});
				p_individualView.charInfoSkillItem.Add(t_itemSlot);
				
				t_item = t_itemUIInfo.slotVTA.Instantiate().ElementAt(0);
				t_item.Q(classes: "item-slot-in").style.backgroundImage = itemDatas[i].itemImage;
				t_item.RegisterCallback<PointerDownEvent>((evt) => 
				{
					t_itemSlot.AddToClassList("item-slot-out__select-effect-1");
					t_itemSlot.schedule.Execute(() => 
					{
						t_itemSlot.AddToClassList("item-slot-out__select-effect-2");
						
						t_itemSlot.schedule.Execute(() => 
						{
							t_itemSlot.RemoveFromClassList("item-slot-out__select-effect-1");
							t_itemSlot.RemoveFromClassList("item-slot-out__select-effect-2");
						}).StartingIn(500);
					}).StartingIn(fixedDeltaTimeInMilli);
					
					if (p_individualView.charInfoAccessories.childCount < 6)
					{
						t_itemSlot.transform.position = Vector3.zero;
						t_itemSlot.style.visibility = Visibility.Visible;
						t_itemSlot.style.position = Position.Relative;
						p_individualView.charInfoAccessories.Add(t_itemSlot);
					}
				});
			}
			
			gameUIManager.AddTooltipHandlerForVisualElement(t_item, itemTooltip);
			
			p_individualView.inventory.Add(t_item);
		}
	}

	void NewSkillTreeHandler(IndividualView p_individualView, VisualElement p_skillTree, VisualElement p_skillTreeContainer)
	{
		p_individualView.charInfoSkillItem.Add(p_skillTree);
		p_skillTree.SendToBack();
		p_individualView.skillTrees.Add(p_skillTree);
		
		/* Block swiping event when you swipe a skill inside the skill tree  */
		p_skillTreeContainer.RegisterCallback<PointerMoveEvent>((evt) => 
		{
			if (skillTreeNumPointerMoving != 0) evt.StopPropagation();
		});
	}
	
	IEnumerator DraggingUsableSlotCoroutine(IndividualView p_individualView, TouchInfo p_touchInfo, UsableSlotUIInfo p_usableSlotUIInfo
	, Action<int, int> p_saveDataAction)
	{
		skillTreeNumPointerMoving++;
		p_usableSlotUIInfo.usableSlot.style.visibility = Visibility.Visible;
		p_usableSlotUIInfo.usableSlot.style.position = Position.Absolute;
		p_usableSlotUIInfo.usableSlot.pickingMode = PickingMode.Ignore;
		
		if (p_usableSlotUIInfo.inScrollView)
		{
			p_individualView.charInfoSkillItem.Add(p_usableSlotUIInfo.usableSlot);
			gameUIManager.mainView.HideUsableHolder(p_individualView, p_usableSlotUIInfo.usableHolder);
		}
	
		p_usableSlotUIInfo.scrollViewTouchedLastFrame = null;
		
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{	
			p_usableSlotUIInfo.inScrollView = false;
			p_usableSlotUIInfo.currentTouchRect.position = p_touchInfo.panelPosition;
			p_usableSlotUIInfo.scrollViewTouchedThisFrame = null;
			p_usableSlotUIInfo.usableSlot.transform.position = p_usableSlotUIInfo.usableSlot.parent.WorldToLocal(p_usableSlotUIInfo.currentTouchRect.position - skillSlotDragOffset);
			
			for (int i=0;i<p_individualView.usableSlotScrollView.Count;i++)
			{
				if (p_individualView.usableSlotScrollView[i].worldBound.Overlaps(p_usableSlotUIInfo.currentTouchRect))
				{
					p_individualView.usableSlotScrollView[i].AddToClassList("usable-slot-scrollview-selected");
					p_usableSlotUIInfo.scrollViewTouchedThisFrame = p_individualView.usableSlotScrollView[i];
					p_usableSlotUIInfo.inScrollView = true;
					p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex = i;
					break;
				}
			}
			
			if (p_usableSlotUIInfo.scrollViewTouchedThisFrame != p_usableSlotUIInfo.scrollViewTouchedLastFrame)
			{
				p_usableSlotUIInfo.scrollViewTouchedLastFrame?.RemoveFromClassList("usable-slot-scrollview-selected");
			}
			
			p_usableSlotUIInfo.scrollViewTouchedLastFrame = p_usableSlotUIInfo.scrollViewTouchedThisFrame;
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
			p_touchInfo.UpdateSelf();
		}
		
		skillTreeNumPointerMoving--;
		p_usableSlotUIInfo.scrollViewTouchedLastFrame?.RemoveFromClassList("usable-slot-scrollview-selected");
		
		if (p_usableSlotUIInfo.inScrollView)
		{
			p_usableSlotUIInfo.dropIntoScrollViewIndex = 
			Mathf.FloorToInt
			(
				(p_usableSlotUIInfo.currentTouchRect.y - p_usableSlotUIInfo.scrollViewTouchedThisFrame.worldBound.y) 
				* p_usableSlotUIInfo.scrollViewTouchedThisFrame.childCount / p_usableSlotUIInfo.scrollViewTouchedThisFrame.worldBound.height
			);
			
			AddUsableSlotToScrollView(p_individualView, p_usableSlotUIInfo, p_usableSlotUIInfo.dropIntoScrollViewIndex, p_saveDataAction);
		}
		else
		{
			p_usableSlotUIInfo.usableSlot.style.visibility = Visibility.Hidden;
		}
	}

	void AddUsableSlotToScrollView(IndividualView p_individualView, UsableSlotUIInfo p_usableSlotUIInfo, int p_dropIntoScrollViewIndex
	, Action<int, int> p_saveDataAction)
	{
	    p_usableSlotUIInfo.usableSlot.style.position = Position.Relative;
		p_usableSlotUIInfo.usableSlot.transform.position = new Vector3(0, 0);
		p_usableSlotUIInfo.usableSlot.pickingMode = PickingMode.Position;
		
		if (p_usableSlotUIInfo.scrollViewTouchedThisFrame.ElementAt
		(p_dropIntoScrollViewIndex).name.Equals("usable-slot-start"))
		{
			p_usableSlotUIInfo.scrollViewTouchedThisFrame.Insert
			(
				p_dropIntoScrollViewIndex + 1
				, p_usableSlotUIInfo.usableSlot
			);
			
			gameUIManager.mainView.AddUsableHolderToScrollView(p_individualView, p_usableSlotUIInfo, 0);
		}
		else if (p_usableSlotUIInfo.scrollViewTouchedThisFrame.ElementAt
		(p_dropIntoScrollViewIndex).name.Equals("usable-slot-end"))
		{
			p_usableSlotUIInfo.scrollViewTouchedThisFrame.Insert
			(
				p_dropIntoScrollViewIndex
				, p_usableSlotUIInfo.usableSlot
			);
			
			gameUIManager.mainView.AddUsableHolderToScrollView(p_individualView, p_usableSlotUIInfo, p_dropIntoScrollViewIndex);
		}
		else
		{
			p_usableSlotUIInfo.scrollViewTouchedThisFrame.Insert
			(
				p_dropIntoScrollViewIndex
				, p_usableSlotUIInfo.usableSlot
			);
			
			p_usableSlotUIInfo.replaceUsableSlot = p_usableSlotUIInfo.scrollViewTouchedThisFrame.ElementAt(p_dropIntoScrollViewIndex + 1);

			p_usableSlotUIInfo.replaceUsableSlot.style.visibility = Visibility.Hidden;
			p_individualView.charInfoSkillItem.Add(p_usableSlotUIInfo.replaceUsableSlot);
			
			gameUIManager.mainView.AddUsableHolderToScrollView(p_individualView, p_usableSlotUIInfo, p_dropIntoScrollViewIndex);
			gameUIManager.mainView.HideUsableHolder(p_individualView, usableSlotUIInfoDict[p_usableSlotUIInfo.replaceUsableSlot.GetHashCode()].usableHolder);
		}

		p_saveDataAction(p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex, p_dropIntoScrollViewIndex);
	}

	public void AddUsableSlotToScrollViewDirectly(IndividualView p_individualView, UsableSlotUIInfo p_usableSlotUIInfo, int p_scrollViewIndex
	, int p_dropIntoScrollViewIndex)
	{
	    p_usableSlotUIInfo.usableSlot.style.visibility = Visibility.Visible;
		p_usableSlotUIInfo.scrollViewTouchedThisFrame = p_individualView.usableSlotScrollView[p_scrollViewIndex];
		p_usableSlotUIInfo.inScrollView = true;
		p_usableSlotUIInfo.scrollViewTouchedThisFrameIndex = p_scrollViewIndex;
		AddUsableSlotToScrollView(p_individualView, p_usableSlotUIInfo, p_dropIntoScrollViewIndex, (i1, i2) => {});
	}
	
	VisualElement HandleTooltipView(IndividualView p_individualView, VisualElement p_tooltip, string name, Texture2D image, string description)
	{
		VisualElement t_tooltipHelperImage;
		p_tooltip.Q<Label>("tooltip__text").text = name;
		p_tooltip.transform.position = VectorExtension.veryFar;
		p_tooltip.RegisterCallback<PointerDownEvent>((evt) => p_tooltip.BringToFront());
		t_tooltipHelperImage = p_tooltip.Q<VisualElement>("tooltip__helper-image");
		t_tooltipHelperImage.style.backgroundImage = new StyleBackground(image);
		t_tooltipHelperImage.RegisterCallback<PointerDownEvent>((evt) => 
		{
			TouchInfo t_touchInfo = TouchExtension.GetTouchInfoAt(evt.position, uIDocument.rootVisualElement);
			StartCoroutine(MoveTooltip(t_touchInfo, t_tooltipHelperImage));	
		});
		
		p_tooltip.Q<Label>("tooltip__description").text = description;
		p_tooltip.Q(name: "tooltip__exit-button").RegisterCallback<PointerDownEvent>
		(
			evt => 
			{
				p_tooltip.transform.position = VectorExtension.veryFar;
				p_tooltip.RemoveFromClassList("tooltip-showup");
			}
		);
		p_individualView.charInfoContentSkillItem.Add(p_tooltip);
		
		return p_tooltip;
	}
	
	IEnumerator MoveTooltip(TouchInfo p_touchInfo, VisualElement p_tooltipHelperImage)
	{
		Vector2 tooltipOffset = p_touchInfo.panelPosition - p_tooltipHelperImage.parent.worldBound.position;
		
		while (p_touchInfo.touch.phase != TouchPhase.Ended)
		{
			p_touchInfo.UpdateSelf();
			p_tooltipHelperImage.parent.transform.position = p_tooltipHelperImage.parent.parent.WorldToLocal(p_touchInfo.panelPosition - tooltipOffset);
			
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		
		p_tooltipHelperImage.parent.transform.position = p_tooltipHelperImage.parent.parent.WorldToLocal(new Vector2
		(
			p_tooltipHelperImage.parent.worldBound.position.x
			, Math.Max(p_tooltipHelperImage.parent.worldBound.position.y, uIDocument.rootVisualElement.worldBound.min.y)
		));
	}
}