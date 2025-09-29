using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GridManager))]
public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Dictionary<int, CustomMono> customMonos = new();
    public int currentSpawn = 0;
    public float maxSpawn = 3;
    public float spawnInterval = 1f;
    int round = 0;
    public List<SpawnEnemyInfo> spawnEnemyInfos = new();
    List<SpawnEnemyInfo> unlockSkillSEI = new();
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
        attackBlendHash = Animator.StringToHash("AttackBlend"),
        mainSkill1BoolHash = Animator.StringToHash("MainSkill1"),
        mainSkill2BoolHash = Animator.StringToHash("MainSkill2"),
        mainSkill3BoolHash = Animator.StringToHash("MainSkill3"),
        mainSkill2BlendHash = Animator.StringToHash("MainSkill2Blend"),
        mainSkill2TriggerHash = Animator.StringToHash("MainSkill2Trigger"),
        mainSkill4BoolHash = Animator.StringToHash("MainSkill4"),
        combo1BoolHash = Animator.StringToHash("Combo1"),
        combo2BoolHash = Animator.StringToHash("Combo2"),
        airRollBoolHash = Animator.StringToHash("AirRoll"),
        landBoolHash = Animator.StringToHash("Land"),
        chargeBoolHash = Animator.StringToHash("Charge"),
        summonBoolHash = Animator.StringToHash("Summon"),
        walkBoolHash = Animator.StringToHash("Walk"),
        dieBoolHash = Animator.StringToHash("Die");
    public Dictionary<int, GameObject> colliderOwner = new();
    IEnumerator roundTimerCountDownIE;
    public List<ActionFieldInfo> actionFieldInfos = new();
    Dictionary<string, ActionFieldInfo> actionFieldInfoDict = new();
    Dictionary<ActionFieldName, Type> actionFieldMapper = new();
    GridManager gridManager;
    List<GridNode> returnPath;

    public void InitializeControllableCharacter(CustomMono p_customMono) { }

    public void InitializeControllableCharacterRevamp(CustomMono p_customMono)
    {
        GameUIManagerRevamp.Instance.InitializeCharacterUI(p_customMono);
    }

    private void Awake()
    {
        Instance = this;
        camera = Camera.main;
        foreach (SpawnEnemyInfo spawnEnemyInfo in spawnEnemyInfos)
        {
            spawnObjectPools.Add(
                new ObjectPool(
                    spawnEnemyInfo.prefab,
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            spawnEnemyInfo.Init();
            unlockSkillSEI.Add(spawnEnemyInfo);
        }
        gridManager = GetComponent<GridManager>();

        InitAllEffectPools();
        LoadOtherResources();
        ConstructActionFieldInfoDict();
        MapActionFieldName();
    }

    void LoadOtherResources() { }

    void ConstructActionFieldInfoDict()
    {
        actionFieldInfos.ForEach(aFI =>
        {
            actionFieldInfoDict.Add(aFI.actionType, aFI);
        });
    }

    public ActionFieldInfo GetActionFieldInfo(string p_actionType) =>
        actionFieldInfoDict[p_actionType];

    private void MapActionFieldName()
    {
        List<ActionFieldName> allAFN = Enum.GetValues(typeof(ActionFieldName))
            .Cast<ActionFieldName>()
            .ToList();
        allAFN.ForEach(aFN =>
        {
            actionFieldMapper.Add(
                aFN,
                aFN switch
                {
                    ActionFieldName.Cooldown => typeof(ActionFloatField),
                    ActionFieldName.Duration => typeof(ActionFloatField),
                    ActionFieldName.Range => typeof(ActionFloatField),
                    ActionFieldName.Damage => typeof(ActionFloatField),
                    ActionFieldName.Variants => typeof(ActionIntField),
                    ActionFieldName.ManaCost => typeof(ActionFloatField),
                    ActionFieldName.LifeStealPercent => typeof(ActionFloatField),
                    ActionFieldName.CurrentTime => typeof(ActionFloatField),
                    ActionFieldName.Speed => typeof(ActionFloatField),
                    ActionFieldName.ActionIE => typeof(ActionIEnumeratorField),
                    ActionFieldName.ActionIE1 => typeof(ActionIEnumeratorField),
                    ActionFieldName.ActionIE2 => typeof(ActionIEnumeratorField),
                    ActionFieldName.EffectCount => typeof(ActionIntField),
                    ActionFieldName.EffectDuration => typeof(ActionFloatField),
                    ActionFieldName.ComboEffects => typeof(ActionListComboEffectField),
                    ActionFieldName.Interval => typeof(ActionFloatField),
                    ActionFieldName.CustomGameObject => typeof(ActionGameObjectField),
                    ActionFieldName.PoisonInfo => typeof(ActionPoisonInfoField),
                    ActionFieldName.SlowInfo => typeof(ActionSlowInfoField),
                    ActionFieldName.GameEffect => typeof(ActionGameEffectField),
                    ActionFieldName.Origin => typeof(ActionVector3Field),
                    ActionFieldName.StopWatch => typeof(ActionStopWatchField),
                    ActionFieldName.Angle => typeof(ActionFloatField),
                    ActionFieldName.Direction => typeof(ActionVector3Field),
                    ActionFieldName.Blend => typeof(ActionFloatField),
                    ActionFieldName.Target => typeof(ActionCustomMonoField),
                    ActionFieldName.CurrentPhase => typeof(ActionIntField),
                    ActionFieldName.ComboActions => typeof(ActionListComboActionField),
                    ActionFieldName.ComboEndAction => typeof(ActionActionField),
                    ActionFieldName.Distance => typeof(ActionFloatField),
                    ActionFieldName.SelectedVariant => typeof(ActionIntField),
                    ActionFieldName.AllPhases => typeof(ActionIntField),
                    ActionFieldName.UseCount => typeof(ActionIntField),
                    ActionFieldName.MaxUseCount => typeof(ActionIntField),
                    _ => null,
                }
            );
        });
    }

    public Type GetActionFieldTypeFromName(ActionFieldName p_actionFieldName) =>
        actionFieldMapper[p_actionFieldName];

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
                p_poolObject.CustomMono.stat.currentHealthPointReachZeroEvent += () =>
                    PawnDeathHandler(p_poolObject.CustomMono);
            };
        }

        // InitRound();
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
                CustomMono p_customMono = pickedObjectPool.PickOne().CustomMono;
                p_customMono.transform.position = new Vector3(
                    Random.Range(spawnRangeObject.bounds.min.x, spawnRangeObject.bounds.max.x),
                    Random.Range(spawnRangeObject.bounds.min.y, spawnRangeObject.bounds.max.y)
                );
                HandlePawnStatThisRound(p_customMono);
                StartCoroutine(
                    HandleSpawnSkill(p_customMono, spawnEnemyInfos[rand].spawnEnemySkillInfos)
                );

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
        UnlockSpawnSkillEvery3Round();
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

    /// <summary>
    /// Unlock skills for spawned enemies if any
    /// </summary>
    /// <param name="p_customMono"></param>
    /// <param name="p_sEKI"></param>
    /// <returns></returns>
    IEnumerator HandleSpawnSkill(CustomMono p_customMono, List<SpawnEnemySkillInfo> p_sEKI)
    {
        /* Since customMono might awake or start at this point, we should wait for
        a frame before continuing */
        yield return null;

        p_customMono.skill.HandleSkillUnlock(p_sEKI);
    }

    void UnlockSpawnSkillEvery3Round()
    {
        if (round % 3 == 0 && round > 0)
        {
            if (unlockSkillSEI.Count > 0)
            {
                rand = Random.Range(0, unlockSkillSEI.Count);

                while (!unlockSkillSEI[rand].UnlockNextSkill())
                {
                    unlockSkillSEI.RemoveAt(rand);
                    rand = Random.Range(0, unlockSkillSEI.Count);
                }
            }
        }
    }

    public void AddCustomMono(CustomMono customMono)
    {
        customMonos.Add(customMono.gameObject.GetHashCode(), customMono);
    }

    public CustomMono GetCustomMono(GameObject p_gameObject)
    {
        return customMonos.GetValueOrDefault(p_gameObject.GetHashCode());
    }

    public void RemoveCustomMono(GameObject p_gameObject)
    {
        customMonos.Remove(p_gameObject.GetHashCode());
    }

    public CustomMono GetRandomEnemy(string p_yourTag) =>
        customMonos.FirstOrDefault(kV => !kV.Value.allyTags.Contains(p_yourTag)).Value;

    public Vector2 GetPathFindingDirectionToTarget(Vector2 p_currentPos, Vector2 p_targetPos)
    {
        /* Temporary removing self && target from grid because it will block the path */
        RemoveObstacle(p_currentPos);
        RemoveObstacle(p_targetPos);

        returnPath = gridManager.SolvePath(
            gridManager.GetNodeAtPosition(p_currentPos),
            gridManager.GetNodeAtPosition(p_targetPos)
        );

        /* Adding self grid back */
        SetObstacle(p_currentPos);
        SetObstacle(p_targetPos);

        if (returnPath.Count > 1)
        {
            return returnPath[^2].pos - p_currentPos;
        }
        else
        {
            print("No path found");
            return p_targetPos - p_currentPos;
        }
    }

    public void SetObstacle(Vector2 p_pos) => gridManager.MarkNodeAsObstacle(p_pos);

    public void RemoveObstacle(Vector2 p_pos) => gridManager.MarkNodeAsNormal(p_pos);
}
