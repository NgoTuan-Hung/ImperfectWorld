using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(HexGridManager))]
public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Dictionary<int, CustomMono> customMonos = new();
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
    public List<ActionFieldInfo> actionFieldInfos = new();
    Dictionary<string, ActionFieldInfo> actionFieldInfoDict = new();
    Dictionary<ActionFieldName, Type> actionFieldMapper = new();
    HexGridManager gridManager;
    List<HexGridNode> returnPath;
    public RoomSystem roomSystem;
    Dictionary<GameObject, ObjectPool> enemyPools = new();
    int enemyCount = 0;
    public List<CustomMono> playerChampions = new(),
        currentRoomEnemies;

    public void InitializeControllableCharacter(CustomMono p_customMono) { }

    public void InitializeControllableCharacterRevamp(CustomMono p_customMono)
    {
        GameUIManager.Instance.InitializeCharacterUI(p_customMono);
    }

    private void Awake()
    {
        Instance = this;
        camera = Camera.main;
        gridManager = GetComponent<HexGridManager>();

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

        HandleMapInteraction();
        BattleInitialize();
    }

    private void BattleInitialize()
    {
        GameUIManager.Instance.startBattleButton.pointerClickEvent += BattleRoomStart;
    }

    private void HandleMapInteraction()
    {
        MapPlayerTracker.Instance.onNodeEnter += (p_mapNode) =>
        {
            GameUIManager.Instance.startBattleButton.Show();
            switch (p_mapNode.Node.nodeType)
            {
                case NodeType.MinorEnemy:
                {
                    Debug.Log("Room Minor Enemy Encountered");
                    LoadNormalEnemyRoom();
                    break;
                }
                case NodeType.EliteEnemy:
                    break;
                case NodeType.RestSite:
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.Store:
                    break;
                case NodeType.Boss:
                    break;
                case NodeType.Mystery:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };
    }

    void LoadNormalEnemyRoom()
    {
        GameUIManager.Instance.TurnOffMap();
        enemyCount = 0;
        roomSystem.allNormalEnemyRooms.Shuffle();
        var nERI = roomSystem.allNormalEnemyRooms[0];
        currentRoomEnemies = new();

        for (int i = 0; i < nERI.roomEnemyInfos.Count; i++)
        {
            ObjectPool t_pool;
            if (!enemyPools.TryGetValue(nERI.roomEnemyInfos[i].prefab, out t_pool))
            {
                enemyPools.Add(
                    nERI.roomEnemyInfos[i].prefab,
                    new ObjectPool(
                        nERI.roomEnemyInfos[i].prefab,
                        new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                    )
                );

                enemyPools[nERI.roomEnemyInfos[i].prefab].handleCachedComponentRefs += (
                    p_poolObject
                ) =>
                {
                    p_poolObject.CustomMono.stat.currentHealthPointReachZeroEvent += () =>
                        PawnDeathHandler(p_poolObject.CustomMono);
                };
            }

            var t_customMono = enemyPools[nERI.roomEnemyInfos[i].prefab].PickOne().CustomMono;
            t_customMono.transform.position = nERI.roomEnemyInfos[i].position;
            currentRoomEnemies.Add(t_customMono);
            enemyCount++;
        }

        currentRoomEnemies.ForEach(cRE =>
        {
            cRE.botAIManager.aiBehavior.pausableScript.pauseFixedUpdate();
            cRE.botSensor.pausableScript.pauseFixedUpdate();
        });
    }

    void BattleRoomStart(PointerEventData p_pED)
    {
        GameUIManager.Instance.startBattleButton.Hide();
        currentRoomEnemies.ForEach(cRE =>
        {
            cRE.botAIManager.aiBehavior.pausableScript.resumeFixedUpdate();
            cRE.botSensor.pausableScript.resumeFixedUpdate();
        });

        playerChampions.ForEach(pC =>
        {
            pC.botAIManager.aiBehavior.pausableScript.resumeFixedUpdate();
            pC.botSensor.pausableScript.resumeFixedUpdate();
        });
    }

    void PawnDeathHandler(CustomMono p_customMono)
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            playerChampions.ForEach(pC =>
            {
                pC.botAIManager.aiBehavior.pausableScript.pauseFixedUpdate();
                pC.botSensor.pausableScript.pauseFixedUpdate();
            });
            GameUIManager.Instance.TurnOnMap();
            ;
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
