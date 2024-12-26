
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillTooltipView
{
	public readonly VisualElement root;
	public SkillTooltipView(VisualElement root, string skillName, Texture2D skillHelperImage, string skillDescription, StyleSheet styleSheet)
	{
		this.root = root;
		root = root.ElementAt(0);
		root.Q<Label>("tooltip__text").text = skillName;
		root.Q<VisualElement>("tooltip__helper-image").style.backgroundImage = new StyleBackground(skillHelperImage);
		root.Q<Label>("tooltip__description").text = skillDescription;
		root.Q<Button>("tooltip__exit-button").RegisterCallback<ClickEvent>
		(
			evt => 
			{
				root.RemoveFromClassList("tooltip-showup");
				root.style.visibility = Visibility.Hidden;
			}
		);
	}
	
	public VisualElement visualElement => root;
}