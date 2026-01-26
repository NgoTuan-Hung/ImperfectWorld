using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public enum GameState
{
    MainMenu,
    MapTravelingPhase,
    PositioningPhase,
    BattlePhase,
    RewardPhase,
    ShopPhase,
    MysteryEventPhase,
    RestSitePhase,
}

public enum ComplexTextID
{
    PASSIVE,
    ACTIVE,
    POSITIVENUMBER,
    CURHP,
    HP,
    HPREGEN,
    CURMP,
    MP,
    MPREGEN,
    MIGHT,
    REFLEX,
    WISDOM,
    ASPD,
    ARMOR,
    MSPD,
    DMGMOD,
    OMNIVAMP,
    ATK,
    CRIT,
    CRITMOD,
    DMGREDUC,
    ATKRANGE,
    STRIKELOCK,
    BUFF,
    DEBUFF,
    SUMMON,
    MOVED,
    GOLD,
    ONATTACK,
    ONCAST,
    ONTAKEDMG,
    ONDEALDMG,
}

public partial class GameManager
{
    TakeDamageGameEventData takeDamageGameEventData = new(0);
    DealDamageGameEventData dealDamageGameEventData = new();
    bool isRestSiteHandled = false;
    public int playerGold;
    public Material team1DirectionIndicatorMat;
    public Material team2DirectionIndicatorMat;
    public Material damagePopupMat,
        weakenPopupMat,
        armorBuffPopupMat;
    Dictionary<string, string> descriptionDB = new()
    {
        {
            GetComplexTextIDAsText(ComplexTextID.CURHP),
            ConstructComplexText(ComplexTextID.CURHP) + ": current health point."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.HP),
            ConstructComplexText(ComplexTextID.HP) + ": health point."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.HPREGEN),
            ConstructComplexText(ComplexTextID.HPREGEN)
                + ": how many health point is regenerated per second."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.CURMP),
            ConstructComplexText(ComplexTextID.CURMP) + ": current mana point."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.MP),
            ConstructComplexText(ComplexTextID.MP)
                + ": mana point, champion use active skill when current mana point reach mana point."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.MPREGEN),
            ConstructComplexText(ComplexTextID.MPREGEN)
                + ": how many mana point is regenerated per second."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.MIGHT),
            ConstructComplexText(ComplexTextID.MIGHT)
                + ": increases health point by 15 and health point regeneration by 0.1."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.REFLEX),
            ConstructComplexText(ComplexTextID.REFLEX)
                + ": increases armor by 0.17 and attack speed by 0.01."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.WISDOM),
            ConstructComplexText(ComplexTextID.WISDOM)
                + ": decreases mana point by 0.25 and Increases mana point regeneration by 1.5."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ASPD),
            ConstructComplexText(ComplexTextID.ASPD)
                + ": attack speed, 1 aspd mean 1 attack per second."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ARMOR),
            ConstructComplexText(ComplexTextID.ARMOR) + ": 1 armor mitigates 1 damage."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.MSPD),
            ConstructComplexText(ComplexTextID.MSPD) + ": move speed."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.DMGMOD),
            ConstructComplexText(ComplexTextID.DMGMOD)
                + ": damage multiplier, apply to all damage dealt."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.OMNIVAMP),
            ConstructComplexText(ComplexTextID.OMNIVAMP)
                + ": how much damage dealt is converted to healing self, 1 omnivamp heal for 100% of damage dealt."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ATK),
            ConstructComplexText(ComplexTextID.ATK) + ": attack damage."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.CRIT),
            ConstructComplexText(ComplexTextID.CRIT) + ": crit chance."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.CRITMOD),
            ConstructComplexText(ComplexTextID.CRITMOD)
                + ": crit damage modifier, how much damage is multiplied on crit."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.DMGREDUC),
            ConstructComplexText(ComplexTextID.DMGREDUC)
                + ": reduces total damage taken, applied after armor."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ATKRANGE),
            ConstructComplexText(ComplexTextID.ATKRANGE) + ": attack range."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.STRIKELOCK),
            ConstructComplexText(ComplexTextID.STRIKELOCK)
                + ": one affected by strike lock cannot attack."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.BUFF),
            ConstructComplexText(ComplexTextID.BUFF)
                + ": A positive effect that benefits the character."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.DEBUFF),
            ConstructComplexText(ComplexTextID.DEBUFF)
                + ": A negative effect that hinders the character."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ONATTACK),
            ConstructComplexText(ComplexTextID.ONATTACK) + ": Triggered when character attack."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ONCAST),
            ConstructComplexText(ComplexTextID.ONCAST)
                + ": Triggered when character cast main ability."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ONTAKEDMG),
            ConstructComplexText(ComplexTextID.ONTAKEDMG)
                + ": Triggered when character take damage."
        },
        {
            GetComplexTextIDAsText(ComplexTextID.ONDEALDMG),
            ConstructComplexText(ComplexTextID.ONDEALDMG)
                + ": Triggered when character deal damage."
        },
    };
    Dictionary<ItemBehaviourType, Type> itemBehaviourMapper = new()
    {
        { ItemBehaviourType.KuraiKōraBehaviour, typeof(KuraiKōraBehaviour) },
        { ItemBehaviourType.PhoenixHeartBehaviour, typeof(PhoenixHeartBehaviour) },
        { ItemBehaviourType.MoonCleaverBehaviour, typeof(MoonCleaverBehaviour) },
        { ItemBehaviourType.BlinkspineScepterBehaviour, typeof(BlinkspineScepterBehaviour) },
        { ItemBehaviourType.MercuryGraspBehaviour, typeof(MercuryGraspBehaviour) },
    };

    public Type MapItemBehaviour(ItemBehaviourType type) => itemBehaviourMapper[type];

    public List<StatUpgrade> statUpgrades = new();
    public List<ChampionReward> championRewards = new();
    public UIEffectPreset championRewardSelectedEffectPreset,
        rareItemEffectPreset,
        epicItemEffectPreset,
        legendaryItemEffectPreset;
    public float largePositiveNumber = 999999f;

    /* Complex Text Colors */
    public const string passiveColor = "#3498DB";
    public const string activeColor = "#E67E22";
    public const string positiveNumber = "#00FF00";
    public const string currentHealthPointColor = "#D72638";
    public const string healthPointColor = "#C71F37";
    public const string healthRegenColor = "#FF6B6B";
    public const string currentManaPointColor = "#3E8EDE";
    public const string manaPointColor = "#2F75C0";
    public const string manaRegenColor = "#7FDBFF";
    public const string mightColor = "#F39C12";
    public const string reflexColor = "#27AE60";
    public const string wisdomColor = "#9B59B6";
    public const string attackSpeedColor = "#E67E22";
    public const string armorColor = "#95A5A6";
    public const string moveSpeedColor = "#1ABC9C";
    public const string damageModifierColor = "#FFC107";
    public const string omnivampColor = "#C62828";
    public const string attackDamageColor = "#E53935";
    public const string critChanceColor = "#FFD54F";
    public const string critDamageModifierColor = "#AB47BC";
    public const string damageReductionColor = "#4A90E2";
    public const string attackRangeColor = "#FFD447";
    public const string strikeLockColor = "#fc03d7";
    public const string buffColor = "#4CAF50";
    public const string debuffColor = "#E53935";
    public const string summonColor = "#9B6BFF";
    public const string movedColor = "#2ECCB0";
    public const string goldColor = "#F1C40F";
    public const string onAttackColor = "#E04646";
    public const string onCastColor = "#4AA8FF";
    public const string onTakeDamageColor = "#F2A93B";
    public const string onDealDamageColor = "#D64550";
    static Dictionary<ComplexTextID, string> linkDict = new()
    {
        { ComplexTextID.PASSIVE, "PASSIVE" },
        { ComplexTextID.ACTIVE, "ACTIVE" },
        { ComplexTextID.POSITIVENUMBER, "POSITIVENUMBER" },
        { ComplexTextID.CURHP, "CURHP" },
        { ComplexTextID.HP, "HP" },
        { ComplexTextID.HPREGEN, "HPREGEN" },
        { ComplexTextID.CURMP, "CURMP" },
        { ComplexTextID.MP, "MP" },
        { ComplexTextID.MPREGEN, "MPREGEN" },
        { ComplexTextID.MIGHT, "MIGHT" },
        { ComplexTextID.REFLEX, "REFLEX" },
        { ComplexTextID.WISDOM, "WISDOM" },
        { ComplexTextID.ASPD, "ASPD" },
        { ComplexTextID.ARMOR, "ARMOR" },
        { ComplexTextID.MSPD, "MSPD" },
        { ComplexTextID.DMGMOD, "DMGMOD" },
        { ComplexTextID.OMNIVAMP, "OMNIVAMP" },
        { ComplexTextID.ATK, "ATK" },
        { ComplexTextID.CRIT, "CRIT" },
        { ComplexTextID.CRITMOD, "CRITMOD" },
        { ComplexTextID.DMGREDUC, "DMGREDUC" },
        { ComplexTextID.ATKRANGE, "ATKRANGE" },
        { ComplexTextID.STRIKELOCK, "STRIKELOCK" },
        { ComplexTextID.BUFF, "BUFF" },
        { ComplexTextID.DEBUFF, "DEBUFF" },
        { ComplexTextID.SUMMON, "SUMMON" },
        { ComplexTextID.MOVED, "MOVED" },
        { ComplexTextID.GOLD, "GOLD" },
        { ComplexTextID.ONATTACK, "ONATTACK" },
        { ComplexTextID.ONCAST, "ONCAST" },
        { ComplexTextID.ONTAKEDMG, "ONTAKEDMG" },
        { ComplexTextID.ONDEALDMG, "ONDEALDMG" },
    };

    static Dictionary<ComplexTextID, string> colorDict = new()
    {
        { ComplexTextID.PASSIVE, passiveColor },
        { ComplexTextID.ACTIVE, activeColor },
        { ComplexTextID.POSITIVENUMBER, positiveNumber },
        { ComplexTextID.CURHP, currentHealthPointColor },
        { ComplexTextID.HP, healthPointColor },
        { ComplexTextID.HPREGEN, healthRegenColor },
        { ComplexTextID.CURMP, currentManaPointColor },
        { ComplexTextID.MP, manaPointColor },
        { ComplexTextID.MPREGEN, manaRegenColor },
        { ComplexTextID.MIGHT, mightColor },
        { ComplexTextID.REFLEX, reflexColor },
        { ComplexTextID.WISDOM, wisdomColor },
        { ComplexTextID.ASPD, attackSpeedColor },
        { ComplexTextID.ARMOR, armorColor },
        { ComplexTextID.MSPD, moveSpeedColor },
        { ComplexTextID.DMGMOD, damageModifierColor },
        { ComplexTextID.OMNIVAMP, omnivampColor },
        { ComplexTextID.ATK, attackDamageColor },
        { ComplexTextID.CRIT, critChanceColor },
        { ComplexTextID.CRITMOD, critDamageModifierColor },
        { ComplexTextID.DMGREDUC, damageReductionColor },
        { ComplexTextID.ATKRANGE, attackRangeColor },
        { ComplexTextID.STRIKELOCK, strikeLockColor },
        { ComplexTextID.BUFF, buffColor },
        { ComplexTextID.DEBUFF, debuffColor },
        { ComplexTextID.SUMMON, summonColor },
        { ComplexTextID.MOVED, movedColor },
        { ComplexTextID.GOLD, goldColor },
        { ComplexTextID.ONATTACK, onAttackColor },
        { ComplexTextID.ONCAST, onCastColor },
        { ComplexTextID.ONTAKEDMG, onTakeDamageColor },
        { ComplexTextID.ONDEALDMG, onDealDamageColor },
    };

    public static string ConstructComplexText(ComplexTextID id, string innerText) =>
        $"<link={linkDict[id]}><color={colorDict[id]}>{linkDict[id]}{innerText}</color></link>";

    public static string ConstructComplexText(ComplexTextID id) =>
        $"<link={linkDict[id]}><color={colorDict[id]}>{linkDict[id]}</color></link>";

    public static string ConstructColoredText(ComplexTextID id, string innerText) =>
        $"<color={colorDict[id]}>{innerText}</color>";

    public static string GetComplexTextIDAsText(ComplexTextID id) => linkDict[id];

    public static List<string> GetAllIDsAsText() => linkDict.Values.ToList();

    public Action<GameState> onGameStateChange = (newState) => { };
    public List<ItemDataSO> itemRewardSOs;
    public List<ItemDataSO> normalItemDataSOs,
        rareItemDataSOs,
        epicItemDataSOs,
        legendaryItemDataSOs;
    public float[] itemRewardSOWeights;
    public int formationIndex = 0;
    public GameObject raft;
    public Vector3 screenSpaceToWorldSpaceUIScale;
    public GameObject invisibleWall;
    float halfInvisibleWallW,
        halfInvisibleWallH;
    public Trader trader;
    public ListSpriteSO goldSprite;

    public Sprite GetRandomGoldSprite() =>
        goldSprite.sprites[Random.Range(0, goldSprite.sprites.Count)];

    public int enemyItemCount = 0,
        enemyStatUpgradeCount = 0;
    public NPC campfire;

    IEnumerator DistributeItemForEnemies()
    {
        /* Wait for UI to finish in CustomMono Start,
        store this value since it might change next frame */
        int itemToDistribute = enemyItemCount;
        yield return null;

        List<Item> items = GetRandomItemRewards(itemToDistribute);
        List<CustomMono> enemiesToDistribute = new(GetEnemyTeamChampions());

        items.ForEach(item =>
        {
            var randomEnemy = enemiesToDistribute[Random.Range(0, enemiesToDistribute.Count)];
            while (!randomEnemy.stat.EquipItem(item))
            {
                enemiesToDistribute.Remove(randomEnemy);
                if (enemiesToDistribute.Count == 0)
                    return;
                randomEnemy = enemiesToDistribute[Random.Range(0, enemiesToDistribute.Count)];
            }
        });
    }

    void ClearItemsForEnemy(CustomMono customMono)
    {
        var items = customMono.stat.equippedItems;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            RemoveItemFromChampionAndScheduleDestroy(customMono, item);
        }
    }

    public void RemoveItemFromChampionAndScheduleDestroy(CustomMono customMono, Item item)
    {
        customMono.stat.UnEquipItem(item);
        item.deactivate();
    }

    public void PutChampionItemToInventory(CustomMono customMono, Item item)
    {
        customMono.stat.UnEquipItem(item);
        GameUIManager.Instance.AddToInventory(item);
        item.SetAsInventory();
    }

    IEnumerator DistributeStatUpgradeForEnemies()
    {
        int statUpgradeToDistribute = enemyStatUpgradeCount;
        yield return null;

        List<StatUpgrade> statUpgrades = GetRandomStatUpgradesCanDuplicate(statUpgradeToDistribute);
        var enemies = GetEnemyTeamChampions();

        statUpgrades.ForEach(statUpgrade =>
            UpgradeStat(enemies[Random.Range(0, enemies.Count)], statUpgrade)
        );
    }

    public List<StatUpgrade> GetRandomStatUpgradesCanDuplicate(int count)
    {
        List<StatUpgrade> sUs = new();

        for (int i = 0; i < count; i++)
            sUs.Add(statUpgrades[Random.Range(0, statUpgrades.Count)]);

        return sUs;
    }

    public void PlayerChampionDeathHandler(CustomMono customMono)
    {
        ReturnItemToInventoryOnDeath(customMono);
    }

    public void ReturnItemToInventoryOnDeath(CustomMono customMono)
    {
        for (int i = customMono.stat.equippedItems.Count - 1; i >= 0; i--)
            PutChampionItemToInventory(customMono, customMono.stat.equippedItems[i]);
    }

    public List<GameObject> startingChampions = new();

    public void SpawnStartingChampions()
    {
        startingChampions.ForEach(champion => RewardChampion(champion));
    }

    public Dictionary<int, NPC> nPCs = new();

    public void AddNPC(NPC nPC)
    {
        nPCs.Add(nPC.boxCollider2D.GetHashCode(), nPC);
    }

    public NPC GetNPC(Collider2D collider2D)
    {
        return nPCs.GetValueOrDefault(collider2D.GetHashCode());
    }

    public NPC guide;
    public Color transparentWhite = new(1, 1, 1, 0.5f),
        transparentRed = new(1, 0, 0, 0.5f);

    void RevealAllEnemies()
    {
        GetEnemyTeamChampions().ForEach(enemy => enemy.Reveal());
    }

    void ShowAllEnemies()
    {
        GetEnemyTeamChampions()
            .ForEach(enemy =>
            {
                enemy.Show();
                var showupEffect = enemyShowupEffectPool.PickOne();
                showupEffect.gameObject.transform.position = enemy
                    .rotationAndCenterObject
                    .transform
                    .position;
            });
    }

    bool bribed = false;

    void GuideInteraction()
    {
        if (!bribed && gameState == GameState.PositioningPhase)
        {
            GameUIManager.Instance.ShowGuideDialogBox();
        }
    }

    void HandleGuideBribe(PointerEventData pointerEventData)
    {
        if (BuyWithValue(25))
            RevealAllEnemies();

        GameUIManager.Instance.GuideBribeSuccess();
        bribed = true;
    }

    void NewFloorReachCallback()
    {
        bribed = false;
        shoveTurn++;
        ApplyRelicsEffect();
    }

    void SetOccupyNodeForPlayerChampion()
    {
        GetPlayerTeamChampions()
            .ForEach(c => HexGridManager.Instance.SetOccupiedNode(c, c.transform.position));
    }

    int shoveTurn = 0;

    void ShoveAsidePlayerChampion()
    {
        GetEnemyTeamChampions()
            .ForEach(e =>
            {
                var node = HexGridManager.Instance.GetNodeAtPosition(e.transform.position);
                if (HexGridManager.Instance.IsOccupied(node))
                {
                    var playerChamp = HexGridManager.Instance.GetOccupier(node);
                    HexGridManager.Instance.RemoveOccupy(node);
                    HexGridManager.Instance.SetOccupiedNode(e, node);

                    bool shoved = false;
                    foreach (var n in node.neighbors)
                    {
                        if (
                            n.type != HexGridNodeType.Obstacle
                            && !HexGridManager.Instance.IsOccupied(n)
                        )
                        {
                            HexGridManager.Instance.SetOccupiedNode(playerChamp, n);
                            shoved = true;
                            break;
                        }
                    }

                    if (!shoved)
                        HexGridManager.Instance.SetOccupyNextAvailable(playerChamp);
                }
            });
    }

    void ShoveAsideEnemyChampion()
    {
        GetEnemyTeamChampions()
            .ForEach(e =>
            {
                var node = HexGridManager.Instance.GetNodeAtPosition(e.transform.position);
                if (HexGridManager.Instance.IsOccupied(node))
                    HexGridManager.Instance.RemoveOccupy(node);
                HexGridManager.Instance.SetOccupiedNode(e, node);
            });

        GetPlayerTeamChampions()
            .ForEach(a =>
            {
                var node = HexGridManager.Instance.GetNodeAtPosition(a.transform.position);
                if (HexGridManager.Instance.IsOccupied(node))
                {
                    var enemy = HexGridManager.Instance.GetOccupier(node);
                    HexGridManager.Instance.RemoveOccupy(node);
                    HexGridManager.Instance.SetOccupiedNode(a, node);

                    bool shoved = false;
                    foreach (var n in node.neighbors)
                    {
                        if (
                            n.type != HexGridNodeType.Obstacle
                            && !HexGridManager.Instance.IsOccupied(n)
                        )
                        {
                            HexGridManager.Instance.SetOccupiedNode(enemy, n);
                            shoved = true;
                            break;
                        }
                    }

                    if (!shoved)
                        HexGridManager.Instance.SetOccupyNextAvailable(enemy);
                }
            });
    }

    List<MysteryEventDataSO> mysteryEventDataSOs;

    public void LoadMysteryEventRoom()
    {
        ChangeGameState(GameState.MysteryEventPhase);
        GameUIManager.Instance.TurnOffMap();
        var mysteryEventDataSO = mysteryEventDataSOs[Random.Range(0, mysteryEventDataSOs.Count)];
        GameUIManager.Instance.ShowMysteryEvent(mysteryEventDataSO);
    }

    public void ExitMysteryEventRoom()
    {
        ChangeGameState(GameState.MapTravelingPhase);
        GameUIManager.Instance.TurnOnMap();
        GameUIManager.Instance.CloseMysteryEvent();
    }

    public void HealAllPlayerAlliesByPercentage(float ammount)
    {
        if (ammount < 0 || ammount > 1)
            return;

        GetPlayerTeamChampions()
            .ForEach(c => c.statusEffect.Heal(ammount * c.stat.healthPoint.FinalValue));
    }

    public void DamageAllPlayerAlliesByPercentage(float ammount)
    {
        // TODO
    }

    Dictionary<ERelicBehavior, Type> behaviorMapper = new()
    {
        { ERelicBehavior.BloodWingBlessingBehavior, typeof(BloodWingBlessingBehavior) },
        { ERelicBehavior.None, null },
    };

    public Type GetRelicBehaviorType(ERelicBehavior behavior) => behaviorMapper[behavior];

    public void MysteryEventChoiceSelectedCallback(
        int choiceIndex,
        MysteryEventDataSO mysteryEventDataSO
    )
    {
        switch (mysteryEventDataSO.eventType)
        {
            case MysteryEventType.TreasureUnderTheSea:
                HandleTreasureUnderTheSeaEvent(choiceIndex);
                break;
            default:
                break;
        }
    }

    public List<RelicDataSO> relicDataSOs;

    void HandleTreasureUnderTheSeaEvent(int choice)
    {
        GameUIManager.Instance.HideAllEventChoices();
        switch (choice)
        {
            case 0:
                if (Random.Range(0, 1f) < 0.25f)
                {
                    var relic = relicPool
                        .PickOne()
                        .Relic.Setup(relicDataSOs.First(r => r.name.Equals("BloodWingBlessing")));

                    playerRelics.Add(relic);
                    GameUIManager.Instance.RewardRelicFromEvent(relic, ExitMysteryEventRoom);
                    DamageAllPlayerAlliesByPercentage(0.1f);
                    GameUIManager.Instance.TakeDamageFromEvent(null);
                }
                else
                {
                    DamageAllPlayerAlliesByPercentage(0.1f);
                    GameUIManager.Instance.TakeDamageFromEvent(ExitMysteryEventRoom);
                }
                break;
            case 1:
                ExitMysteryEventRoom();
                break;
        }
    }

    public Color relicTooltipColor = ColorExtension.FromHex("#00B2FF"),
        itemTooltipColor = ColorExtension.FromHex("#5E00FF");
    List<Relic> playerRelics = new();

    void ApplyRelicsEffect()
    {
        playerRelics.ForEach(r =>
        {
            if (r.relicDataSO.relicType == RelicType.PerFloor)
            {
                r.relicBehavior.PerFloorCallback();
            }
        });
    }

    public void AddEliteRoom(EnemyRoomInfo enemyRoomInfo)
    {
        roomSystem.eliteEnemyRoomInfos.Add(enemyRoomInfo);
    }

    public void AddBossRoom(EnemyRoomInfo enemyRoomInfo)
    {
        roomSystem.bossRoomInfos.Add(enemyRoomInfo);
    }

    public CustomMono FindLowestHPAlly(CustomMono self, float range)
    {
        float minHP = float.MaxValue;
        CustomMono t_target = null;
        foreach (var kvp in customMonos)
        {
            if (
                !self.allyTags.Contains(kvp.Value.tag)
                || !kvp.Value.stat.alive
                || kvp.Value == self
                || Vector2.Distance(kvp.Value.transform.position, self.transform.position) > range
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

    void LoadRestSiteRoom()
    {
        ChangeGameState(GameState.RestSitePhase);
        GameUIManager.Instance.ChangeGameInteractionButtonRestSite();
        GameUIManager.Instance.TurnOffMap();

        ShowCampfire();
        isRestSiteHandled = false;
    }

    void ExitResiteRoom()
    {
        HideCampfire();
        ChangeGameState(GameState.MapTravelingPhase);
        GameUIManager.Instance.TurnOnMap();
    }

    public void ShowCampfire()
    {
        campfire.gameObject.SetActive(true);
    }

    public void HideCampfire()
    {
        campfire.gameObject.SetActive(false);
    }

    void CampfireInteraction()
    {
        if (!isRestSiteHandled)
        {
            GameUIManager.Instance.ShowCampfireDialogBox();
        }
    }

    public void DisableCampfireInteraction()
    {
        isRestSiteHandled = true;
    }

    public float ResolveDamage(CustomMono attacker, CustomMono target, float damage)
    {
        var finalDamage =
            Math.Clamp(
                attacker.stat.CalculateDamageWithAppliedModifier(damage)
                    - target.stat.armor.FinalValue,
                1f,
                float.MaxValue
            ) * (1 - target.stat.damageReduction.FinalValue);

        target.statusEffect.GetHit(finalDamage);

        takeDamageGameEventData.Setup(finalDamage);
        GetSelfEvent(target, GameEventType.TakeDamage).action(takeDamageGameEventData);

        dealDamageGameEventData.Setup(attacker, target, finalDamage);
        GetSelfEvent(attacker, GameEventType.DealDamage).action(dealDamageGameEventData);

        return finalDamage;
    }

    public Vector2 GetRandomLocationOnMap()
    {
        var mapBound = mapArea.transform.localScale / 2;
        return new Vector2(
            Random.Range(-mapBound.x, mapBound.x),
            Random.Range(-mapBound.y, mapBound.y)
        );
    }
}
