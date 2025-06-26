using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public enum EffectPool
{
    MoonSlashExplode,
    MagicLaserImpact,
    SlaughterExplosion,
    StrongDudePunchImpact,
    SamuraiSlash,
    BladeOfPhongTornadoImpact,
    BladeOfVuImpact,
    Arrow,
    ElementalLeafRangerArrow,
    ElementalLeafRangerPoisonArrowImpact,
    ElementalLeafRangerVineArrowImpact,
    WanderMagicianProjectile,

    /// <summary>
    /// For attack collider with different collision effect
    /// </summary>
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
    public new Camera camera;
    public CinemachineCamera cinemachineCamera;
    public Dictionary<EffectPool, ObjectPool> effectPoolDict = new();
    GameObject vanishEffectPrefab,
        bladeOfMinhKhaiSlashEffectPrefab,
        bladeOfPhongTornadoEffectPrefab,
        ghostPrefab,
        bladeOfVuStarPrefab,
        bladeOfVuSlashPrefab,
        pierceStrikePrefab,
        pierceStrikeSecondPhasePrefab,
        deepBladeSlashPrefab,
        rayOfJungleBeamPrefab,
        woodCryArrowPrefab,
        elementalLeafRangerPoisonArrowPrefab,
        elementalLeafRangerVineArrowPrefab,
        lightingForwardLightingPrefab,
        blueHolePrefab,
        nuclearBombExplosionPrefab,
        swordTempestSlash1Prefab,
        swordTempestSlash2Prefab,
        swordTempestSlash3Prefab;
    public ObjectPool vanishEffectPool,
        bladeOfMinhKhaiSlashEffectPool,
        bladeOfPhongTornadoEffectPool,
        ghostPool,
        bladeOfVuStarPool,
        bladeOfVuSlashPool,
        pierceStrikePool,
        pierceStrikeSecondPhasePool,
        deepBladeSlashPool,
        rayOfJungleBeamPool,
        woodCryArrowPool,
        elementalLeafRangerPoisonArrowPool,
        elementalLeafRangerVineArrowPool,
        lightingForwardLightingPool,
        blueHolePool,
        nuclearBombExplosionPool,
        swordTempestSlash1Pool,
        swordTempestSlash2Pool,
        swordTempestSlash3Pool;
    public int attackBoolHash = Animator.StringToHash("Attack"),
        mainSkill1BoolHash = Animator.StringToHash("MainSkill1"),
        mainSkill2BoolHash = Animator.StringToHash("MainSkill2"),
        mainSkill3BoolHash = Animator.StringToHash("MainSkill3");

    public void InitializeControllableCharacter(CustomMono p_customMono)
    {
        // CharData t_charData = new(p_customMono);
        // currentControlledCharData ??= t_charData;

        // t_charData.individualView = GameUIManager.Instance.AddNewIndividualView(
        //     p_customMono.charUIData,
        //     () =>
        //     {
        //         cinemachineCamera.Follow = p_customMono.transform;
        //         currentControlledCharData.customMono.ResumeBot();
        //         currentControlledCharData = t_charData;
        //         currentControlledCharData.customMono.PauseBot();
        //     }
        // );

        // charDataDict.Add(p_customMono.GetHashCode(), t_charData);

        // t_charData.individualView.joyStickMoveEvent += (vector) =>
        // {
        //     vector.Scale(VectorExtension.inverseY);
        //     p_customMono.movable.moveVector = vector;
        // };

        // GameUIManager.Instance.CheckFirstIndividualView();
    }

    public void InitializeControllableCharacterRevamp(CustomMono p_customMono)
    {
        GameUIManagerRevamp.Instance.InitializeCharacterUI(p_customMono);
    }

    private void Awake()
    {
        camera = Camera.main;
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
        LoadOtherResources();
    }

    void InitAllCollisionEffectPools()
    {
        effectPoolDict.Add(
            EffectPool.MoonSlashExplode,
            new(
                Resources.Load("MoonSlashExplode") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.MagicLaserImpact,
            new(
                Resources.Load("MagicLaserImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.SlaughterExplosion,
            new(
                Resources.Load("SlaughterExplosion") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.StrongDudePunchImpact,
            new(
                Resources.Load("StrongDudePunchImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.SamuraiSlash,
            new(
                Resources.Load("SamuraiSlash") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.BladeOfPhongTornadoImpact,
            new(
                Resources.Load("BladeOfPhongTornadoImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.BladeOfVuImpact,
            new(
                Resources.Load("BladeOfVuImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.Arrow,
            new(
                Resources.Load("Arrow") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.ElementalLeafRangerArrow,
            new(
                Resources.Load("ElementalLeafRangerArrow") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.ElementalLeafRangerPoisonArrowImpact,
            new(
                Resources.Load("ElementalLeafRangerPoisonArrowImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.ElementalLeafRangerVineArrowImpact,
            new(
                Resources.Load("ElementalLeafRangerVineArrowImpact") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(
            EffectPool.WanderMagicianProjectile,
            new(
                Resources.Load("WanderMagicianProjectile") as GameObject,
                10,
                new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
            )
        );
        effectPoolDict.Add(EffectPool.LaterDecide, null);
    }

    void InitAllEffectPools()
    {
        vanishEffectPrefab = Resources.Load("VanishEffect") as GameObject;
        vanishEffectPool ??= new ObjectPool(
            vanishEffectPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfMinhKhaiSlashEffectPrefab = Resources.Load("BladeOfMinhKhaiSlash") as GameObject;
        bladeOfMinhKhaiSlashEffectPool ??= new ObjectPool(
            bladeOfMinhKhaiSlashEffectPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfPhongTornadoEffectPrefab = Resources.Load("BladeOfPhongTornado") as GameObject;
        bladeOfPhongTornadoEffectPool ??= new(
            bladeOfPhongTornadoEffectPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        ghostPrefab = Resources.Load("Ghost") as GameObject;
        ghostPool ??= new(
            ghostPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfVuStarPrefab = Resources.Load("BladeOfVuStar") as GameObject;
        bladeOfVuStarPool ??= new(
            bladeOfVuStarPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        bladeOfVuSlashPrefab = Resources.Load("BladeOfVuSlash") as GameObject;
        bladeOfVuSlashPool ??= new(
            bladeOfVuSlashPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pierceStrikePrefab = Resources.Load("PierceStrike") as GameObject;
        pierceStrikePool ??= new(
            pierceStrikePrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        pierceStrikeSecondPhasePrefab = Resources.Load("PierceStrikeSecondPhase") as GameObject;
        pierceStrikeSecondPhasePool ??= new(
            pierceStrikeSecondPhasePrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        deepBladeSlashPrefab = Resources.Load("DeepBladeSlash") as GameObject;
        deepBladeSlashPool ??= new(
            deepBladeSlashPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        rayOfJungleBeamPrefab = Resources.Load("RayOfJungleBeam") as GameObject;
        rayOfJungleBeamPool ??= new(
            rayOfJungleBeamPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        woodCryArrowPrefab = Resources.Load("WoodCryArrow") as GameObject;
        woodCryArrowPool ??= new(
            woodCryArrowPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerPoisonArrowPrefab =
            Resources.Load("ElementalLeafRangerPoisonArrow") as GameObject;
        elementalLeafRangerPoisonArrowPool ??= new(
            elementalLeafRangerPoisonArrowPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        elementalLeafRangerVineArrowPrefab =
            Resources.Load("ElementalLeafRangerVineArrow") as GameObject;
        elementalLeafRangerVineArrowPool ??= new(
            elementalLeafRangerVineArrowPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        lightingForwardLightingPrefab = Resources.Load("LightingForwardLighting") as GameObject;
        lightingForwardLightingPool ??= new(
            lightingForwardLightingPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        blueHolePrefab = Resources.Load("BlueHole") as GameObject;
        blueHolePool ??= new(
            blueHolePrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        nuclearBombExplosionPrefab = Resources.Load("NuclearBombExplosion") as GameObject;
        nuclearBombExplosionPool ??= new(
            nuclearBombExplosionPrefab,
            10,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash1Prefab = Resources.Load("SwordTempestSlash1") as GameObject;
        swordTempestSlash1Pool ??= new(
            swordTempestSlash1Prefab,
            5,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash2Prefab = Resources.Load("SwordTempestSlash2") as GameObject;
        swordTempestSlash2Pool ??= new(
            swordTempestSlash2Prefab,
            5,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
        swordTempestSlash3Prefab = Resources.Load("SwordTempestSlash3") as GameObject;
        swordTempestSlash3Pool ??= new(
            swordTempestSlash3Prefab,
            5,
            new PoolArgument(ComponentType.GameEffect, PoolArgument.WhereComponent.Self)
        );
    }

    void LoadOtherResources() { }

    public ObjectPool GetEffectPool(EffectPool p_effectPool)
    {
        return effectPoolDict[p_effectPool];
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
        NewWavePreparation();
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
