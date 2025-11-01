using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum GameEventType
{
    HPChange,
    Attack,
    DealDamage,
    TakeDamage,
}

[RequireComponent(typeof(HexGridManager))]
[DefaultExecutionOrder(-1)]
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
    public HexGridManager gridManager;
    List<HexGridNode> returnPath;
    public RoomSystem roomSystem;
    Dictionary<GameObject, ObjectPool> enemyPools = new();
    public int enemyCount = 0;

    /// <summary>
    /// Team 1 = Player
    /// Team 2 = Enemy
    /// </summary>
    public Dictionary<string, List<CustomMono>> teamChampions = new();

    /// <summary>
    /// Hex grid nodes where enemy stand
    /// </summary>
    public List<HexGridNode> enemyNodes = new();

    /// <summary>
    /// When clicking start battle button, this will be set to false, meaning we can't position
    /// characters around anymore. When battle finish, this will be set to true.
    /// </summary>
    public bool positioningPhase = true;
    Dictionary<CustomMono, Dictionary<GameEventType, GameEvent>> selfEvents = new();
    Dictionary<string, Dictionary<GameEventType, GameEvent>> teamBasedEvents = new();

    public static IEnumerator VoidIE()
    {
        yield return null;
    }

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
        InitGameEvents();
        AddTeam();
    }

    private void AddTeam()
    {
        teamChampions.Add("Team1", new List<CustomMono>());
        teamChampions.Add("Team2", new List<CustomMono>());
    }

    private void InitGameEvents()
    {
        teamBasedEvents.Add("Team1", new Dictionary<GameEventType, GameEvent>());
        teamBasedEvents.Add("Team2", new Dictionary<GameEventType, GameEvent>());
        teamBasedEvents["Team1"].Add(GameEventType.HPChange, new GameEvent());
        teamBasedEvents["Team2"].Add(GameEventType.HPChange, new GameEvent());
    }

    public GameEvent GetTeamBasedEvent(string p_teamName, GameEventType p_gameEventType) =>
        teamBasedEvents[p_teamName][p_gameEventType];

    void LoadOtherResources()
    {
        team1DirectionIndicatorMat = Resources.Load<Material>("Material/Team1DirectionIndicator");
        team2DirectionIndicatorMat = Resources.Load<Material>("Material/Team2DirectionIndicator");
    }

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
        var nERI = roomSystem.allNormalEnemyRooms[
            Random.Range(0, roomSystem.allNormalEnemyRooms.Count)
        ];
        GetEnemyTeamChampions().Clear();

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
            MarkGridNodeAsEnemyNode(t_customMono.transform.position);
            GetEnemyTeamChampions().Add(t_customMono);
            enemyCount++;
        }

        GetEnemyTeamChampions().ForEach(cRE => DisableBattleMode(cRE));
    }

    /// <summary>
    /// Paint enemies hex grid node as red and remember them
    /// </summary>
    /// <param name="p_pos"></param>
    void MarkGridNodeAsEnemyNode(Vector3 p_pos)
    {
        var t_node = gridManager.GetNodeAtPosition(p_pos);
        t_node.SetAsEnemyPosition();
        enemyNodes.Add(t_node);
    }

    /// <summary>
    /// Paint enemies hex grid node back to normal
    /// </summary>
    void ClearEnemyNodes()
    {
        enemyNodes.ForEach(eN => eN.ClearEnemyPosition());
        enemyNodes.Clear();
    }

    void BattleRoomStart(PointerEventData p_pED)
    {
        positioningPhase = false;
        GameUIManager.Instance.startBattleButton.Hide();
        GetEnemyTeamChampions().ForEach(cRE => EnableBattleMode(cRE));

        GetPlayerTeamChampions().ForEach(pC => EnableBattleMode(pC));
    }

    void PawnDeathHandler(CustomMono p_customMono)
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            GetPlayerTeamChampions().ForEach(pC => DisableBattleMode(pC));
            GameUIManager.Instance.TurnOnMap();
            positioningPhase = true;
            ClearEnemyNodes();
        }
    }

    public void AddCustomMono(CustomMono customMono)
    {
        customMonos.Add(customMono.combatCollider2D.GetHashCode(), customMono);
        selfEvents.Add(
            customMono,
            new()
            {
                { GameEventType.Attack, new() },
                { GameEventType.DealDamage, new() },
                { GameEventType.TakeDamage, new() },
            }
        );
        teamChampions[customMono.tag].Add(customMono);
    }

    public CustomMono GetCustomMono(Collider2D p_cld)
    {
        return customMonos.GetValueOrDefault(p_cld.GetHashCode());
    }

    public CustomMono FindLowestHPEnemy(CustomMono p_self)
    {
        float minHP = float.MaxValue;
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (
                p_self.allyTags.Contains(kvp.Value.tag)
                || kvp.Value.stat.currentHealthPoint.Value <= 0
            )
                continue;

            if (kvp.Value.stat.currentHealthPoint.Value < minHP)
            {
                minHP = kvp.Value.stat.currentHealthPoint.Value;
                t_target = kvp.Value;
            }
        }

        return t_target;
    }

    public CustomMono FindLowestMPEnemy(CustomMono p_self)
    {
        float minHP = float.MaxValue;
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (
                p_self.allyTags.Contains(kvp.Value.tag)
                || kvp.Value.stat.currentManaPoint.Value <= 0
            )
                continue;

            if (kvp.Value.stat.currentManaPoint.Value < minHP)
            {
                minHP = kvp.Value.stat.currentManaPoint.Value;
                t_target = kvp.Value;
            }
        }

        return t_target;
    }

    public CustomMono FindAliveEnemyNotInList(CustomMono self, List<CustomMono> list)
    {
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (self.allyTags.Contains(kvp.Value.tag) || !kvp.Value.stat.alive)
                continue;

            if (!list.Contains(kvp.Value))
                t_target = kvp.Value;
        }

        return t_target;
    }

    public void RemoveCustomMono(CustomMono p_customMono)
    {
        customMonos.Remove(p_customMono.combatCollider2D.GetHashCode());
        selfEvents.Remove(p_customMono);
        teamChampions[p_customMono.tag].Remove(p_customMono);
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

    public GameEvent GetSelfEvent(CustomMono p_customMono, GameEventType p_gameEventType) =>
        selfEvents[p_customMono][p_gameEventType];

    public List<CustomMono> GetPlayerTeamChampions() => teamChampions["Team1"];

    public List<CustomMono> GetEnemyTeamChampions() => teamChampions["Team2"];

    void EnableBattleMode(CustomMono customMono)
    {
        customMono.botAIManager.aiBehavior.pausableScript.resumeFixedUpdate();
        customMono.botSensor.pausableScript.resumeFixedUpdate();
    }

    void DisableBattleMode(CustomMono customMono)
    {
        customMono.botAIManager.aiBehavior.pausableScript.pauseFixedUpdate();
        customMono.botSensor.pausableScript.pauseFixedUpdate();
    }

    public string GetDescription(string p_key) => descriptionDB.GetValueOrDefault(p_key);
}
