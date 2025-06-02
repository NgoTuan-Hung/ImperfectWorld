using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteList", menuName = "ScriptableObjects/SpriteList", order = 0)]
public class SpriteList : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}
