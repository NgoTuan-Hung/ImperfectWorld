using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public partial class Stat
{
    public bool alive = true;
    CustomMono customMono;
    private PoolObject healthAndManaIndicatorPO;
    public float moveSpeedPerFrame;
    float dissolveTime = 5f;
    Action onEnable = () => { };

    [Header("<color='#D72638'>CURRENT HP")]
    public FloatStatWithCap currentHealthPoint = new();
    public Action currentHealthPointReachZeroEvent = () => { };

    [Header("<color='#C71F37'>HP")]
    public FloatStatWithModifier healthPoint = new();

    [Header("<color='#FF6B6B'>HP REGEN")]
    public FloatStatWithModifier healthRegen = new();

    [Header("<color='#3E8EDE'>CURRENT MP")]
    public FloatStatWithCap currentManaPoint;

    [Header("<color='#2F75C0'>MP")]
    public FloatStatWithModifier manaPoint = new();

    [Header("<color='#7FDBFF'>MP REGEN")]
    public FloatStatWithModifier manaRegen = new();

    [Header("<color='#F39C12'>MIGHT")]
    public FloatStatWithModifier might = new();

    [Header("<color='#27AE60'>REFLEX")]
    public FloatStatWithModifier reflex = new();

    [Header("<color='#9B59B6'>WISDOM")]
    public FloatStatWithModifier wisdom = new();

    [Header("<color='#E67E22'>ATTACK SPEED")]
    public FloatStatWithModifier attackSpeed = new();

    [Header("<color='#95A5A6'>ARMOR")]
    public FloatStatWithModifier armor = new();

    [Header("<color='#1ABC9C'>MOVE SPEED")]
    public FloatStatWithModifier moveSpeed = new();

    [Header("<color='#FFC107'>DAMAGE MODIFIER")]
    public FloatModifier damageModifier = new();

    [Header("<color='#C62828'>OMNIVAMP")]
    public FloatStatWithModifier omnivamp = new();

    [Header("<color='#E53935'>ATTACK DAMAGE")]
    public FloatStatWithModifier attackDamage = new();

    [Header("<color='#FFD54F'>CRIT CHANCE")]
    public FloatStatWithModifier critChance = new();

    [Header("<color='#AB47BC'>CRIT DAMAGE MODIFIER")]
    public FloatStatWithModifier critDamageModifier = new();

    [Header("<color='#4A90E2'>DAMAGE REDUCTION")]
    public FloatStatWithModifier damageReduction = new();

    [HideInInspector]
    public FloatStatWithModifier actionMoveSpeedReduceRate = new();

    [HideInInspector]
    public FloatStatModifier actionSlowModifier = new(0, FloatStatModifierType.Multiplicative);
    public ValueChangeGameEventData hpChangeED;
    DealDamageGameEventData dealDamageGameEventData;
}
