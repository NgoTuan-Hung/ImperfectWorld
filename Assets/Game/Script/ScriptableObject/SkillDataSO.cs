using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "ScriptableObjects/SkillDataSO")]
public class SkillDataSo : ScriptableObject
{
	public enum InputType {Click, Hold, HoldAndRelease}
	public InputType inputType;
	public string skillName;
	public Texture2D skillImage;
	public Texture2D skillHelperImage;
	public string skillHelperDescription;
	public int skillButtonIndex;
}