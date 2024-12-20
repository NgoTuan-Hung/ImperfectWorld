using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Used for caching reference to UIs and some other stuffs
/// </summary>
public class SkillHolderView
{
	public VisualElement root, holderIn, holderOut;
	public ScrollView skillScrollView;
	/// <summary>
	/// Mid position of the scroll view
	/// </summary>
	public Vector2 midPos;
	
	public SkillHolderView(VisualElement root, ScrollView skillScrollView)
	{
		this.root = root;
		holderOut = root.Q<VisualElement>("skill-holder-out");
		holderIn = root.Q<VisualElement>("skill-holder-in");
		this.skillScrollView = skillScrollView;
	}
}