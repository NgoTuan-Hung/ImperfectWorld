using UnityEngine;

public enum SkillType {Active, Passive, Special}
[CreateAssetMenu(fileName = "SkillUIData", menuName = "ScriptableObjects/SkillUIData", order = 1)]
public class SkillUIData : ScriptableObject
{
	public enum InputType {Click, Hold, HoldAndRelease}
	public InputType inputType;
	public string skillName;
	public Texture2D skillImage;
	public Texture2D skillHelperImage;
	[TextArea] public string skillHelperDescription;
	public Texture2D skillTreeIcon;
	public SkillType skillType;
}