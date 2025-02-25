using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ScrollViewLockState {Locked = 0, Unlocked = 1, AutoLocked = 2}
public class SkillScrollViewUIInfo
{
	public ScrollView scrollView;
	public VisualElement scrollViewLock;
	public ScrollViewLockState scrollViewLockState;
	public int scrollViewListIndex;
	public Coroutine scrollSnapCoroutine;
	public int skillScrollViewPreviousIndex = 0;
	public int skillScrollViewNewIndex = 0;
	public float scrollViewHeight = 0f;
	public float distanceToSnap = 0f;
	public List<SkillHolderView> skillHolderViews = new();

	public SkillScrollViewUIInfo(ScrollView scrollView, int scrollViewListIndex, Coroutine scrollSnapCoroutine)
	{
		this.scrollView = scrollView;
		this.scrollViewListIndex = scrollViewListIndex;
		this.scrollSnapCoroutine = scrollSnapCoroutine;
	}
}

