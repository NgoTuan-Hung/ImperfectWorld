using UnityEngine;

public enum SkillType {Active, Passive, Special}
[CreateAssetMenu(fileName = "SkillDataSO", menuName = "ScriptableObjects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
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