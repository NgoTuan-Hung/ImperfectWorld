using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Melee,
    Ranged,
}

[CreateAssetMenu(fileName = "ChampionData", menuName = "ScriptableObjects/ChampionData")]
public class ChampionData : ScriptableObject
{
    public AttackType attackType = AttackType.Melee;

    [Serializable]
    public class MeleeAttackInfo
    {
        public GameEffectSO meleeAttackEffectSO,
            meleeImpactEffectSO;
    }

    public List<MeleeAttackInfo> meleeAttackInfos = new();
    public int variant;
    public GameEffectSO rangedProjectileEffectSO,
        rangedImpactEffectSO;

    [Header("<color='#C71F37'>HP")]
    public float healthPoint;

    [Header("<color='#FF6B6B'>HP REGEN")]
    public float healthRegen = new();

    [Header("<color='#2F75C0'>MP")]
    public float manaPoint = new();

    [Header("<color='#7FDBFF'>MP REGEN")]
    public float manaRegen = new();

    [Header("<color='#F39C12'>MIGHT")]
    public float might = new();

    [Header("<color='#27AE60'>REFLEX")]
    public float reflex = new();

    [Header("<color='#9B59B6'>WISDOM")]
    public float wisdom = new();

    [Header("<color='#E67E22'>ATTACK SPEED")]
    public float attackSpeed = new();

    [Header("<color='#95A5A6'>ARMOR")]
    public float armor = new();

    [Header("<color='#1ABC9C'>MOVE SPEED")]
    public float moveSpeed = new();

    [Header("<color='#FFC107'>DAMAGE MODIFIER")]
    public float damageModifier = new();

    [Header("<color='#C62828'>OMNIVAMP")]
    public float omnivamp = new();

    [Header("<color='#E53935'>ATTACK DAMAGE")]
    public float attackDamage = new();

    [Header("<color='#FFD54F'>CRIT CHANCE")]
    public float critChance = new();

    [Header("<color='#AB47BC'>CRIT DAMAGE MODIFIER")]
    public float critDamageModifier = new();

    [Header("<color='#4A90E2'>DAMAGE REDUCTION")]
    public float damageReduction = new();

    [Header("<color='#FFD447'>ATTACK RANGE")]
    public float attackRange = new();

    ChampionDataPrecompute championDataPrecompute;

    public GameEffect GetMeleeAttackEffect(int p_variant) =>
        GameManager
            .Instance.poolLink[meleeAttackInfos[p_variant].meleeAttackEffectSO]
            .PickOneGameEffect();

    public GameEffect GetMeleeImpactEffect(int p_variant) =>
        GameManager
            .Instance.poolLink[meleeAttackInfos[p_variant].meleeImpactEffectSO]
            .PickOneGameEffect();

    public bool CheckMeleeAttackEffect(int p_variant) =>
        meleeAttackInfos[p_variant].meleeAttackEffectSO != null;

    public bool CheckMeleeImpactEffect(int p_variant) =>
        meleeAttackInfos[p_variant].meleeImpactEffectSO != null;

    public GameEffect GetRangedProjectileEffect() =>
        GameManager.Instance.poolLink[rangedProjectileEffectSO].PickOneGameEffect();

    public ChampionDataPrecompute GetPrecomputeData()
    {
        championDataPrecompute ??= new ChampionDataPrecompute(this);
        return championDataPrecompute;
    }
}
