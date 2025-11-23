using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using UnityEngine;

public enum GameState
{
    MainMenu,
    MapTravelingPhase,
    PositioningPhase,
    BattlePhase,
    RewardPhase,
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
}

public partial class GameManager
{
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
                + ": Decreases mana point by 0.25 and Increases mana point regeneration by 1.5."
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
    };
    public GameObject itemTooltipPrefab;
    Dictionary<ItemBehaviourType, Type> itemBehaviourMapper = new()
    {
        { ItemBehaviourType.KuraiKōraBehaviour, typeof(KuraiKōraBehaviour) },
        { ItemBehaviourType.PhoenixHeartBehaviour, typeof(PhoenixHeartBehaviour) },
    };

    public Type MapItemBehaviour(ItemBehaviourType type) => itemBehaviourMapper[type];

    public List<StatUpgrade> statUpgrades = new();
    public List<ChampionReward> championRewards = new();
    public UIEffectPreset championRewardSelectedEffectPreset;
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
    };

    public static string ConstructComplexText(ComplexTextID id, string innerText) =>
        $"<link={linkDict[id]}><color={colorDict[id]}>{linkDict[id]}{innerText}</color></link>";

    public static string ConstructComplexText(ComplexTextID id) =>
        $"<link={linkDict[id]}><color={colorDict[id]}>{linkDict[id]}</color></link>";

    public static string ConstructColoredText(ComplexTextID id, string innerText) =>
        $"<color={colorDict[id]}>{innerText}</color>";

    public static string GetComplexTextIDAsText(ComplexTextID id) => linkDict[id];

    public static List<string> GetAllIDsAsText() => linkDict.Values.ToList();
}
