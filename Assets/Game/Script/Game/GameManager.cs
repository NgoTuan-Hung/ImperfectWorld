using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using DG.Tweening;
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
    float defaultFixedDeltaTime = 0.02f;
    public float scaledFixedDeltaTime = 0.02f;
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
        dieBoolHash = Animator.StringToHash("Die"),
        dashBoolHash = Animator.StringToHash("Dash"),
        jumpBoolHash = Animator.StringToHash("Jump"),
        meleeStanceBoolHash = Animator.StringToHash("MeleeStance"),
        itemTierBlendHash = Animator.StringToHash("ItemTierBlend"),
        itemCycleOffsetFloatHash = Animator.StringToHash("ItemCycleOffset");
    public Dictionary<int, GameObject> colliderOwner = new();
    public List<ActionFieldInfo> actionFieldInfos = new();
    Dictionary<string, ActionFieldInfo> actionFieldInfoDict = new();
    Dictionary<ActionFieldName, Type> actionFieldMapper = new();
    public HexGridManager gridManager;
    List<HexGridNode> returnPath;
    public RoomSystem roomSystem;
    Dictionary<GameObject, ObjectPool> championPools = new();
    public int enemyCount = 0;

    /// <summary>
    /// Team 1 = Player
    /// Team 2 = Enemy
    /// </summary>
    public Dictionary<string, List<CustomMono>> teamChampions = new();
    Dictionary<CustomMono, Dictionary<GameEventType, GameEvent>> selfEvents = new();
    Dictionary<string, Dictionary<GameEventType, GameEvent>> teamBasedEvents = new();
    public Action battleEndCallback = () => { };
    public GameState gameState = GameState.MapTravelingPhase;
    public GameObject mapArea;
    List<Vector2> formationPositions = new();

    public static IEnumerator VoidIE()
    {
        yield return null;
    }

    private void Awake()
    {
        Instance = this;
        camera = Camera.main;
        gridManager = GetComponent<HexGridManager>();

        InitAllEffectPools();
        LoadOtherResources();
        InitOtherPools();
        ConstructActionFieldInfoDict();
        MapActionFieldName();
        InitGameEvents();
        InitOtherFields();
        AddTeam();
        BuildFormation();
        RegisterOtherEvents();
    }

    private void RegisterOtherEvents()
    {
        guide.interact += GuideInteraction;
        GameUIManager.Instance.guideBribeButton.pointerClickEvent += HandleGuideBribe;
    }

    private void InitOtherPools()
    {
        itemPool = new(
            item,
            new PoolArgument(ComponentType.Item, PoolArgument.WhereComponent.Self)
        );
        championRewardUIPool = new(
            championRewardUI,
            new PoolArgument(ComponentType.ChampionRewardUI, PoolArgument.WhereComponent.Self)
        );
        goldPool = new(
            gold,
            new PoolArgument(ComponentType.BasicUI, PoolArgument.WhereComponent.Self)
        );
    }

    private void InitOtherFields()
    {
        itemDataSOWeights = new float[itemDataSOs.Count];
        BuildItemDataSOWeights();
        halfInvisibleWallW = invisibleWall.transform.localScale.x / 2;
        halfInvisibleWallH = invisibleWall.transform.localScale.y / 2;
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
        damagePopupMat = Resources.Load<Material>("Material/LiberationSans SDF - Damage");
        weakenPopupMat = Resources.Load<Material>("Material/LiberationSans SDF - Weaken");
        armorBuffPopupMat = Resources.Load<Material>("Material/LiberationSans SDF - Armor Buff");
        itemTooltipPrefab = Resources.Load<GameObject>("ItemTooltip");
        championRewardSelectedEffectPreset = Resources.Load<UIEffectPreset>(
            "UIEffectPreset/ChampionRewardSelected"
        );
        rareItemEffectPreset = Resources.Load<UIEffectPreset>("UIEffectPreset/RareItem");
        epicItemEffectPreset = Resources.Load<UIEffectPreset>("UIEffectPreset/EpicItem");
        item = Resources.Load<GameObject>("Item");
        championRewardUI = Resources.Load<GameObject>("ChampionRewardUI");
        itemDataSOs = Resources.LoadAll<ItemDataSO>("ScriptableObject/ItemDataSO").ToList();
        gold = Resources.Load<GameObject>("Gold");
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
        SpawnStartingChampions();
    }

    private void BattleInitialize()
    {
        GameUIManager.Instance.gameInteractionButton.pointerClickEvent += HandleInteractionButton;
    }

    private void HandleMapInteraction()
    {
        MapPlayerTracker.Instance.onNodeEnter += (p_mapNode) =>
        {
            NewFloorReachCallback();
            switch (p_mapNode.Node.nodeType)
            {
                case NodeType.MinorEnemy:
                {
                    Debug.Log("Room Minor Enemy Encountered");
                    GameUIManager.Instance.gameInteractionButton.Show();
                    LoadNormalEnemyRoom(p_mapNode);
                    break;
                }
                case NodeType.EliteEnemy:
                    break;
                case NodeType.RestSite:
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.Store:
                {
                    LoadShopRoom();
                    break;
                }
                case NodeType.Boss:
                    break;
                case NodeType.Mystery:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            enemyItemCount++;
            enemyStatUpgradeCount++;
        };
    }

    void LoadNormalEnemyRoom(MapNode mapNode)
    {
        ChangeGameState(GameState.PositioningPhase);
        GameUIManager.Instance.TurnOffMap();
        enemyCount = 0;

        var nERI = roomSystem
            .normalEnemyFloors[mapNode.Node.point.y]
            .normalEnemyRoomInfos.GetRandomElement();
        GetEnemyTeamChampions().Clear();

        for (int i = 0; i < nERI.roomEnemyInfos.Count; i++)
        {
            if (!championPools.TryGetValue(nERI.roomEnemyInfos[i].prefab, out ObjectPool t_pool))
            {
                championPools.Add(
                    nERI.roomEnemyInfos[i].prefab,
                    new ObjectPool(
                        nERI.roomEnemyInfos[i].prefab,
                        new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                    )
                );
            }

            var t_customMono = championPools[nERI.roomEnemyInfos[i].prefab].PickOne().CustomMono;
            t_customMono.transform.position = HexGridManager
                .Instance.GetNodeAtPosition(nERI.roomEnemyInfos[i].position)
                .pos;
            t_customMono.SetupForReuse();
            t_customMono.stat.currentHealthPointReachZeroEvent += PawnDeathHandler;
            GetEnemyTeamChampions().Add(t_customMono);
            enemyCount++;
        }

        StartCoroutine(DistributeItemForEnemies());
        StartCoroutine(DistributeStatUpgradeForEnemies());
        GetEnemyTeamChampions().ForEach(cRE => DisableBattleMode(cRE));
        StartCoroutine(WaitHideAllEnemies());
    }

    void LoadShopRoom()
    {
        raft.SetActive(true);
        ChangeGameState(GameState.ShopPhase);
        GameUIManager.Instance.ChangeGameInteractionButtonShop();
        GameUIManager.Instance.TurnOffMap();
        GameUIManager.Instance.HandleTraderUI(
            GetRandomChampionRewardUIs(6),
            GetRandomItemRewards(6)
        );

        GameUIManager
            .Instance.cameraFollowObject.transform.DOMove(trader.transform.position, 1)
            .SetEase(Ease.OutQuart);
    }

    void GetOutOfShopRoom()
    {
        raft.SetActive(false);
        ChangeGameState(GameState.MapTravelingPhase);
        GameUIManager.Instance.ChangeGameInteractionButtonBattle();
        GameUIManager.Instance.TurnOnMap();
        GameUIManager.Instance.CloseTraderUI();
    }

    void HandleInteractionButton(PointerEventData p_pED)
    {
        switch (gameState)
        {
            case GameState.PositioningPhase:
            {
                ChangeGameState(GameState.BattlePhase);
                GameUIManager.Instance.gameInteractionButton.Hide();
                ShowAllEnemies();
                ShoveAsidePlayerChampion();
                GetEnemyTeamChampions().ForEach(cRE => EnableBattleMode(cRE));
                GetPlayerTeamChampions().ForEach(pC => EnableBattleMode(pC));
                HexGridManager.Instance.ClearAllOccupy();
                break;
            }
            case GameState.ShopPhase:
            {
                GetOutOfShopRoom();
                break;
            }
            default:
                break;
        }
    }

    void PawnDeathHandler(CustomMono p_customMono)
    {
        enemyCount--;
        p_customMono.stat.currentHealthPointReachZeroEvent -= PawnDeathHandler;
        SpawnGoldFromDeadEnemy(p_customMono);
        GetEnemyTeamChampions().Remove(p_customMono);
        ClearItemsForEnemy(p_customMono);
        if (enemyCount <= 0)
        {
            GetPlayerTeamChampions().ForEach(pC => DisableBattleMode(pC));
            ChangeGameState(GameState.RewardPhase);
            battleEndCallback();
            ReorganizeFormationAfterBattle();
        }
    }

    void SpawnGoldFromDeadEnemy(CustomMono customMono)
    {
        GameUIManager.Instance.SpawnGoldFromDeadEnemies(
            customMono.championData.goldDrop,
            customMono.transform.position,
            goldPool.PickOne().BasicUI
        );
    }

    public void FinishReward()
    {
        GameUIManager.Instance.TurnOnMap();
        GameUIManager.Instance.UnlockMap();
        ChangeGameState(GameState.MapTravelingPhase);
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

    public void SwithTeam(CustomMono customMono, string newTeam)
    {
        teamChampions[customMono.tag].Remove(customMono);
        customMono.allyTags.Clear();

        switch (customMono.tag)
        {
            case "Team1":
            {
                customMono.stat.currentHealthPointReachZeroEvent -= PlayerChampionDeathHandler;
                break;
            }
            case "Team2":
            {
                customMono.stat.currentHealthPointReachZeroEvent -= PawnDeathHandler;
                break;
            }
            default:
                break;
        }

        switch (newTeam)
        {
            case "Team1":
            {
                customMono.tag = "Team1";
                customMono.allyTags.Add("Team1");
                customMono.arrowIndicator.material = team1DirectionIndicatorMat;
                customMono.stat.currentHealthPointReachZeroEvent += PlayerChampionDeathHandler;
                break;
            }
            case "Team2":
            {
                customMono.tag = "Team2";
                customMono.allyTags.Add("Team2");
                customMono.arrowIndicator.material = team2DirectionIndicatorMat;
                customMono.stat.currentHealthPointReachZeroEvent += PawnDeathHandler;
                break;
            }
            default:
                break;
        }

        teamChampions[newTeam].Add(customMono);
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

    public CustomMono GetNearestAlly(CustomMono self)
    {
        float minDistance = float.MaxValue;
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (
                !self.allyTags.Contains(kvp.Value.tag)
                || !kvp.Value.stat.alive
                || kvp.Value == self
            )
                continue;

            float t_distance = (
                kvp.Value.transform.position - self.transform.position
            ).sqrMagnitude;
            if (t_distance < minDistance)
            {
                minDistance = t_distance;
                t_target = kvp.Value;
            }
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

    public CustomMono GetNearestEnemy(CustomMono self)
    {
        float minDistance = float.MaxValue;
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (self.allyTags.Contains(kvp.Value.tag) || !kvp.Value.stat.alive)
                continue;

            float t_distance = (
                kvp.Value.transform.position - self.transform.position
            ).sqrMagnitude;
            if (t_distance < minDistance)
            {
                minDistance = t_distance;
                t_target = kvp.Value;
            }
        }

        return t_target;
    }

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

    public void ChangeTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        scaledFixedDeltaTime = defaultFixedDeltaTime / Time.timeScale;
    }

    public void AddNormalEnemyRoom(NormalEnemyRoomInfo normalEnemyRoomInfo, int floor)
    {
        while (roomSystem.normalEnemyFloors.Count <= floor)
            roomSystem.normalEnemyFloors.Add(new());

        roomSystem.normalEnemyFloors[floor].normalEnemyRoomInfos.Add(normalEnemyRoomInfo);
    }

    void ShowRewardForPlayer()
    {
        GameUIManager.Instance.SpawnReward();
    }

    /// <summary>
    /// Reservoir Sampling for selecting count stat upgrades
    /// </summary>
    public List<StatUpgrade> GetRandomStatUpgrades(int count)
    {
        if (statUpgrades.Count < count)
            return null;

        List<StatUpgrade> sUs = new();

        for (int i = 0; i < count; i++)
            sUs.Add(statUpgrades[i]);
        for (int i = count; i < statUpgrades.Count; i++)
        {
            int r = Random.Range(0, i + 1);
            if (r < count)
                sUs[r] = statUpgrades[i];
        }

        return sUs;
    }

    public List<ChampionReward> GetRandomChampionRewards(int count)
    {
        if (championRewards.Count < count)
            return null;

        List<ChampionReward> cRs = new();

        for (int i = 0; i < count; i++)
            cRs.Add(championRewards[i]);
        for (int i = count; i < championRewards.Count; i++)
        {
            int r = Random.Range(0, i + 1);
            if (r < count)
                cRs[r] = championRewards[i];
        }

        return cRs;
    }

    public List<ChampionRewardUI> GetRandomChampionRewardUIs(int count)
    {
        if (championRewards.Count < count)
            return null;

        List<ChampionReward> cRs = new();

        for (int i = 0; i < count; i++)
            cRs.Add(championRewards[i]);
        for (int i = count; i < championRewards.Count; i++)
        {
            int r = Random.Range(0, i + 1);
            if (r < count)
                cRs[r] = championRewards[i];
        }

        List<ChampionRewardUI> cRUs = new();

        for (int i = 0; i < cRs.Count; i++)
        {
            cRUs.Add(championRewardUIPool.PickOne().ChampionRewardUI);
            cRUs[i].Init(cRs[i]);
        }

        return cRUs;
    }

    public List<Item> GetRandomItemRewards(int count)
    {
        List<int> pickedItemIndexes = WeightedSampler.SampleUnique(count, itemDataSOWeights);

        List<Item> items = new();
        for (int i = 0; i < count; i++)
        {
            items.Add(itemPool.PickOne().Item);
            items[i].Init(itemDataSOs[pickedItemIndexes[i]]);
        }

        return items;
    }

    public void UpgradeStat(CustomMono customMono, StatUpgrade statUpgrade)
    {
        AddBuff(customMono, statUpgrade.statBuff);
    }

    public void AddBuff(CustomMono customMono, StatBuff statBuff)
    {
        switch (statBuff.statBuffType)
        {
            case StatBuffType.HP:
                customMono.stat.healthPoint.AddModifier(statBuff.modifier);
                customMono.stat.currentHealthPoint.Value += statBuff.modifier.value;
                break;
            case StatBuffType.MP:
                customMono.stat.manaPoint.AddModifier(statBuff.modifier);
                customMono.stat.currentManaPoint.Value += statBuff.modifier.value;
                break;
            case StatBuffType.MIGHT:
            {
                var oldHP = customMono.stat.healthPoint.FinalValue;
                customMono.stat.might.AddModifier(statBuff.modifier);
                customMono.stat.currentHealthPoint.Value +=
                    customMono.stat.healthPoint.FinalValue - oldHP;
                break;
            }
            case StatBuffType.REFLEX:
                customMono.stat.reflex.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.WISDOM:
            {
                var oldMP = customMono.stat.manaPoint.FinalValue;
                customMono.stat.wisdom.AddModifier(statBuff.modifier);
                customMono.stat.currentManaPoint.Value +=
                    customMono.stat.manaPoint.FinalValue - oldMP;
                break;
            }
            case StatBuffType.ATK:
                customMono.stat.attackDamage.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.ASPD:
                customMono.stat.attackSpeed.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.ARMOR:
                customMono.stat.armor.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.HPREGEN:
                customMono.stat.healthRegen.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.MPREGEN:
                customMono.stat.manaRegen.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.MSPD:
                customMono.stat.moveSpeed.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.DMGMOD:
                customMono.stat.damageModifier.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.OMNIVAMP:
                customMono.stat.omnivamp.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.CRIT:
                customMono.stat.critChance.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.CRITMOD:
                customMono.stat.critDamageModifier.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.DMGREDUC:
                customMono.stat.damageReduction.AddModifier(statBuff.modifier);
                break;
            case StatBuffType.ATKRANGE:
                customMono.stat.attackRange.AddModifier(statBuff.modifier);
                break;
            default:
                break;
        }
    }

    public void RemoveBuff(CustomMono customMono, StatBuff statBuff)
    {
        switch (statBuff.statBuffType)
        {
            case StatBuffType.HP:
                customMono.stat.healthPoint.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.MP:
                customMono.stat.manaPoint.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.MIGHT:
                customMono.stat.might.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.REFLEX:
                customMono.stat.reflex.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.WISDOM:
                customMono.stat.wisdom.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.ATK:
                customMono.stat.attackDamage.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.ASPD:
                customMono.stat.attackSpeed.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.ARMOR:
                customMono.stat.armor.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.HPREGEN:
                customMono.stat.healthRegen.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.MPREGEN:
                customMono.stat.manaRegen.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.MSPD:
                customMono.stat.moveSpeed.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.DMGMOD:
                customMono.stat.damageModifier.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.OMNIVAMP:
                customMono.stat.omnivamp.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.CRIT:
                customMono.stat.critChance.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.CRITMOD:
                customMono.stat.critDamageModifier.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.DMGREDUC:
                customMono.stat.damageReduction.RemoveModifier(statBuff.modifier);
                break;
            case StatBuffType.ATKRANGE:
                customMono.stat.attackRange.RemoveModifier(statBuff.modifier);
                break;
            default:
                break;
        }
    }

    public void SacrificeChampionRewardAsStat(ChampionData offering, CustomMono recipient)
    {
        var hp = recipient.stat.healthPoint.FinalValue;
        var offer = offering.GetPrecomputeData();

        recipient.stat.healthPoint.AddModifier(new(offer.offerHealthPoint, ModifierType.Additive));
        recipient.stat.healthRegen.AddModifier(new(offer.offerHealthRegen, ModifierType.Additive));
        recipient.stat.manaPoint.AddModifier(new(offer.offerManaPoint, ModifierType.Additive));
        recipient.stat.manaRegen.AddModifier(new(offer.offerManaRegen, ModifierType.Additive));
        recipient.stat.might.AddModifier(new(offer.offerMight, ModifierType.Additive));
        recipient.stat.reflex.AddModifier(new(offer.offerReflex, ModifierType.Additive));
        recipient.stat.wisdom.AddModifier(new(offer.offerWisdom, ModifierType.Additive));
        recipient.stat.attackSpeed.AddModifier(new(offer.offerAttackSpeed, ModifierType.Additive));
        recipient.stat.armor.AddModifier(new(offer.offerArmor, ModifierType.Additive));
        recipient.stat.moveSpeed.AddModifier(new(offer.offerMoveSpeed, ModifierType.Additive));
        recipient.stat.damageModifier.AddModifier(
            new(offer.offerDamageModifier, ModifierType.Additive)
        );
        recipient.stat.omnivamp.AddModifier(new(offer.offerOmnivamp, ModifierType.Additive));
        recipient.stat.attackDamage.AddModifier(
            new(offer.offerAttackDamage, ModifierType.Additive)
        );
        recipient.stat.critChance.AddModifier(new(offer.offerCritChance, ModifierType.Additive));
        recipient.stat.critDamageModifier.AddModifier(
            new(offer.offerCritDamageModifier, ModifierType.Additive)
        );
        recipient.stat.damageReduction.AddModifier(
            new(offer.offerDamageReduction, ModifierType.Additive)
        );
        recipient.stat.attackRange.AddModifier(new(offer.offerAttackRange, ModifierType.Additive));

        recipient.stat.currentHealthPoint.Value += recipient.stat.healthPoint.FinalValue - hp;
        recipient
            .skill.skillBases[1]
            .GetActionField<ActionFloatField>(ActionFieldName.Range)
            .value += offering.attackRange * 0.05f;
    }

    void ChangeGameState(GameState newState)
    {
        gameState = newState;
        onGameStateChange(newState);
    }

    void BuildItemDataSOWeights()
    {
        for (int i = 0; i < itemDataSOs.Count; i++)
        {
            switch (itemDataSOs[i].itemTier)
            {
                case ItemTier.Normal:
                    itemDataSOWeights[i] = 1;
                    break;
                case ItemTier.Rare:
                    itemDataSOWeights[i] = 0.3f;
                    break;
                case ItemTier.Epic:
                    itemDataSOWeights[i] = 0.1f;
                    break;
                case ItemTier.Legendary:
                    itemDataSOWeights[i] = 0.03f;
                    break;
                default:
                    break;
            }
        }
    }

    public CustomMono RewardChampion(GameObject champPrefab)
    {
        if (champPrefab == null)
            return null;

        if (!championPools.TryGetValue(champPrefab, out ObjectPool t_pool))
        {
            championPools.Add(
                champPrefab,
                new ObjectPool(
                    champPrefab,
                    new PoolArgument(ComponentType.CustomMono, PoolArgument.WhereComponent.Self)
                )
            );

            t_pool = championPools[champPrefab];
        }

        var t_customMono = t_pool.PickOne().CustomMono;
        t_customMono.transform.position = new Vector3(0, 0, 0);
        t_customMono.SetupForReuse();
        SwithTeam(t_customMono, "Team1");
        DisableBattleMode(t_customMono);
        FallIn(t_customMono);

        return t_customMono;
    }

    void BuildFormation()
    {
        Vector2 upperRight = mapArea.transform.localScale / 2;
        Vector2 upperLeft = new(-upperRight.x + 1, upperRight.y - 1);
        Vector2 lowerRight = new(upperRight.x - 1, -upperRight.y);

        var pointer = upperLeft;
        int level = 0;
        while (pointer.y >= lowerRight.y)
        {
            formationPositions.Add(pointer);
            pointer.x += 2;
            if (pointer.x >= lowerRight.x)
            {
                pointer.x = level % 2 == 0 ? upperLeft.x : upperLeft.x + 1;
                pointer.y--;
                level++;
            }
        }
    }

    public void ReorganizeFormationAfterBattle()
    {
        formationIndex = 0;
        GameUIManager
            .Instance.cameraFollowObject.transform.DOMove(formationPositions[formationIndex], 1)
            .SetEase(Ease.OutQuad)
            .OnComplete(ShowRewardForPlayer);
        GetPlayerTeamChampions()
            .ForEach(c =>
            {
                if (c.stat.alive)
                    FallIn(c);
            });
    }

    void FallIn(CustomMono customMono)
    {
        var finalPos = formationPositions[formationIndex];
        HexGridManager.Instance.SetOccupiedNode(customMono, finalPos);
        championTeleportEffectPool.PickOne().gameObject.transform.position = customMono
            .transform
            .position;
        formationIndex++;
    }

    public bool BuyChampion(ChampionRewardUI championRewardUI)
    {
        if (playerGold >= championRewardUI.rewardCD.price)
        {
            UpdateGold(-championRewardUI.rewardCD.price);
            RewardChampion(championRewardUI.championReward.prefab);
            return true;
        }
        else
            return false;
    }

    public bool BuyItem(Item item)
    {
        if (playerGold >= item.itemDataSO.price)
        {
            UpdateGold(-item.itemDataSO.price);
            item.GetBought();
            return true;
        }
        else
            return false;
    }

    public void UpdateGold(int gold)
    {
        playerGold += gold;
        GameUIManager.Instance.UpdatePlayerGold(playerGold);
    }

    public bool BuyWithValue(int gold)
    {
        if (playerGold >= gold)
        {
            UpdateGold(-gold);
            return true;
        }
        else
            return false;
    }

    public bool CheckInsideGlobalWall(Vector2 point)
    {
        return (
            point.x >= -halfInvisibleWallW
            && point.x <= halfInvisibleWallW
            && point.y >= -halfInvisibleWallH
            && point.y <= halfInvisibleWallH
        );
    }
}
