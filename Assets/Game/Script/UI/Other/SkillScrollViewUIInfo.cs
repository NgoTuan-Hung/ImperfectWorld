using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillScrollViewUIInfo
{
	private ScrollView scrollView;
	private VisualElement scrollViewLock;
	private ScrollViewLockState scrollViewLockState;
	private int scrollViewListIndex;
	private Coroutine scrollSnapCoroutine;
	private int skillScrollViewPreviousIndex = 0;
	private int skillScrollViewNewIndex = 0;
	private float scrollViewHeight = 0f;
	private float distanceToSnap = 0f;
	public List<SkillHolderView> skillHolderViews = new();

	public SkillScrollViewUIInfo(ScrollView scrollView, int scrollViewListIndex, Coroutine scrollSnapCoroutine)
	{
		this.scrollView = scrollView;
		this.scrollViewListIndex = scrollViewListIndex;
		this.scrollSnapCoroutine = scrollSnapCoroutine;
	}

	public ScrollView ScrollView { get => scrollView; set => scrollView = value; }
	public Coroutine ScrollSnapCoroutine { get => scrollSnapCoroutine; set => scrollSnapCoroutine = value; }
	public int SkillScrollViewPreviousIndex { get => skillScrollViewPreviousIndex; set => skillScrollViewPreviousIndex = value; }
	public int SkillScrollViewNewIndex { get => skillScrollViewNewIndex; set => skillScrollViewNewIndex = value; }
	public float ScrollViewHeight { get => scrollViewHeight; set => scrollViewHeight = value; }
	public float DistanceToSnap { get => distanceToSnap; set => distanceToSnap = value; }
	public int ScrollViewListIndex { get => scrollViewListIndex; set => scrollViewListIndex = value; }
	public VisualElement ScrollViewLock { get => scrollViewLock; set => scrollViewLock = value; }
	public ScrollViewLockState ScrollViewLockState { get => scrollViewLockState; set => scrollViewLockState = value; }
}

