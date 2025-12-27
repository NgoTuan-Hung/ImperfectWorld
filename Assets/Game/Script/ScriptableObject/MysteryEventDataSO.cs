using System.Collections.Generic;
using UnityEngine;

public enum MysteryEventType
{
    TreasureUnderTheSea,
}

[CreateAssetMenu(
    fileName = "MysteryEventDataSO",
    menuName = "ScriptableObjects/MysteryEventDataSO"
)]
public class MysteryEventDataSO : ScriptableObject
{
    public MysteryEventType eventType;
    public string description;
    public Sprite sprite;
    public List<string> choices;
}
