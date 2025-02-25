using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharUIData", menuName = "ScriptableObjects/CharUIData")]
public class CharUIData : ScriptableObject 
{
	public string charName;
	public Texture2D charImage;
	public int currentLevel;
	[Range(0, 1)] public float currentHP;
	[Range(0, 1)] public float currentXP;
	public List<SkillUIData> skillUIDatas;
	public List<ItemData> itemDatas;
}