using System;
using UnityEngine;

public enum SpawnType {Forever, Once}
public enum SpawnCountIncrementalType {OnePerWave, None}
[Serializable]
public class SpawnEnemyInfo
{
	public GameObject prefab;
	public int spawnChance;
	public float spawnInterval;
	public int maxSpawnCount;
	public int currentSpawn;
	public SpawnType spawnType = SpawnType.Forever;
	public SpawnCountIncrementalType spawnCountIncrementalType = SpawnCountIncrementalType.None;
}