using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public enum CollisionEffectPool
{
    MoonSlashExplode,
    MagicLaserImpact,
    SlaughterExplosion,
    StrongDudePunchImpact,
    SamuraiSlash,
    BladeOfPhongTornadoImpact,
    LaterDecide,
}

public class GameManager : MonoSingleton<GameManager>
{
    public readonly int attackButtonScrollViewIndex = 7;
    public readonly int attackButtonIndex = 0;
    BinarySearchTree<CustomMono> customMonos = new BinarySearchTree<CustomMono>();
    List<CustomMono> playerAllies = new();
    int wave = 0;
    public List<SpawnEnemyInfo> spawnEnemyInfos = new();
    public List<int> spawnChances = new();
    public List<float> spawnCumulativeDistribution = new();
    List<ObjectPool> spawnObjectPools = new();
    float rand;
    SpawnEnemyInfo pickedSpawnEnemyInfo;
    int pickedSpawnEnemyIndex = 0;
    bool newWave = false;
    public float waveInterval = 30f;
    public SpriteRenderer spawnRangeObject;
    Stopwatch stopwatch = new();
    public float waveDuration = 60f;
    public CinemachineCamera cinemachineCamera;
    Dictionary<int, CharData> charDataDict = new();
    CharData currentControlledCharData;
    public Dictionary<CollisionEffectPool, ObjectPool> collisionEffectPoolDict = new();
    GameObject vanishEffectPrefab,
        bladeOfMinhKhaiSlashEffectPrefab,
        bladeOfPhongTornadoEffectPrefab;
    public ObjectPool vanishEffectPool,
        bladeOfMinhKhaiSlashEffectPool,
        bladeOfPhongTornadoEffectPool;
    public int bladeOfMinhKhaiBoolHash = Animator.StringToHash("BladeOfMinhKhai"),
        bladeOfPhongBoolHash = Animator.StringToHash("BladeOfPhong");

    public void InitializeControllableCharacter(CustomMono p_customMono)
    {
        CharData t_charData = new(p_customMono);
        currentControlledCharData ??= t_charData;

        t_charData.individualView = GameUIManager.Instance.AddNewIndividualView(
            p_customMono.charUIData,
            () =>
            {
                cinemachineCamera.Follow = p_customMono.transform;
                currentControlledCharData.customMono.ResumeBot();
                currentControlledCharData = t_charData;
                currentControlledCharData.customMono.PauseBot();
            }
        );

        charDataDict.Add(p_customMono.GetHashCode(), t_charData);

        t_charData.individualView.joyStickMoveEvent += (vector) =>
        {
            vector.Scale(VectorExtension.inverseY);
            p_customMono.movable.moveVector = vector;
        };

        GameUIManager.Instance.CheckFirstIndividualView();
    }

    public CharData GetCharData(CustomMono p_customMono) =>
        charDataDict[p_customMono.GetHashCode()];

