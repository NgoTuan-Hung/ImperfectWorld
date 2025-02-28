using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ScrollViewLockState {Locked = 0, Unlocked = 1, AutoLocked = 2}
public class UsableScrollViewUIInfo
{
	public ScrollView scrollView;
	public VisualElement scrollViewLock;
	public ScrollViewLockState scrollViewLockState;
	public Coroutine scrollSnapCoroutine;
	public int newChildIndex = 0;
	public int previousChildIndex = 0;
	public float scrollViewHeight = 0f;
	public float distanceToSnap = 0f;

	public UsableScrollViewUIInfo(ScrollView p_scrollView, Coroutine p_scrollSnapCoroutine, VisualElement p_scrollViewLock
	, ScrollViewLockState p_scrollViewLockState)
	{
		scrollView = p_scrollView;
		scrollSnapCoroutine = p_scrollSnapCoroutine;
		scrollViewLock = p_scrollViewLock;
		scrollViewLockState = p_scrollViewLockState;
	}
}

