using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListSprite", menuName = "ScriptableObjects/ListSprite")]
public class ListSpriteSO : ScriptableObject
{
    public List<Sprite> sprites;
}