    private void Awake()
    {
        foreach (SpawnEnemyInfo spawnEnemyInfo in spawnEnemyInfos)
        {
            spawnObjectPools.Add(
                new ObjectPool(
                    spawnEnemyInfo.prefab,
                    100,
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            spawnChances.Add(0);
            spawnCumulativeDistribution.Add(0);
        }

        InitAllCollisionEffectPools();
        InitAllEffectPools();
    }

    void InitAllCollisionEffectPools()
    {
        collisionEffectPoolDict.Add(
            CollisionEffectPool.MoonSlashExplode,
            new(
                Resources.Load("MoonSlashExplode") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(
            CollisionEffectPool.MagicLaserImpact,
            new(
                Resources.Load("MagicLaserImpact") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(
            CollisionEffectPool.SlaughterExplosion,
            new(
                Resources.Load("SlaughterExplosion") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(
            CollisionEffectPool.StrongDudePunchImpact,
            new(
                Resources.Load("StrongDudePunchImpact") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(
            CollisionEffectPool.SamuraiSlash,
            new(
                Resources.Load("SamuraiSlash") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(
            CollisionEffectPool.BladeOfPhongTornadoImpact,
            new(
                Resources.Load("BladeOfPhongTornadoImpact") as GameObject,
                100,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        collisionEffectPoolDict.Add(CollisionEffectPool.LaterDecide, null);
    }

    void InitAllEffectPools()
    {
        vanishEffectPrefab = Resources.Load("VanishEffect") as GameObject;
        vanishEffectPool ??= new ObjectPool(
            vanishEffectPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfMinhKhaiSlashEffectPrefab = Resources.Load("BladeOfMinhKhaiSlash") as GameObject;
        bladeOfMinhKhaiSlashEffectPool ??= new ObjectPool(
            bladeOfMinhKhaiSlashEffectPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfPhongTornadoEffectPrefab = Resources.Load("BladeOfPhongTornado") as GameObject;
        bladeOfPhongTornadoEffectPool ??= new(
            bladeOfPhongTornadoEffectPrefab,
            100,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
    }

    public ObjectPool GetCollisionEffectPool(CollisionEffectPool p_collisionEffectPool)
    {
        return collisionEffectPoolDict[p_collisionEffectPool];
    }

    private void Start()
    {
        for (int i = 0; i < spawnEnemyInfos.Count; i++)
        {
            SpawnEnemyInfo t_spawnEnemyInfo = spawnEnemyInfos[i];
            spawnObjectPools[i]
                .ForEach(poolObject =>
                {
                    poolObject.customMono.stat.healthReachZeroEvent.action += () =>
                        t_spawnEnemyInfo.currentSpawn--;
                });
        }

        stopwatch.Restart();
        StartCoroutine(HandleWave());
    }

    IEnumerator HandleWave()
    {
        while (true)
        {
            if (newWave)
            {
                newWave = false;
                wave++;
                NewWavePreparation();
                yield return new WaitForSeconds(waveInterval);

                stopwatch.Restart();
            }

            rand = Random.Range(0, 1f);
            pickedSpawnEnemyIndex = spawnCumulativeDistribution.CumulativeDistributionBinarySearch(
                0,
                spawnCumulativeDistribution.Count - 1,
                rand
            );
            pickedSpawnEnemyInfo = spawnEnemyInfos[pickedSpawnEnemyIndex];

            if (pickedSpawnEnemyInfo.currentSpawn < pickedSpawnEnemyInfo.maxSpawnCount)
            {
                CustomMono customMono = spawnObjectPools[pickedSpawnEnemyIndex]
                    .PickOne()
                    .customMono;
                customMono.transform.position = new Vector3(
                    Random.Range(spawnRangeObject.bounds.min.x, spawnRangeObject.bounds.max.x),
                    Random.Range(spawnRangeObject.bounds.min.y, spawnRangeObject.bounds.max.y)
                );

                pickedSpawnEnemyInfo.currentSpawn++;

                yield return new WaitForSeconds(pickedSpawnEnemyInfo.spawnInterval);
            }
            else
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            if (stopwatch.Elapsed.TotalSeconds > waveDuration)
                newWave = true;
        }
    }

    void NewWavePreparation()
    {
        for (int i = 0; i < spawnEnemyInfos.Count; i++)
        {
            spawnEnemyInfos[i].currentSpawn = 0;

            switch (spawnEnemyInfos[i].spawnType)
            {
                case SpawnType.Forever:
                    spawnChances[i] = spawnEnemyInfos[i].spawnChance;
                    break;
                default:
                    break;
            }
        }

        CalculateSpawnCumulativeDistribution();
    }

    void CalculateSpawnCumulativeDistribution()
    {
        float t_total = spawnChances.Sum();

        spawnCumulativeDistribution[0] = spawnChances[0] / t_total;
        for (int i = 1; i < spawnCumulativeDistribution.Count; i++)
        {
            spawnCumulativeDistribution[i] =
                spawnCumulativeDistribution[i - 1] + spawnChances[i] / t_total;
        }
    }

    public void AddCustomMono(CustomMono customMono)
    {
        customMonos.Insert(customMono);
        if (customMono.allyTags.Contains("GamePlayer"))
            playerAllies.Add(customMono);
    }

    public CustomMono GetCustomMono(GameObject gameObject)
    {
        return customMonos.Search(
            (customMono) => customMono.gameObject.GetHashCode(),
            gameObject.GetHashCode()
        );
    }

    public CustomMono GetRandomPlayerAlly() => playerAllies[Random.Range(0, playerAllies.Count)];
}
