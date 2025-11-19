using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "ScriptableObjects/StatUpgrade")]
public class StatUpgrade : ScriptableObject
{
    public string description;
    public StatBuff statBuff;
}
