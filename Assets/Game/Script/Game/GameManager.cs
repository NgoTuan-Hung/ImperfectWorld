using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public partial class GameManager : MonoSingleton<GameManager>
{
    public readonly int attackButtonScrollViewIndex = 7;
    public readonly int attackButtonIndex = 0;
    Dictionary<int, CustomMono> customMonos = new();
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
    public Dictionary<GameEffectSO, ObjectPool> poolLink = new();
    public int attackBoolHash = Animator.StringToHash("Attack"),
        mainSkill1BoolHash = Animator.StringToHash("MainSkill1"),
        mainSkill2BoolHash = Animator.StringToHash("MainSkill2"),
        mainSkill3BoolHash = Animator.StringToHash("MainSkill3"),
        mainSkill2BlendHash = Animator.StringToHash("MainSkill2Blend");
    public Dictionary<int, GameObject> colliderOwner = new();

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
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            spawnChances.Add(0);
            spawnCumulativeDistribution.Add(0);
        }

        InitAllEffectPools();
        LoadOtherResources();
    }

    void LoadOtherResources() { }

    private void Start()
    {
        /* Set the frame rate */
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        for (int i = 0; i < spawnEnemyInfos.Count; i++)
        {
            SpawnEnemyInfo t_spawnEnemyInfo = spawnEnemyInfos[i];

            spawnObjectPools[i].handleCachedComponentRefs += (p_poolObject) =>
            {
                p_poolObject.customMono.stat.currentHealthPointReachZeroEvent.action += () =>
                    t_spawnEnemyInfo.currentSpawn--;
            };
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
        customMonos.Add(customMono.gameObject.GetHashCode(), customMono);
        if (customMono.allyTags.Contains("GamePlayer"))
            playerAllies.Add(customMono);
    }

    public CustomMono GetCustomMono(GameObject p_gameObject)
    {
        return customMonos.GetValueOrDefault(p_gameObject.GetHashCode());
    }

    public void RemoveCustomMono(GameObject p_gameObject)
    {
        customMonos.Remove(p_gameObject.GetHashCode());
    }

    public CustomMono GetRandomPlayerAlly() => playerAllies[Random.Range(0, playerAllies.Count)];
}
