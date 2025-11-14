using System;
using UnityEngine;

[Serializable]
public class RoomEnemyInfo
{
    public GameObject prefab;
    public Vector2 position;

    public RoomEnemyInfo(GameObject prefab, Vector2 position)
    {
        this.prefab = prefab;
        this.position = position;
    }
}
