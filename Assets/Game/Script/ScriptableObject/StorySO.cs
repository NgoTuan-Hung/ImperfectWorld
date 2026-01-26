using UnityEngine;

[CreateAssetMenu(fileName = "StoryPageSO", menuName = "ScriptableObjects/StoryPageSO")]
public class StoryPageSO : ScriptableObject
{
    public Sprite sprite;
    public string text;
    public StoryPageSO nextPage;
}
