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
    public int currentSpawn = 0;
    public float maxSpawn = 3;
    public float spawnInterval = 1f;
    int round = 0;
    public List<SpawnEnemyInfo> spawnEnemyInfos = new();
    public List<int> spawnChances = new();
    public List<float> spawnCumulativeDistribution = new();
    List<ObjectPool> spawnObjectPools = new();
    Dictionary<CustomMono, SpawnedPawnInfo> spawnedPawnsThisRound = new();
    ObjectPool pickedObjectPool;
    int rand;
    bool newRound = false;
    public float nextRoundInterval = 5f;
    public SpriteRenderer spawnRangeObject;
    Stopwatch stopwatch = new();
    public float roundDuration = 15f;
    public new Camera camera;
    public CinemachineCamera cinemachineCamera;
    public Dictionary<GameEffectSO, ObjectPool> poolLink = new();
    public int attackBoolHash = Animator.StringToHash("Attack"),
        mainSkill1BoolHash = Animator.StringToHash("MainSkill1"),
        mainSkill2BoolHash = Animator.StringToHash("MainSkill2"),
        mainSkill3BoolHash = Animator.StringToHash("MainSkill3"),
        mainSkill2BlendHash = Animator.StringToHash("MainSkill2Blend");
    public Dictionary<int, GameObject> colliderOwner = new();
    IEnumerator roundTimerCountDownIE;

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
                p_poolObject.customMono.stat.currentHealthPointReachZeroEvent += () =>
                    PawnDeathHandler(p_poolObject.customMono);
            };
        }

        InitRound();
    }

    void InitRound()
    {
        roundTimerCountDownIE = RoundTimerCountdown();
        stopwatch.Restart();
        StartCoroutine(roundTimerCountDownIE);
        StartCoroutine(HandleRound());
    }

    IEnumerator HandleRound()
    {
        while (true)
        {
            if (newRound)
            {
                newRound = false;
                round++;

                HandleEndRound();
                StopCoroutine(roundTimerCountDownIE);
                yield return NewRoundIE();
                StartCoroutine(roundTimerCountDownIE);
            }

            rand = Random.Range(0, spawnObjectPools.Count);
            pickedObjectPool = spawnObjectPools[rand];

            if (currentSpawn < (int)maxSpawn)
            {
                CustomMono p_customMono = pickedObjectPool.PickOne().customMono;
                p_customMono.transform.position = new Vector3(
                    Random.Range(spawnRangeObject.bounds.min.x, spawnRangeObject.bounds.max.x),
                    Random.Range(spawnRangeObject.bounds.min.y, spawnRangeObject.bounds.max.y)
                );
                HandlePawnStatThisRound(p_customMono);

                currentSpawn++;

                if (spawnedPawnsThisRound.TryGetValue(p_customMono, out var t_spawnedPawnInfo))
                {
                    t_spawnedPawnInfo.alive = true;
                }
                else
                    spawnedPawnsThisRound.Add(p_customMono, new(p_customMono));

                yield return new WaitForSeconds(spawnInterval);
            }
            else
                yield return new WaitForSeconds(Time.fixedDeltaTime);

            if (stopwatch.Elapsed.TotalSeconds > roundDuration)
                newRound = true;
        }
    }

    BasicSpawnSchemeEnemyStat basicSpawnSchemeEnemyStat = new();

    private void HandleEndRound()
    {
        foreach (var pawn in spawnedPawnsThisRound.Values)
        {
            if (pawn.alive)
                pawn.customMono.stat.currentHealthPoint.Value = 0;
        }

        currentSpawn = 0;
        spawnedPawnsThisRound.Clear();

        basicSpawnSchemeEnemyStat.currentMight += 0.5f;
        basicSpawnSchemeEnemyStat.currentReflex += 0.5f;
        basicSpawnSchemeEnemyStat.currentWisdom += 0.5f;
        basicSpawnSchemeEnemyStat.currentMoveSpeed += 0.5f;
    }

    void HandlePawnStatThisRound(CustomMono p_customMono)
    {
        p_customMono.stat.might.BaseValue = basicSpawnSchemeEnemyStat.currentMight;
        p_customMono.stat.reflex.BaseValue = basicSpawnSchemeEnemyStat.currentReflex;
        p_customMono.stat.wisdom.BaseValue = basicSpawnSchemeEnemyStat.currentWisdom;
        p_customMono.stat.moveSpeed.BaseValue = basicSpawnSchemeEnemyStat.currentMoveSpeed;
    }

    void PawnDeathHandler(CustomMono p_customMono)
    {
        currentSpawn--;
        spawnedPawnsThisRound[p_customMono].alive = false;
    }

    IEnumerator RoundTimerCountdown()
    {
        while (true)
        {
            GameUIManagerRevamp.Instance.roundTimer.text = (
                (int)(roundDuration - stopwatch.Elapsed.TotalSeconds)
            ).ToString();
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    IEnumerator NewRoundIE()
    {
        GameUIManagerRevamp.Instance.roundText.text = "NEXT ROUND IN";
        stopwatch.Restart();
        while (stopwatch.Elapsed.TotalSeconds < nextRoundInterval)
        {
            GameUIManagerRevamp.Instance.roundTimer.text = (
                (int)(nextRoundInterval - stopwatch.Elapsed.TotalSeconds)
            ).ToString();
            yield return null;
        }

        GameUIManagerRevamp.Instance.roundText.text = "ROUND " + round;
        stopwatch.Restart();
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
