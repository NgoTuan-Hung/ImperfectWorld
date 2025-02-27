using UnityEngine;
using UnityEngine.UIElements;

public class UsableSlotUIInfo
{
	public Rect currentTouchRect = new(0, 0, 1, 1);
	public VisualElement scrollViewTouchedThisFrame = null;
	public int scrollViewTouchedThisFrameIndex;
	public VisualElement scrollViewTouchedLastFrame = null;
	public VisualElement replaceUsableSlot = null;
	public int dropIntoScrollViewIndex;
	public bool inScrollView = false;
	public VisualElement usableSlot;
    public VisualElement usableHolder;
	public Vector2 usableHolderMidPos;
	public UsableSlotUIInfo(VisualElement p_usableSlot) => usableSlot = p_usableSlot;
}