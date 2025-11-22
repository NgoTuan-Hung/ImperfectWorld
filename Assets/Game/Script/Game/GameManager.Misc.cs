using System;
using System.Collections.Generic;
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
            "current hp",
            "<link=current hp><color=#D72638>current hp</color></link>: current health point."
        },
        { "hp", "<link=hp><color=#C71F37>hp</color></link>: health point." },
        {
            "hp regen",
            "<link=hp regen><color=#FF6B6B>hp regen</color></link>: how many <link=hp><color=#C71F37>hp</color></link> is regenerated per second."
        },
        {
            "current mp",
            "<link=current mp><color=#3E8EDE>current mp</color></link>: current mana point."
        },
        {
            "mp",
            "<link=mp><color=#2F75C0>mp</color></link>: mana point, champion use active skill when <link=current mp><color=#3E8EDE>current mp</color></link> reach <link=mp><color=#2F75C0>mp</color></link>."
        },
        {
            "mp regen",
            "<link=mp regen><color=#7FDBFF>mp regen</color></link>: how many <link=mp><color=#2F75C0>mp</color></link> is regenerated per second."
        },
        {
            "might",
            "<link=might><color=#F39C12>might</color></link>: increases <link=hp><color=#C71F37>hp</color></link> and <link=hp regen><color=#FF6B6B>hp regen</color></link>."
        },
        {
            "reflex",
            "<link=reflex><color=#27AE60>reflex</color></link>: increases <link=armor><color=#95A5A6>armor</color></link> and <link=aspd><color=#E67E22>aspd</color></link>."
        },
        {
            "wisdom",
            "<link=wisdom><color=#9B59B6>wisdom</color></link>: increases <link=mp><color=#2F75C0>mp</color></link> and <link=mp regen><color=#7FDBFF>mp regen</color></link>."
        },
        { "aspd", "<link=aspd><color=#E67E22>aspd</color></link>: attack speed." },
        { "armor", "<link=armor><color=#95A5A6>armor</color></link>: 1 armor mitigates 1 damage." },
        { "mspd", "<link=mspd><color=#1ABC9C>mspd</color></link>: move speed." },
        { "dmgmod", "<link=dmgmod><color=#FFC107>dmgmod</color></link>: damage multiplier." },
        {
            "omnivamp",
            "<link=omnivamp><color=#C62828>omnivamp</color></link>: heals from damage dealt."
        },
        { "atk", "<link=atk><color=#E53935>atk</color></link>: attack damage." },
        { "crit", "<link=crit><color=#FFD54F>crit</color></link>: crit chance." },
        {
            "critmod",
            "<link=critmod><color=#AB47BC>critmod</color></link>: crit damage modifier, how much damage is multiplied on crit."
        },
        {
            "damage reduction",
            "<link=damage reduction><color=#4A90E2>damage reduction</color></link>: reduces total damage taken, applied after armor."
        },
        { "atkrange", "<link=atkrange><color=#FFD447>atkrange</color></link>: attack range." },
        {
            "strike lock",
            "<link=strike lock><color=#fc03d7>strike lock</color></link>: one affected by strike lock cannot attack."
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

    /* Stat Colors */
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
    Dictionary<ComplexTextID, string> linkDict = new()
    {
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
    };

    Dictionary<ComplexTextID, string> colorDict = new()
    {
        { ComplexTextID.HP, healthPointColor },
        { ComplexTextID.HPREGEN, healthRegenColor },
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
    };

    public string ConstructComplexText(ComplexTextID id, string innerText) =>
        $"<link={linkDict[id]}><color={colorDict[id]}>{linkDict[id]}{innerText}</color></link>";
}
